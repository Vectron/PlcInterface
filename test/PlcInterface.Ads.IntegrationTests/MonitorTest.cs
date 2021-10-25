﻿using System;
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
public class MonitorTest : IMonitorTestBase
{
    private static AdsClient? adsClient;
    private static PlcConnection? connection;
    private static Monitor? monitor;
    private static ReadWrite? readWrite;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionsettings = new ConnectionSettings() { AmsNetId = Settings.AmsNetId, Port = Settings.Port };
        var symbolhandlersettings = new SymbolHandlerSettings() { StoreSymbolsToDisk = false };
        var typeConverter = new AdsTypeConverter();
        adsClient = new AdsClient();

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClient);
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetOptionsMoq(symbolhandlersettings), MockHelpers.GetLoggerMock<SymbolHandler>(), Mock.Of<IFileSystem>(), new SymbolLoaderFactoryAbstraction());
        var sumSymbolFactory = new SumSymbolFactory();
        readWrite = new ReadWrite(connection, symbolHandler, typeConverter, sumSymbolFactory);
        monitor = new Monitor(connection, symbolHandler, typeConverter, MockHelpers.GetLoggerMock<Monitor>());
        await connection.ConnectAsync();
        _ = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(1));
    }

    [ClassCleanup]
    public static void Disconnect()
    {
        connection?.Dispose();
        symbolHandler?.Dispose();
        monitor?.Dispose();
        adsClient?.Dispose();
    }

    protected override IMonitor GetMonitor()
        => monitor!;

    protected override IPlcConnection GetPLCConnection()
        => connection!;

    protected override IReadWrite GetReadWrite()
        => readWrite!;
}