using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;
using TestUtilities;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
public sealed class ReadValueTest : IReadValueTestBase, IDisposable
{
    private PlcConnection? connection;
    private bool disposed;
    private ReadWrite? readWrite;
    private SymbolHandler? symbolHandler;

    [TestInitialize]
    public async Task ConnectAsync()
    {
        var connectionSettings = new OpcPlcConnectionOptions();
        new DefaultOpcPlcConnectionConfigureOptions().Configure(connectionSettings);
        connectionSettings.Address = Settings.PLCUri;

        connection?.Dispose();
        symbolHandler?.Dispose();
        readWrite?.Dispose();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        var typeConverter = new OpcTypeConverter(symbolHandler);
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<ReadWrite>());
        if (!await connection.ConnectAsync())
        {
            throw new InvalidOperationException("Connection to PLC Failed");
        }
    }

    [TestCleanup]
    public void Disconnect()
        => Dispose();

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        connection!.Dispose();
        symbolHandler!.Dispose();
        readWrite!.Dispose();
    }

    protected override IReadWrite GetReadWrite()
    {
        Assert.IsNotNull(readWrite);
        return readWrite;
    }
}