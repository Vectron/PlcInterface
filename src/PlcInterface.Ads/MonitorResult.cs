namespace PlcInterface.Ads;

/// <summary>
/// Implementation for <see cref="IMonitorResult"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MonitorResult"/> class.
/// </remarks>
/// <param name="Name">The name of the tag.</param>
/// <param name="Value">The value of the tag.</param>
internal sealed record class MonitorResult(string Name, object Value) : IMonitorResult;
