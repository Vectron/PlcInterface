using PlcInterface;
using PlcInterface.Ads;

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
        public static IServiceCollection AddAdsPLC(this IServiceCollection serviceDescriptors)
            => serviceDescriptors
                .AddSingletonFactory<ReadWrite, IReadWrite, IAdsReadWrite>()
                .AddSingletonFactory<Monitor, IMonitor, IAdsMonitor>()
                .AddSingletonFactory<SymbolHandler, ISymbolHandler, IAdsSymbolHandler>()
                .AddSingletonFactory<PlcConnection, IPlcConnection<TwinCAT.Ads.IAdsConnection>, IAdsPlcConnection>()
                .AddSingleton<IPlcConnection>(x => x.GetRequiredService<IAdsPlcConnection>())
                .AddTransient<IAdsTypeConverter, AdsTypeConverter>(x => x.GetRequiredService<AdsTypeConverter>())
                .AddTransient<AdsTypeConverter, AdsTypeConverter>()
                .ConfigureOptions<DefaultConnectionSettingsConfigureOptions>()
                .ConfigureOptions<DefaultSymbolHandlerSettingsConfigureOptions>();

        private static IServiceCollection AddSingletonFactory<TImplementation, TService1, TService2>(this IServiceCollection serviceDescriptors)
            where TService1 : class
            where TService2 : class, TService1
            where TImplementation : class, TService1, TService2
            => serviceDescriptors
                .AddSingleton<TService2, TImplementation>()
                .AddSingleton(x => (TService1)x.GetRequiredService<TService2>());
    }
}