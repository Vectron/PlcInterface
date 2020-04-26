using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.S7.Tests
{
    [TestClass]
    public class SymbolHandlerTest : ISymbolHandlerTestBase
    {
        private static PlcConnection connection;
        private static SymbolHandler symbolHandler;

        public override IEnumerable<object[]> Data
            => Settings.GetReadData();

        [ClassInitialize]
        public static async Task ConnectAsync(TestContext testContext)
        {
            var connectionsettings = new ConnectionSettings();
            new DefaultConnectionSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Adress = Settings.Ip;

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetLoggerMock<SymbolHandler>());
            await connection.ConnectAsync();

            _ = await connection.SessionStream.FirstAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
            => connection.Dispose();

        protected override IPlcConnection GetPLCConnection()
            => connection;

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler;
    }
}