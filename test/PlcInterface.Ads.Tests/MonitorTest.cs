using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;

namespace PlcInterface.Ads.Tests
{
    [TestClass]
    public class MonitorTest : IMonitorTestBase
    {
        private static PlcConnection? connection;
        private static Monitor? monitor;
        private static ReadWrite? readWrite;
        private static SymbolHandler? symbolHandler;

        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public Api")]
        public static async Task ConnectAsync(TestContext testContext)
        {
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            var symbolhandlersettings = new SymbolHandlerSettings() { StoreSymbolsToDisk = false };
            var typeConverter = new AdsTypeConverter();

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetOptionsMoq(symbolhandlersettings), GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, typeConverter);
            monitor = new Monitor(symbolHandler, typeConverter, GetLoggerMock<Monitor>());

            await connection.ConnectAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
        {
            connection?.Dispose();
            symbolHandler?.Dispose();
        }

        protected override IMonitor GetMonitor()
            => monitor ?? throw new NotSupportedException();

        protected override IPlcConnection GetPLCConnection()
            => connection ?? throw new NotSupportedException();

        protected override IReadWrite GetReadWrite()
            => readWrite ?? throw new NotSupportedException();

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler ?? throw new NotSupportedException();
    }
}