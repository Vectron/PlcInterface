using PlcInterface;
using PlcInterface.Ads;
using TwinCAT.Ads;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Configure the <see cref="IServiceCollection"/> for this PLC.
        /// </summary>
        /// <param name="serviceDescriptors">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddPLC(this IServiceCollection serviceDescriptors)
            => serviceDescriptors
                .AddSingleton(typeof(ReadWrite), typeof(ReadWrite))
                .AddSingleton(typeof(IReadWrite), x => x.GetRequiredService<ReadWrite>())
                .AddSingleton(typeof(Monitor), typeof(Monitor))
                .AddSingleton(typeof(IMonitor), x => x.GetRequiredService<Monitor>())
                .AddSingleton(typeof(SymbolHandler), typeof(SymbolHandler))
                .AddSingleton(typeof(ISymbolHandler), x => x.GetRequiredService<SymbolHandler>())
                .AddSingleton(typeof(PlcConnection), typeof(PlcConnection))
                .AddSingleton(typeof(IPlcConnection<IAdsConnection>), x => x.GetRequiredService<PlcConnection>())
                .AddSingleton(typeof(IPlcConnection), x => x.GetRequiredService<IPlcConnection<IAdsConnection>>())
                .AddSingleton(typeof(DynamicValueConverter), typeof(DynamicValueConverter))
                .AddSingleton(typeof(IDynamicValueConverter), x => x.GetRequiredService<DynamicValueConverter>())
                .ConfigureOptions<DefaultConnectionSettingsConfigureOptions>()
                .ConfigureOptions<DefaultSymbolHandlerSettingsConfigureOptions>();
    }
}