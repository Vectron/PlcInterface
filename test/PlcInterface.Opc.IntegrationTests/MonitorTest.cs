using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.OpcUa;
using PlcInterface.Tests;
using TestUtilities;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
public class MonitorTest : IMonitorTestBase
{
    private static PlcConnection? connection;
    private static Monitor? monitor;
    private static ReadWrite? readWrite;
    private static SymbolHandler? symbolHandler;

    [ClassCleanup]
    public static void Disconnect()
    {
        connection!.Dispose();
        symbolHandler!.Dispose();
        readWrite!.Dispose();
        monitor!.Dispose();
    }

    [ClassInitialize]
    public static void Setup(TestContext testContext)
    {
        var connectionsettings = new OPCSettings();
        new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
        connectionsettings.Address = Settings.PLCUri;
        var typeConverter = new OpcTypeConverter();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<ReadWrite>());
        monitor = new Monitor(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<Monitor>());
    }

    protected override IMonitor GetMonitor()
        => monitor!;

    protected override IPlcConnection GetPLCConnection(bool connected = true)
    {
        if (connected)
        {
            connection?.Connect();
            _ = connection?.GetConnectedClient(TimeSpan.FromSeconds(1));
        }

        return connection!;
    }

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}