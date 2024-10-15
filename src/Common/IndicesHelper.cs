using System.Globalization;

namespace PlcInterface;

/// <summary>
/// Extension methods for <see cref="Array"/>.
/// </summary>
internal static class IndicesHelper
{
    /// <summary>
    /// Iterate over all array indices.
    /// </summary>
    /// <remarks>only one array will be made and updated every iteration.</remarks>
    /// <param name="array">The <see cref="Array"/> to get indices from.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> with the array indices.</returns>
    public static IEnumerable<int[]> GetIndices(Array array)
    {
        var indices = new int[array.Rank];
        for (var dimension = 0; dimension < array.Rank; dimension++)
        {
            indices[dimension] = array.GetLowerBound(dimension);
        }

        yield return indices;
        for (var i = 0; i < array.Length - 1; i++)
        {
            indices[^1]++;
            for (var dimension = indices.Length - 1; dimension >= 0; dimension--)
            {
                var length = array.GetLength(dimension);
                var lowerBound = array.GetLowerBound(dimension);
                if (indices[dimension] == length + lowerBound)
                {
                    indices[dimension - 1]++;
                    indices[dimension] = lowerBound;
                }
            }

            yield return indices;
        }
    }

    /// <summary>
    /// Gets the array indices from the given <see cref="string"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to filter the indices from.</param>
    /// <returns>An <see cref="Array"/> containing the indices of every array dimension.</returns>
    public static int[] GetIndices(string value)
        => GetIndices(value.AsSpan());

    /// <summary>
    /// Gets the array indices from the given <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to filter the indices from.</param>
    /// <returns>An <see cref="Array"/> containing the indices of every array dimension.</returns>
    public static int[] GetIndices(ReadOnlySpan<char> span)
    {
        var sliced = span[(span.IndexOf('[') + 1)..];
        var end = sliced.IndexOfAny(']', ',');
        var dimensions = new List<int>();

        while (end != -1)
        {
            var value = sliced[..end];
            var dimension = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
            dimensions.Add(dimension);
            if (sliced[end] == ']')
            {
                break;
            }

            sliced = sliced[(end + 1)..];
            end = sliced.IndexOfAny(']', ',');
        }

        return [.. dimensions];
    }
}
