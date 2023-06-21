using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlcInterface.Ads.TwinCATAbstractions;
using PlcInterface.IntegrationTests;
using TestUtilities;
using TwinCAT.Ads;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public sealed class ReadValueTest : IReadValueTestBase, IDisposable
{
    private AdsClient? adsClient;
    private PlcConnection? connection;
    private bool disposed;
    private ReadWrite? readWrite;
    private SymbolHandler? symbolHandler;

    [TestInitialize]
    public async Task ConnectAsync()
    {
        var connectionSettings = new AdsPlcConnectionOptions() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        var symbolHandlerSettings = new AdsSymbolHandlerOptions() { StoreSymbolsToDisk = false };
        var typeConverter = new AdsTypeConverter();
        adsClient = new AdsClient();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), Mock.Of<IFileSystem>(), new SymbolLoaderFactoryAbstraction());
        var sumSymbolFactory = new SumSymbolFactory();
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, sumSymbolFactory);
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
        adsClient!.Dispose();
        symbolHandler!.Dispose();
    }

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}