using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.Ads.ValueAccess;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;

namespace PlcInterface.Ads
{
    /// <summary>
    /// Implementation of <see cref="ISymbolHandler"/>.
    /// </summary>
    public class SymbolHandler : IAdsSymbolHandler, IDisposable
    {
        private readonly Dictionary<string, ISymbolInfo> allSymbols = new(StringComparer.OrdinalIgnoreCase);
        private readonly CompositeDisposable disposables = new();
        private readonly IFileSystem fileSystem;
        private readonly ILogger<SymbolHandler> logger;
        private readonly IOptions<SymbolHandlerSettings> settings;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolHandler"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
        /// <param name="settings">A <see cref="IOptions{TOptions}"/> of <see cref="SymbolHandlerSettings"/> implementation.</param>
        /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
        /// <param name="fileSystem">A <see cref="IFileSystem"/> for interacting with the file system.</param>
        public SymbolHandler(IAdsPlcConnection connection, IOptions<SymbolHandlerSettings> settings, ILogger<SymbolHandler> logger, IFileSystem fileSystem)
        {
            this.settings = settings;
            this.logger = logger;
            this.fileSystem = fileSystem;
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
        public ISymbolInfo GetSymbolinfo(string ioName)
        {
            if (!allSymbols.TryGetValue(ioName.ToLower(CultureInfo.InvariantCulture), out var value))
            {
                throw new SymbolException($"{ioName} Does not excist in the PLC");
            }

            return value;
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

            if (!allSymbols.ContainsKey(symbolInfo.NameLower))
            {
                allSymbols.Add(symbolInfo.NameLower, symbolInfo);
            }

            foreach (var subSymbol in symbol.SubSymbols)
            {
                AddSymbol(subSymbol);
            }
        }

        private void StoreSymbolListOnDisk()
        {
            if (!settings.Value.StoreSymbolsToDisk)
            {
                return;
            }

            var outputPath = settings.Value.OutputPath;

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

                var symbolLoader = SymbolLoaderFactory.Create(client, symbolLoaderSettings);
                allSymbols.Clear();

                foreach (var symbol in symbolLoader.Symbols)
                {
                    AddSymbol(symbol);
                }

                StoreSymbolListOnDisk();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception updating symbols");
            }
        }
    }
}