using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlcInterface.Ads.TwincatAbstractions;
using PlcInterface.Tests;
using TestUtilities;
using TwinCAT.Ads;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public class WriteValueTest : IWriteValueTestBase
{
    private static AdsClient? adsClient;
    private static PlcConnection? connection;
    private static ReadWrite? readWrite;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionsettings = new AdsPlcConnectionOptions() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        var symbolhandlersettings = new AdsSymbolHandlerOptions() { StoreSymbolsToDisk = false };
        var typeConverter = new AdsTypeConverter();
        adsClient = new AdsClient();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetOptionsMoq(symbolhandlersettings), MockHelpers.GetLoggerMock<SymbolHandler>(), Mock.Of<IFileSystem>(), new SymbolLoaderFactoryAbstraction());
        var sumSymbolFactory = new SumSymbolFactory();
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, sumSymbolFactory);
        if (!await connection.ConnectAsync())
        {
            throw new InvalidOperationException("Connection to PLC Failed");
        }
    }

    [ClassCleanup]
    public static void Disconnect()
    {
        connection!.Dispose();
        adsClient!.Dispose();
        symbolHandler!.Dispose();
    }

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}