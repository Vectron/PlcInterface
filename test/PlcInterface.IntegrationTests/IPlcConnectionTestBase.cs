using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.IntegrationTests;

public abstract class IPlcConnectionTestBase
{
    [TestMethod]
    public void OpenCloseConnection()
    {
        // Arrange
        var connection = GetPLCConnection();

        // Act
        _ = connection.Connect();

        // Assert
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
        var connection = GetPLCConnection();

        // Act
        _ = await connection.ConnectAsync();

        // Assert
        Assert.IsTrue(connection.IsConnected);

        // Act
        await connection.DisconnectAsync();

        // Assert
        Assert.IsFalse(connection.IsConnected);
        await connection.DisconnectAsync();
    }

    protected abstract IPlcConnection GetPLCConnection();
}