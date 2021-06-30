using System;
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
            var connection = GetPLCConnection() as PlcConnection;

            // Act
            _ = Assert.ThrowsException<TimeoutException>(() => connection.GetConnectedClient());
        }

        protected override IPlcConnection GetPLCConnection()
        {
            var connectionsettings = new OPCSettings();
            new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Address = Settings.PLCUriNoRoot;

            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}