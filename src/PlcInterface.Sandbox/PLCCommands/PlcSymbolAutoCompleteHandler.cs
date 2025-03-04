using System.Diagnostics.CodeAnalysis;
using Vectron.InteractiveConsole.AutoComplete;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Ads auto complete handler.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcSymbolAutoCompleteHandler"/> class.
/// </remarks>
/// <param name="name">The name of the plc type.</param>
/// <param name="symbolHandler">A <see cref="ISymbolHandler"/> instance.</param>
internal sealed class PlcSymbolAutoCompleteHandler(string name, ISymbolHandler symbolHandler) : IAutoCompleteHandler
{
    private const char ArgumentSeparator = ' ';
    private LinkedList<string> autoCompletions = new();
    private LinkedListNode<string>? current;
    private string rootCommand = string.Empty;

    /// <inheritdoc/>
    public string? NextSuggestions(string text)
    {
        if (!CanHandle(text))
        {
            return null;
        }

        Init(text);
        if (current == null)
        {
            current = autoCompletions.First;
            return current?.Value;
        }

        current = current.Next;
        return current?.Value;
    }

    /// <inheritdoc/>
    public string? PreviousSuggestions(string text)
    {
        if (!CanHandle(text))
        {
            return null;
        }

        Init(text);
        if (current == null)
        {
            current = autoCompletions.Last;
            return current?.Value;
        }

        current = current.Previous;
        return current?.Value;
    }

    private bool CanHandle(string text)
    {
        var parameters = text.Split(' ');
        if (parameters.Length is > 3 or < 2)
        {
            return false;
        }

        if (!string.Equals(parameters[0], name, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(parameters[1], PlcReadCommand.Parameter, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(parameters[1], PlcWriteCommand.Parameter, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(parameters[1], PlcToggleCommand.Parameter, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(parameters[1], PlcMonitorCommand.Parameter, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(parameters[1], PlcStopMonitorCommand.Parameter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    [SuppressMessage("Style", "IDE0306:Simplify collection initialization", Justification = "Doesn't work on LinkedList See https://github.com/dotnet/roslyn/issues/77174")]
    private void Init(string text)
    {
        if (rootCommand.Equals(text, StringComparison.OrdinalIgnoreCase)
            && autoCompletions.Count > 0)
        {
            return;
        }

        rootCommand = text;
        var arguments = rootCommand.Split(ArgumentSeparator);
        current = null;
        autoCompletions.Clear();

        var preparedText = arguments.Length > 2 ? arguments[2] : string.Empty;
        var validCommandOptions = symbolHandler.AllSymbols
            .Where(x => x.Name.StartsWith(preparedText, StringComparison.OrdinalIgnoreCase))
            .Select(x => $"{arguments[0]} {arguments[1]} {x.Name}");

        autoCompletions = new LinkedList<string>(validCommandOptions);
    }
}
