using Microsoft.Extensions.Options;

namespace PlcInterface.OpcUa;

/// <summary>
/// A <see cref="IConfigureOptions{TOptions}"/> for configuring <see cref="OpcSymbolHandlerOptions"/> with default values.
/// </summary>
public class DefaultOpcSymbolHandlerSettingsConfigureOptions : IConfigureOptions<OpcSymbolHandlerOptions>
{
    /// <inheritdoc/>
    public void Configure(OpcSymbolHandlerOptions options)
        => options.RootNodePath = string.Empty;
}
