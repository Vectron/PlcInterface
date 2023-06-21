using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcInterface.Sandbox.Interactive;

/// <summary>
/// A <see cref="IAutoCompleteHandler"/> implementation.
/// </summary>
internal sealed class AutoCompletionHandler : IAutoCompleteHandler
{
    private readonly IEnumerable<IApplicationCommand> commands;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCompletionHandler"/> class.
    /// </summary>
    /// <param name="commands">A <see cref="IEnumerable{T}"/> of valid <see cref="IApplicationCommand"/>.</param>
    public AutoCompletionHandler(IEnumerable<IApplicationCommand> commands)
    {
        Separators = new char[] { ' ' };
        this.commands = commands;
    }

    /// <inheritdoc/>
    public char[] Separators
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public string[] GetSuggestions(string text, int index)
    {
        var commandTree = text.Split(Separators);
        var validCommands = commands.Where(c => c.Name.StartsWith(commandTree[0], StringComparison.OrdinalIgnoreCase));
        if (commandTree.Length == 1)
        {
            return validCommands
                .Select(c => c.Name)
                .ToArray();
        }

        return GetValidSubParameter(commandTree, 1, validCommands.SelectMany(c => c.ValidParameters))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private IEnumerable<string> GetValidSubParameter(string[] commandTree, int index, IEnumerable<string> validParameters)
    {
        if (index + 1 < commandTree.Length)
        {
            var subParameters = validParameters
                .Select(p => p.Split(Separators))
                .Where(p => string.Equals(p[0], commandTree[index], StringComparison.OrdinalIgnoreCase) && p.Length > 1).Select(p => p[1]);
            return GetValidSubParameter(commandTree, index + 1, subParameters);
        }

        return validParameters
            .Where(p => p.StartsWith(commandTree[index], StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Split(Separators)[0]);
    }
}