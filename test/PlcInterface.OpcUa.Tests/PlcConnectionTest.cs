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
            var result = connection.GetConnectedClient();
        }

        protected override IPlcConnection GetPLCConnection()
        {
            var settings = new OPCSettings(Settings.OpcIp, Settings.OpcPort, string.Empty, "PlcConnectionTest");
            return new PlcConnection(GetOptionsMoq(settings), GetLoggerMock<PlcConnection>());
        }
    }
}