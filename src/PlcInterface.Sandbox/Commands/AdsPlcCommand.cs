using PlcInterface.Ads;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IApplicationCommand" /> implementation for interacting with a ads PLC.
/// </summary>
internal sealed class AdsPlcCommand : PlcCommandBase
{
    private const string CommandName = "ads";

    /// <summary>
    /// Initializes a new instance of the <see cref="AdsPlcCommand"/> class.
    /// </summary>
    /// <param name="plcConnection">A <see cref="IAdsPlcConnection"/>.</param>
    /// <param name="readWrite">A <see cref="IAdsReadWrite"/>.</param>
    /// <param name="symbolHandler">A <see cref="IAdsSymbolHandler"/>.</param>
    /// <param name="monitor">A <see cref="IAdsMonitor"/>.</param>
    public AdsPlcCommand(IAdsPlcConnection plcConnection, IAdsReadWrite readWrite, IAdsSymbolHandler symbolHandler, IAdsMonitor monitor)
        : base(CommandName, plcConnection, readWrite, symbolHandler, monitor)
    {
    }
}