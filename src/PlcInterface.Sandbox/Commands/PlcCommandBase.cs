using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IApplicationCommand"/> implementation that connects to a plc.
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

    /// <inheritdoc/>
    public override Response Execute(string[] parameters)
    {
        if (parameters.Length <= 0)
        {
            return new Response(HelpTekst);
        }

        var command = parameters[0].ToLowerInvariant();
        return command switch
        {
            "connect" => ExecuteConnect(),
            "disconnect" => ExecuteDisconnect(),
            "read" => ExecuteRead(parameters[1]),
            "write" => ExecuteWrite(parameters[1], parameters[2]),
            "dump" => ExecuteDump(),
            "toggle" => ExecuteToggle(parameters[1]),
            "monitor" => ExecuteMonitor(parameters[1]),
            "unmonitor" => ExecuteUnMonitor(parameters[1]),
            _ => new Response("Invalid parameter"),
        };
    }

    /// <summary>
    /// Execute the 'connect' command.
    /// </summary>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteConnect()
    {
        _ = plcConnection.ConnectAsync();
        return new Response("Connecting to the plc");
    }

    /// <summary>
    /// Execute the 'disconnect' command.
    /// </summary>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteDisconnect()
    {
        _ = plcConnection.DisconnectAsync();
        return new Response("Disconnecting from the plc");
    }

    /// <summary>
    /// Execute the 'dump' command.
    /// </summary>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteDump()
    {
        foreach (var name in symbolHandler.AllSymbols.Select(x => x.Name))
        {
            ConsoleHelper.ConsoleWriteLineColored(name, ConsoleColor.Cyan);
        }

        return new Response("Done");
    }

    /// <summary>
    /// Execute the 'monitor' command.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteMonitor(string symbolName)
    {
        monitor.RegisterIO(symbolName, 10);
        var subscription = monitor.SymbolStream.Where(x => string.Equals(x.Name, symbolName, StringComparison.OrdinalIgnoreCase))
            .Finally(() => monitor.UnregisterIO(symbolName))
            .Subscribe(x => ConsoleHelper.ConsoleWriteLineColored($"{symbolName} updated to {x.Value}", ConsoleColor.Cyan));
        disposables.Add(symbolName, subscription);
        return new Response("Started monitoring");
    }

    /// <summary>
    /// Execute the 'read' command.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteRead(string symbolName)
    {
        var value = readWrite.Read(symbolName);
        var result = value is IDictionary<string, object?> multiValues
            ? FlattenExpando(multiValues)
            : value.ToString();

        return value == null ? new Response($"Failed reading value from {symbolName}") : new Response(result ?? string.Empty);
    }

    /// <summary>
    /// Execute the 'unmonitor' command.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteUnMonitor(string symbolName)
    {
        _ = disposables.Remove(symbolName, out var disposable);
        disposable?.Dispose();
        return new Response("Stopped monitoring");
    }

    /// <summary>
    /// Execute the 'write' command.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    protected virtual Response ExecuteWrite(string symbolName, string value)
    {
        readWrite.Write(symbolName, value);
        return new Response("Value written to PLC");
    }

    /// <summary>
    /// Execute the 'toggle' command.
    /// </summary>
    /// <returns>A <see cref="Response"/> with the command result.</returns>
    private Response ExecuteToggle(string symbolName)
    {
        readWrite.ToggleBool(symbolName);
        return new Response("Value toggled");
    }

    private string FlattenExpando(IDictionary<string, object?> parameters, int indentation = 0)
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
                    .Append(FlattenExpando(dictionary, indentation + 1));
                continue;
            }

            _ = builder.AppendLine(value?.ToString());
        }

        return builder.ToString();
    }
}