using System;
using System.Reactive.Linq;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to monitor a variable in the PLC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlcMonitorCommand"/> class.
/// </remarks>
/// <param name="name">The name of the interface.</param>
/// <param name="monitor">A <see cref="IMonitor"/> instance.</param>
internal sealed class PlcMonitorCommand(string name, IMonitor monitor) : IConsoleCommand, IDisposable
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "monitor";

    private readonly IDisposable monitorSubscription = monitor.SymbolStream.Subscribe(x => Console.WriteLine($"{x.Name}: {x.Value}"));
    private bool disposed;

    /// <inheritdoc/>
    public string[]? ArgumentNames => ["tag"];

    /// <inheritdoc/>
    public string[] CommandParameters { get; } = [name, Parameter];

    /// <inheritdoc/>
    public string HelpText => "Monitor a variable in the PLC with " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 1;

    /// <inheritdoc/>
    public int MinArguments => 1;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        monitorSubscription?.Dispose();
    }

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        var symbolName = arguments[0];
        monitor.RegisterIO(symbolName, 10);
        Console.WriteLine($"Started monitoring {symbolName}");
    }
}
