using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Opc.Ua.Client;
using PlcInterface.OpcUa.Extensions;

namespace PlcInterface.OpcUa;

/// <summary>
/// Implementation of <see cref="ISymbolHandler"/>.
/// </summary>
public class SymbolHandler : IOpcSymbolHandler, IDisposable
{
    private readonly IOpcPlcConnection connection;
    private readonly CompositeDisposable disposables = new();
    private readonly ILogger logger;
    private IDictionary<string, SymbolInfo> allSymbols = new Dictionary<string, SymbolInfo>(StringComparer.OrdinalIgnoreCase);
    private bool disposedValue;
    private Session? session;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolHandler"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public SymbolHandler(IOpcPlcConnection connection, ILogger<SymbolHandler> logger)
    {
        this.connection = connection;
        this.logger = logger;
        disposables.Add(connection.SessionStream.Subscribe(x =>
        {
            if (x.IsConnected && x.Value != null)
            {
                session = x.Value;
                UpdateSymbols(session);
            }
            else
            {
                session = null;
            }
        }));
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<ISymbolInfo> AllSymbols
        => allSymbols.Values.AsReadOnly();

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ISymbolInfo GetSymbolinfo(string ioName)
    {
        if (TryGetSymbolinfo(ioName, out var symbolInfo) && symbolInfo != null)
        {
            return symbolInfo;
        }

        throw new SymbolException($"{ioName} Does not excist in the PLC");
    }

    /// <inheritdoc/>
    public bool TryGetSymbolinfo(string ioName, [MaybeNullWhen(false)] out ISymbolInfo symbolInfo)
    {
        if (allSymbols.TryGetValue(ioName.ToLower(CultureInfo.InvariantCulture), out var symbolInfoResult))
        {
            symbolInfo = symbolInfoResult;
            return true;
        }

        logger.LogError("{IoName} Does not excist in the PLC", ioName);
        symbolInfo = null;
        return false;
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">Value indicating if we need to cleanup managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                disposables.Dispose();
                allSymbols.Clear();
                session = null;
            }

            disposedValue = true;
        }
    }

    private void UpdateSymbols(Session session)
    {
        // create a browser to browse the node tree
        if (connection.Settings is not OpcPlcConnectionOptions settings)
        {
            logger.LogCritical("No valid OPCSettings found");
            return;
        }

        logger.LogInformation("Updating Symbols");
        var elapsedWatch = Stopwatch.StartNew();
        var browser = new TreeBrowser(session);
        allSymbols.Clear();
        try
        {
            allSymbols = browser.BrowseTree(settings.Address ?? new Uri(string.Empty));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception thrown while building symbol tree");
        }

        elapsedWatch.Stop();
        logger.LogInformation("Symbols updated in {Time} ms, found {Amount} symbols", elapsedWatch.ElapsedMilliseconds, allSymbols.Count);
    }
}