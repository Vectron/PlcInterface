using System;
using System.Linq;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> dump all symbols from the PLC to the console.
/// </summary>
internal sealed class PlcSymbolDumpCommand : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "dump";

    private readonly ISymbolHandler symbolHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcSymbolDumpCommand"/> class.
    /// </summary>
    /// <param name="name">The name of the interface.</param>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> instance.</param>
    public PlcSymbolDumpCommand(string name, ISymbolHandler symbolHandler)
    {
        CommandParameters = new[] { name, Parameter };
        this.symbolHandler = symbolHandler;
    }

    /// <inheritdoc/>
    public string[]? ArgumentNames => Array.Empty<string>();

    /// <inheritdoc/>
    public string[] CommandParameters
    {
        get;
        init;
    }

    /// <inheritdoc/>
    public string HelpText => "Dump all symbols from the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        foreach (var name in symbolHandler.AllSymbols.Select(x => x.Name))
        {
            Console.WriteLine(name);
        }
    }
}