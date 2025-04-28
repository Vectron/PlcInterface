using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
public partial class SymbolHandler : IAdsSymbolHandler, IDisposable
{
    private readonly IAdsPlcConnection connection;
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
        this.connection = connection;
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
    public event EventHandler? SymbolsUpdated;

    /// <inheritdoc/>
    public IReadOnlyCollection<ISymbolInfo> AllSymbols
        => allSymbols.Values;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
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

        if (!connection.IsConnected)
        {
            throw new SymbolException($"PLC not connected");
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

        if (!connection.IsConnected)
        {
            LogPlcNotConnected();
            return false;
        }

        LogVariableDoesNotExist(ioName);
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
        LogUpdatingSymbols();
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
            var rootSymbolPart = options.RootVariable.Split('.').FirstOrDefault() ?? string.Empty;

            allSymbols = symbolLoader.Symbols
                .Where(x => x.InstancePath.StartsWith(rootSymbolPart, StringComparison.OrdinalIgnoreCase))
                .DepthFirstTreeTraversal(x => x.SubSymbols)
                .Where(x => x.InstancePath.StartsWith(options.RootVariable, StringComparison.OrdinalIgnoreCase))
                .Select(x => new SymbolInfo(x, options.RootVariable))
                .Cast<IAdsSymbolInfo>()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            LogSymbolsUpdated(watch.ElapsedMilliseconds, allSymbols.Count);
            SymbolsUpdated?.Invoke(this, EventArgs.Empty);

            if (options.StoreSymbolsToDisk)
            {
                _ = StoreSymbolListOnDiskAsync();
            }
        }
        catch (Exception ex)
        {
            LogUpdatingSymbolsFailed(ex);
        }
    }
}
