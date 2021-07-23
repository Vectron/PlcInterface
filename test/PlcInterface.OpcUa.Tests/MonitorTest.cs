﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;

namespace PlcInterface.OpcUa.Tests
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
            var connectionsettings = new OPCSettings();
            new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
            connectionsettings.Address = Settings.PLCUri;
            var typeConverter = new OpcTypeConverter();

            connection = new PlcConnection(GetOptionsMoq(connectionsettings), GetLoggerMock<PlcConnection>());
            symbolHandler = new SymbolHandler(connection, GetLoggerMock<SymbolHandler>());
            readWrite = new ReadWrite(connection, symbolHandler, typeConverter, GetLoggerMock<ReadWrite>());
            monitor = new Monitor(connection, symbolHandler, typeConverter, GetLoggerMock<Monitor>());

            await connection.ConnectAsync();
            _ = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(1));
        }

        [ClassCleanup]
        public static void Disconnect()
        {
            connection?.Dispose();
            symbolHandler?.Dispose();
            readWrite?.Dispose();
            monitor?.Dispose();
        }

        protected override IMonitor GetMonitor()
            => monitor ?? throw new System.NotSupportedException();

        protected override IPlcConnection GetPLCConnection()
            => connection ?? throw new System.NotSupportedException();

        protected override IReadWrite GetReadWrite()
            => readWrite ?? throw new System.NotSupportedException();

        protected override ISymbolHandler GetSymbolHandler()
            => symbolHandler ?? throw new System.NotSupportedException();
    }
}