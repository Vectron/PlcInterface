using Microsoft.Extensions.Logging;
using PlcInterface.Ads.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    public class Monitor : IMonitor
    {
        private readonly IPlcConnection<IAdsConnection> connection;
        private readonly ILogger<Monitor> logger;
        private readonly Dictionary<string, DisposableMonitorItem> streams = new Dictionary<string, DisposableMonitorItem>();
        private readonly ISymbolHandler symbolHandler;
        private Subject<IMonitorResult> symbolStream = new Subject<IMonitorResult>();

        public Monitor(IPlcConnection<IAdsConnection> connection, ISymbolHandler symbolHandler, ILogger<Monitor> logger)
        {
            this.connection = connection;
            this.symbolHandler = symbolHandler;
            this.logger = logger;
        }

        public IObservable<IMonitorResult> SymbolStream
            => symbolStream.AsObservable();

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
                if (streams.TryGetValue(ioName, out DisposableMonitorItem disposableMonitorItem))
                {
                    disposableMonitorItem.Subscriptions += 1;
                    continue;
                }

                streams.Add(ioName, monitoredItem);
                monitoredItem.Subscriptions += 1;
            }
        }

        public void RegisterIO(string ioName, int updateInterval = 1000)
        {
            if (streams.TryGetValue(ioName, out DisposableMonitorItem disposableMonitorItem))
            {
                disposableMonitorItem.Subscriptions += 1;
                return;
            }

            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            symbolInfo.ThrowIfNull(nameof(symbolInfo));
            var adsSymbol = symbolInfo?.Symbol as IValueSymbol;
            var subscription = adsSymbol
                   .WhenValueChanged()
                   .Select(x => new MonitorResult(ioName, x))
                   .Subscribe(x => symbolStream.OnNext(x));

            streams.Add(ioName, new DisposableMonitorItem(subscription));
        }

        public void UnregisterIO(IEnumerable<string> ioNames)
        {
            foreach (var ioName in ioNames)
            {
                UnregisterIO(ioName);
            }
        }

        public void UnregisterIO(string ioName)
        {
            if (!streams.TryGetValue(ioName, out DisposableMonitorItem disposableMonitorItem))
            {
                return;
            }

            disposableMonitorItem.Subscriptions -= 1;
            if (disposableMonitorItem.Subscriptions != 0)
            {
                return;
            }

            streams.Remove(ioName);
            disposableMonitorItem.Dispose();
        }
    }
}