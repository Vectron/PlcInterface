namespace PlcInterface;

/// <summary>
/// Extensions for the <see cref="IArrayWrapper"/>.
/// </summary>
public static class ArrayWrapperExtensions
{
    /// <summary>
    /// Convert the array to a zero based version.
    /// </summary>
    /// <param name="arrayWrapper">The array to turn into a zero based.</param>
    /// <returns>The zero based array.</returns>
    public static Array ConvertZeroBased(this IArrayWrapper arrayWrapper)
    {
        var array = arrayWrapper.BackingArray;
        var sizes = Enumerable.Range(0, array.Rank).Select(array.GetLength).ToArray();
        var newArray = Array.CreateInstance(arrayWrapper.ElementType, sizes);
        Array.Copy(array, newArray, newArray.Length);
        return newArray;
    }
}
