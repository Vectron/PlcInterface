using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.OpcUa;
using PlcInterface.Tests;
using TestUtilities;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
public sealed class PlcConnectionTest : IPlcConnectionTestBase, IDisposable
{
    private bool disposed;
    private PlcConnection? plcConnection;

    [TestInitialize]
    public void ConnectAsync()
    {
        var connectionsettings = new OpcPlcConnectionOptions();
        new DefaultOpcPlcConnectionConfigureOptions().Configure(connectionsettings);
        connectionsettings.Address = Settings.PLCUriNoRoot;

        plcConnection?.Dispose();
        plcConnection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>());
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

        plcConnection?.Dispose();
        disposed = true;
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
        Assert.IsTrue(client.Connected);
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
        Assert.IsTrue(client.Connected);
    }

    protected override IPlcConnection GetPLCConnection()
    {
        Assert.IsNotNull(plcConnection);
        return plcConnection;
    }
}