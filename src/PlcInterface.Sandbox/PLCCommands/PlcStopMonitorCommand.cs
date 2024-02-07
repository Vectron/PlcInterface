using System;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to monitor a variable in the PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcStopMonitorCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="monitor">A <see cref="IMonitor"/> instance.</param>
internal sealed class PlcStopMonitorCommand(string name, IMonitor monitor) : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "unmonitor";

    /// <inheritdoc/>
    public string[]? ArgumentNames => new[] { "tag" };

    /// <inheritdoc/>
    public string[] CommandParameters { get; } = [name, Parameter];

    /// <inheritdoc/>
    public string HelpText => "Stop monitoring a variable in the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 1;

    /// <inheritdoc/>
    public int MinArguments => 1;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        var symbolName = arguments[0];
        monitor.UnregisterIO(symbolName);
        Console.WriteLine($"Stopped monitoring {symbolName}");
    }
}
