using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests;

public abstract class IPlcConnectionTestBase
{
    [TestMethod]
    public void OpenCloseConnection()
    {
        // Arrange
        var connection = GetPLCConnection();

        // Act
        connection.Connect();
        var result = connection
            .SessionStream
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .Wait();

        // Assert
        Assert.IsTrue(result.IsConnected);

        // Act
        connection.Disconnect();
        result = connection
            .SessionStream
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .Wait();

        // Assert
        Assert.IsFalse(result.IsConnected);
        connection.Disconnect();
    }

    [TestMethod]
    public async Task OpenCloseConnectionAsync()
    {
        // Arrange
        var connection = GetPLCConnection();

        // Act
        await connection.ConnectAsync();
        var result = await connection.SessionStream.FirstAsync();

        // Assert
        Assert.IsTrue(result.IsConnected);

        // Act
        await connection.DisconnectAsync();
        result = await connection.SessionStream.FirstAsync();

        // Assert
        Assert.IsFalse(result.IsConnected);
        await connection.DisconnectAsync();
    }

    protected abstract IPlcConnection GetPLCConnection();
}