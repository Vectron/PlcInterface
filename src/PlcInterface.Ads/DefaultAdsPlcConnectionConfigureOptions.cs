using Microsoft.Extensions.Options;

namespace PlcInterface.Ads;

/// <summary>
/// A <see cref="IConfigureOptions{TOptions}"/> for configuring <see cref="AdsPlcConnectionOptions"/> with default values.
/// </summary>
public class DefaultAdsPlcConnectionConfigureOptions : IConfigureOptions<AdsPlcConnectionOptions>
{
    /// <inheritdoc/>
    public void Configure(AdsPlcConnectionOptions options)
    {
        options.AmsNetId = "local";
        options.Port = 851;
        options.AutoConnect = false;
    }
}