using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PlcInterface.IntegrationTests;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public class WriteValueTest : IWriteValueTestBase
{
    protected override ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection()
            .AddAdsPLC()
            .Configure<AdsPlcConnectionOptions>(o =>
            {
                o.AmsNetId = Settings.AmsNetId;
                o.Port = Settings.Port;
            })
            .Configure<AdsSymbolHandlerOptions>(o =>
            {
                o.StoreSymbolsToDisk = false;
                o.RootVariable = Settings.RootVariable;
            });

        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        return services.BuildServiceProvider();
    }
}
