using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;

namespace PlcInterface.Ads.Tests
{
    [TestClass]
    public class PlcConnectionTest : IPlcConnectionTestBase
    {
        [TestMethod]
        public void GetConnectedClient()
        {
            // Arrange
            using var connection = GetPLCConnection() as PlcConnection;

            // Act
            Assert.IsNotNull(connection);
            _ = Assert.ThrowsException<TimeoutException>(() => connection.GetConnectedClient());
        }

        [TestMethod]
        public void GetConnectedClientReturnsTheActiveConnection()
        {
            // Arrange
            using var connection = GetPLCConnection() as PlcConnection;
            Assert.IsNotNull(connection);

            // Act
            var connectionTask = connection.ConnectAsync();
            var client = connection.GetConnectedClient(TimeSpan.FromSeconds(10));
            connectionTask.GetAwaiter().GetResult();

            // Assert
            Assert.IsNotNull(client);
            Assert.IsTrue(client.IsConnected);
        }

        [TestMethod]
        public async Task GetConnectedClientReturnsTheActiveConnectionAsync()
        {
            // Arrange
            using var connection = GetPLCConnection() as PlcConnection;
            Assert.IsNotNull(connection);

            // Act
            var connectionTask = connection.ConnectAsync();
            var client = await connection.GetConnectedClientAsync();
            await connectionTask;

            // Assert
            Assert.IsNotNull(client);
            Assert.IsTrue(client.IsConnected);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP005:Return type should indicate that the value should be disposed.", Justification = "Can't mark interface as IDisposable")]
        protected override IPlcConnection GetPLCConnection()
        {
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}