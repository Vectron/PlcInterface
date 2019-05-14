using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PlcInterface.OpcUa.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddPLC(this IServiceCollection serviceDescriptors)
            => serviceDescriptors
                .AddSingleton(typeof(IReadWrite), x => x.GetService<ReadWrite>())
                .AddSingleton(typeof(ReadWrite), typeof(ReadWrite))
                .AddSingleton(typeof(IMonitor), x => x.GetService<Monitor>())
                .AddSingleton(typeof(Monitor), typeof(Monitor))
                .AddSingleton(typeof(ISymbolHandler), x => x.GetService<SymbolHandler>())
                .AddSingleton(typeof(SymbolHandler), typeof(SymbolHandler))
                .AddSingleton(typeof(PlcConnection), typeof(PlcConnection))
                .AddSingleton(typeof(IPlcConnection<Opc.Ua.Client.Session>), x => x.GetService<PlcConnection>())
                .AddSingleton(typeof(IPlcConnection), x => x.GetService<IPlcConnection<Opc.Ua.Client.Session>>())
                .AddSingleton<IConfigureOptions<OPCSettings>, DefaultOPCSettingsConfigureOptions>();
    }
}