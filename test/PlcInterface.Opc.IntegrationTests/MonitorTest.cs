using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests;
using TestUtilities;

namespace PlcInterface.OpcUa.Tests;

[TestClass]
public class MonitorTest : IMonitorTestBase
{
    private static PlcConnection? connection;
    private static Monitor? monitor;
    private static ReadWrite? readWrite;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionsettings = new OPCSettings();
        new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
        connectionsettings.Address = Settings.PLCUri;
        var typeConverter = new OpcTypeConverter();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<ReadWrite>());
        monitor = new Monitor(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<Monitor>());

        await connection.ConnectAsync();
        _ = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(1));
    }

    [ClassCleanup]
    public static void Disconnect()
    {
        connection!.Dispose();
        symbolHandler!.Dispose();
        readWrite!.Dispose();
        monitor!.Dispose();
    }

    protected override IMonitor GetMonitor()
        => monitor!;

    protected override IPlcConnection GetPLCConnection()
        => connection!;

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}