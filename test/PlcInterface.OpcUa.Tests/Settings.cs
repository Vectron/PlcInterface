using PlcInterface.OpcUa.Tests.DataTypes;
using System;
using System.Collections.Generic;

namespace PlcInterface.OpcUa.Tests
{
    internal static class Settings
    {
        public static string OpcIp
            //=> "172.22.50.5";
            => "localhost";

        public static int OpcPort
            => 4840;

        public static Uri PLCUri
            => new UriBuilder("opc.tcp", OpcIp, OpcPort, RootNode).Uri;

        public static Uri PLCUriNoRoot
            => new UriBuilder("opc.tcp", OpcIp, OpcPort, string.Empty).Uri;

        public static string RootNode
            => "PLC1";

        public static IEnumerable<string> GetMonitorData()
        {
            yield return "MonitorTestData.BoolValue1";
            yield return "MonitorTestData.BoolValue2";
            yield return "MonitorTestData.BoolValue3";
            yield return "MonitorTestData.BoolValue4";
            yield return "MonitorTestData.BoolValue5";
            yield return "MonitorTestData.BoolValue6";
            yield return "MonitorTestData.BoolValue7";
            yield return "MonitorTestData.BoolValue8";
        }

        public static IEnumerable<object[]> GetReadData()
        {
            yield return new object[] { "ReadTestData.BoolValue", true };
            yield return new object[] { "ReadTestData.ByteValue", byte.MaxValue };
            yield return new object[] { "ReadTestData.WordValue", ushort.MaxValue };
            yield return new object[] { "ReadTestData.DWordValue", uint.MaxValue };
            yield return new object[] { "ReadTestData.LWordValue", ulong.MaxValue };
            yield return new object[] { "ReadTestData.ShortValue", sbyte.MinValue };
            yield return new object[] { "ReadTestData.IntValue", short.MinValue };
            yield return new object[] { "ReadTestData.DIntValue", int.MinValue };
            yield return new object[] { "ReadTestData.LongValue", long.MinValue };
            yield return new object[] { "ReadTestData.UShortValue", byte.MaxValue };
            yield return new object[] { "ReadTestData.UIntValue", ushort.MaxValue };
            yield return new object[] { "ReadTestData.UDIntValue", uint.MaxValue };
            yield return new object[] { "ReadTestData.ULongValue", ulong.MaxValue };
            yield return new object[] { "ReadTestData.FloatValue", -3.402823E+38F };
            yield return new object[] { "ReadTestData.DoubleValue", -1.79769313486231E+308 };
            yield return new object[] { "ReadTestData.TimeValue", 1000u };
            yield return new object[] { "ReadTestData.TimeOfDay", 3600000u };
            yield return new object[] { "ReadTestData.LTimeValue", 1000ul };
            yield return new object[] { "ReadTestData.DateValue", new DateTime(2106, 02, 06) };
            yield return new object[] { "ReadTestData.DateAndTimeValue", new DateTime(2106, 02, 06, 06, 28, 15) };
            yield return new object[] { "ReadTestData.StringValue", "Test String" };
            yield return new object[] { "ReadTestData.WStringValue", "Test WString" };
            yield return new object[] { "ReadTestData.EnumValue", (int)TestEnum.second };
            yield return new object[] { "ReadTestData.IntArray", new short[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010 } };
        }

        public static IEnumerable<object[]> GetWriteData()
        {
            yield return new object[] { "WriteTestData.BoolValue", false };
            yield return new object[] { "WriteTestData.ByteValue", byte.MinValue };
            yield return new object[] { "WriteTestData.WordValue", ushort.MinValue };
            yield return new object[] { "WriteTestData.DWordValue", uint.MinValue };
            yield return new object[] { "WriteTestData.LWordValue", ulong.MinValue };
            yield return new object[] { "WriteTestData.ShortValue", sbyte.MaxValue };
            yield return new object[] { "WriteTestData.IntValue", short.MaxValue };
            yield return new object[] { "WriteTestData.DIntValue", int.MaxValue };
            yield return new object[] { "WriteTestData.LongValue", long.MaxValue };
            yield return new object[] { "WriteTestData.UShortValue", byte.MinValue };
            yield return new object[] { "WriteTestData.UIntValue", ushort.MinValue };
            yield return new object[] { "WriteTestData.UDIntValue", uint.MinValue };
            yield return new object[] { "WriteTestData.ULongValue", ulong.MinValue };
            yield return new object[] { "WriteTestData.FloatValue", float.MaxValue };
            yield return new object[] { "WriteTestData.DoubleValue", double.MaxValue };
            yield return new object[] { "WriteTestData.TimeValue", 3000u };
            yield return new object[] { "WriteTestData.TimeOfDay", 36000000u };
            yield return new object[] { "WriteTestData.LTimeValue", 10000ul };
            yield return new object[] { "WriteTestData.DateValue", new DateTime(2019, 02, 22) };
            yield return new object[] { "WriteTestData.DateAndTimeValue", new DateTime(2019, 02, 22, 12, 15, 10) };
            yield return new object[] { "WriteTestData.StringValue", "new Test String" };
            yield return new object[] { "WriteTestData.WStringValue", "new Test WString" };
            yield return new object[] { "WriteTestData.EnumValue", (int)TestEnum.third };
            yield return new object[] { "WriteTestData.IntArray", new short[] { 10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 } };
        }
    }
}