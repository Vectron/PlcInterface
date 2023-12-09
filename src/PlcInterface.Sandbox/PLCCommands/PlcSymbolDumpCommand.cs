using System;
using System.Linq;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> dump all symbols from the PLC to the console.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcSymbolDumpCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="symbolHandler">A <see cref="ISymbolHandler"/> instance.</param>
internal sealed class PlcSymbolDumpCommand(string name, ISymbolHandler symbolHandler) : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "dump";

    /// <inheritdoc/>
    public string[]? ArgumentNames => Array.Empty<string>();

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public string[] CommandParameters { get; } = [name, Parameter];

    /// <inheritdoc/>
    public string HelpText => "Dump all symbols from the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        foreach (var symbolName in symbolHandler.AllSymbols.Select(x => x.Name))
        {
            Console.WriteLine(symbolName);
        }
    }
}