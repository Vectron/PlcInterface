using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PlcInterface.Ads;
using PlcInterface.OpcUa;
using PlcInterface.Sandbox.PLCCommands;
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

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        => services.AddOptions()
            .AddSingleton(context.Configuration)
            .AddInteractiveConsole()
            .AddConsoleCommand()
            .AddScoped<IConsoleCommand, PlcConnectCommand>(x => new PlcConnectCommand(AdsBaseCommand, x.GetRequiredService<IAdsPlcConnection>()))
            .AddScoped<IConsoleCommand, PlcDisconnectCommand>(x => new PlcDisconnectCommand(AdsBaseCommand, x.GetRequiredService<IAdsPlcConnection>()))
            .AddScoped<IConsoleCommand, PlcReadCommand>(x => new PlcReadCommand(AdsBaseCommand, x.GetRequiredService<IAdsReadWrite>()))
            .AddScoped<IConsoleCommand, PlcToggleCommand>(x => new PlcToggleCommand(AdsBaseCommand, x.GetRequiredService<IAdsReadWrite>()))
            .AddScoped<IConsoleCommand, AdsWriteCommand>()
            .AddScoped<IConsoleCommand, PlcMonitorCommand>(x => new PlcMonitorCommand(AdsBaseCommand, x.GetRequiredService<IAdsMonitor>()))
            .AddScoped<IConsoleCommand, PlcStopMonitorCommand>(x => new PlcStopMonitorCommand(AdsBaseCommand, x.GetRequiredService<IAdsMonitor>()))
            .AddScoped<IConsoleCommand, PlcSymbolDumpCommand>(x => new PlcSymbolDumpCommand(AdsBaseCommand, x.GetRequiredService<IAdsSymbolHandler>()))
            .AddScoped<IConsoleCommand, PlcConnectCommand>(x => new PlcConnectCommand(OpcBaseCommand, x.GetRequiredService<IOpcPlcConnection>()))
            .AddScoped<IConsoleCommand, PlcDisconnectCommand>(x => new PlcDisconnectCommand(OpcBaseCommand, x.GetRequiredService<IOpcPlcConnection>()))
            .AddScoped<IConsoleCommand, PlcReadCommand>(x => new PlcReadCommand(OpcBaseCommand, x.GetRequiredService<IOpcReadWrite>()))
            .AddScoped<IConsoleCommand, PlcToggleCommand>(x => new PlcToggleCommand(OpcBaseCommand, x.GetRequiredService<IOpcReadWrite>()))
            .AddScoped<IConsoleCommand, OpcWriteCommand>()
            .AddScoped<IConsoleCommand, PlcMonitorCommand>(x => new PlcMonitorCommand(OpcBaseCommand, x.GetRequiredService<IOpcMonitor>()))
            .AddScoped<IConsoleCommand, PlcStopMonitorCommand>(x => new PlcStopMonitorCommand(OpcBaseCommand, x.GetRequiredService<IOpcMonitor>()))
            .AddScoped<IConsoleCommand, PlcSymbolDumpCommand>(x => new PlcSymbolDumpCommand(OpcBaseCommand, x.GetRequiredService<IOpcSymbolHandler>()))
            .AddScoped<IAutoCompleteHandler, PlcSymbolAutoCompleteHandler>(x => new PlcSymbolAutoCompleteHandler(AdsBaseCommand, x.GetRequiredService<IAdsSymbolHandler>()))
            .AddScoped<IAutoCompleteHandler, PlcSymbolAutoCompleteHandler>(x => new PlcSymbolAutoCompleteHandler(OpcBaseCommand, x.GetRequiredService<IOpcSymbolHandler>()))
            .AddAdsPLC()
            .AddOpcPLC()
            .Configure<AdsPlcConnectionOptions>(context.Configuration.GetSection(AdsBaseCommand))
            .Configure<OpcPlcConnectionOptions>(context.Configuration.GetSection(OpcBaseCommand));

    /// <summary>
    /// The main entry point.
    /// </summary>
    private static async Task Main(string[] args)
    {
        var logger = NLog.LogManager.GetCurrentClassLogger();

        try
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureLogging(SetupLogging)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync(CancellationToken.None)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Stopped program because of exception");
            throw;
        }
        finally
        {
            NLog.LogManager.Shutdown();
        }
    }

    // configure Logging with NLog
    private static void SetupLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        => loggingBuilder.ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog(context.Configuration);
}