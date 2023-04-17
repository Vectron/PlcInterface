using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.OpcUa;
using PlcInterface.Tests;
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
        var connectionsettings = new OpcPlcConnectionOptions();
        new DefaultOpcPlcConnectionConfigureOptions().Configure(connectionsettings);
        connectionsettings.Address = Settings.PLCUri;

        connection?.Dispose();
        symbolHandler?.Dispose();
        readWrite?.Dispose();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        var typeConverter = new OpcTypeConverter(symbolHandler);
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<ReadWrite>());

        await connection.ConnectAsync();
        _ = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(1));
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
        connection?.Dispose();
        symbolHandler?.Dispose();
        readWrite?.Dispose();
    }

    protected override IReadWrite GetReadWrite()
    {
        Assert.IsNotNull(readWrite);
        return readWrite;
    }
}