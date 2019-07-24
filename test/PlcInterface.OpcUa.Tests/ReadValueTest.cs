using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.OpcUa.Tests.DataTypes;
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