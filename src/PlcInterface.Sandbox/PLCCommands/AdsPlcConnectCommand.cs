using System;
using System.Threading;
using PlcInterface.Ads;
using TwinCAT.Ads.TcpRouter;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to connect to the PLC through ads.
/// </summary>
internal sealed class AdsPlcConnectCommand : PlcConnectCommand
{
    private readonly IAmsRouter amsRouter;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdsPlcConnectCommand"/> class.
    /// </summary>
    /// <param name="plcConnection">A <see cref="IPlcConnection"/> instance.</param>
    /// <param name="amsRouter">A <see cref="IAmsRouter"/> for running without TwinCAT installation.</param>
    public AdsPlcConnectCommand(IAdsPlcConnection plcConnection, IAmsRouter amsRouter)
        : base("ads", plcConnection)
        => this.amsRouter = amsRouter;

    /// <inheritdoc/>
    public override void Execute(string[] arguments)
    {
        if (Environment.GetEnvironmentVariable("TWINCAT3DIR") == null)
        {
            using var cancellationTokenSource = new CancellationTokenSource(5000);
            _ = amsRouter.StartAsync(cancellationTokenSource.Token);
            while (!amsRouter.IsRunning)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Failed to start the AMS router.");
                    return;
                }

                Thread.Sleep(100);
            }
        }

        base.Execute(arguments);
    }
}