namespace PlcInterface.Ads;

/// <summary>
/// Implementation for <see cref="IMonitorResult"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MonitorResult"/> class.
/// </remarks>
/// <param name="name">The name of the tag.</param>
/// <param name="value">The value of the tag.</param>
internal sealed class MonitorResult(string name, object value) : IMonitorResult
{
    /// <inheritdoc/>
    public string Name => name;

    /// <inheritdoc/>
    public object Value => value;
}
