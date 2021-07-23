using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// A implementation of <see cref="IMonitor"/>.
    /// </summary>
    public class Monitor : IMonitor, IDisposable
    {
        private readonly CompositeDisposable disposables = new();
        private readonly ILogger<Monitor> logger;
        private readonly Dictionary<string, RegisteredSymbol> registeredSymbols = new(StringComparer.OrdinalIgnoreCase);
        private readonly Subscription subscription;
        private readonly ISymbolHandler symbolHandler;
        private readonly Subject<IMonitorResult> symbolStream = new();
        private readonly IOpcTypeConverter typeConverter;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
        /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
        /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
        public Monitor(IPlcConnection<Session> connection, ISymbolHandler symbolHandler, IOpcTypeConverter typeConverter, ILogger<Monitor> logger)
        {
            this.symbolHandler = symbolHandler;
            this.typeConverter = typeConverter;
            this.logger = logger;
            subscription = new Subscription()
            {
                DisplayName = null,
                PublishingEnabled = true,
                PublishingInterval = 1000,
                Priority = 1,
                KeepAliveCount = 10,
                LifetimeCount = 100,
                MaxNotificationsPerPublish = 1000,
                TimestampsToReturn = TimestampsToReturn.Both,
            };

            disposables.Add(connection.SessionStream.Where(x => x.IsConnected).Subscribe(x =>
            {
                if (x.Value == null)
                {
                    return;
                }

                foreach (var keyValue in registeredSymbols)
                {
                    var name = keyValue.Key;
                    var value = keyValue.Value;
                    var symbolInfo = symbolHandler.GetSymbolinfo(name).ConvertAndValidate();
                    value.MonitoredItem.StartNodeId = symbolInfo.Handle;
                    value.MonitoredItem.Handle = symbolInfo;
                }

                if (x.Value.Subscriptions.Contains(subscription))
                {
                    subscription.ApplyChanges();
                }
                else
                {
                    _ = x.Value.AddSubscription(subscription);
                    subscription.Create();
                    subscription.ApplyChanges();
                }
            }));
        }

        /// <inheritdoc/>
        public IObservable<IMonitorResult> SymbolStream => symbolStream.AsObservable();

        /// <inheritdoc/>
        public ITypeConverter TypeConverter
            => typeConverter;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000)
        {
            foreach (var name in ioNames)
            {
                RegisterIOInternal(name, updateInterval);
            }

            subscription.ApplyChanges();
        }

        /// <inheritdoc/>
        public void RegisterIO(string ioName, int updateInterval = 1000)
        {
            RegisterIOInternal(ioName, updateInterval);
            subscription.ApplyChanges();
        }

        /// <inheritdoc/>
        public void UnregisterIO(IEnumerable<string> ioNames)
        {
            foreach (var name in ioNames)
            {
                UnregisterIOInternal(name);
            }

            subscription.ApplyChanges();
        }

        /// <inheritdoc/>
        public void UnregisterIO(string ioName)
        {
            UnregisterIOInternal(ioName);
            subscription.ApplyChanges();
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
                    symbolStream?.Dispose();
                    subscription?.Dispose();
                    disposables?.Dispose();
                    registeredSymbols?.Clear();
                }

                disposedValue = true;
            }
        }

        private void RegisterIOInternal(string name, int updateInterval)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is null or whitespace", nameof(name));
            }

            if (registeredSymbols.TryGetValue(name, out var registeredSymbol))
            {
                registeredSymbol.Subscriptions++;
                return;
            }

            logger.LogDebug($"Registered {name} for monitoring");
            registeredSymbol = RegisteredSymbol.Create(name, updateInterval, symbolStream, typeConverter);
            registeredSymbols.Add(name, registeredSymbol);
            subscription.AddItem(registeredSymbol.MonitoredItem);
            registeredSymbol.UpdateMonitoredItem(symbolHandler);
        }

        private void UnregisterIOInternal(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is null or whitespace", nameof(name));
            }

            if (!registeredSymbols.TryGetValue(name, out var registeredSymbol))
            {
                return;
            }

            logger.LogDebug($"Unregistered {name} from monitoring");
            registeredSymbol.Subscriptions--;

            if (registeredSymbol.Subscriptions == 0)
            {
                subscription.RemoveItem(registeredSymbol.MonitoredItem);
                _ = registeredSymbols.Remove(name);
                registeredSymbol.Dispose();
                logger.LogDebug($"Removed {name} from monitoring");
            }
        }

        private sealed class RegisteredSymbol : IDisposable
        {
            private readonly string name = string.Empty;
            private readonly IDisposable symbolStream;
            private bool disposedValue;

            private RegisteredSymbol(string name, MonitoredItem monitoredItem, IDisposable symbolStream)
            {
                this.name = name;
                MonitoredItem = monitoredItem;
                this.symbolStream = symbolStream;
                Subscriptions = 1;
            }

            public MonitoredItem MonitoredItem
            {
                get;
            }

            public int Subscriptions
            {
                get;
                set;
            }

            public static RegisteredSymbol Create(string name, int updateInterval, ISubject<IMonitorResult> symbolStream, IOpcTypeConverter typeConverter)
            {
                var monitoredItem = new MonitoredItem()
                {
                    AttributeId = Attributes.Value,
                    SamplingInterval = updateInterval,
                    QueueSize = 1,
                    DiscardOldest = true,
                    DisplayName = name,
                };

                var stream = Observable.FromEventPattern<MonitoredItemNotificationEventHandler, MonitoredItem, MonitoredItemNotificationEventArgs>(
                    h => monitoredItem.Notification += h,
                    h => monitoredItem.Notification -= h)
                    .Select(x =>
                    {
                        if (x.Sender?.Handle is SymbolInfo s
                        && x.EventArgs.NotificationValue is MonitoredItemNotification datachange)
                        {
                            return new MonitorResult(s.Name, typeConverter.Convert(datachange.Value.Value));
                        }

                        return null;
                    })
                    .WhereNotNull()
                    .SubscribeSafe(symbolStream);

                return new RegisteredSymbol(name, monitoredItem, stream);
            }

            public void Dispose()
                => Dispose(disposing: true);

            public void UpdateMonitoredItem(ISymbolHandler symbolHandler)
            {
                var symbol = symbolHandler.GetSymbolinfo(name);
                if (symbol != null)
                {
                    var symbolInfo = symbol.ConvertAndValidate();
                    MonitoredItem.StartNodeId = symbolInfo.Handle;
                    MonitoredItem.Handle = symbolInfo;
                }
            }

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        symbolStream?.Dispose();
                    }

                    disposedValue = true;
                }
            }
        }
    }
}