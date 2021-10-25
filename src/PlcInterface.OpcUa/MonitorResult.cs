namespace PlcInterface.OpcUa;

/// <summary>
/// Implementation for <see cref="IMonitorResult"/>.
/// </summary>
internal sealed class MonitorResult : IMonitorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MonitorResult"/> class.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="value">The value of the tag.</param>
    public MonitorResult(string name, object value)
    {
        Name = name;
        Value = value;
    }

    /// <inheritdoc/>
    public string Name
    {
        get;
    }

    /// <inheritdoc/>
    public object Value
    {
        get;
    }
}