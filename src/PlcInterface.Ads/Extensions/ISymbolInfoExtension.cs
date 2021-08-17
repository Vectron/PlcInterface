using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PlcInterface.Ads;

namespace PlcInterface
{
    /// <summary>
    /// Extension methods for <see cref="ISymbolInfo"/>.
    /// </summary>
    internal static class ISymbolInfoExtension
    {
        /// <summary>
        /// Convert the <see cref="ISymbolInfo"/> to <see cref="SymbolInfo"/> and throw a exception if the conversion fails.
        /// </summary>
        /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to change.</param>
        /// <returns>The cast object.</returns>
        /// <exception cref="SymbolException">If the cast fails.</exception>
        public static SymbolInfo CastAndValidate(this ISymbolInfo symbolInfo)
        {
            if (symbolInfo is not SymbolInfo symbol)
            {
                throw new SymbolException($"Symbol is not a {typeof(SymbolInfo)}");
            }

            return symbol;
        }

        /// <summary>
        /// Flatten the type hierarchy.
        /// </summary>
        /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to flatten.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
        /// <param name="value">The <see cref="object"/> to flatten.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of all child symbols and their value.</returns>
        public static IEnumerable<(ISymbolInfo SymbolInfo, object Value)> FlattenWithValue(this ISymbolInfo symbolInfo, ISymbolHandler symbolHandler, object value)
        {
            if (symbolInfo.ChildSymbols.Count == 0)
            {
                return (symbolInfo, value).Yield();
            }

            return symbolInfo.ChildSymbols
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .SelectMany(x =>
                {
                    object childValue;
                    if (value is Array array)
                    {
                        var indices = x.Name.AsSpan().GetIndices();
                        childValue = array.GetValue(indices);
                    }
                    else
                    {
                        var type = value.GetType();
                        var property = type.GetProperty(x.ShortName);
                        childValue = property.GetValue(value);
                    }

                    return x.FlattenWithValue(symbolHandler, childValue);
                });
        }

        /// <summary>
        /// Gets the array indices from the given <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to filter the indices from.</param>
        /// <returns>An <see cref="Array"/> containing the indices of every array dimension.</returns>
        private static int[] GetIndices(this ReadOnlySpan<char> span)
        {
            var sliced = span.Slice(span.IndexOf('[') + 1);
            var end = sliced.IndexOfAny(']', ',');
            var dimensions = new List<int>();

            while (end != -1)
            {
                var value = sliced.Slice(0, end);
                var dimension = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
                dimensions.Add(dimension);
                if (sliced[end] == ']')
                {
                    break;
                }

                sliced = sliced.Slice(end + 1);
                end = sliced.IndexOfAny(']', ',');
            }

            return dimensions.ToArray();
        }

        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        private static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}