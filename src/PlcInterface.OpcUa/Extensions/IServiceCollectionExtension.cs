using PlcInterface;
using PlcInterface.OpcUa;

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
        public static IServiceCollection AddOpcPLC(this IServiceCollection serviceDescriptors)
            => serviceDescriptors
                .AddSingletonFactory<ReadWrite, IReadWrite, IOpcReadWrite>()
                .AddSingletonFactory<Monitor, IMonitor, IOpcMonitor>()
                .AddSingletonFactory<SymbolHandler, ISymbolHandler, IOpcSymbolHandler>()
                .AddSingletonFactory<PlcConnection, IPlcConnection<Opc.Ua.Client.Session>, IOpcPlcConnection>()
                .AddSingleton<IPlcConnection>(x => x.GetRequiredService<IOpcPlcConnection>())
                .AddTransient<IOpcTypeConverter, OpcTypeConverter>(x => x.GetRequiredService<OpcTypeConverter>())
                .AddTransient<OpcTypeConverter, OpcTypeConverter>()
                .ConfigureOptions<DefaultOPCSettingsConfigureOptions>();

        private static IServiceCollection AddSingletonFactory<TImplementation, TService1, TService2>(this IServiceCollection serviceDescriptors)
            where TService1 : class
            where TService2 : class, TService1
            where TImplementation : class, TService1, TService2
            => serviceDescriptors
                .AddSingleton<TService2, TImplementation>()
                .AddSingleton(x => (TService1)x.GetRequiredService<TService2>());
    }
}