namespace PlcInterface
{
    /// <summary>
    /// A <see cref="IConnected{T}"/> implementation.
    /// </summary>
    /// <typeparam name="T">The type that is connected.</typeparam>
    public class Connected<T> : IConnected<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connected{T}"/> class.
        /// </summary>
        /// <param name="value">A <typeparamref name="T"/> containg the connection.</param>
        internal Connected(T value)
        {
            Value = value;
            IsConnected = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connected{T}"/> class.
        /// </summary>
        internal Connected()
        {
        }

        /// <inheritdoc/>
        public bool IsConnected
        {
            get;
        }

        /// <inheritdoc/>
        public T? Value
        {
            get;
        }
    }
}