using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;
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
        var connectionSettings = new OpcPlcConnectionOptions();
        new DefaultOpcPlcConnectionConfigureOptions().Configure(connectionSettings);
        connectionSettings.Address = Settings.PLCUri;

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        var typeConverter = new OpcTypeConverter(symbolHandler);
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<ReadWrite>());
        monitor = new Monitor(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<Monitor>());
    }

    protected override IMonitor GetMonitor()
        => monitor!;

    protected override IPlcConnection GetPLCConnection(bool connected = true)
    {
        if (!connected)
        {
            return connection!;
        }

        var isConnected = connection?.Connect();
        if (isConnected == false)
        {
            throw new InvalidOperationException("Connection to PLC Failed");
        }

        return connection!;
    }

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}