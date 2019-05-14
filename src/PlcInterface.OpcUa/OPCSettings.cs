using Opc.Ua;
using System;

namespace PlcInterface.OpcUa
{
    public class OPCSettings
    {
        public Uri Address
        {
            get;
            set;
        }

        public ApplicationConfiguration ApplicationConfiguration
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

        public bool UseSecurity
        {
            get;
            set;
        }
    }
}