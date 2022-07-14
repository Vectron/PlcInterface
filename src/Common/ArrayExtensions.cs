using System;
using System.Collections.Generic;

namespace PlcInterface.Extensions;

/// <summary>
/// Extension methods for <see cref="Array"/>.
/// </summary>
internal static class ArrayExtensions
{
    /// <summary>
    /// Itterate over all array indices.
    /// </summary>
    /// <remarks>only one array will be made and updated every itteration.</remarks>
    /// <param name="array">The <see cref="Array"/> to get indices from.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> with the array indeces.</returns>
    public static IEnumerable<int[]> Indices(this Array array)
    {
        var indices = new int[array.Rank];
        indices[^1]--;

        while (array.IncrementIndices(ref indices))
        {
            yield return indices;
        }
    }

    /// <summary>
    /// A helper function for incrementing indices of a multi demensional <see cref="Array"/>.
    /// </summary>
    /// <param name="array">The array being itterated.</param>
    /// <param name="indices">The indices array that has to be incremented.</param>
    /// <returns><see langword="true"/> if the new indices is vallid, else <see langword="false"/>.</returns>
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