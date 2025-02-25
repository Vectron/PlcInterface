using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
[CICondition(ConditionMode.Exclude)]
public sealed class ReadValueTest : IReadValueTestBase
{
    protected override ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection()
            .AddOpcPLC()
            .Configure<OpcPlcConnectionOptions>(o =>
            {
                o.Address = Settings.OpcIp;
                o.Port = Settings.OpcPort;
            })
            .Configure<OpcSymbolHandlerOptions>(o => o.RootVariable = Settings.RootVariable);

        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        return services.BuildServiceProvider();
    }
}
