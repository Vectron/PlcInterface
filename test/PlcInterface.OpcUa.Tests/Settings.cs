using System;
using System.Collections.Generic;

namespace PlcInterface.OpcUa.Tests
{
    internal static class Settings
    {
        public static string OpcIp
            => "172.22.50.5";

        public static int OpcPort
            => 4840;

        public static string RootNode
            => "PLC_MCC/DataBlocksGlobal/VIS1_OPC_DB";

        public static IEnumerable<string> GetMonitorData()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<object[]> GetReadData()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<object[]> GetWriteData()
        {
            throw new NotImplementedException();
        }
    }
}