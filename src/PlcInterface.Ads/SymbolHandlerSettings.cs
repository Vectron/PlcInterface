namespace PlcInterface.Ads
{
    public class SymbolHandlerSettings
    {
        public SymbolHandlerSettings()
        {
            StoreSymbolsToDisk = false;
            OutputPath = string.Empty;
        }

        public string OutputPath
        {
            get;
            set;
        }

        public bool StoreSymbolsToDisk
        {
            get;
            set;
        }
    }
}