namespace System
{
    /// <summary>
    /// Extension methods for all classes and structs.
    /// </summary>
    internal static class ObjectExtension
    {
        /// <summary>
        /// Throw a exception when the type is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to check.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <param name="parameterName">The name of the parameter to check.</param>
        /// <returns>The non <see langword="null"/> <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="obj"/> is <see langword="null"/>.</exception>
        public static T ThrowIfNull<T>(this T? obj, string parameterName = "")
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return obj;
        }
    }
}