using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// Implementation of <see cref="ISymbolHandler"/>.
/// </summary>
public partial class SymbolHandler : IOpcSymbolHandler, IDisposable
{
    private readonly IOpcPlcConnection connection;

    private readonly CompositeDisposable disposables = [];

    private readonly ILogger logger;
    private IDictionary<string, IOpcSymbolInfo> allSymbols = new Dictionary<string, IOpcSymbolInfo>(StringComparer.OrdinalIgnoreCase);
    private bool disposedValue;
    private ISession? session;

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
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    ISymbolInfo ISymbolHandler.GetSymbolInfo(string ioName)
        => GetSymbolInfo(ioName);

    /// <inheritdoc/>
    public IOpcSymbolInfo GetSymbolInfo(string ioName)
    {
        if (TryGetSymbolInfo(ioName, out var symbolInfo) && symbolInfo != null)
        {
            return symbolInfo;
        }

        throw new SymbolException($"{ioName} Does not exist in the PLC");
    }

    /// <inheritdoc/>
    bool ISymbolHandler.TryGetSymbolInfo(string ioName, [MaybeNullWhen(false)] out ISymbolInfo symbolInfo)
    {
        var result = TryGetSymbolInfo(ioName, out var symbolInfoResult);
        symbolInfo = symbolInfoResult;
        return result;
    }

    /// <inheritdoc/>
    public bool TryGetSymbolInfo(string ioName, [MaybeNullWhen(false)] out IOpcSymbolInfo symbolInfo)
    {
        if (allSymbols.TryGetValue(ioName.ToLower(CultureInfo.InvariantCulture), out var symbolInfoResult))
        {
            symbolInfo = symbolInfoResult;
            return true;
        }

        LogVariableDoesNotExist(ioName);
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

    private void UpdateSymbols(ISession session)
    {
        // create a browser to browse the node tree
        if (connection.Settings is not OpcPlcConnectionOptions settings)
        {
            // TODO: this should not be done, it's to error prone when the settings are overridden.
            // logger.LogCritical("No valid OPCSettings found");
            return;
        }

        LogUpdatingSymbols();
        var elapsedWatch = Stopwatch.StartNew();
        var browser = new TreeBrowser(session);
        allSymbols.Clear();
        try
        {
            allSymbols = browser.BrowseTree(settings.Address ?? new Uri(string.Empty));
        }
        catch (Exception ex)
        {
            LogUpdatingSymbolsFailed(ex);
        }

        elapsedWatch.Stop();
        LogSymbolsUpdated(elapsedWatch.ElapsedMilliseconds, allSymbols.Count);
    }
}
