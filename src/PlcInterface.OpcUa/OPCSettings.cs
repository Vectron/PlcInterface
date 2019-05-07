using System;
using System.Reflection;

namespace PlcInterface.OpcUa
{
    public class OPCSettings
    {
        public OPCSettings()
            : this("127.0.0.1", 48010, string.Empty, Assembly.GetEntryAssembly().GetName().Name, string.Empty, string.Empty, false)
        {
        }

        public OPCSettings(string host, int port, string rootFolder, string applicationName)
            : this(host, port, rootFolder, applicationName, string.Empty, string.Empty, false)
        {
        }

        public OPCSettings(string host, int port, string rootFolder, string applicationName, bool autoConnect)
            : this(host, port, rootFolder, applicationName, string.Empty, string.Empty, autoConnect)
        {
        }

        public OPCSettings(string host, int port, string rootFolder, string applicationName, string userName, string password, bool autoConnect)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("A host address must be specified", nameof(host));
            }

            if (rootFolder == null)
            {
                throw new ArgumentNullException(nameof(rootFolder));
            }

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentException("A applicationName must be specified", nameof(applicationName));
            }

            var builder = new UriBuilder("opc.tcp", host, port, rootFolder);
            Address = builder.Uri;
            ApplicationName = applicationName;
            UserName = userName;
            Password = password;
            AutoConnect = autoConnect;
        }

        public Uri Address
        {
            get;
            set;
        }

        public string ApplicationName
        {
            get;
            set;
        }

        public bool AutoConnect
        {
            get;
            set;
        }

        public bool AutoGenCertificate
        {
            get;
            set;
        }

        public Uri DiscoveryAdress => new UriBuilder(Address)
        {
            Path = "/discovery"
        }.Uri;

        public string Password
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
    }
}