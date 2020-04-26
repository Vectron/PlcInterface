namespace PlcInterface.S7
{
    public class ConnectionSettings
    {
        public string Adress
        {
            get;
            set;
        }

        public bool AutoConnect
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the rack number
        /// S7 300 CPU: 0
        /// S7 400 CPU: Follow the hardware configuration.
        /// WinAC CPU: Follow the hardware configuration.
        /// S7 1200 CPU: 0
        /// S7 1500 CPU: 0
        /// WinAC IE: 0 or Follow the hardware configuration.
        /// </summary>
        public int Rack
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the slot number
        /// S7 300 CPU: 2
        /// S7 400 CPU: Follow the hardware configuration.
        /// WinAC CPU: Follow the hardware configuration.
        /// S7 1200 CPU: 0 or 1
        /// S7 1500 CPU: 0 or 1
        /// WinAC IE: 0 or Follow the hardware configuration.
        /// </summary>
        public int Slot
        {
            get;
            set;
        }
    }
}