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
public class SymbolHandlerTest : ISymbolHandlerTestBase
{
    private static AdsClient? adsClient;
    private static PlcConnection? connection;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionsettings = new AdsPlcConnectionOptions() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        var symbolhandlersettings = new AdsSymbolHandlerOptions() { StoreSymbolsToDisk = false };
        adsClient = new AdsClient();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetOptionsMoq(symbolhandlersettings), MockHelpers.GetLoggerMock<SymbolHandler>(), Mock.Of<IFileSystem>(), new SymbolLoaderFactoryAbstraction());
        await connection.ConnectAsync();
        _ = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(1));
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