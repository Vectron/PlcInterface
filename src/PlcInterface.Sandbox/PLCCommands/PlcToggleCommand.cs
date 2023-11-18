using System;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to toggle a boolean variable in the PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcToggleCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="readWrite">A <see cref="IReadWrite"/> instance.</param>
internal sealed class PlcToggleCommand(string name, IReadWrite readWrite) : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "toggle";

    /// <inheritdoc/>
    public string[]? ArgumentNames => new[] { "tag" };

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public string[] CommandParameters { get; } = [name, Parameter];

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