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
        private readonly Subject<IMonitorResult> subject = new();
        private readonly Subscription subscription;
        private readonly ISymbolHandler symbolHandler;
        private readonly IOpcTypeConverter typeConverter;
        private bool disposedValue;
        private Session? session;

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

            disposables.Add(connection.SessionStream.Subscribe(x =>
            {
                if (!x.IsConnected || x.Value == null)
                {
                    session = null;
                    return;
                }

                session = x.Value;
                if (session.Subscriptions.Contains(subscription))
                {
                    Update();
                }
                else
                {
                    _ = session.AddSubscription(subscription);
                    subscription.Create();
                    Update();
                }
            }));
        }

        /// <inheritdoc/>
        public IObservable<IMonitorResult> SymbolStream => subject.AsObservable();

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

            Update();
        }

        /// <inheritdoc/>
        public void RegisterIO(string ioName, int updateInterval = 1000)
        {
            RegisterIOInternal(ioName, updateInterval);
            Update();
        }

        /// <inheritdoc/>
        public void UnregisterIO(IEnumerable<string> ioNames)
        {
            foreach (var name in ioNames)
            {
                UnregisterIOInternal(name);
            }

            if (session != null && session.Connected && subscription.Created)
            {
                subscription.ApplyChanges();
            }
        }

        /// <inheritdoc/>
        public void UnregisterIO(string ioName)
        {
            UnregisterIOInternal(ioName);

            if (session != null && session.Connected && subscription.Created)
            {
                subscription.ApplyChanges();
            }
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
                    subject?.Dispose();
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

            var nameLower = name.ToLowerInvariant();

            if (registeredSymbols.ContainsKey(nameLower))
            {
                registeredSymbols[nameLower].Subscriptions += 1;
                return;
            }

            logger.LogDebug($"Registered {name} for monitoring");
            registeredSymbols.Add(nameLower, new RegisteredSymbol() { Subscriptions = 1, UpdateInterval = updateInterval });
        }

        private void UnregisterIOInternal(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is null or whitespace", nameof(name));
            }

            var nameLower = name.ToLowerInvariant();

            if (registeredSymbols.ContainsKey(nameLower) && registeredSymbols[nameLower].Subscriptions > 1)
            {
                logger.LogDebug($"Unregistered {name} from monitoring");
                registeredSymbols[nameLower].Subscriptions -= 1;
                return;
            }

            MonitoredItem? item = null;

            if (session == null || !session.Connected || !subscription.Created)
            {
                item = subscription.MonitoredItems.FirstOrDefault(x => string.Equals(x.DisplayName, nameLower, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                var symbol = symbolHandler.GetSymbolinfo(name).ConvertAndValidate();
                item = subscription.MonitoredItems.FirstOrDefault(x => x.Handle == symbol.Handle);
            }

            if (item != null)
            {
                subscription.RemoveItem(item);
                _ = registeredSymbols.Remove(nameLower);
                logger.LogDebug($"Removed {name} from monitoring");
            }
        }

        private void Update()
        {
            if (session == null || !session.Connected || !subscription.Created)
            {
                return;
            }

            foreach (var item in registeredSymbols)
            {
                try
                {
                    var symbol = symbolHandler.GetSymbolinfo(item.Key).ConvertAndValidate();

                    if (subscription.MonitoredItems.Any(x => x.StartNodeId == symbol.Handle))
                    {
                        continue;
                    }

                    var monitoredItem = new MonitoredItem()
                    {
                        StartNodeId = symbol.Handle,
                        AttributeId = Attributes.Value,
                        Handle = symbol,
                        SamplingInterval = item.Value.UpdateInterval,
                        QueueSize = 1,
                        DiscardOldest = true,
                        DisplayName = item.Key,
                    };

                    disposables.Add(Observable
                        .FromEventPattern<MonitoredItemNotificationEventHandler, MonitoredItem, MonitoredItemNotificationEventArgs>(
                        h => monitoredItem.Notification += h,
                        h => monitoredItem.Notification -= h)
                        .Select(x =>
                        {
                            if (x.Sender?.Handle is SymbolInfo s
                                && x.EventArgs.NotificationValue is MonitoredItemNotification datachange)
                            {
                                return new MonitorResult(s.Name, datachange.Value.Value);
                            }

                            return null;
                        })
                        .WhereNotNull()
                        .SubscribeSafe(subject));
                    logger.LogDebug($"Added {item.Key} to plc monitor");
                    subscription.AddItem(monitoredItem);
                }
                catch (SymbolException ex)
                {
                    logger.LogCritical(ex, $"Failed to monitor {item.Key}");
                }
            }

            subscription.ApplyChanges();
        }

        private sealed class RegisteredSymbol
        {
            public int Subscriptions
            {
                get; set;
            }

            public int UpdateInterval
            {
                get; set;
            }
        }
    }
}