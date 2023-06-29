using System;
using InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to monitor a variable in the PLC.
/// </summary>
internal sealed class PlcStopMonitorCommand : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "unmonitor";

    private readonly IMonitor monitor;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcStopMonitorCommand"/> class.
    /// </summary>
    /// <param name="name">The name of the interface.</param>
    /// <param name="monitor">A <see cref="IMonitor"/> instance.</param>
    public PlcStopMonitorCommand(string name, IMonitor monitor)
    {
        CommandParameters = new[] { name, Parameter };
        this.monitor = monitor;
    }

    /// <inheritdoc/>
    public string[] CommandParameters
    {
        get;
        init;
    }

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