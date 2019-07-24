using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.OpcUa.Tests
{
    [TestClass]
    public class ReadValueTest : IReadValueTestBase
    {
        private static PlcConnection connection;
        private static ReadWrite readWrite;
        private static SymbolHandler symbolHandler;

        protected override string BoolValue
            => (string)Settings.GetWriteData().First()[0];

        protected override IEnumerable<object[]> Data
            => Settings.GetReadData();

        [ClassInitialize]
        public static async Task ConnectAsync(TestContext testContext)
        {
            var connectionsettings = new OPCSettings();
            new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Address = Settings.PLCUri;

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, GetLoggerMock<ReadWrite>());

            await connection.ConnectAsync();
            _ = await connection.SessionStream.FirstAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
            => connection.Dispose();

        public override void ReadDynamic()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadDynamicAsync()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public override void ReadGeneric()
        {
            ReadValueGenericHelper("ReadTestData.BoolValue", true);
        }

        [TestMethod]
        public override async Task ReadGenericAsync()
        {
            await ReadValueGenericHelperAsync("ReadTestData.BoolValue", true);
        }

        protected override IPlcConnection GetPLCConnection()
            => connection;

        protected override IReadWrite GetReadWrite()
            => readWrite;

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler;
    }
}