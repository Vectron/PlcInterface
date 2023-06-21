using System;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlcInterface.Ads.TwinCATAbstractions;
using PlcInterface.IntegrationTests;
using TestUtilities;
using TwinCAT.Ads;

namespace PlcInterface.Ads.IntegrationTests;

[TestClass]
public class MonitorTest : IMonitorTestBase
{
    private static AdsClient? adsClient;
    private static PlcConnection? connection;
    private static Monitor? monitor;
    private static ReadWrite? readWrite;
    private static SymbolHandler? symbolHandler;

    [ClassCleanup]
    public static void Disconnect()
    {
        connection?.Dispose();
        symbolHandler?.Dispose();
        monitor?.Dispose();
        adsClient?.Dispose();
    }

    [ClassInitialize]
    public static void Setup(TestContext testContext)
    {
        var connectionSettings = new AdsPlcConnectionOptions() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        var symbolHandlerSettings = new AdsSymbolHandlerOptions() { StoreSymbolsToDisk = false };
        var typeConverter = new AdsTypeConverter();
        adsClient = new AdsClient();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionSettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), Mock.Of<IFileSystem>(), new SymbolLoaderFactoryAbstraction());
        var sumSymbolFactory = new SumSymbolFactory();
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, sumSymbolFactory);
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