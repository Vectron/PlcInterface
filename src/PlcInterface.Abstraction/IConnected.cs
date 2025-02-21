namespace PlcInterface;

/// <summary>
/// A generic implementation of <see cref="IConnected"/>.
/// </summary>
/// <typeparam name="T">The type that is connected.</typeparam>
public interface IConnected<out T> : IConnected
{
    /// <summary>
    /// Gets the value containing the lost or opened connection.
    /// </summary>
    /// <exception cref="InvalidOperationException">When <see cref="IConnected.IsConnected"/> returns false.</exception>
    public T Value
    {
        get;
    }
}

/// <summary>
/// Represents a type containing a opened or closed connection.
/// </summary>
public interface IConnected
{
    /// <summary>
    /// Gets a value indicating whether a value indicating of the connection is open.
    /// </summary>
    public bool IsConnected
    {
        get;
    }
}
