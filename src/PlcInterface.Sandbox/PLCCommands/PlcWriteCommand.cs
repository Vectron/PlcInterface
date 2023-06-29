using System;
using InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to read a variable from the PLC.
/// </summary>
internal abstract class PlcWriteCommand : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "write";

    private readonly IReadWrite readWrite;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcWriteCommand"/> class.
    /// </summary>
    /// <param name="name">The name of the interface.</param>
    /// <param name="readWrite">A <see cref="IReadWrite"/> instance.</param>
    protected PlcWriteCommand(string name, IReadWrite readWrite)
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