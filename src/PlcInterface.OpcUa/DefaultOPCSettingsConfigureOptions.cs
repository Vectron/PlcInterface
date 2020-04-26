using Microsoft.Extensions.Options;
using Opc.Ua;
using System;
using System.Reflection;

namespace PlcInterface.OpcUa
{
    public class DefaultOPCSettingsConfigureOptions : IConfigureOptions<OPCSettings>
    {
        public void Configure(OPCSettings options)
        {
            var builder = new UriBuilder("opc.tcp", "127.0.0.1", 48010, string.Empty);
            options.Address = builder.Uri;
            options.UserName = string.Empty;
            options.Password = string.Empty;
            options.AutoConnect = false;
            options.UseSecurity = true;
            options.AutoGenCertificate = false;
            options.ApplicationConfiguration = CreateApplicationConfiguration();
        }

        private ApplicationConfiguration CreateApplicationConfiguration()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                entryAssembly = Assembly.GetExecutingAssembly();
            }

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
                        SubjectName = appName
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
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000, }
            };
        }
    }
}