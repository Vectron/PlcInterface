using Microsoft.Extensions.Options;

namespace PlcInterface.Ads
{
    /// <summary>
    /// A <see cref="IConfigureOptions{TOptions}"/> for configuring <see cref="ConnectionSettings"/> with default values.
    /// </summary>
    public class DefaultConnectionSettingsConfigureOptions : IConfigureOptions<ConnectionSettings>
    {
        /// <inheritdoc/>
        public void Configure(ConnectionSettings options)
        {
            options.AmsNetId = "local";
            options.Port = 851;
            options.AutoConnect = false;
        }
    }
}