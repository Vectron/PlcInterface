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

            if (value is IDictionary<string, object?> multiValues)
            {
                ObjectToString(builder, multiValues, 1);
            }
            else if (value is Array array)
            {
                ArrayToString(builder, array, 1);
            }
            else
            {
                _ = builder.Append(value);
            }

            Console.WriteLine(builder);
        }
        catch (Exception ex)
        {
            var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            Console.WriteLine($"Failed to read {symbolName} ({message})");
        }
    }

    private static void ArrayToString(StringBuilder builder, Array array, int indentation = 0)
    {
        var prefix = string.Join(string.Empty, Enumerable.Repeat("  ", indentation));
        foreach (var indices in IndicesHelper.GetIndices(array))
        {
            _ = builder
                .AppendLine()
                .Append(prefix)
                .Append('[')
                .Append(indices[0]);

            for (var i = 1; i < indices.Length; i++)
            {
                _ = builder
                    .Append(", ")
                    .Append(indices[i]);
            }

            _ = builder
                .Append("]: ");

            try
            {
                var itemValue = array.GetValue(indices)?.ToString() ?? "Null";
                _ = builder.Append(itemValue);
            }
            catch (IndexOutOfRangeException)
            {
                _ = builder.Append("Out of range");
            }
        }
    }

    private static void ObjectToString(StringBuilder builder, IDictionary<string, object?> parameters, int indentation = 0)
    {
        var prefix = string.Join(string.Empty, Enumerable.Repeat("  ", indentation));
        foreach (var (key, value) in parameters)
        {
            _ = builder
                .Append(prefix)
                .Append(key)
                .Append(": ");

            if (value is IDictionary<string, object?> dictionary)
            {
                _ = builder
                    .AppendLine();

                ObjectToString(builder, dictionary, indentation + 1);
                continue;
            }

            _ = builder.AppendLine(value?.ToString());
        }
    }
}
