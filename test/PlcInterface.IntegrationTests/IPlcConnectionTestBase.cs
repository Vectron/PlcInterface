using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.IntegrationTests;

public abstract class IPlcConnectionTestBase
{
    [TestMethod]
    public void OpenCloseConnection()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();

        // Act
        var connected = connection.Connect();

        // Assert
        Assert.IsTrue(connected, "Plc could not connect");
        Assert.IsTrue(connection.IsConnected);

        // Act
        connection.Disconnect();

        // Assert
        Assert.IsFalse(connection.IsConnected);
        connection.Disconnect();
    }

    [TestMethod]
    public async Task OpenCloseConnectionAsync()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();

        // Act
        var connected = await connection.ConnectAsync();

        // Assert
        Assert.IsTrue(connected, "Plc could not connect");
        Assert.IsTrue(connection.IsConnected);

        // Act
        await connection.DisconnectAsync();

        // Assert
        Assert.IsFalse(connection.IsConnected);
        await connection.DisconnectAsync();
    }

    protected abstract IServiceProvider GetServiceProvider();
}
