using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using TestUtilities;

namespace PlcInterface.OpcUa.Tests
{
    [TestClass]
    public class PlcConnectionTest : IPlcConnectionTestBase
    {
        private static PlcConnection? plcConnection;

        [ClassInitialize]
        public static void ConnectAsync(TestContext testContext)
        {
            var connectionsettings = new OPCSettings();
            new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Address = Settings.PLCUriNoRoot;

            plcConnection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>());
        }

        [ClassCleanup]
        public static void Disconnect()
            => plcConnection!.Dispose();

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
            => plcConnection!;
    }
}