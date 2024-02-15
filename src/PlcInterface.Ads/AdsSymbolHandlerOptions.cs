namespace PlcInterface.Ads;

/// <summary>
/// Settings for the <see cref="SymbolHandler"/>.
/// </summary>
public class AdsSymbolHandlerOptions
{
    /// <summary>
    /// Gets or sets path where to store the found symbols.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the root node.
    /// </summary>
    /// <remarks>
    /// Sub items are separated by a '.'.
    /// </remarks>
    public string RootVariable { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the symbol list should be written to disk.
    /// </summary>
    public bool StoreSymbolsToDisk
    {
        get;
        set;
    }
}
