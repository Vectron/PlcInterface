using System;
using Opc.Ua;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// Settings for the <see cref="PlcConnection"/>.
    /// </summary>
    public class OPCSettings
    {
        /// <summary>
        /// Gets or sets the adress to connect to.
        /// </summary>
        public Uri? Address
        {
            get;
            set;
        }

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
        /// Gets the discovery adress.
        /// </summary>
        public Uri DiscoveryAdress
            => new UriBuilder(Address)
            {
                Path = "/discovery",
            }.Uri;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string? UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether use secure connection when availible.
        /// </summary>
        public bool UseSecurity
        {
            get;
            set;
        }
    }
}