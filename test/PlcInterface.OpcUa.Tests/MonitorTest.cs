﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlcInterface.OpcUa.Tests
{
    [TestClass]
    public class MonitorTest : IMonitorTestBase
    {
        private static PlcConnection connection;
        private static Monitor monitor;
        private static ReadWrite readWrite;
        private static SymbolHandler symbolHandler;

        protected override IEnumerable<string> BooleanVarIONames
            => Settings.GetMonitorData();

        [ClassInitialize]
        public static async Task ConnectAsync(TestContext testContext)
        {
            var connectionsettings = new OPCSettings();
            new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Address = Settings.PLCUri;

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, GetLoggerMock<ReadWrite>());
            monitor = new Monitor(connection, symbolHandler, GetLoggerMock<Monitor>());

            await connection.ConnectAsync();
            _ = await connection.SessionStream.FirstAsync();
        }

        [ClassCleanup]
        public static void Disconnect()
            => connection.Dispose();

        protected override IMonitor GetMonitor()
            => monitor;

        protected override IPlcConnection GetPLCConnection()
            => connection;

        protected override IReadWrite GetReadWrite()
            => readWrite;

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler;
    }
}