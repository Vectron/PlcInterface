using Microsoft.Extensions.Options;

namespace PlcInterface.Ads;

/// <summary>
/// A <see cref="IConfigureOptions{TOptions}"/> for configuring <see cref="SymbolHandlerSettings"/> with default values.
/// </summary>
public class DefaultSymbolHandlerSettingsConfigureOptions : IConfigureOptions<SymbolHandlerSettings>
{
    /// <inheritdoc/>
    public void Configure(SymbolHandlerSettings options)
    {
        options.OutputPath = string.Empty;
        options.StoreSymbolsToDisk = false;
    }
}