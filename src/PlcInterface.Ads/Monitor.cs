using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace PlcInterface.Ads;

/// <summary>
/// A implementation of <see cref="IMonitor"/>.
/// </summary>
public class Monitor : IAdsMonitor, IDisposable
{
    private readonly ILogger logger;
    private readonly IDisposable sessionStream;
    private readonly Dictionary<string, DisposableMonitorItem> streams = new(StringComparer.OrdinalIgnoreCase);
    private readonly IAdsSymbolHandler symbolHandler;
    private readonly Subject<IMonitorResult> symbolStream = new();
    private readonly IAdsTypeConverter typeConverter;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Monitor"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
    /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public Monitor(IAdsPlcConnection connection, IAdsSymbolHandler symbolHandler, IAdsTypeConverter typeConverter, ILogger<Monitor> logger)
    {
        this.symbolHandler = symbolHandler;
        this.typeConverter = typeConverter;
        this.logger = logger;

        sessionStream = connection.SessionStream.Where(x => x.IsConnected).Subscribe(x =>
        {
            logger.LogDebug("Updating all subscriptions.");
            foreach (var keyValue in streams)
            {
                keyValue.Value.Update(symbolHandler, symbolStream, typeConverter);
            }
        });
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
        if (streams.TryGetValue(ioName, out var disposableMonitorItem))
        {
            disposableMonitorItem.Subscriptions += 1;
            return;
        }

        disposableMonitorItem = DisposableMonitorItem.Create(ioName);
        disposableMonitorItem.Update(symbolHandler, symbolStream, typeConverter);
        streams.Add(ioName, disposableMonitorItem);
        logger.LogDebug("Registered IO {IOName} with {UpdateInterval}", ioName, updateInterval);
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
        if (!streams.TryGetValue(ioName, out var disposableMonitorItem))
        {
            logger.LogDebug("{IOName} was not registered.", ioName);
            return;
        }

        disposableMonitorItem.Subscriptions -= 1;
        if (disposableMonitorItem.Subscriptions != 0)
        {
            logger.LogDebug("{IOName} has {Subscriptions} subscriptions left.", ioName, disposableMonitorItem.Subscriptions);
            return;
        }

        _ = streams.Remove(ioName);
        disposableMonitorItem.Dispose();
        logger.LogDebug("{IOName} subscription remove.", ioName);
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
                sessionStream.Dispose();
            }

            disposedValue = true;
        }
    }
}