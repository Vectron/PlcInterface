using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlcInterface.Ads;
using PlcInterface.OpcUa;
using PlcInterface.Sandbox.PLCCommands;
using TwinCAT.Ads.TcpRouter;
using Vectron.Extensions.Logging.Console.Formatter;
using Vectron.InteractiveConsole;
using Vectron.InteractiveConsole.AutoComplete;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox;

/// <summary>
/// The main entry point class.
/// </summary>
internal static class Program
{
    private const string AdsBaseCommand = "ads";
    private const string OpcBaseCommand = "opc";

    /// <summary>
    /// The main entry point.
    /// </summary>
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        _ = builder.Logging
            .AddSingleLineConsole();

        _ = builder.Services
            .AddConsoleCommand()
            .AddScoped<IConsoleCommand, AdsPlcConnectCommand>()
            .AddScoped<IConsoleCommand, AdsPlcDisconnectCommand>()
            .AddScoped<IConsoleCommand, PlcReadCommand>(x => new PlcReadCommand(AdsBaseCommand, x.GetRequiredService<IAdsReadWrite>()))
            .AddScoped<IConsoleCommand, PlcToggleCommand>(x => new PlcToggleCommand(AdsBaseCommand, x.GetRequiredService<IAdsReadWrite>()))
            .AddScoped<IConsoleCommand, AdsWriteCommand>()
            .AddScoped<IConsoleCommand, PlcMonitorCommand>(x => new PlcMonitorCommand(AdsBaseCommand, x.GetRequiredService<IAdsMonitor>()))
            .AddScoped<IConsoleCommand, PlcStopMonitorCommand>(x => new PlcStopMonitorCommand(AdsBaseCommand, x.GetRequiredService<IAdsMonitor>()))
            .AddScoped<IConsoleCommand, PlcSymbolDumpCommand>(x => new PlcSymbolDumpCommand(AdsBaseCommand, x.GetRequiredService<IAdsSymbolHandler>()))
            .AddScoped<IAutoCompleteHandler, PlcSymbolAutoCompleteHandler>(x => new PlcSymbolAutoCompleteHandler(AdsBaseCommand, x.GetRequiredService<IAdsSymbolHandler>()))
            .AddScoped<IConsoleCommand, PlcConnectCommand>(x => new PlcConnectCommand(OpcBaseCommand, x.GetRequiredService<IOpcPlcConnection>()))
            .AddScoped<IConsoleCommand, PlcDisconnectCommand>(x => new PlcDisconnectCommand(OpcBaseCommand, x.GetRequiredService<IOpcPlcConnection>()))
            .AddScoped<IConsoleCommand, PlcReadCommand>(x => new PlcReadCommand(OpcBaseCommand, x.GetRequiredService<IOpcReadWrite>()))
            .AddScoped<IConsoleCommand, PlcToggleCommand>(x => new PlcToggleCommand(OpcBaseCommand, x.GetRequiredService<IOpcReadWrite>()))
            .AddScoped<IConsoleCommand, OpcWriteCommand>()
            .AddScoped<IConsoleCommand, PlcMonitorCommand>(x => new PlcMonitorCommand(OpcBaseCommand, x.GetRequiredService<IOpcMonitor>()))
            .AddScoped<IConsoleCommand, PlcStopMonitorCommand>(x => new PlcStopMonitorCommand(OpcBaseCommand, x.GetRequiredService<IOpcMonitor>()))
            .AddScoped<IConsoleCommand, PlcSymbolDumpCommand>(x => new PlcSymbolDumpCommand(OpcBaseCommand, x.GetRequiredService<IOpcSymbolHandler>()))
            .AddScoped<IAutoCompleteHandler, PlcSymbolAutoCompleteHandler>(x => new PlcSymbolAutoCompleteHandler(OpcBaseCommand, x.GetRequiredService<IOpcSymbolHandler>()));

        _ = builder.Services
            .AddAdsPLC()
            .AddSingleton<IAmsRouter>(x => new AmsTcpIpRouter(x.GetRequiredService<ILogger<AmsTcpIpRouter>>(), x.GetRequiredService<IConfiguration>()))
            .AddOptions<AdsPlcConnectionOptions>()
                .BindConfiguration(AdsBaseCommand);

        _ = builder.Services
            .AddOpcPLC()
            .AddOptions<OpcPlcConnectionOptions>()
                .BindConfiguration(OpcBaseCommand);

        try
        {
            var host = builder.Build();
            await host.RunAsync(CancellationToken.None)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            System.Diagnostics.Debugger.Break();
        }
    }
}
