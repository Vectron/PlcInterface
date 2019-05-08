using Microsoft.Extensions.DependencyInjection;
using TwinCAT.Ads;

namespace PlcInterface.Ads.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddPLC(this IServiceCollection serviceDescriptors)
            => serviceDescriptors
                .AddSingleton(typeof(ReadWrite), typeof(ReadWrite))
                .AddSingleton(typeof(IReadWrite), x => x.GetService<ReadWrite>())
                .AddSingleton(typeof(Monitor), typeof(Monitor))
                .AddSingleton(typeof(IMonitor), x => x.GetService<Monitor>())
                .AddSingleton(typeof(SymbolHandler), typeof(SymbolHandler))
                .AddSingleton(typeof(ISymbolHandler), x => x.GetService<SymbolHandler>())
                .AddSingleton(typeof(PlcConnection), typeof(PlcConnection))
                .AddSingleton(typeof(IPlcConnection<IAdsConnection>), x => x.GetService<PlcConnection>())
                .AddSingleton(typeof(IPlcConnection), x => x.GetService<IPlcConnection<IAdsConnection>>())
                .AddSingleton(typeof(DynamicValueConverter), typeof(DynamicValueConverter))
                .AddSingleton(typeof(IDynamicValueConverter), x => x.GetService<DynamicValueConverter>());
    }
}