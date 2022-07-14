using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PlcInterface.Ads;
using PlcInterface.OpcUa;
using PlcInterface.Sandbox.Commands;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox;

/// <summary>
/// The main entry point class.
/// </summary>
internal static class Program
{
    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        => services.AddOptions()
            .AddSingleton(context.Configuration)
            .AddHostedService<InteractiveConsoleService>()
            .AddScoped<IAutoCompleteHandler, AutoCompletionHandler>()
            .AddTransient<IApplicationCommand, CloseApplicationCommand>()
            .AddTransient<IApplicationCommand, HelpCommand>()
            .AddTransient<IApplicationCommand, AdsPlcCommand>()
            .AddTransient<IApplicationCommand, OpcPlcCommand>()
            .AddAdsPLC()
            .AddOpcPLC()
            .Configure<AdsPlcConnectionOptions>(context.Configuration.GetSection("Ads"))
            .Configure<OpcPlcConnectionOptions>(context.Configuration.GetSection("Opc"));

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