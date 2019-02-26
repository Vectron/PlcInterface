using System;
using System.Reactive.Linq;

namespace PlcInterface
{
    public static class IMonitorExtension
    {
        public static IDisposable SubscribeIO<T>(this IMonitor monitor, string ioName, Action<T> action, int updateInterval = 1000)
            => monitor
                .SubscribeIO<T>(ioName, updateInterval)
                .Subscribe(action);

        public static IDisposable SubscribeIO<T>(this IMonitor monitor, string ioName, T filterValue, Action action, int updateInterval = 1000)
            => monitor
                .SubscribeIO(ioName, filterValue, updateInterval)
                .Subscribe(_ => action());

        public static IObservable<T> SubscribeIO<T>(this IMonitor monitor, string ioName, T filterValue, int updateInterval = 1000)
            => monitor
                .SubscribeIO<T>(ioName, updateInterval)
                .Where(x => x.Equals(filterValue));

        public static IObservable<T> SubscribeIO<T>(this IMonitor monitor, string ioName, int updateInterval)
        {
            monitor.RegisterIO(ioName, updateInterval);
            return monitor
                .SymbolStream
                .Where(x => x.Name.ToLowerInvariant() == ioName.ToLowerInvariant())
                .Select(x => x.Value)
                .Cast<T>()
                .Finally(() => monitor.UnregisterIO(ioName));
        }
    }
}