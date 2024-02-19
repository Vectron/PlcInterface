using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to read a variable from the PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcReadCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="readWrite">A <see cref="IReadWrite"/> instance.</param>
internal sealed class PlcReadCommand(string name, IReadWrite readWrite) : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "read";

    /// <inheritdoc/>
    public string[]? ArgumentNames => ["tag"];

    /// <inheritdoc/>
    public string[] CommandParameters { get; } = [name, Parameter];

    /// <inheritdoc/>
    public string HelpText => "Read a variable from the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 1;

    /// <inheritdoc/>
    public int MinArguments => 1;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        var symbolName = arguments[0];
        try
        {
            var value = readWrite.Read(symbolName);
            if (value == null)
            {
                Console.WriteLine($"Failed reading value from {symbolName}");
                return;
            }

            var builder = new StringBuilder()
                .Append(symbolName)
                .Append(": ");
            _ = value is IDictionary<string, object?> multiValues
                ? builder.AppendLine().Append(ObjectToString(multiValues, 1))
                : builder.Append(value);

            Console.WriteLine(builder);
        }
        catch (Exception ex)
        {
            var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            Console.WriteLine($"Failed to read {symbolName} ({message})");
        }
    }

    private static string ObjectToString(IDictionary<string, object?> parameters, int indentation = 0)
    {
        var builder = new StringBuilder();
        var prefix = string.Join(string.Empty, Enumerable.Repeat("  ", indentation));
        foreach (var (key, value) in parameters)
        {
            _ = builder.Append(prefix)
                .Append(key)
                .Append(": ");

            if (value is IDictionary<string, object?> dictionary)
            {
                _ = builder.AppendLine()
                    .Append(ObjectToString(dictionary, indentation + 1));
                continue;
            }

            _ = builder.AppendLine(value?.ToString());
        }

        return builder.ToString();
    }
}
