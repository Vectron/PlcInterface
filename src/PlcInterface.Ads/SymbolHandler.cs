using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.ValueAccess;

namespace PlcInterface.Ads;

/// <summary>
/// Implementation of <see cref="ISymbolHandler"/>.
/// </summary>
public class SymbolHandler : IAdsSymbolHandler, IDisposable
{
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up")]
    private readonly CompositeDisposable disposables = [];

    private readonly IFileSystem fileSystem;
    private readonly ILogger<SymbolHandler> logger;
    private readonly AdsSymbolHandlerOptions options;
    private readonly ISymbolLoaderFactory symbolLoaderFactory;
    private Dictionary<string, IAdsSymbolInfo> allSymbols = new(StringComparer.OrdinalIgnoreCase);
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolHandler"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="options">
    /// A <see cref="IOptions{TOptions}"/> of <see cref="AdsSymbolHandlerOptions"/> implementation.
    /// </param>
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
    ISymbolInfo ISymbolHandler.GetSymbolInfo(string ioName)
        => GetSymbolInfo(ioName);

    /// <inheritdoc/>
    public IAdsSymbolInfo GetSymbolInfo(string ioName)
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
    public bool TryGetSymbolInfo(string ioName, [MaybeNullWhen(false)] out IAdsSymbolInfo symbolInfo)
    {
        if (allSymbols.TryGetValue(ioName, out symbolInfo))
        {
            return true;
        }

        logger.LogError("{IoName} Does not exist in the PLC", ioName);
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

    private async Task StoreSymbolListOnDiskAsync()
    {
        var outputPath = options.OutputPath;

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            outputPath = fileSystem.Path.Combine("logs");
        }

        _ = fileSystem.Directory.CreateDirectory(outputPath);
        var filePath = fileSystem.Path.Combine(outputPath, "vars.txt");
        await fileSystem.File.WriteAllLinesAsync(filePath, allSymbols.Values.Select(x => x.Name), System.Text.Encoding.UTF8, CancellationToken.None)
            .ConfigureAwait(false);
    }

    private void UpdateSymbols(IAdsConnection client)
    {
        logger.LogInformation("Updating Symbols");
        var watch = Stopwatch.StartNew();
        try
        {
            var symbolLoaderSettings = new SymbolLoaderSettings(SymbolsLoadMode.DynamicTree)
            {
                AutomaticReconnection = false,
                NonCachedArrayElements = true,
                ValueAccessMode = ValueAccessMode.SymbolicByHandle,
                ValueCreation = ValueCreationModes.Primitives,
                SymbolsLoadMode = SymbolsLoadMode.DynamicTree,
                ValueUpdateMode = ValueUpdateMode.None,
            };

            var symbolLoader = symbolLoaderFactory.Create(client, symbolLoaderSettings);
            allSymbols = symbolLoader.Symbols
                .DepthFirstTreeTraversal(x => x.SubSymbols)
                .Select(x => new SymbolInfo(x))
                .Cast<IAdsSymbolInfo>()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            logger.LogInformation("Symbols updated in {Time} ms, found {Amount} symbols", watch.ElapsedMilliseconds, allSymbols.Count);

            if (options.StoreSymbolsToDisk)
            {
                _ = StoreSymbolListOnDiskAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception updating symbols");
        }
    }
}
