namespace PlcInterface;

/// <summary>
/// Represents a generic type used to connect to a PLC.
/// </summary>
/// <typeparam name="T">The underlying connection type.</typeparam>
public interface IPlcConnection<T> : IPlcConnection
{
    /// <summary>
    /// Gets the generic session stream.
    /// </summary>
    public new IObservable<IConnected<T>> SessionStream
    {
        get;
    }
}

/// <summary>
/// Represents a type used to connect to a PLC.
/// </summary>
public interface IPlcConnection
{
    /// <summary>
    /// Gets a value indicating whether the connections is connected.
    /// </summary>
    public bool IsConnected
    {
        get;
    }

    /// <summary>
    /// Gets the session stream.
    /// </summary>
    public IObservable<IConnected> SessionStream
    {
        get;
    }

    /// <summary>
    /// Gets a settings object for this PLC.
    /// </summary>
    public object Settings
    {
        get;
    }

    /// <summary>
    /// Connect to the PLC.
    /// </summary>
    /// <returns><see langword="true"/> when connection is opened successful, otherwise <see langword="false"/>.</returns>
    public bool Connect();

    /// <summary>
    /// Asynchronously connect to the PLC.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that handles the connection. <see langword="true"/> when connection is
    /// opened successful, otherwise <see langword="false"/>.
    /// </returns>
    public Task<bool> ConnectAsync();

    /// <summary>
    /// Disconnect from the PLC.
    /// </summary>
    public void Disconnect();

    /// <summary>
    /// Asynchronously disconnect from the PLC.
    /// </summary>
    /// <returns>A <see cref="Task"/> that handles the disconnection.</returns>
    public Task DisconnectAsync();
}
