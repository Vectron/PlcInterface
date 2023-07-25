using System;
using PlcInterface.Ads;
using TwinCAT.Ads.TcpRouter;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to disconnect from the ads PLC.
/// </summary>
internal class AdsPlcDisconnectCommand : PlcDisconnectCommand
{
    private readonly IAmsRouter amsRouter;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdsPlcDisconnectCommand"/> class.
    /// </summary>
    /// <param name="plcConnection">A <see cref="IPlcConnection"/> instance.</param>
    /// <param name="amsRouter">A <see cref="IAmsRouter"/> for running without TwinCAT installation.</param>
    public AdsPlcDisconnectCommand(IAdsPlcConnection plcConnection, IAmsRouter amsRouter)
        : base("ads", plcConnection)
        => this.amsRouter = amsRouter;

    /// <inheritdoc/>
    public override void Execute(string[] arguments)
    {
        base.Execute(arguments);
        if (Environment.GetEnvironmentVariable("TWINCAT3DIR") == null)
        {
            amsRouter.Stop();
        }
    }
}