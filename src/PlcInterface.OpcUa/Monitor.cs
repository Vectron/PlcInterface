using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PlcInterface.OpcUa
{
    public class Monitor : IMonitor, IDisposable
    {
        private readonly IPlcConnection<Session> client;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ILogger<Monitor> logger;
        private readonly Dictionary<string, RegisteredSymbol> registeredSymbols = new Dictionary<string, RegisteredSymbol>();
        private readonly ISymbolHandler symbolHandler;
        private bool disposedValue = false;
        private Session session;
        private Subject<IMonitorResult> subject = new Subject<IMonitorResult>();
        private Subscription subscription;

        public Monitor(IPlcConnection<Session> client, ISymbolHandler symbolHandler, ILogger<Monitor> logger)
        {
            this.client = client;
            this.symbolHandler = symbolHandler;
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
                TimestampsToReturn = TimestampsToReturn.Both
            };

            disposables.Add(client.SessionStream.Subscribe(x =>
            {
                if (!x.IsConnected)
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
                    session.AddSubscription(subscription);
                    subscription.Create();
                    Update();
                }
            }));
        }

        public IObservable<IMonitorResult> SymbolStream => subject.AsObservable();

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public void RegisterIO(IEnumerable<string> ioNames, int updateInterval)
        {
            foreach (var name in ioNames)
            {
                RegisterIOInternal(name, updateInterval);
            }

            Update();
        }

        public void RegisterIO(string ioName, int updateInterval)
        {
            RegisterIOInternal(ioName, updateInterval);
            Update();
        }

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

        public void UnregisterIO(string ioName)
        {
            UnregisterIOInternal(ioName);

            if (session != null && session.Connected && subscription.Created)
            {
                subscription.ApplyChanges();
            }
        }

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

            MonitoredItem item = null;

            if (session == null || !session.Connected || !subscription.Created)
            {
                item = subscription.MonitoredItems.Where(x => x.DisplayName == nameLower).FirstOrDefault();
            }
            else
            {
                var symbol = symbolHandler.GetSymbolinfo(name).ConvertAndValidate();
                item = subscription.MonitoredItems.Where(x => x.Handle == symbol.Handle).FirstOrDefault();
            }

            if (item != null)
            {
                subscription.RemoveItem(item);
                registeredSymbols.Remove(nameLower);
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

                    if (subscription.MonitoredItems.Where(x => x.StartNodeId == symbol.Handle).Count() > 0)
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
                        DisplayName = item.Key
                    };

                    disposables.Add(Observable
                        .FromEventPattern<MonitoredItemNotificationEventHandler, MonitoredItem, MonitoredItemNotificationEventArgs>(
                        h => monitoredItem.Notification += h,
                        h => monitoredItem.Notification -= h)
                        .Select(x =>
                        {
                            if (x.Sender.Handle is SymbolInfo s
                                && x.EventArgs.NotificationValue is MonitoredItemNotification datachange)
                            {
                                return new MonitorResult(s.Name, datachange.Value.Value);
                            }

                            return null;
                        })
                        .Where(x => x != null)
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

        private class RegisteredSymbol
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