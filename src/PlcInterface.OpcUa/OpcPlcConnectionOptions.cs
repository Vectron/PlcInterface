using System;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// Settings for the <see cref="PlcConnection"/>.
/// </summary>
public class OpcPlcConnectionOptions
{
    /// <summary>
    /// Gets or sets the address to connect to.
    /// </summary>
    public string Address { get; set; } = "127.0.0.1";

    /// <summary>
    /// Gets or sets the application configuration.
    /// </summary>
    public ApplicationConfiguration? ApplicationConfiguration
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
    /// Gets or sets a value indicating whether certificate should be generated automatically.
    /// </summary>
    public bool AutoGenCertificate
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the discovery address.
    /// </summary>
    public Uri DiscoveryAddress => new UriBuilder(FullAddress) { Path = "/discovery", }.Uri;

    /// <summary>
    /// Gets the address to connect to.
    /// </summary>
    public Uri FullAddress => new UriBuilder(UriSchema, Address, Port, string.Empty).Uri;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string? Password
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the port to connect to.
    /// </summary>
    public int Port { get; set; } = 4840;

    /// <summary>
    /// Gets or sets the connection schema to use.
    /// </summary>
    /// <remarks>
    /// Default value is 'opc.tcp'.
    /// </remarks>
    public string UriSchema { get; set; } = "opc.tcp";

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string? UserName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use secure connection when available.
    /// </summary>
    public bool UseSecurity
    {
        get;
        set;
    }
}
