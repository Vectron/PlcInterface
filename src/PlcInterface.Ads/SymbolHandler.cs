using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcInterface.Extensions;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.Ads.ValueAccess;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;

namespace PlcInterface.Ads;

/// <summary>
/// Implementation of <see cref="ISymbolHandler"/>.
/// </summary>
public class SymbolHandler : IAdsSymbolHandler, IDisposable
{
    private readonly Dictionary<string, IAdsSymbolInfo> allSymbols = new(StringComparer.OrdinalIgnoreCase);
    private readonly CompositeDisposable disposables = new();
    private readonly IFileSystem fileSystem;
    private readonly ILogger<SymbolHandler> logger;
    private readonly AdsSymbolHandlerOptions options;
    private readonly ISymbolLoaderFactory symbolLoaderFactory;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolHandler"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="options">A <see cref="IOptions{TOptions}"/> of <see cref="AdsSymbolHandlerOptions"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    /// <param name="fileSystem">A <see cref="IFileSystem"/> for interacting with the file system.</param>
    /// <param name="symbolLoaderFactory">A factory for creating a <see cref="SymbolLoaderFactory"/>.</param>
    public SymbolHandler(IAdsPlcConnection connection, IOptions<AdsSymbolHandlerOptions> options, ILogger<SymbolHandler> logger, IFileSystem fileSystem, ISymbolLoaderFactory symbolLoaderFactory)
    {
        this.options = options.Value;
        this.logger = logger;
        this.fileSystem = fileSystem;
        this.symbolLoaderFactory = symbolLoaderFactory;
        var session = connection.SessionStream
            .Where(x => x.IsConnected)
            .Select(x => x.Value)
            .WhereNotNull()
            .Subscribe(UpdateSymbols);
        disposables.Add(session);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<ISymbolInfo> AllSymbols
        => allSymbols.Values;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    ISymbolInfo ISymbolHandler.GetSymbolinfo(string ioName)
        => GetSymbolinfo(ioName);

    /// <inheritdoc/>
    public IAdsSymbolInfo GetSymbolinfo(string ioName)
    {
        if (TryGetSymbolinfo(ioName, out var symbolInfo) && symbolInfo != null)
        {
            return symbolInfo;
        }

        throw new SymbolException($"{ioName} Does not excist in the PLC");
    }

    /// <inheritdoc/>
    bool ISymbolHandler.TryGetSymbolinfo(string ioName, [MaybeNullWhen(false)] out ISymbolInfo symbolInfo)
    {
        var result = TryGetSymbolinfo(ioName, out var symbolInfoResult);
        symbolInfo = symbolInfoResult;
        return result;
    }

    /// <inheritdoc/>
    public bool TryGetSymbolinfo(string ioName, [MaybeNullWhen(false)] out IAdsSymbolInfo symbolInfo)
    {
        if (allSymbols.TryGetValue(ioName.ToLower(CultureInfo.InvariantCulture), out symbolInfo))
        {
            return true;
        }

        logger.LogError("{IoName} Does not excist in the PLC", ioName);
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
            }

            disposedValue = true;
        }
    }

    private void AddSymbol(ISymbol symbol)
    {
        var symbolInfo = new SymbolInfo(symbol);

        if (!allSymbols.ContainsKey(symbolInfo.Name))
        {
            allSymbols.Add(symbolInfo.Name, symbolInfo);
        }

        foreach (var subSymbol in symbol.SubSymbols)
        {
            AddSymbol(subSymbol);
        }
    }

    private void StoreSymbolListOnDisk()
    {
        var outputPath = options.OutputPath;

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            outputPath = fileSystem.Path.Combine("logs");
        }

        _ = fileSystem.Directory.CreateDirectory(outputPath);
        var filePath = fileSystem.Path.Combine(outputPath, "vars.txt");
        fileSystem.File.WriteAllLines(filePath, allSymbols.Values.Select(x => x.Name), System.Text.Encoding.UTF8);
    }

    private void UpdateSymbols(IAdsConnection client)
    {
        logger.LogInformation("Updating Symbols");
        try
        {
            var symbolLoaderSettings = new SymbolLoaderSettings(SymbolsLoadMode.DynamicTree)
            {
                AutomaticReconnection = false,
                NonCachedArrayElements = true,
                ValueAccessMode = ValueAccessMode.IndexGroupOffsetPreferred,
                ValueCreation = ValueCreationModes.Primitives,
            };

            var symbolLoader = symbolLoaderFactory.Create(client, symbolLoaderSettings);
            allSymbols.Clear();

            foreach (var symbol in symbolLoader.Symbols)
            {
                AddSymbol(symbol);
            }

            if (options.StoreSymbolsToDisk)
            {
                StoreSymbolListOnDisk();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception updating symbols");
        }
    }
}