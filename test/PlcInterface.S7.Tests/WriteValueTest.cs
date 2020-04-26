using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.S7.Tests.DataTypes;
using PlcInterface.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.S7.Tests
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
            var connectionsettings = new ConnectionSettings();
            new DefaultConnectionSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Adress = Settings.Ip;

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, GetLoggerMock<ReadWrite>());
            await connection.ConnectAsync();
            _ = await connection.SessionStream.FirstAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
            => connection.Dispose();

        [TestInitialize]
        public void ResetPLCValues()
        {
            var readWrite = GetReadWrite();
            readWrite.Write("MAIN.Reset", true);
        }

        [TestMethod]
        public override void WriteGeneric()
        {
            WriteValueGenericHelper("WriteTestData.BoolValue", false);
            WriteValueGenericHelper("WriteTestData.ByteValue", byte.MinValue);
            WriteValueGenericHelper("WriteTestData.WordValue", ushort.MinValue);
            WriteValueGenericHelper("WriteTestData.DWordValue", uint.MinValue);
            WriteValueGenericHelper("WriteTestData.LWordValue", ulong.MinValue);
            WriteValueGenericHelper("WriteTestData.ShortValue", sbyte.MaxValue);
            WriteValueGenericHelper("WriteTestData.IntValue", short.MaxValue);
            WriteValueGenericHelper("WriteTestData.DIntValue", int.MaxValue);
            WriteValueGenericHelper("WriteTestData.LongValue", long.MaxValue);
            WriteValueGenericHelper("WriteTestData.UShortValue", byte.MinValue);
            WriteValueGenericHelper("WriteTestData.UIntValue", ushort.MinValue);
            WriteValueGenericHelper("WriteTestData.UDIntValue", uint.MinValue);
            WriteValueGenericHelper("WriteTestData.ULongValue", ulong.MinValue);
            WriteValueGenericHelper("WriteTestData.FloatValue", float.MaxValue);
            WriteValueGenericHelper("WriteTestData.DoubleValue", double.MaxValue);
            WriteValueGenericHelper("WriteTestData.TimeValue", 3000u);
            WriteValueGenericHelper("WriteTestData.TimeOfDay", 36000000u);
            WriteValueGenericHelper("WriteTestData.LTimeValue", 10000ul);
            WriteValueGenericHelper("WriteTestData.DateValue", new DateTime(2019, 02, 22));
            WriteValueGenericHelper("WriteTestData.DateAndTimeValue", new DateTime(2019, 02, 22, 12, 15, 10));
            WriteValueGenericHelper("WriteTestData.StringValue", "new Test String");
            WriteValueGenericHelper("WriteTestData.WStringValue", "new Test WString");
            WriteValueGenericHelper("WriteTestData.EnumValue", (int)TestEnum.third);
            WriteValueGenericHelper("WriteTestData.IntArray", new short[] { 10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 });
        }

        [TestMethod]
        public override async Task WriteGenericAsync()
        {
            await WriteValueGenericHelperAsync("WriteTestData.BoolValue", false);
            await WriteValueGenericHelperAsync("WriteTestData.ByteValue", byte.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.WordValue", ushort.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.DWordValue", uint.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.LWordValue", ulong.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.ShortValue", sbyte.MaxValue);
            await WriteValueGenericHelperAsync("WriteTestData.IntValue", short.MaxValue);
            await WriteValueGenericHelperAsync("WriteTestData.DIntValue", int.MaxValue);
            await WriteValueGenericHelperAsync("WriteTestData.LongValue", long.MaxValue);
            await WriteValueGenericHelperAsync("WriteTestData.UShortValue", byte.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.UIntValue", ushort.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.UDIntValue", uint.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.ULongValue", ulong.MinValue);
            await WriteValueGenericHelperAsync("WriteTestData.FloatValue", float.MaxValue);
            await WriteValueGenericHelperAsync("WriteTestData.DoubleValue", double.MaxValue);
            await WriteValueGenericHelperAsync("WriteTestData.TimeValue", 3000u);
            await WriteValueGenericHelperAsync("WriteTestData.TimeOfDay", 36000000u);
            await WriteValueGenericHelperAsync("WriteTestData.LTimeValue", 10000ul);
            await WriteValueGenericHelperAsync("WriteTestData.DateValue", new DateTime(2019, 02, 22));
            await WriteValueGenericHelperAsync("WriteTestData.DateAndTimeValue", new DateTime(2019, 02, 22, 12, 15, 10));
            await WriteValueGenericHelperAsync("WriteTestData.StringValue", "new Test String");
            await WriteValueGenericHelperAsync("WriteTestData.WStringValue", "new Test WString");
            await WriteValueGenericHelperAsync("WriteTestData.EnumValue", (int)TestEnum.third);
            await WriteValueGenericHelperAsync("WriteTestData.IntArray", new short[] { 10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 });
        }

        protected override IPlcConnection GetPLCConnection()
            => connection;

        protected override IReadWrite GetReadWrite()
            => readWrite;

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler;
    }
}