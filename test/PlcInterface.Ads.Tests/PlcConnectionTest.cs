using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System;

namespace PlcInterface.Ads.Tests
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
            var result = connection.GetConnectedClient();
        }

        protected override IPlcConnection GetPLCConnection()
        {
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}