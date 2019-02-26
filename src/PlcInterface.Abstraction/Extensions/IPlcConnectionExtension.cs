using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace PlcInterface
{
    public static class IPlcConnectionExtension
    {
        /// <summary>
        /// Gets the PLC Connection.
        /// Throws a TimeoutException if no client is returned in 2 seconds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plcConnection"></param>
        /// <returns></returns>
        public static T GetConnectedClient<T>(this IPlcConnection<T> plcConnection)
            => plcConnection.GetConnectedClient(TimeSpan.FromSeconds(2));

        /// <summary>
        /// Gets the PLC Connection.
        /// Throws a TimeoutException if no client is returned in timeout
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plcConnection"></param>
        /// <returns></returns>
        public static T GetConnectedClient<T>(this IPlcConnection<T> plcConnection, TimeSpan timeout)
            => plcConnection
                .SessionStream
                .FirstAsync(x => x.IsConnected)
                .Timeout(timeout)
                .ToTask()
                .GetAwaiter()
                .GetResult()
                .Value;
    }
}