namespace System
{
    public static class ArrayExtensions
    {
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
    }
}