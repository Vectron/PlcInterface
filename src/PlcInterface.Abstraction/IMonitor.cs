namespace PlcInterface;

/// <summary>
/// Represents a type used to perform IO monitoring.
/// </summary>
public interface IMonitor
{
    /// <summary>
    /// Gets a <see cref="IObservable{T}"/> for getting IO updates.
    /// </summary>
    public IObservable<IMonitorResult> SymbolStream
    {
        get;
    }

    /// <summary>
    /// Gets a <see cref="ITypeConverter"/>.
    /// </summary>
    public ITypeConverter TypeConverter
    {
        get;
    }

    /// <summary>
    /// Register IO tags for monitoring.
    /// </summary>
    /// <param name="ioNames">The names of the tags.</param>
    /// <param name="updateInterval">The interval between IO updates.</param>
    public void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000);

    /// <summary>
    /// Register a IO tag for monitoring.
    /// </summary>
    /// <param name="ioName">The name of the tag.</param>
    /// <param name="updateInterval">The interval between IO updates.</param>
    public void RegisterIO(string ioName, int updateInterval = 1000);

    /// <summary>
    /// Unregister IO tags for monitoring.
    /// </summary>
    /// <param name="ioNames">The names of the tags.</param>
    public void UnregisterIO(IEnumerable<string> ioNames);

    /// <summary>
    /// Register a IO tag for monitoring.
    /// </summary>
    /// <param name="ioName">The name of the tag.</param>
    public void UnregisterIO(string ioName);
}
