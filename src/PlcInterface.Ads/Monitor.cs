﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    /// <summary>
    /// A implementation of <see cref="IMonitor"/>.
    /// </summary>
    public class Monitor : IMonitor
    {
        private readonly IPlcConnection<IAdsConnection> connection;
        private readonly ILogger logger;
        private readonly Dictionary<string, DisposableMonitorItem> streams = new(StringComparer.OrdinalIgnoreCase);
        private readonly ISymbolHandler symbolHandler;
        private readonly Subject<IMonitorResult> symbolStream = new();
        private readonly IAdsTypeConverter typeConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
        /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
        /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
        public Monitor(IPlcConnection<IAdsConnection> connection, ISymbolHandler symbolHandler, IAdsTypeConverter typeConverter, ILogger<Monitor> logger)
        {
            this.connection = connection;
            this.symbolHandler = symbolHandler;
            this.typeConverter = typeConverter;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public IObservable<IMonitorResult> SymbolStream
            => symbolStream.AsObservable();

        /// <inheritdoc/>
        public ITypeConverter TypeConverter
            => typeConverter;

        /// <inheritdoc/>
        public void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000)
        {
            var symbols = ioNames
                .Where(x => !streams.ContainsKey(x))
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Cast<SymbolInfo>()
                .Select(x => x.Symbol)
                .Cast<IValueSymbol>();

            var subscription = connection.GetConnectedClient()
                    .WhenValueChangedAnnotated(symbols)
                    .Select(x => new MonitorResult(x.Symbol.InstancePath, x.Value))
                    .Subscribe(x => symbolStream.OnNext(x));

            var monitoredItem = new DisposableMonitorItem(subscription);

            foreach (var ioName in ioNames)
            {
                if (streams.TryGetValue(ioName, out var disposableMonitorItem))
                {
                    disposableMonitorItem.Subscriptions += 1;
                    continue;
                }

                streams.Add(ioName, monitoredItem);
                monitoredItem.Subscriptions += 1;
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

            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            _ = symbolInfo.ThrowIfNull(nameof(symbolInfo));
            streams.Add(ioName, DisposableMonitorItem.Create(symbolStream, symbolInfo!, typeConverter));
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
                return;
            }

            disposableMonitorItem.Subscriptions -= 1;
            if (disposableMonitorItem.Subscriptions != 0)
            {
                return;
            }

            if (!streams.Remove(ioName))
            {
                logger.LogError("Unable to unregister {0}", ioName);
            }

            disposableMonitorItem.Dispose();
        }
    }
}