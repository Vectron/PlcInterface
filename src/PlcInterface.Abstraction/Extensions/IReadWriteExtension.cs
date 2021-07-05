using System;

namespace PlcInterface
{
    /// <summary>
    /// Extension methods for <see cref="IReadWrite"/>.
    /// </summary>
    public static class IReadWriteExtension
    {
        /// <summary>
        /// Read a tag untill we get the expected value, or a timeout happens.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="readWrite">A <see cref="IReadWrite"/> implementation.</param>
        /// <param name="tag">Tag name to monitor.</param>
        /// <param name="filterValue">The value to wait for.</param>
        /// <param name="timeout">Time befor <see cref="TimeoutException"/> is thrown.</param>
        /// <exception cref="TimeoutException">If no value is returned after <paramref name="timeout"/>.</exception>
        public static void WaitForValue<T>(this IReadWrite readWrite, string tag, T filterValue, TimeSpan timeout)
        {
            using var source = new System.Threading.CancellationTokenSource(timeout);
            var token = source.Token;
            while (!token.IsCancellationRequested)
            {
                var value = readWrite.Read<T>(tag);
                if (value != null && value.Equals(filterValue))
                {
                    return;
                }
            }

            throw new TimeoutException(FormattableString.Invariant($"Couldnt get a proper response from the PLC in {timeout.TotalSeconds} seconds"));
        }
    }
}