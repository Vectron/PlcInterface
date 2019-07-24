using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System;

namespace PlcInterface.OpcUa.Tests
{
    [TestClass]
    public class PlcConnectionTest : IPlcConnectionTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void GetConnectedClient()
        {
            // Arrange
            var connection = GetPLCConnection() as PlcConnection;

            // Act
            _ = connection.GetConnectedClient();
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