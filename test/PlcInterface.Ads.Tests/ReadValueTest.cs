using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Ads.Tests.DataTypes;
using PlcInterface.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.Ads.Tests
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
            var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
            var symbolhandlersettings = new SymbolHandlerSettings() { StoreSymbolsToDisk = false };
            var dynamicValueConverter = new DynamicValueConverter();

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetOptionsMoq(symbolhandlersettings), GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, dynamicValueConverter, GetLoggerMock<ReadWrite>());
            await connection.ConnectAsync();
            var result = await connection.SessionStream.FirstAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
            => connection.Dispose();

        [TestMethod]
        public override void ReadDynamic()
        {
            var structValue = readWrite.ReadDynamic("ReadTestData.StructValue");
            var expected = DUT_TestStruct.Default;
            AssertDUT_TestStruct(expected, structValue);
        }

        [TestMethod]
        public override async Task ReadDynamicAsync()
        {
            var structValue = await readWrite.ReadDynamicAsync("ReadTestData.StructValue");
            var expected = DUT_TestStruct.Default;
            AssertDUT_TestStruct(expected, structValue);
        }

        [TestMethod]
        public override void ReadGeneric()
        {
            ReadValueGenericHelper("ReadTestData.BoolValue", true);
            ReadValueGenericHelper("ReadTestData.ByteValue", byte.MaxValue);
            ReadValueGenericHelper("ReadTestData.WordValue", ushort.MaxValue);
            ReadValueGenericHelper("ReadTestData.DWordValue", uint.MaxValue);
            ReadValueGenericHelper("ReadTestData.LWordValue", ulong.MaxValue);
            ReadValueGenericHelper("ReadTestData.ShortValue", sbyte.MinValue);
            ReadValueGenericHelper("ReadTestData.IntValue", short.MinValue);
            ReadValueGenericHelper("ReadTestData.DIntValue", int.MinValue);
            ReadValueGenericHelper("ReadTestData.LongValue", long.MinValue);
            ReadValueGenericHelper("ReadTestData.UShortValue", byte.MaxValue);
            ReadValueGenericHelper("ReadTestData.UIntValue", ushort.MaxValue);
            ReadValueGenericHelper("ReadTestData.UDIntValue", uint.MaxValue);
            ReadValueGenericHelper("ReadTestData.ULongValue", ulong.MaxValue);
            ReadValueGenericHelper("ReadTestData.FloatValue", -3.402823E+38F);
            ReadValueGenericHelper("ReadTestData.DoubleValue", -1.79769313486231E+308);
            ReadValueGenericHelper("ReadTestData.TimeValue", TimeSpan.FromMilliseconds(1000));
            ReadValueGenericHelper("ReadTestData.TimeOfDay", TimeSpan.FromHours(1));
            ReadValueGenericHelper("ReadTestData.LTimeValue", TimeSpan.FromTicks(10));
            ReadValueGenericHelper("ReadTestData.DateValue", new DateTime(2106, 02, 05));
            ReadValueGenericHelper("ReadTestData.DateAndTimeValue", new DateTime(2106, 02, 05, 06, 28, 15));
            ReadValueGenericHelper("ReadTestData.StringValue", "Test String");
            ReadValueGenericHelper("ReadTestData.WStringValue", "Test WString");
            ReadValueGenericHelper("ReadTestData.EnumValue", TestEnum.second);
            ReadValueGenericHelper("ReadTestData.IntArray", new short[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010 });
            ReadValueGenericHelper("ReadTestData.StructValue", DUT_TestStruct.Default);
            ReadValueGenericHelper("ReadTestData.StructValue", DUT_TestStructClass.Default);
        }

        [TestMethod]
        public override async Task ReadGenericAsync()
        {
            await ReadValueGenericHelperAsync("ReadTestData.BoolValue", true);
            await ReadValueGenericHelperAsync("ReadTestData.ByteValue", byte.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.WordValue", ushort.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.DWordValue", uint.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.LWordValue", ulong.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.ShortValue", sbyte.MinValue);
            await ReadValueGenericHelperAsync("ReadTestData.IntValue", short.MinValue);
            await ReadValueGenericHelperAsync("ReadTestData.DIntValue", int.MinValue);
            await ReadValueGenericHelperAsync("ReadTestData.LongValue", long.MinValue);
            await ReadValueGenericHelperAsync("ReadTestData.UShortValue", byte.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.UIntValue", ushort.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.UDIntValue", uint.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.ULongValue", ulong.MaxValue);
            await ReadValueGenericHelperAsync("ReadTestData.FloatValue", -3.402823E+38F);
            await ReadValueGenericHelperAsync("ReadTestData.DoubleValue", -1.79769313486231E+308);
            await ReadValueGenericHelperAsync("ReadTestData.TimeValue", TimeSpan.FromMilliseconds(1000));
            await ReadValueGenericHelperAsync("ReadTestData.TimeOfDay", TimeSpan.FromHours(1));
            await ReadValueGenericHelperAsync("ReadTestData.LTimeValue", TimeSpan.FromTicks(10));
            await ReadValueGenericHelperAsync("ReadTestData.DateValue", new DateTime(2106, 02, 05));
            await ReadValueGenericHelperAsync("ReadTestData.DateAndTimeValue", new DateTime(2106, 02, 05, 06, 28, 15));
            await ReadValueGenericHelperAsync("ReadTestData.StringValue", "Test String");
            await ReadValueGenericHelperAsync("ReadTestData.WStringValue", "Test WString");
            await ReadValueGenericHelperAsync("ReadTestData.EnumValue", TestEnum.second);
            await ReadValueGenericHelperAsync("ReadTestData.IntArray", new short[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010 });
            await ReadValueGenericHelperAsync("ReadTestData.StructValue", DUT_TestStruct.Default);
            await ReadValueGenericHelperAsync("ReadTestData.StructValue", DUT_TestStructClass.Default);
        }

        protected override IPlcConnection GetPLCConnection()
            => connection;

        protected override IReadWrite GetReadWrite()
            => readWrite;

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler;

        private void AssertDUT_TestStruct(DUT_TestStruct expected, dynamic current)
        {
            Assert.AreEqual(expected.BoolValue, current.BoolValue);

            Assert.AreEqual(expected.ByteValue, current.ByteValue);
            Assert.AreEqual(expected.WordValue, current.WordValue);
            Assert.AreEqual(expected.DWordValue, current.DWordValue);
            Assert.AreEqual(expected.LWordValue, current.LWordValue);

            Assert.AreEqual(expected.ShortValue, current.ShortValue);
            Assert.AreEqual(expected.IntValue, current.IntValue);
            Assert.AreEqual(expected.DIntValue, current.DIntValue);
            Assert.AreEqual(expected.LongValue, current.LongValue);

            Assert.AreEqual(expected.UShortValue, current.UShortValue);
            Assert.AreEqual(expected.UIntValue, current.UIntValue);
            Assert.AreEqual(expected.UDIntValue, current.UDIntValue);
            Assert.AreEqual(expected.ULongValue, current.ULongValue);

            Assert.AreEqual(expected.FloatValue, current.FloatValue);
            Assert.AreEqual(expected.DoubleValue, current.DoubleValue);
            Assert.AreEqual(expected.TimeValue, current.TimeValue);
            Assert.AreEqual(expected.TimeOfDay, current.TimeOfDay);
            Assert.AreEqual(expected.LTimeValue, current.LTimeValue);
            Assert.AreEqual(expected.DateValue, current.DateValue);
            Assert.AreEqual(expected.DateAndTimeValue, current.DateAndTimeValue);

            Assert.AreEqual(expected.StringValue, current.StringValue);
            Assert.AreEqual(expected.WStringValue, current.WStringValue);

            CollectionAssert.AreEqual(expected.IntArray, current.IntArray);
            CollectionAssert.AreEqual(expected.MultiDimensionArray, current.MultiDimensionArray);

            for (int i = 0; i < expected.ComplexArray.Length - 1; i++)
            {
                Assert.AreEqual(expected.ComplexArray[i].ByteValue, current.ComplexArray[i].ByteValue);
                Assert.AreEqual(expected.ComplexArray[i].WordValue, current.ComplexArray[i].WordValue);
                Assert.AreEqual(expected.ComplexArray[i].DWordValue, current.ComplexArray[i].DWordValue);
                Assert.AreEqual(expected.ComplexArray[i].LWordValue, current.ComplexArray[i].LWordValue);
            }

            for (int i = 0; i < expected.MultiDimensionComplexArray.GetLength(0); i++)
            {
                for (int j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                {
                    for (int k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                    {
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].ByteValue, current.MultiDimensionComplexArray[i, j, k].ByteValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].WordValue, current.MultiDimensionComplexArray[i, j, k].WordValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].DWordValue, current.MultiDimensionComplexArray[i, j, k].DWordValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].LWordValue, current.MultiDimensionComplexArray[i, j, k].LWordValue);
                    }
                }
            }

            Assert.AreEqual(expected.Nested.ByteValue, current.Nested.ByteValue);
            Assert.AreEqual(expected.Nested.WordValue, current.Nested.WordValue);
            Assert.AreEqual(expected.Nested.DWordValue, current.Nested.DWordValue);
            Assert.AreEqual(expected.Nested.LWordValue, current.Nested.LWordValue);
        }
    }
}