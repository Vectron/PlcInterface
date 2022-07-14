using Microsoft.Extensions.Options;

namespace PlcInterface.Ads;

/// <summary>
/// A <see cref="IConfigureOptions{TOptions}"/> for configuring <see cref="AdsSymbolHandlerOptions"/> with default values.
/// </summary>
public class DefaultAdsSymbolHandlerSettingsConfigureOptions : IConfigureOptions<AdsSymbolHandlerOptions>
{
    /// <inheritdoc/>
    public void Configure(AdsSymbolHandlerOptions options)
    {
        options.OutputPath = string.Empty;
        options.StoreSymbolsToDisk = false;
    }
}