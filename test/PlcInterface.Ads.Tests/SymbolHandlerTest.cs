using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.Ads.Tests
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
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            var symbolhandlersettings = new SymbolHandlerSettings() { StoreSymbolsToDisk = false };

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetOptionsMoq(symbolhandlersettings), GetLoggerMock<SymbolHandler>());
            await connection.ConnectAsync();
            var result = await connection.SessionStream.FirstAsync();
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