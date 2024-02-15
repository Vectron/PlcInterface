namespace PlcInterface.OpcUa;

/// <summary>
/// Settings for the <see cref="SymbolHandler"/>.
/// </summary>
public class OpcSymbolHandlerOptions
{
    /// <summary>
    /// Gets or sets the path to the root node.
    /// </summary>
    /// <remarks>
    /// Sub items are separated by a '.'.
    /// </remarks>
    public string RootVariable { get; set; } = string.Empty;
}
