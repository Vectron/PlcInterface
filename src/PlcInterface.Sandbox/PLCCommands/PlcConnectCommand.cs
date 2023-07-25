﻿using System;
using Vectron.InteractiveConsole.Commands;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Base class for a <see cref="IConsoleCommand"/> to connect to the PLC.
/// </summary>
internal sealed class PlcConnectCommand : IConsoleCommand
{
    /// <summary>
    /// The parameter needed for this command.
    /// </summary>
    public const string Parameter = "connect";

    private readonly IPlcConnection plcConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcConnectCommand"/> class.
    /// </summary>
    /// <param name="name">The name of the interface.</param>
    /// <param name="plcConnection">A <see cref="IPlcConnection"/> instance.</param>
    public PlcConnectCommand(string name, IPlcConnection plcConnection)
    {
        CommandParameters = new[] { name, Parameter };
        this.plcConnection = plcConnection;
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
    public string HelpText => "Connect to the PLC through " + CommandParameters[0];

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        _ = plcConnection.ConnectAsync();
        Console.WriteLine("Connecting to the PLC");
    }
}