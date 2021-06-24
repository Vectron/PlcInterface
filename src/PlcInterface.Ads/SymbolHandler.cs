using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
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
    public class SymbolHandler : ISymbolHandler
    {
        private readonly Dictionary<string, ISymbolInfo> allSymbols = new Dictionary<string, ISymbolInfo>();
        private readonly IPlcConnection<IAdsConnection> connection;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ILogger<SymbolHandler> logger;
        private readonly IOptions<SymbolHandlerSettings> settings;
        private bool disposedValue = false;

        public SymbolHandler(IPlcConnection<IAdsConnection> connection, IOptions<SymbolHandlerSettings> settings, ILogger<SymbolHandler> logger)
        {
            this.connection = connection;
            this.settings = settings;
            this.logger = logger;

            var session = connection.SessionStream
                .Where(x => x.IsConnected)
                .Select(x => x.Value)
                .Subscribe(UpdateSymbols);
            disposables.Add(session);
        }

        public static string AssemblyDirectory
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                {
                    return string.Empty;
                }

                var codeBase = assembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public IReadOnlyCollection<ISymbolInfo> AllSymbols
            => allSymbols.Values;

        public void Dispose()
        {
            Dispose(true);
        }

        public ISymbolInfo GetSymbolinfo(string ioName)
        {
            allSymbols.TryGetValue(ioName.ToLower(), out ISymbolInfo value);

            if (value == null)
            {
                throw new SymbolException($"{ioName} Does not excist in the PLC");
            }

            return value;
        }

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

            string outputPath = settings.Value.OutputPath;

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = Path.Combine(AssemblyDirectory, "logs");
            }

            Directory.CreateDirectory(outputPath);
            using (var streamWriter = new StreamWriter(Path.Combine(outputPath, "vars.txt"), false))
            {
                foreach (var symbol in allSymbols.Values)
                {
                    streamWriter.WriteLine("{0}", symbol.Name);
                }
            }
        }

        private void UpdateSymbols(IAdsConnection client)
        {
            logger.LogInformation("Updating Symbols");
            try
            {
                var settings = new SymbolLoaderSettings(SymbolsLoadMode.DynamicTree)
                {
                    AutomaticReconnection = false,
                    NonCachedArrayElements = true,
                    ValueAccessMode = ValueAccessMode.IndexGroupOffsetPreferred,
                    ValueCreation = ValueCreationModes.Primitives
                };

                var symbolLoader = SymbolLoaderFactory.Create(client, settings);
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