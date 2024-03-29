using System.Reflection;
using Microsoft.Extensions.Options;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// A <see cref="IConfigureOptions{TOptions}"/> for configuring <see cref="OpcPlcConnectionOptions"/> with default values.
/// </summary>
public class DefaultOpcPlcConnectionConfigureOptions : IConfigureOptions<OpcPlcConnectionOptions>
{
    /// <inheritdoc/>
    public void Configure(OpcPlcConnectionOptions options)
    {
        options.Address = "127.0.0.1";
        options.Port = 4840;
        options.UriSchema = "opc.tcp";
        options.UserName = string.Empty;
        options.Password = string.Empty;
        options.AutoConnect = false;
        options.UseSecurity = true;
        options.AutoGenCertificate = false;
        options.ApplicationConfiguration = CreateApplicationConfiguration();
    }

    private static ApplicationConfiguration CreateApplicationConfiguration()
    {
        var entryAssembly = Assembly.GetEntryAssembly()
            ?? Assembly.GetExecutingAssembly();

        var appName = entryAssembly.GetName().Name;

        return new ApplicationConfiguration()
        {
            ApplicationName = appName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = "urn:" + Utils.GetHostName() + ":" + appName,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = "X509Store",
                    StorePath = "CurrentUser\\My",
                    SubjectName = appName,
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = "Directory",
                    StorePath = "OPC Foundation/CertificateStores/UA Applications",
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = "Directory",
                    StorePath = "OPC Foundation/CertificateStores/UA Certificate Authorities",
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = "Directory",
                    StorePath = "OPC Foundation/CertificateStores/RejectedCertificates",
                },
                NonceLength = 32,
                AutoAcceptUntrustedCertificates = true,
            },
            TransportConfigurations = [],
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000, },
        };
    }
}
