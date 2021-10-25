using System;
using System.Collections.Generic;
using System.Globalization;

namespace PlcInterface.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the array indices from the given <see cref="string"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to filter the indices from.</param>
    /// <returns>An <see cref="Array"/> containing the indices of every array dimension.</returns>
    public static int[] GetIndices(this string value)
        => value.AsSpan().GetIndices();

    /// <summary>
    /// Gets the array indices from the given <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to filter the indices from.</param>
    /// <returns>An <see cref="Array"/> containing the indices of every array dimension.</returns>
    public static int[] GetIndices(this ReadOnlySpan<char> span)
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
}