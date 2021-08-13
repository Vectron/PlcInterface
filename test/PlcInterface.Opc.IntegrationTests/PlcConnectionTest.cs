using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;

namespace PlcInterface.OpcUa.Tests
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
            Assert.IsTrue(client.Connected);
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
            Assert.IsTrue(client.Connected);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP005:Return type should indicate that the value should be disposed.", Justification = "Can't mark interface as IDisposable")]
        protected override IPlcConnection GetPLCConnection()
        {
            var connectionsettings = new OPCSettings();
            new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Address = Settings.PLCUriNoRoot;

            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}