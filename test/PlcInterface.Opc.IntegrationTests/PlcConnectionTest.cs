using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
[CICondition(ConditionMode.Exclude)]
public sealed class PlcConnectionTest : IPlcConnectionTestBase
{
    [TestMethod]
    public void GetConnectedClient()
    {
        // Arrange
        using var serviceProvider = GetServiceProvider();
        var connection = serviceProvider.GetRequiredService<IOpcPlcConnection>();

        // Act Assert
        _ = Assert.ThrowsException<TimeoutException>(() => connection.GetConnectedClient());
    }

    [TestMethod]
    public void GetConnectedClientReturnsTheActiveConnection()
    {
        // Arrange
        using var serviceProvider = GetServiceProvider();
        var connection = serviceProvider.GetRequiredService<IOpcPlcConnection>();

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var client = connection.GetConnectedClient(TimeSpan.FromSeconds(2));

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.Connected);
    }

    [TestMethod]
    public async Task GetConnectedClientReturnsTheActiveConnectionAsync()
    {
        // Arrange
        using var serviceProvider = GetServiceProvider();
        var connection = serviceProvider.GetRequiredService<IOpcPlcConnection>();

        // Act
        var connected = await connection.ConnectAsync();
        Assert.IsTrue(connected, "Plc could not connect");
        var client = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(2));

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.Connected);
    }

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
