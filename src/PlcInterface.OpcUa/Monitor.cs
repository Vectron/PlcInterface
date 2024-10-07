using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// A implementation of <see cref="IMonitor"/>.
/// </summary>
public partial class Monitor : IOpcMonitor, IDisposable
{
    private readonly ILogger<Monitor> logger;
    private readonly Dictionary<string, RegisteredSymbol> registeredSymbols = new(StringComparer.OrdinalIgnoreCase);
    private readonly IDisposable sessionStream;
    private readonly IOpcSymbolHandler symbolHandler;
    private readonly Subject<IMonitorResult> symbolStream = new();
    private readonly IOpcTypeConverter typeConverter;
    private bool disposedValue;
    private Subscription subscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="Monitor"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
    /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public Monitor(IOpcPlcConnection connection, IOpcSymbolHandler symbolHandler, IOpcTypeConverter typeConverter, ILogger<Monitor> logger)
    {
        this.symbolHandler = symbolHandler;
        this.typeConverter = typeConverter;
        this.logger = logger;
        subscription = new Subscription()
        {
            DisplayName = null,
            PublishingEnabled = true,
            PublishingInterval = 50,
            Priority = 1,
            KeepAliveCount = 10,
            LifetimeCount = 100,
            MaxNotificationsPerPublish = 1000,
            TimestampsToReturn = TimestampsToReturn.Both,
        };

        sessionStream = connection.SessionStream
            .Where(x => x.IsConnected)
            .Select(x => x.Value)
            .Select(x => Observable.FromAsync(() => OnSessionConnectedAsync(x)))
            .Concat()
            .Subscribe();
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
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000)
    {
        var needApply = false;
        foreach (var name in ioNames)
        {
            if (RegisterIOInternal(name, updateInterval))
            {
                needApply = true;
            }
        }

        if (needApply)
        {
            _ = ApplyChangesAsync();
        }
    }

    /// <inheritdoc/>
    public void RegisterIO(string ioName, int updateInterval = 1000)
    {
        if (RegisterIOInternal(ioName, updateInterval))
        {
            _ = ApplyChangesAsync();
        }
    }

    /// <inheritdoc/>
    public void UnregisterIO(IEnumerable<string> ioNames)
    {
        foreach (var name in ioNames)
        {
            UnregisterIOInternal(name);
        }

        _ = ApplyChangesAsync();
    }

    /// <inheritdoc/>
    public void UnregisterIO(string ioName)
    {
        UnregisterIOInternal(ioName);
        _ = ApplyChangesAsync();
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
                symbolStream.Dispose();
                subscription.Dispose();
                sessionStream.Dispose();
                registeredSymbols?.Clear();
            }

            disposedValue = true;
        }
    }

    private async Task ApplyChangesAsync()
    {
        if (subscription.Created && subscription.Session.Connected)
        {
            await subscription.ApplyChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    private async Task OnSessionConnectedAsync(ISession session)
    {
        LogUpdatingSubscriptions();
        foreach (var keyValue in registeredSymbols)
        {
            var value = keyValue.Value;
            value.UpdateMonitoredItem(symbolHandler);
        }

        if (session.Subscriptions.Contains(subscription))
        {
            await subscription.ApplyChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
        else
        {
            if (subscription.Created)
            {
                var previous = subscription;
                subscription = new Subscription(subscription, copyEventHandlers: true);
                previous.Dispose();
            }

            _ = session.AddSubscription(subscription);
            await subscription.CreateAsync(CancellationToken.None).ConfigureAwait(false);
            await subscription.ApplyChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    private bool RegisterIOInternal(string name, int updateInterval)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is null or whitespace", nameof(name));
        }

        if (registeredSymbols.TryGetValue(name, out var registeredSymbol))
        {
            registeredSymbol.Subscriptions++;
            return false;
        }

        LogVariableRegistered(name);
        registeredSymbol = RegisteredSymbol.Create(name, updateInterval, symbolStream, typeConverter);
        registeredSymbols.Add(name, registeredSymbol);
        subscription.AddItem(registeredSymbol.MonitoredItem);
        registeredSymbol.UpdateMonitoredItem(symbolHandler);
        return true;
    }

    private void UnregisterIOInternal(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is null or whitespace", nameof(name));
        }

        if (!registeredSymbols.TryGetValue(name, out var registeredSymbol))
        {
            LogVariableNotRegistered(name);
            return;
        }

        registeredSymbol.Subscriptions--;
        if (registeredSymbol.Subscriptions > 0)
        {
            LogVariableStillHasSubscriptions(name, registeredSymbol.Subscriptions);
            return;
        }

        subscription.RemoveItem(registeredSymbol.MonitoredItem);
        _ = registeredSymbols.Remove(name);
        registeredSymbol.Dispose();
        LogVariableUnregistered(name);
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
                QueueSize = 0,
                DiscardOldest = true,
                DisplayName = name,
            };

            var stream = Observable.FromEventPattern<MonitoredItemNotificationEventHandler, MonitoredItem, MonitoredItemNotificationEventArgs>(
                h => monitoredItem.Notification += h,
                h => monitoredItem.Notification -= h)
                .Select(x =>
                {
                    if (x.Sender?.Handle is IOpcSymbolInfo s
                    && x.EventArgs.NotificationValue is MonitoredItemNotification monitoredItemNotification)
                    {
                        return new MonitorResult(s.Name, typeConverter.Convert(s.Name, monitoredItemNotification.Value.Value));
                    }

                    return null;
                })
                .WhereNotNull()
                .Subscribe(symbolStream);

            return new RegisteredSymbol(name, monitoredItem, stream);
        }

        public void Dispose()
            => Dispose(disposing: true);

        public void UpdateMonitoredItem(IOpcSymbolHandler symbolHandler)
        {
            if (symbolHandler.TryGetSymbolInfo(name, out var symbolInfo))
            {
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
                    symbolStream.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}
