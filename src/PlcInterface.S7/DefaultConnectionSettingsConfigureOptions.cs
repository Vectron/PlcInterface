using Microsoft.Extensions.Options;

namespace PlcInterface.S7
{
    public class DefaultConnectionSettingsConfigureOptions : IConfigureOptions<ConnectionSettings>
    {
        public void Configure(ConnectionSettings options)
        {
            options.Adress = "127.0.0.1";
            options.AutoConnect = false;
            options.Rack = 0;
            options.Slot = 2;
        }
    }
}