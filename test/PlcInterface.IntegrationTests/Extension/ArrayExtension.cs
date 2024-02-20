using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests.Extension;

internal static class ArrayExtension
{
    public static bool SequenceEqual<T>(this Array array, Array other)
        => array.Rank == other.Rank
        && Enumerable.Range(0, array.Rank).All(dimension => array.GetLength(dimension) == other.GetLength(dimension))
        && array.Cast<T>().SequenceEqual(other.Cast<T>());
}
