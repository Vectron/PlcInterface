using System;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to disconnect from the PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcDisconnectCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="plcConnection">A <see cref="IPlcConnection"/> instance.</param>
internal class PlcDisconnectCommand(string name, IPlcConnection plcConnection) : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "disconnect";

    /// <inheritdoc/>
    public string[]? ArgumentNames => Array.Empty<string>();

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public string[] CommandParameters { get; } = [name, Parameter];

    /// <inheritdoc/>
    public string HelpText => "Disconnect from the PLC through " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public virtual void Execute(string[] arguments)
    {
        _ = plcConnection.DisconnectAsync();
        Console.WriteLine("Disconnecting from the PLC");
    }
}