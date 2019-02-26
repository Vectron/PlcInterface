using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.OpcUa.Tests
{
    [TestClass]
    public class WriteValueTest : IWriteValueTestBase
    {
        private static PlcConnection connection;
        private static ReadWrite readWrite;
        private static SymbolHandler symbolHandler;

        protected override string BoolValue
            => (string)Settings.GetWriteData().First()[0];

        protected override IEnumerable<object[]> Data
            => Settings.GetWriteData();

        [ClassInitialize]
        public static async Task ConnectAsync(TestContext testContext)
        {
            var connectionsettings = new OPCSettings(Settings.OpcIp, Settings.OpcPort, Settings.RootNode, "PlcConnectionTest", true);
            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, GetLoggerMock<ReadWrite>());
            await connection.ConnectAsync();
            var result = await connection.SessionStream.FirstAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
            => connection.Dispose();

        [TestMethod]
        public override void WriteGeneric()
        {
            WriteValueGenericHelper("WriteTestData.BoolValue", true);
        }

        [TestMethod]
        public override async Task WriteGenericAsync()
        {
            await WriteValueGenericHelperAsync("WriteTestData.BoolValue", true);
        }

        protected override IPlcConnection GetPLCConnection()
            => connection;

        protected override IReadWrite GetReadWrite()
            => readWrite;

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler;
    }
}