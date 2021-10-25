using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.OpcUa;
using PlcInterface.Tests;
using TestUtilities;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
public class SymbolHandlerTest : ISymbolHandlerTestBase
{
    private static PlcConnection? connection;
    private static SymbolHandler? symbolHandler;

    [ClassInitialize]
    public static async Task ConnectAsync(TestContext testContext)
    {
        var connectionsettings = new OPCSettings();
        new DefaultOPCSettingsConfigureOptions().Configure(connectionsettings);
        connectionsettings.Address = Settings.PLCUri;

        connection = new PlcConnection(MockHelpers.GetOptionsMoq(connectionsettings), MockHelpers.GetLoggerMock<PlcConnection>());
        symbolHandler = new SymbolHandler(connection, MockHelpers.GetLoggerMock<SymbolHandler>());
        await connection.ConnectAsync();
        _ = await connection.GetConnectedClientAsync(TimeSpan.FromSeconds(1));
    }

    [ClassCleanup]
    public static void Disconnect()
    {
        connection!.Dispose();
        symbolHandler!.Dispose();
    }

    protected override ISymbolHandler GetSymbolHandler()
        => symbolHandler!;
}