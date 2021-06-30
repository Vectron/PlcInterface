using System;
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
            var connection = GetPLCConnection() as PlcConnection;

            // Act
            _ = Assert.ThrowsException<TimeoutException>(() => connection.GetConnectedClient());
        }

        protected override IPlcConnection GetPLCConnection()
        {
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}