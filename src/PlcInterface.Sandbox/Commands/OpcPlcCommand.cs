using PlcInterface.OpcUa;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IApplicationCommand" /> implementation for interacting with a ads PLC.
/// </summary>
internal sealed class OpcPlcCommand : PlcCommandBase
{
    private const string CommandName = "opc";

    /// <summary>
    /// Initializes a new instance of the <see cref="OpcPlcCommand"/> class.
    /// </summary>
    /// <param name="plcConnection">A <see cref="IOpcPlcConnection"/>.</param>
    /// <param name="readWrite">A <see cref="IOpcReadWrite"/>.</param>
    /// <param name="symbolHandler">A <see cref="IOpcSymbolHandler"/>.</param>
    /// <param name="monitor">A <see cref="IOpcMonitor"/>.</param>
    public OpcPlcCommand(IOpcPlcConnection plcConnection, IOpcReadWrite readWrite, IOpcSymbolHandler symbolHandler, IOpcMonitor monitor)
        : base(CommandName, plcConnection, readWrite, symbolHandler, monitor)
    {
    }
}