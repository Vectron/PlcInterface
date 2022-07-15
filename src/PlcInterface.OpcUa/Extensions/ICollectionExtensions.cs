using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace PlcInterface.OpcUa;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source as IReadOnlyCollection<T> ?? new ReadOnlyCollectionAdapter<T>(source);
    }

    private sealed class ReadOnlyCollectionAdapter<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> source;

        public ReadOnlyCollectionAdapter(ICollection<T> source) => this.source = source;

        public int Count => source.Count;

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}