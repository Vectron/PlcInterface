using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcInterface.Sandbox.Interactive;

/// <summary>
/// A <see cref="IAutoCompleteHandler" /> implementation.
/// </summary>
internal sealed class AutoCompletionHandler : IAutoCompleteHandler
{
    private readonly IEnumerable<IApplicationCommand> commands;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCompletionHandler" /> class.
    /// </summary>
    /// <param name="commands">A <see cref="IEnumerable{T}" /> of vallid <see cref="IApplicationCommand" />.</param>
    public AutoCompletionHandler(IEnumerable<IApplicationCommand> commands)
    {
        Separators = new char[] { ' ' };
        this.commands = commands;
    }

    /// <inheritdoc />
    public char[] Separators
    {
        get;
        set;
    }

    /// <inheritdoc />
    public string[] GetSuggestions(string text, int index)
    {
        var commandTree = text.Split(Separators);
        var vallidCommands = commands.Where(c => c.Name.StartsWith(commandTree[0], StringComparison.OrdinalIgnoreCase));
        if (commandTree.Length == 1)
        {
            return vallidCommands
                .Select(c => c.Name)
                .ToArray();
        }

        return GetVallidSubParameter(commandTree, 1, vallidCommands.SelectMany(c => c.VallidParameters))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private IEnumerable<string> GetVallidSubParameter(string[] commandTree, int index, IEnumerable<string> vallidParameters)
    {
        if (index + 1 < commandTree.Length)
        {
            var subParameters = vallidParameters
                .Select(p => p.Split(Separators))
                .Where(p => string.Equals(p[0], commandTree[index], StringComparison.OrdinalIgnoreCase) && p.Length > 1).Select(p => p[1]);
            return GetVallidSubParameter(commandTree, index + 1, subParameters);
        }

        return vallidParameters
            .Where(p => p.StartsWith(commandTree[index], StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Split(Separators)[0]);
    }
}