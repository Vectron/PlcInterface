namespace PlcInterface.S7.SymbolExporter
{
    internal class Settings
    {
        public Settings()
        {
            WithoutUI = false;
            ProjectFile = @"Z:\Projects\Buvo\17002_4 - Copy\17002_4.ap15";
        }

        public string ProjectFile
        {
            get;
            set;
        }

        public bool WithoutUI
        {
            get;
            set;
        }
    }
}