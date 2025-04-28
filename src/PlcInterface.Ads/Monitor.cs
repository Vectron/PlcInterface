using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads.Reactive;

namespace PlcInterface.Ads;

/// <summary>
/// A implementation of <see cref="IMonitor"/>.
/// </summary>
public partial class Monitor : IAdsMonitor, IDisposable
{
    private readonly ILogger logger;
    private readonly ConcurrentDictionary<string, SubscriptionCounter> streams = new(StringComparer.OrdinalIgnoreCase);
    private readonly IAdsSymbolHandler symbolHandler;
    private readonly Subject<IMonitorResult> symbolStream = new();
    private readonly IAdsTypeConverter typeConverter;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Monitor"/> class.
    /// </summary>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
    /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public Monitor(IAdsSymbolHandler symbolHandler, IAdsTypeConverter typeConverter, ILogger<Monitor> logger)
    {
        this.symbolHandler = symbolHandler;
        this.typeConverter = typeConverter;
        this.logger = logger;
        symbolHandler.SymbolsUpdated += SymbolHandlerSymbolsUpdated;
    }

    /// <inheritdoc/>
    public IObservable<IMonitorResult> SymbolStream
        => symbolStream.AsObservable();

    /// <inheritdoc/>
    public ITypeConverter TypeConverter
        => typeConverter;

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000)
    {
        foreach (var ioName in ioNames)
        {
            RegisterIO(ioName, updateInterval);
        }
    }

    /// <inheritdoc/>
    public void RegisterIO(string ioName, int updateInterval = 1000)
    {
        var subscriptionCounter = streams.GetOrAdd(ioName, (ioName, monitor) => new SubscriptionCounter(ioName, monitor), this);
        subscriptionCounter.IncrementSubscription();
        LogVariableRegistered(ioName, updateInterval);
    }

    /// <inheritdoc/>
    public void UnregisterIO(IEnumerable<string> ioNames)
    {
        foreach (var ioName in ioNames)
        {
            UnregisterIO(ioName);
        }
    }

    /// <inheritdoc/>
    public void UnregisterIO(string ioName)
    {
        if (!streams.TryGetValue(ioName, out var subscriptionCounter))
        {
            LogVariableNotRegistered(ioName);
            return;
        }

        subscriptionCounter.DecrementSubscription();
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
                symbolHandler.SymbolsUpdated -= SymbolHandlerSymbolsUpdated;
                foreach (var (ioName, counter) in streams)
                {
                    counter.Dispose();
                }

                streams.Clear();
            }

            disposedValue = true;
        }
    }

    private IDisposable? CreateStream(string name)
    {
        if (!symbolHandler.TryGetSymbolInfo(name, out var symbolInfo))
        {
            return null;
        }

        var valueSymbol = symbolInfo.Symbol.CastAndValidate();
        if (valueSymbol.Connection == null)
        {
            return null;
        }

        if (!valueSymbol.Connection.IsConnected)
        {
            return null;
        }

        return valueSymbol
            .WhenValueChangedAnnotated()
            .Where(v => v.Symbol.DataType != null)
            .Select(v => typeConverter.Convert(v.Value, v.Symbol.DataType!))
            .Where(x => x != null)
            .Select(x => new MonitorResult(name, x!))
            .Subscribe(symbolStream);
    }

    private void SymbolHandlerSymbolsUpdated(object? sender, EventArgs e)
    {
        LogUpdatingSubscriptions();
        foreach (var (_, subscriptionCounter) in streams)
        {
            subscriptionCounter.RefreshSubscription();
        }
    }

    private sealed record class SubscriptionCounter(string IoName, Monitor Monitor) : IDisposable
    {
        private int subscriptions;
#if NET9_0_OR_GREATER
        private readonly Lock lockObj = new();
#else
        private readonly object lockObj = new();
#endif
        private IDisposable? stream;
        private bool disposedValue;

        public void DecrementSubscription()
        {
            lock (lockObj)
            {
                subscriptions--;
                subscriptions = Math.Clamp(subscriptions, 0, int.MaxValue);
                if (subscriptions > 0)
                {
                    Monitor.LogVariableStillHasSubscriptions(IoName, subscriptions);
                    return;
                }

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                    Monitor.LogVariableUnregistered(IoName);
                }
            }
        }

        public void Dispose() => Dispose(disposing: true);

        public void IncrementSubscription()
        {
            lock (lockObj)
            {
                subscriptions++;
                subscriptions = Math.Clamp(subscriptions, 0, int.MaxValue);
                if (subscriptions != 1)
                {
                    return;
                }

                stream?.Dispose();
                stream = Monitor.CreateStream(IoName);
            }
        }

        public void RefreshSubscription()
        {
            lock (lockObj)
            {
                if (subscriptions != 0)
                {
                    stream?.Dispose();
                    stream = Monitor.CreateStream(IoName);
                }
            }
        }

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
