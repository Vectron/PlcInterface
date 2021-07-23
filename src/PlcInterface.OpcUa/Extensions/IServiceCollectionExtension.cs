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
                .AddSingleton(typeof(IReadWrite), x => x.GetRequiredService<ReadWrite>())
                .AddSingleton(typeof(ReadWrite), typeof(ReadWrite))
                .AddSingleton(typeof(IMonitor), x => x.GetRequiredService<Monitor>())
                .AddSingleton(typeof(Monitor), typeof(Monitor))
                .AddSingleton(typeof(ISymbolHandler), x => x.GetRequiredService<SymbolHandler>())
                .AddSingleton(typeof(SymbolHandler), typeof(SymbolHandler))
                .AddSingleton(typeof(PlcConnection), typeof(PlcConnection))
                .AddSingleton(typeof(IPlcConnection<Opc.Ua.Client.Session>), x => x.GetRequiredService<PlcConnection>())
                .AddSingleton(typeof(IPlcConnection), x => x.GetRequiredService<IPlcConnection<Opc.Ua.Client.Session>>())
                .ConfigureOptions<DefaultOPCSettingsConfigureOptions>();
    }
}