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
            using var connection = GetPLCConnection() as PlcConnection;

            // Act
            Assert.IsNotNull(connection);
            _ = Assert.ThrowsException<TimeoutException>(() => connection.GetConnectedClient());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP005:Return type should indicate that the value should be disposed.", Justification = "Can't mark interface as IDisposable")]
        protected override IPlcConnection GetPLCConnection()
        {
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            return new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
        }
    }
}