namespace PlcInterface.Ads;

/// <summary>
/// Settings for the <see cref="SymbolHandler"/>.
/// </summary>
public class SymbolHandlerSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolHandlerSettings"/> class.
    /// </summary>
    public SymbolHandlerSettings()
    {
        OutputPath = string.Empty;
        StoreSymbolsToDisk = false;
    }

    /// <summary>
    /// Gets or sets path where to store the found symbols.
    /// </summary>
    public string OutputPath
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the symbol list should be written to disk.
    /// </summary>
    public bool StoreSymbolsToDisk
    {
        get;
        set;
    }
}