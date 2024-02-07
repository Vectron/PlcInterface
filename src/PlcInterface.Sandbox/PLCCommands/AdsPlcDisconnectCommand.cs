using System;
using PlcInterface.Ads;
using TwinCAT.Ads.TcpRouter;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to disconnect from the ads PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AdsPlcDisconnectCommand"/> class.
/// </remarks>
/// <param name="plcConnection">A <see cref="IPlcConnection"/> instance.</param>
/// <param name="amsRouter">A <see cref="IAmsRouter"/> for running without TwinCAT installation.</param>
internal sealed class AdsPlcDisconnectCommand(IAdsPlcConnection plcConnection, IAmsRouter amsRouter) : PlcDisconnectCommand("ads", plcConnection)
{
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
