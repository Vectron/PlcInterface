using System.Globalization;

namespace PlcInterface;

/// <summary>
/// Extension methods for <see cref="IReadWrite"/>.
/// </summary>
public static class IReadWriteExtension
{
    /// <summary>
    /// Read a tag until we get the expected value, or a timeout happens.
    /// </summary>
    /// <typeparam name="T">The type of the tag.</typeparam>
    /// <param name="readWrite">A <see cref="IReadWrite"/> implementation.</param>
    /// <param name="tag">Tag name to monitor.</param>
    /// <param name="filterValue">The value to wait for.</param>
    /// <param name="timeout">Time before <see cref="TimeoutException"/> is thrown.</param>
    /// <exception cref="TimeoutException">If no value is returned after <paramref name="timeout"/>.</exception>
    public static void WaitForValue<T>(this IReadWrite readWrite, string tag, T filterValue, TimeSpan timeout)
    {
        using var source = new CancellationTokenSource(timeout);
        var token = source.Token;
        while (!token.IsCancellationRequested)
        {
            var value = readWrite.Read<T>(tag);

            // We use object.Equals here to prevent null pointer exceptions, but still allow null as a filter value.
            if (Equals(value, filterValue))
            {
                return;
            }
        }

        throw new TimeoutException(string.Create(CultureInfo.InvariantCulture, $"Couldn't get a proper response from the PLC in {timeout.TotalSeconds} seconds"));
    }

    /// <summary>
    /// Read a tag until we get the expected value, or a timeout happens.
    /// </summary>
    /// <typeparam name="T">The type of the tag.</typeparam>
    /// <param name="readWrite">A <see cref="IReadWrite"/> implementation.</param>
    /// <param name="tag">Tag name to monitor.</param>
    /// <param name="filterValue">The value to wait for.</param>
    /// <param name="timeout">Time before <see cref="TimeoutException"/> is thrown.</param>
    /// <exception cref="TimeoutException">If no value is returned after <paramref name="timeout"/>.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task WaitForValueAsync<T>(this IReadWrite readWrite, string tag, T filterValue, TimeSpan timeout)
    {
        using var source = new CancellationTokenSource(timeout);
        var token = source.Token;

        while (!token.IsCancellationRequested)
        {
            var value = await readWrite.ReadAsync<T>(tag).ConfigureAwait(false);

            // We use object.Equals here to prevent null pointer exceptions, but still allow null as a filter value.
            if (Equals(value, filterValue))
            {
                return;
            }
        }

        throw new TimeoutException(string.Create(CultureInfo.InvariantCulture, $"Couldn't get a proper response from the PLC in {timeout.TotalSeconds} seconds"));
    }
}
