using System;
using InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to disconnect from the PLC.
/// </summary>
internal sealed class PlcDisconnectCommand : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "disconnect";

    private readonly IPlcConnection plcConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcDisconnectCommand"/> class.
    /// </summary>
    /// <param name="name">The name of the interface.</param>
    /// <param name="plcConnection">A <see cref="IPlcConnection"/> instance.</param>
    public PlcDisconnectCommand(string name, IPlcConnection plcConnection)
    {
        CommandParameters = new[] { name, Parameter };
        this.plcConnection = plcConnection;
    }

    /// <inheritdoc/>
    public string[] CommandParameters
    {
        get;
        init;
    }

    /// <inheritdoc/>
    public string HelpText => "Disconnect from the PLC through " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        _ = plcConnection.DisconnectAsync();
        Console.WriteLine("Disconnecting from the PLC");
    }
}