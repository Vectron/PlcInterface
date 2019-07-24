using Microsoft.Extensions.Options;

namespace PlcInterface.Ads
{
    public class DefaultConnectionSettingsConfigureOptions : IConfigureOptions<ConnectionSettings>
    {
        public void Configure(ConnectionSettings options)
        {
            options.AmsNetId = "local";
            options.Port = 851;
            options.AutoConnect = false;
        }
    }
}