﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;
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
        var connectionSettings = new OpcPlcConnectionOptions();
        new DefaultOpcPlcConnectionConfigureOptions().Configure(connectionSettings);
        connectionSettings.Address = Settings.PLCUriNoRoot;

        plcConnection?.Dispose();
        plcConnection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>());
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
        _ = connectionTask.GetAwaiter().GetResult();

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
        _ = await connectionTask;

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