using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Extension methods for <see cref="Array"/>.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// A helper function for incrementing indices of a multi demensional <see cref="Array"/>.
        /// </summary>
        /// <param name="array">The array being itterated.</param>
        /// <param name="indices">The indices array that has to be incremented.</param>
        /// <returns><see langword="true"/> if the new indices is vallid, else <see langword="false"/>.</returns>
        public static bool IncrementIndices(this Array array, int[] indices)
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

        /// <summary>
        /// Itterate over all array indices.
        /// </summary>
        /// <param name="array">The <see cref="Array"/> to get indices from.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> with the array indeces.</returns>
        public static IEnumerable<int[]> Indices(this Array array)
        {
            var indices = new int[array.Rank];
            indices[indices.Length - 1]--;

            while (IncrementIndices(array, indices))
            {
                yield return indices;
            }
        }
    }
}