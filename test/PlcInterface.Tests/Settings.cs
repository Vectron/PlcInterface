using System;
using System.Collections.Generic;
using System.Linq;
using PlcInterface.Tests.DataTypes;

namespace PlcInterface.Tests
{
    internal static class Settings
    {
        public static IEnumerable<object[]> GetMonitorData()
        {
            yield return new object[] { "MonitorTestData.BoolValue1" };
            yield return new object[] { "MonitorTestData.BoolValue2" };
            yield return new object[] { "MonitorTestData.BoolValue3" };
            yield return new object[] { "MonitorTestData.BoolValue4" };
            yield return new object[] { "MonitorTestData.BoolValue5" };
            yield return new object[] { "MonitorTestData.BoolValue6" };
            yield return new object[] { "MonitorTestData.BoolValue7" };
            yield return new object[] { "MonitorTestData.BoolValue8" };
        }

        public static IEnumerable<string> GetMonitorMultiple()
            => GetMonitorData().Select(x => x[0]).Cast<string>();

        public static IEnumerable<object[]> GetReadData()
            => GetReadMultiple().Select(kv => new object[] { kv.Key, kv.Value });

        public static Dictionary<string, object> GetReadMultiple()
            => new Dictionary<string, object>()
            {
                { "ReadTestData.BoolValue", true },
                { "ReadTestData.ByteValue", byte.MaxValue },
                { "ReadTestData.WordValue", ushort.MaxValue },
                { "ReadTestData.DWordValue", uint.MaxValue },
                { "ReadTestData.LWordValue", ulong.MaxValue },
                { "ReadTestData.ShortValue", sbyte.MinValue },
                { "ReadTestData.IntValue", short.MinValue },
                { "ReadTestData.DIntValue", int.MinValue },
                { "ReadTestData.LongValue", long.MinValue },
                { "ReadTestData.UShortValue", byte.MaxValue },
                { "ReadTestData.UIntValue", ushort.MaxValue },
                { "ReadTestData.UDIntValue", uint.MaxValue },
                { "ReadTestData.ULongValue", ulong.MaxValue },
                { "ReadTestData.FloatValue", -3.402823E+38F },
                { "ReadTestData.DoubleValue", -1.79769313486231E+308 },
                { "ReadTestData.TimeValue", TimeSpan.FromSeconds(1) },
                { "ReadTestData.TimeOfDay", TimeSpan.FromHours(1) },
                { "ReadTestData.LTimeValue", TimeSpan.FromTicks(10) },
                { "ReadTestData.DateValue", new DateTimeOffset(2106, 02, 05, 0, 0, 0, TimeSpan.FromHours(1)) },
                { "ReadTestData.DateAndTimeValue", new DateTimeOffset(2106, 02, 05, 06, 28, 15, TimeSpan.FromHours(1)) },
                { "ReadTestData.StringValue", "Test String" },
                { "ReadTestData.WStringValue", "Test WString" },
                { "ReadTestData.EnumValue", (short)TestEnum.second },
                { "ReadTestData.IntArray", new short[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010 } },
                { "ReadTestData.MultiDimensionArray",  new short[,,]
                    {
                        {
                            { 0100, 0200, 0300, 0400 },
                            { 0500, 0600, 0700, 0800 },
                            { 0900, 1000, 1100, 1200 },
                        },
                        {
                            { 1300, 1400, 1500, 1600 },
                            { 1700, 1800, 1900, 2000 },
                            { 2100, 2200, 2300, 2400 },
                        },
                    }
                },
                { "ReadTestData.ComplexArray", new DUT_TestStruct2[] { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default } },
                { "ReadTestData.MultiDimensionComplexArray", new DUT_TestStruct2[,,]
                    {
                        {
                            { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                            { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                            { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                        },
                        {
                            { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                            { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                            { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                        }
                    }
                },
                { "ReadTestData.StructValue", DUT_TestStruct.Default },
                { "ReadTestData.StructValue2", DUT_TestClass.Default },
            };

        public static IEnumerable<object[]> GetWriteData()
        {
            foreach (var keyValue in GetWriteMultiple())
            {
                yield return new object[] { keyValue.Key, keyValue.Value, keyValue.Value };
            }

            yield return new object[] { "WriteTestData.TimeValue", 3000u, TimeSpan.FromSeconds(3) };
            yield return new object[] { "WriteTestData.TimeOfDay", 36000000u, TimeSpan.FromHours(10) };
            yield return new object[] { "WriteTestData.LTimeValue", 10000ul, TimeSpan.FromTicks(100) };
            yield return new object[] { "WriteTestData.DateValue", new DateTime(2019, 02, 21), new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1)) };
            yield return new object[] { "WriteTestData.DateAndTimeValue", new DateTime(2019, 02, 21, 12, 15, 10), new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1)) };
            yield return new object[] { "WriteTestData.EnumValue", (int)TestEnum.third, (short)TestEnum.third };
        }

        public static Dictionary<string, object> GetWriteMultiple()
            => new Dictionary<string, object>()
            {
                { "WriteTestData.BoolValue", false },
                { "WriteTestData.ByteValue", byte.MinValue },
                { "WriteTestData.WordValue", ushort.MinValue },
                { "WriteTestData.DWordValue", uint.MinValue},
                { "WriteTestData.LWordValue", ulong.MinValue },
                { "WriteTestData.ShortValue", sbyte.MaxValue },
                { "WriteTestData.IntValue", short.MaxValue },
                { "WriteTestData.DIntValue", int.MaxValue },
                { "WriteTestData.LongValue", long.MaxValue },
                { "WriteTestData.UShortValue", byte.MinValue },
                { "WriteTestData.UIntValue", ushort.MinValue },
                { "WriteTestData.UDIntValue", uint.MinValue },
                { "WriteTestData.ULongValue", ulong.MinValue },
                { "WriteTestData.FloatValue", float.MaxValue },
                { "WriteTestData.DoubleValue", double.MaxValue },
                { "WriteTestData.TimeValue", TimeSpan.FromSeconds(3) },
                { "WriteTestData.TimeOfDay", TimeSpan.FromHours(10) },
                { "WriteTestData.LTimeValue", TimeSpan.FromTicks(100) },
                { "WriteTestData.DateValue", new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1)) },
                { "WriteTestData.DateAndTimeValue", new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1)) },
                { "WriteTestData.StringValue", "new Test String" },
                { "WriteTestData.WStringValue", "new Test WString" },
                { "WriteTestData.EnumValue", (short)TestEnum.third },
                { "WriteTestData.IntArray", new short[] { 10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 } },
                { "WriteTestData.MultiDimensionArray",  new short[,,]
                    {
                        {
                            { 01000, 02000, 03000, 04000 },
                            { 05000, 06000, 07000, 08000 },
                            { 09000, 10000, 11000, 12000 },
                        },
                        {
                            { 13000, 14000, 15000, 16000 },
                            { 17000, 18000, 19000, 20000 },
                            { 21000, 22000, 23000, 24000 },
                        },
                    }
                },
                { "WriteTestData.ComplexArray", new DUT_TestStruct2[] { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write } },
                { "WriteTestData.MultiDimensionComplexArray", new DUT_TestStruct2[,,]
                    {
                        {
                            { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                            { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                            { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                        },
                        {
                            { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                            { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                            { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                        }
                    }
                },
                // TODO not supported by ADS (nesting problem)
                //{ "WriteTestData.StructValue", DUT_TestStruct.Write },
                //{ "WriteTestData.StructValue2", DUT_TestClass.Write },
                { "WriteTestData.Nested", DUT_TestStruct2.Write },
                { "WriteTestData.Nested2", DUT_TestClass2.Write },
            };
    }
}