using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public class MonitorTest : IMonitorTestBase
{
#if NET6_0
    protected override string DataRoot => "AdsNet6";
#elif NET7_0
    protected override string DataRoot => "AdsNet7";
#endif

    protected override ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection()
            .AddAdsPLC()
            .Configure<AdsPlcConnectionOptions>(o =>
            {
                o.AmsNetId = Settings.AmsNetId;
                o.Port = Settings.Port;
            })
            .Configure<AdsSymbolHandlerOptions>(o => o.StoreSymbolsToDisk = false);

        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        return services.BuildServiceProvider();
    }
}
