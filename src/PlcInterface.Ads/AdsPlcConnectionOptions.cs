namespace PlcInterface.Ads;

/// <summary>
/// Settings for the <see cref="PlcConnection"/>.
/// </summary>
public class AdsPlcConnectionOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdsPlcConnectionOptions"/> class.
    /// </summary>
    public AdsPlcConnectionOptions()
    {
        AmsNetId = string.Empty;
        AutoConnect = false;
        Port = 0;
    }

    /// <summary>
    /// Gets or sets the address to connect to.
    /// </summary>
    public string AmsNetId
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the connection should be opened automatically.
    /// </summary>
    public bool AutoConnect
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the port to connect to.
    /// </summary>
    public int Port
    {
        get;
        set;
    }
}
