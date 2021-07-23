using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    /// <summary>
    /// A <see cref="IDisposable"/> for counting reference to the item.
    /// </summary>
    internal class DisposableMonitorItem : IDisposable
    {
        private readonly IDisposable stream;

        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableMonitorItem"/> class.
        /// </summary>
        /// <param name="stream">The item to dispose when the refcount is 0.</param>
        private DisposableMonitorItem(IDisposable stream)
        {
            this.stream = stream;
            Subscriptions = 1;
        }

        /// <summary>
        /// Gets or sets the number of references to this item.
        /// </summary>
        public int Subscriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Create a <see cref="DisposableMonitorItem"/>.
        /// </summary>
        /// <param name="symbolStream">The stream to publish updates to.</param>
        /// <param name="symbolInfo">The symbol to register with.</param>
        /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
        /// <returns>The created <see cref="DisposableMonitorItem"/>.</returns>
        public static DisposableMonitorItem Create(ISubject<IMonitorResult> symbolStream, SymbolInfo symbolInfo, IAdsTypeConverter typeConverter)
        {
            var valueSymbol = symbolInfo.Symbol as IValueSymbol;
            var subscriptions = valueSymbol
                  .ThrowIfNull()
                  .WhenValueChanged()
                  .Select(x => new MonitorResult(symbolInfo.Name, typeConverter.Convert(x, valueSymbol!)))
                  .SubscribeSafe(symbolStream);

            return new DisposableMonitorItem(subscriptions);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Value indicating if we need to cleanup managed resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007:Don't dispose injected.", Justification = "Class is meant to take ownership")]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stream?.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}