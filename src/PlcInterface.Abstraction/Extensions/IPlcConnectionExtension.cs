using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace PlcInterface;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for <see cref="IPlcConnection{T}"/>.
/// </summary>
public static class IPlcConnectionExtension
{
    /// <summary>
    /// Gets the PLC Connection.
    /// </summary>
    /// <typeparam name="T">The connection type to return.</typeparam>
    /// <param name="plcConnection">The <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <returns>The gotten <typeparamref name="T"/>.</returns>
    /// <exception cref="TimeoutException">If no client is returned in 2 seconds.</exception>
    public static T GetConnectedClient<T>(this IPlcConnection<T> plcConnection)
        => plcConnection.GetConnectedClient(TimeSpan.FromSeconds(2));

    /// <summary>
    /// Gets the PLC Connection.
    /// </summary>
    /// <typeparam name="T">The connection type to return.</typeparam>
    /// <param name="plcConnection">The <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> indicating how long to wait for getting the connection.</param>
    /// <returns>The gotten <typeparamref name="T"/>.</returns>
    /// <exception cref="TimeoutException">If no client is returned after <paramref name="timeout"/>.</exception>
    public static T GetConnectedClient<T>(this IPlcConnection<T> plcConnection, TimeSpan timeout)
        => plcConnection
            .SessionStream
            .FirstAsync(x => x.IsConnected)
            .Timeout(timeout)
            .ToTask()
            .GetAwaiter()
            .GetResult()
            .Value;

    /// <summary>
    /// Gets the PLC Connection asynchronous.
    /// </summary>
    /// <typeparam name="T">The connection type to return.</typeparam>
    /// <param name="plcConnection">The <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <returns>The gotten <typeparamref name="T"/>.</returns>
    /// <exception cref="TimeoutException">If no client is returned in 2 seconds.</exception>
    public static Task<T> GetConnectedClientAsync<T>(this IPlcConnection<T> plcConnection)
        => plcConnection.GetConnectedClientAsync(TimeSpan.FromSeconds(2));

    /// <summary>
    /// Gets the PLC Connection asynchronous.
    /// </summary>
    /// <typeparam name="T">The connection type to return.</typeparam>
    /// <param name="plcConnection">The <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> indicating how long to wait for getting the connection.</param>
    /// <returns>The gotten <typeparamref name="T"/>.</returns>
    /// <exception cref="TimeoutException">If no client is returned after <paramref name="timeout"/>.</exception>
    public static async Task<T> GetConnectedClientAsync<T>(this IPlcConnection<T> plcConnection, TimeSpan timeout)
    {
        var connection = await plcConnection.SessionStream.FirstAsync(x => x.IsConnected).Timeout(timeout);
        return connection.Value;
    }
}