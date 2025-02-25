using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PlcInterface.IntegrationTests;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
[CICondition(ConditionMode.Exclude)]
public sealed class PlcConnectionTest : IPlcConnectionTestBase
{
    [TestMethod]
    public void GetConnectedClient()
    {
        // Arrange
        using var serviceProvider = GetServiceProvider();
        var connection = serviceProvider.GetRequiredService<IAdsPlcConnection>();

        // Act
        _ = Assert.ThrowsException<TimeoutException>(() => connection.GetConnectedClient());
    }

    [TestMethod]
    public void GetConnectedClientReturnsTheActiveConnection()
    {
        // Arrange
        using var serviceProvider = GetServiceProvider();
        var connection = serviceProvider.GetRequiredService<IAdsPlcConnection>();

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var client = connection.GetConnectedClient(TimeSpan.FromSeconds(2));

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.IsConnected);
    }

    [TestMethod]
    public async Task GetConnectedClientReturnsTheActiveConnectionAsync()
    {
        // Arrange
        using var serviceProvider = GetServiceProvider();
        var connection = serviceProvider.GetRequiredService<IAdsPlcConnection>();

        // Act
        var connected = await connection.ConnectAsync();
        Assert.IsTrue(connected, "Plc could not connect");
        var client = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(2));

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.IsConnected);
    }

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
