using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly OpcSymbolHandlerOptions options;
    private IDictionary<string, IOpcSymbolInfo> allSymbols = new Dictionary<string, IOpcSymbolInfo>(StringComparer.OrdinalIgnoreCase);
    private bool disposedValue;
    private ISession? session;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolHandler"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="options">A <see cref="IOptions{TOptions}"/> of <see cref="OpcSymbolHandlerOptions"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public SymbolHandler(IOpcPlcConnection connection, IOptions<OpcSymbolHandlerOptions> options, ILogger<SymbolHandler> logger)
    {
        this.connection = connection;
        this.options = options.Value;
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
        LogUpdatingSymbols();
        var elapsedWatch = Stopwatch.StartNew();
        var browser = new TreeBrowser(session, logger);
        allSymbols.Clear();
        try
        {
            allSymbols = browser.BrowseTree(options.RootVariable);
        }
        catch (Exception ex)
        {
            LogUpdatingSymbolsFailed(ex);
        }

        elapsedWatch.Stop();
        LogSymbolsUpdated(elapsedWatch.ElapsedMilliseconds, allSymbols.Count);
    }
}
