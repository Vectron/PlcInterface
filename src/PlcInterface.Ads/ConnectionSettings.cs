namespace PlcInterface.Ads
{
    public class ConnectionSettings
    {
        public ConnectionSettings()
        {
            AmsNetId = "127.0.0.1.1.1";
            Port = 851;
            AutoConnect = false;
        }

        public string AmsNetId
        {
            get;
            set;
        }

        public bool AutoConnect
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }
    }
}