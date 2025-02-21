namespace PlcInterface;

/// <summary>
/// A generic wrapper over a <see cref="Array"/>.
/// </summary>
public interface IArrayWrapper
{
    /// <summary>
    /// Gets the backing array storage.
    /// </summary>
    public Array BackingArray
    {
        get;
    }

    /// <summary>
    /// Gets the type of the element stored in the array.
    /// </summary>
    public Type ElementType
    {
        get;
    }
}
