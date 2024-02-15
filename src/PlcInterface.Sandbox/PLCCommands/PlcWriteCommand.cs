using System;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to read a variable from the PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcWriteCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="readWrite">A <see cref="IReadWrite"/> instance.</param>
internal abstract class PlcWriteCommand(string name, IReadWrite readWrite) : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "write";

    /// <inheritdoc/>
    public string[]? ArgumentNames => ["tag", "new value"];

    /// <inheritdoc/>
    public string[] CommandParameters { get; init; } = [name, Parameter];

    /// <inheritdoc/>
    public string HelpText => "Write a variable to the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 2;

    /// <inheritdoc/>
    public int MinArguments => 2;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        var symbolName = arguments[0];
        var symbolValue = ConvertToValidInputValue(symbolName, arguments[1]);
        readWrite.Write(symbolName, symbolValue);
        Console.WriteLine("Value written to PLC");
    }

    /// <summary>
    /// Convert the input to the valid write value.
    /// </summary>
    /// <param name="symbolName">The name of the symbol to write.</param>
    /// <param name="value">The value to convert.</param>
    /// <returns>An object with the correct type for writing.</returns>
    protected abstract object ConvertToValidInputValue(string symbolName, string value);
}
