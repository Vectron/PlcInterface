namespace PlcInterface;

/// <summary>
/// Represents a type containing results from a monitoring event.
/// </summary>
public interface IMonitorResult
{
    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    string Name
    {
        get;
    }

    /// <summary>
    /// Gets the new value of the tag.
    /// </summary>
    object Value
    {
        get;
    }
}