using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using TestUtilities;
using TwinCAT.Ads;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class PlcConnectionTest : IPlcConnectionTestBase
{
    private static AdsClient? adsClient;
    private static PlcConnection? plcConnection;

    [ClassInitialize]
    public static void ConnectAsync(TestContext testContext)
    {
        var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        adsClient = new AdsClient();

        plcConnection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
    }

    [ClassCleanup]
    public static void Disconnect()
    {
        plcConnection!.Dispose();
        adsClient!.Dispose();
    }

    [TestMethod]
    public void GetConnectedClient()
    {
        // Arrange
        Assert.IsNotNull(plcConnection);

        // Act
        _ = Assert.ThrowsException<TimeoutException>(() => plcConnection.GetConnectedClient());
    }

    [TestMethod]
    public void GetConnectedClientReturnsTheActiveConnection()
    {
        // Arrange
        Assert.IsNotNull(plcConnection);

        // Act
        var connectionTask = plcConnection.ConnectAsync();
        var client = plcConnection.GetConnectedClient(TimeSpan.FromSeconds(10));
        connectionTask.GetAwaiter().GetResult();

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.IsConnected);
    }

    [TestMethod]
    public async Task GetConnectedClientReturnsTheActiveConnectionAsync()
    {
        // Arrange
        Assert.IsNotNull(plcConnection);

        // Act
        var connectionTask = plcConnection.ConnectAsync();
        var client = await plcConnection.GetConnectedClientAsync();
        await connectionTask;

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.IsConnected);
    }

    protected override IPlcConnection GetPLCConnection()
        => plcConnection!;
}