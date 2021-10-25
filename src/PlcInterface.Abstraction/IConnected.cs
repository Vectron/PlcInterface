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
    T? Value
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
    bool IsConnected
    {
        get;
    }
}