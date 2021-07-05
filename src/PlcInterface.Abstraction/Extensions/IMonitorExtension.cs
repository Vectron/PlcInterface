using System;
using System.Reactive.Linq;

namespace PlcInterface
{
    /// <summary>
    /// Extension methods for <see cref="IMonitor"/>.
    /// </summary>
    public static class IMonitorExtension
    {
        /// <summary>
        /// Subscribe to the IO stream. with a specific type.
        /// </summary>
        /// <typeparam name="T">The type the stream has to return.</typeparam>
        /// <param name="monitor">A <see cref="IMonitor"/> implementation.</param>
        /// <param name="ioName">The name of the IO to monitor.</param>
        /// <param name="action">The <see cref="Action"/> to execute on update.</param>
        /// <param name="updateInterval">The update interval for this subscription.</param>
        /// <returns>A <see cref="IDisposable"/> to unsubscribe from the stream.</returns>
        public static IDisposable SubscribeIO<T>(this IMonitor monitor, string ioName, Action<T> action, int updateInterval = 1000)
            => monitor
                .SubscribeIO<T>(ioName, updateInterval)
                .Subscribe(action);

        /// <summary>
        /// Subscribe to the IO stream. with a specific type and filter the value.
        /// </summary>
        /// <typeparam name="T">The type the stream has to return.</typeparam>
        /// <param name="monitor">A <see cref="IMonitor"/> implementation.</param>
        /// <param name="ioName">The name of the IO to monitor.</param>
        /// <param name="filterValue">Filter the stream for this value.</param>
        /// <param name="action">The <see cref="Action"/> to execute on update.</param>
        /// <param name="updateInterval">The update interval for this subscription.</param>
        /// <returns>A <see cref="IDisposable"/> to unsubscribe from the stream.</returns>
        public static IDisposable SubscribeIO<T>(this IMonitor monitor, string ioName, T filterValue, Action action, int updateInterval = 1000)
            => monitor
                .SubscribeIO(ioName, filterValue, updateInterval)
                .Subscribe(_ => action());

        /// <summary>
        /// Subscribe to the IO stream. with a specific type and filter the value.
        /// </summary>
        /// <typeparam name="T">The type the stream has to return.</typeparam>
        /// <param name="monitor">A <see cref="IMonitor"/> implementation.</param>
        /// <param name="ioName">The name of the IO to monitor.</param>
        /// <param name="filterValue">Filter the stream for this value.</param>
        /// <param name="updateInterval">The update interval for this subscription.</param>
        /// <returns>A <see cref="IDisposable"/> to unsubscribe from the stream.</returns>
        public static IObservable<T> SubscribeIO<T>(this IMonitor monitor, string ioName, T filterValue, int updateInterval = 1000)
            => monitor
                .SubscribeIO<T>(ioName, updateInterval)
                .Where(x => x != null && x.Equals(filterValue));

        /// <summary>
        /// Subscribe to the IO stream. with a specific type and filter the value.
        /// </summary>
        /// <typeparam name="T">The type the stream has to return.</typeparam>
        /// <param name="monitor">A <see cref="IMonitor"/> implementation.</param>
        /// <param name="ioName">The name of the IO to monitor.</param>
        /// <param name="updateInterval">The update interval for this subscription.</param>
        /// <returns>A <see cref="IDisposable"/> to unsubscribe from the stream.</returns>
        public static IObservable<T> SubscribeIO<T>(this IMonitor monitor, string ioName, int updateInterval)
        {
            monitor.RegisterIO(ioName, updateInterval);
            return monitor
                .SymbolStream
                .Where(x => string.Equals(x.Name, ioName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value)
                .Cast<T>()
                .Finally(() => monitor.UnregisterIO(ioName));
        }
    }
}