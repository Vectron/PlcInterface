using Microsoft.Extensions.Logging;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace PlcInterface.S7
{
    public class SymbolHandler : ISymbolHandler, IDisposable
    {
        private readonly Dictionary<string, ISymbolInfo> allSymbols = new Dictionary<string, ISymbolInfo>();
        private readonly IPlcConnection<S7Client> client;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ILogger<SymbolHandler> logger;
        private bool disposedValue = false;

        public SymbolHandler(IPlcConnection<S7Client> client, ILogger<SymbolHandler> logger)
        {
            this.client = client;
            this.logger = logger;
            disposables.Add(client.SessionStream.Where(x => x.IsConnected).Subscribe(x => UpdateSymbols()));
        }

        public IReadOnlyCollection<ISymbolInfo> AllSymbols
            => allSymbols.Values;

        public void Dispose()
            => Dispose(true);

        public ISymbolInfo GetSymbolinfo(string ioName)
        {
            _ = allSymbols.TryGetValue(ioName.ToLower(), out var value);

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
                    disposables?.Dispose();
                }

                disposedValue = true;
            }
        }

        private void UpdateSymbols()
        {
        }
    }
}