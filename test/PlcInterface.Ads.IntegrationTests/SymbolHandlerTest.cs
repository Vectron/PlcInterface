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
public class SymbolHandlerTest : ISymbolHandlerTestBase
{
    private static AdsClient? adsClient;
    private static PlcConnection? connection;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionSettings = new AdsPlcConnectionOptions() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        var symbolHandlerSettings = new AdsSymbolHandlerOptions() { StoreSymbolsToDisk = false };
        adsClient = new AdsClient();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), Mock.Of<IFileSystem>(), new SymbolLoaderFactoryAbstraction());
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

    protected override ISymbolHandler GetSymbolHandler()
        => symbolHandler!;
}