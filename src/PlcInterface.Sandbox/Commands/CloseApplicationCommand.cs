using Microsoft.Extensions.Hosting;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IApplicationCommand"/> implementation that closes the program.
/// </summary>
internal sealed class CloseApplicationCommand : CommandBase
{
    private const string CommandName = "exit";
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseApplicationCommand"/> class.
    /// </summary>
    /// <param name="hostApplicationLifetime">A <see cref="IHostApplicationLifetime"/> implementation.</param>
    public CloseApplicationCommand(IHostApplicationLifetime hostApplicationLifetime)
        : base(CommandName)
    {
        this.hostApplicationLifetime = hostApplicationLifetime;
        HelpText = CommandName + ": Closes the application";
    }

    /// <inheritdoc/>
    public override Response Execute(string[] parameters)
    {
        hostApplicationLifetime.StopApplication();
        return new Response("Application Stopped");
    }
}