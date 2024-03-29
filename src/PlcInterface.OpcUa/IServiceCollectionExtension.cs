using Microsoft.Extensions.DependencyInjection;

namespace PlcInterface.OpcUa;

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
            .AddSingletonFactory<PlcConnection, IPlcConnection<Opc.Ua.Client.ISession>, IOpcPlcConnection>()
            .AddSingleton<IPlcConnection>(x => x.GetRequiredService<IOpcPlcConnection>())
            .AddTransient<IOpcTypeConverter, OpcTypeConverter>()
            .ConfigureOptions<DefaultOpcPlcConnectionConfigureOptions>()
            .ConfigureOptions<DefaultOpcSymbolHandlerSettingsConfigureOptions>();
}
