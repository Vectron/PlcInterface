using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    /// <summary>
    /// A <see cref="IDisposable"/> for counting reference to the item.
    /// </summary>
    internal sealed class DisposableMonitorItem : IDisposable
    {
        private readonly string name;
        private bool disposedValue;
        private IDisposable stream;

        private DisposableMonitorItem(string name)
        {
            stream = Disposable.Empty;
            Subscriptions = 1;
            this.name = name;
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
        /// <param name="name">The name of the symbol.</param>
        /// <returns>The created <see cref="DisposableMonitorItem"/>.</returns>
        public static DisposableMonitorItem Create(string name)
            => new(name);

        /// <inheritdoc/>
        public void Dispose()
            => Dispose(true);

        /// <summary>
        /// Update the subscriptions.
        /// </summary>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/>.</param>
        /// <param name="symbolStream">The stream to subscribe to.</param>
        /// <param name="typeConverter">A <see cref="ITypeConverter"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "MA0032:Use an overload with a CancellationToken argument", Justification = "Dont need a cancelation token.")]
        public void Update(ISymbolHandler symbolHandler, ISubject<IMonitorResult> symbolStream, IAdsTypeConverter typeConverter)
        {
            if (symbolHandler.GetSymbolinfo(name) is SymbolInfo symbolInfo
                && symbolInfo.Symbol is IValueSymbol valueSymbol
                && valueSymbol.Connection != null
                && valueSymbol.Connection.IsConnected)
            {
                try
                {
                    stream.Dispose();
                }
                catch (Exception)
                {
                }
                stream = valueSymbol
                    .WhenValueChanged()
                    .Select(x => new MonitorResult(name, typeConverter.Convert(x, valueSymbol)))
                    .Subscribe(symbolStream);
            }
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Value indicating if we need to cleanup managed resources.</param>
        private void Dispose(bool disposing)
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