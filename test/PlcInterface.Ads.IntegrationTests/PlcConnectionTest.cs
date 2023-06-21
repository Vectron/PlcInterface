using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;
using TestUtilities;
using TwinCAT.Ads;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public sealed class PlcConnectionTest : IPlcConnectionTestBase, IDisposable
{
    private AdsClient? adsClient;
    private bool disposed;
    private PlcConnection? plcConnection;

    [TestInitialize]
    public void ConnectAsync()
    {
        var connectionSettings = new AdsPlcConnectionOptions() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        adsClient = new AdsClient();
        plcConnection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
    }

    [TestCleanup]
    public void Disconnect()
        => Dispose();

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        plcConnection?.Dispose();
        adsClient?.Dispose();
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
        _ = connectionTask.GetAwaiter().GetResult();

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
        _ = await connectionTask;

        // Assert
        Assert.IsNotNull(client);
        Assert.IsTrue(client.IsConnected);
    }

    protected override IPlcConnection GetPLCConnection()
    {
        Assert.IsNotNull(plcConnection);
        return plcConnection;
    }
}