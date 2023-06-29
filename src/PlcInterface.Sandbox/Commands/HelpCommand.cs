using System;
using System.Text;
using InteractiveConsole.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IConsoleCommand"/> that prints a help text.
/// </summary>
internal sealed class HelpCommand : IConsoleCommand
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpCommand"/> class.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> implementation.</param>
    public HelpCommand(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public string[] CommandParameters => new[] { "help" };

    /// <inheritdoc/>
    public string HelpText => "prints this information";

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
    {
        var builder = new StringBuilder();
        var consoleCommands = serviceProvider.GetServices<IConsoleCommand>();
        foreach (var command in consoleCommands)
        {
            _ = builder.AppendJoin(' ', command.CommandParameters)
                .Append(": ")
                .Append(command.HelpText)
                .AppendLine();
        }

        Console.WriteLine(builder.ToString());
    }
}