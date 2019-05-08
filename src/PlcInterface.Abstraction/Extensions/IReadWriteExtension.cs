using System;

namespace PlcInterface.Abstraction.Extensions
{
    public static class IReadWriteExtension
    {
        /// <summary>
        /// Read a tag untill we get the expected value, or a timeout happens
        /// </summary>
        /// <param name="tag">tag name to monitor</param>
        /// <param name="value">the value to wait for</param>
        /// <param name="timeout">time befor timeout exception is thrown</param>
        public static void WaitForValue<T>(this IReadWrite readWrite, string tag, T value, TimeSpan timeout)
        {
            var source = new System.Threading.CancellationTokenSource(timeout);
            var token = source.Token;
            while (!readWrite.Read<T>(tag).Equals(value))
            {
                if (token.IsCancellationRequested)
                {
                    throw new TimeoutException($"Couldnt get a proper response from the PLC in {timeout.TotalSeconds} seconds");
                }
            }
        }
    }
}