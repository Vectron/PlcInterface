using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System;

namespace PlcInterface.S7.Tests
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
            var connectionsettings = new ConnectionSettings();
            new DefaultConnectionSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Adress = Settings.Ip;

            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}