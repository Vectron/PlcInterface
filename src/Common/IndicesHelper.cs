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
        indices[^1]--;

        while (array.IncrementIndices(ref indices))
        {
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

    /// <summary>
    /// A helper function for incrementing indices of a multi dimensional <see cref="Array"/>.
    /// </summary>
    /// <param name="array">The array being iterated.</param>
    /// <param name="indices">The indices array that has to be incremented.</param>
    /// <returns><see langword="true"/> if the new indices is valid, else <see langword="false"/>.</returns>
    private static bool IncrementIndices(this Array array, ref int[] indices)
    {
        for (var i = array.Rank - 1; i >= 0; i--)
        {
            indices[i]++;
            if (indices[i] <= array.GetUpperBound(i))
            {
                return true;
            }

            if (i != 0)
            {
                indices[i] = 0;
            }
        }

        return false;
    }
}
