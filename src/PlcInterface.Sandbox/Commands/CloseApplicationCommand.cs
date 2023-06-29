using InteractiveConsole.Commands;
using Microsoft.Extensions.Hosting;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IConsoleCommand"/> that closes the program.
/// </summary>
internal sealed class CloseApplicationCommand : IConsoleCommand
{
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseApplicationCommand"/> class.
    /// </summary>
    /// <param name="hostApplicationLifetime">A <see cref="IHostApplicationLifetime"/> implementation.</param>
    public CloseApplicationCommand(IHostApplicationLifetime hostApplicationLifetime) => this.hostApplicationLifetime = hostApplicationLifetime;

    /// <inheritdoc/>
    public string[] CommandParameters => new[] { "exit" };

    /// <inheritdoc/>
    public string HelpText => "Close the application";

    /// <inheritdoc/>
    public int MaxArguments => 0;

    /// <inheritdoc/>
    public int MinArguments => 0;

    /// <inheritdoc/>
    public void Execute(string[] arguments)
        => hostApplicationLifetime.StopApplication();
}