using Microsoft.Extensions.Options;

namespace PlcInterface.Ads
{
    public class DefaultSymbolHandlerSettingsConfigureOptions : IConfigureOptions<SymbolHandlerSettings>
    {
        public void Configure(SymbolHandlerSettings options)
        {
            options.OutputPath = string.Empty;
            options.StoreSymbolsToDisk = false;
        }
    }
}