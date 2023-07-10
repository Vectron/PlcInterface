using System;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to toggle a boolean variable in the PLC.
/// </summary>
internal sealed class PlcToggleCommand : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "toggle";

    private readonly IReadWrite readWrite;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcToggleCommand"/> class.
    /// </summary>
    /// <param name="name">The name of the interface.</param>
    /// <param name="readWrite">A <see cref="IReadWrite"/> instance.</param>
    public PlcToggleCommand(string name, IReadWrite readWrite)
    {
        CommandParameters = new[] { name, Parameter };
        this.readWrite = readWrite;
    }

    /// <inheritdoc/>
    public string[] CommandParameters
    {
        get;
        init;
    }

    /// <inheritdoc/>
    public string HelpText => "Toggle a boolean variable in the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 1;

    /// <inheritdoc/>
    public int MinArguments => 1;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        var symbolName = arguments[0];
        readWrite.ToggleBool(symbolName);
        Console.WriteLine("Value toggled");
    }
}