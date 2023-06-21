using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;
using TestUtilities;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
public class WriteValueTest : IWriteValueTestBase
{
    private static PlcConnection? connection;
    private static ReadWrite? readWrite;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionSettings = new OpcPlcConnectionOptions();
        new DefaultOpcPlcConnectionConfigureOptions().Configure(connectionSettings);
        connectionSettings.Address = Settings.PLCUri;

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        var typeConverter = new OpcTypeConverter(symbolHandler);
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<ReadWrite>());
        if (!await connection.ConnectAsync())
        {
            throw new InvalidOperationException("Connection to PLC Failed");
        }
    }

    [ClassCleanup]
    public static void Disconnect()
    {
        connection!.Dispose();
        symbolHandler!.Dispose();
        readWrite!.Dispose();
    }

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}