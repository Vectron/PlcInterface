using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IApplicationCommand" /> implementation that connects to a plc.
/// </summary>
internal abstract class PlcCommandBase : CommandBase, IDisposable
{
    private readonly Dictionary<string, IDisposable> disposables = new(StringComparer.OrdinalIgnoreCase);
    private readonly IMonitor monitor;
    private readonly IPlcConnection plcConnection;
    private readonly IReadWrite readWrite;
    private readonly ISymbolHandler symbolHandler;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcCommandBase"/> class.
    /// </summary>
    /// <param name="commandName">The name of this command.</param>
    /// <param name="plcConnection">A <see cref="IPlcConnection"/>.</param>
    /// <param name="readWrite">A <see cref="IReadWrite"/>.</param>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/>.</param>
    /// <param name="monitor">A <see cref="IMonitor"/>.</param>
    protected PlcCommandBase(string commandName, IPlcConnection plcConnection, IReadWrite readWrite, ISymbolHandler symbolHandler, IMonitor monitor)
        : base(commandName)
    {
        HelpTekst = commandName + ": Common plc actions";
        VallidParameters = new[] { "connect", "disconnect", "read", "write", "dump", "toggle", "monitor", "unmonitor" };
        this.plcConnection = plcConnection;
        this.readWrite = readWrite;
        this.symbolHandler = symbolHandler;
        this.monitor = monitor;
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        foreach (var disposable in disposables)
        {
            disposable.Value.Dispose();
        }

        disposables.Clear();
    }

    /// <inheritdoc />
    public override Response Execute(string[] parameters)
    {
        if (parameters.Length <= 0)
        {
            return new Response(HelpTekst);
        }

        var command = parameters[0].ToLowerInvariant();
        switch (command)
        {
            case "connect":
                _ = plcConnection.ConnectAsync();
                return new Response("Connecting to the plc");

            case "disconnect":
                _ = plcConnection.DisconnectAsync();
                return new Response("Disconnecting from the plc");

            case "read":
                var value = readWrite.Read(parameters[1]).ToString();
                return value == null ? new Response($"Failed reading value from {parameters[1]}") : new Response(value);

            case "write":
                readWrite.Write(parameters[1], true);
                return new Response("Value written to PLC");

            case "dump":
                foreach (var name in symbolHandler.AllSymbols.Select(x => x.Name))
                {
                    ConsoleHelper.ConsoleWriteLineColored(name, ConsoleColor.Cyan);
                }

                return new Response("Done");

            case "toggle":
                readWrite.ToggleBool(parameters[1]);
                return new Response("Value toggled");

            case "monitor":
                monitor.RegisterIO(parameters[1], 10);
                var subscription = monitor.SymbolStream.Where(x => string.Equals(x.Name, parameters[1], StringComparison.OrdinalIgnoreCase))
                    .Finally(() => monitor.UnregisterIO(parameters[1]))
                    .Subscribe(x => ConsoleHelper.ConsoleWriteLineColored($"{parameters[1]} updated to {x.Value}", ConsoleColor.Cyan));
                disposables.Add(parameters[1], subscription);
                return new Response("Started monitoring");

            case "unmonitor":
                _ = disposables.Remove(parameters[1], out var disposable);
                disposable?.Dispose();
                return new Response("Stopped monitoring");

            default:
                return new Response("Invallid parameter");
        }
    }
}