using System;
using System.Collections;
using System.Collections.Generic;

namespace PlcInterface.OpcUa;

/// <summary>
/// Extension methods for <see cref="ICollection{T}"/>.
/// </summary>
internal static class ICollectionExtensions
{
    /// <summary>
    /// Convert a <see cref="ICollection{T}"/> in a <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in this collection.</typeparam>
    /// <param name="source">The source <see cref="ICollection{T}"/>.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/>.</returns>
    public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source as IReadOnlyCollection<T> ?? new ReadOnlyCollectionAdapter<T>(source);
    }

    private sealed class ReadOnlyCollectionAdapter<T>(ICollection<T> source) : IReadOnlyCollection<T>
    {
        public int Count => source.Count;

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
