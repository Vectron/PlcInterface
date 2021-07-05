namespace PlcInterface
{
    /// <summary>
    /// Helpers for creating <see cref="IConnected{T}"/>.
    /// </summary>
    public static class Connected
    {
        /// <summary>
        /// Create a not connected item.
        /// </summary>
        /// <typeparam name="T">The type that is connected.</typeparam>
        /// <returns>The <see cref="IConnected{T}"/> representing the connection.</returns>
        public static IConnected<T> No<T>()
            => new Connected<T>();

        /// <summary>
        /// Create a connected item.
        /// </summary>
        /// <typeparam name="T">The type that is connected.</typeparam>
        /// <param name="value">The new <typeparamref name="T"/> that is connected.</param>
        /// <returns>The <see cref="IConnected{T}"/> representing the connection.</returns>
        public static IConnected<T> Yes<T>(T value)
            => new Connected<T>(value);
    }
}