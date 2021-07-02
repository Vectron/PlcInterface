using System.Collections.Generic;

namespace System
{
    internal static class StringExtensions
    {
        public static int[] GetIndices(this ReadOnlySpan<char> span)
        {
            var sliced = span.Slice(span.IndexOf('[') + 1);
            var end = sliced.IndexOfAny(']', ',');
            var dimensions = new List<int>();

            while (end != -1)
            {
                var value = sliced.Slice(0, end);
                var dimension = int.Parse(value.ToString());
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
}