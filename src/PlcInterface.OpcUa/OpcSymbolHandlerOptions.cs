namespace PlcInterface.OpcUa;

/// <summary>
/// Settings for the <see cref="SymbolHandler"/>.
/// </summary>
public class OpcSymbolHandlerOptions
{
    /// <summary>
    /// Gets or sets the path to the root node.
    /// </summary>
    public string RootNodePath { get; set; } = string.Empty;
}
