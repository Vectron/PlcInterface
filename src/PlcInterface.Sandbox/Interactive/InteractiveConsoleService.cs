using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PlcInterface.Sandbox.Interactive;

/// <summary>
/// A <see cref="IHostedService" /> for interacting with the user through the <see cref="Console" />.
/// </summary>
internal class InteractiveConsoleService : IHostedService, IDisposable
{
    private const string InteractiveModeText = "INTERACTIVE MODE - type 'help' for help or 'exit' to EXIT";
    private readonly IEnumerable<IApplicationCommand> commands;
    private Task? checkForInputTask;
    private bool disposedValue;
    private CancellationTokenSource? stopTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractiveConsoleService" /> class.
    /// </summary>
    /// <param name="commands">A <see cref="IEnumerable{T}" /> of vallid <see cref="IApplicationCommand" />.</param>
    /// <param name="autoCompletionHandler">A <see cref="IAutoCompleteHandler" /> implementation.</param>
    public InteractiveConsoleService(IEnumerable<IApplicationCommand> commands, IAutoCompleteHandler autoCompletionHandler)
    {
        this.commands = commands;
        ReadLine.AutoCompletionHandler = autoCompletionHandler;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        stopTokenSource?.Dispose();
        stopTokenSource = new CancellationTokenSource();
        ConsoleHelper.ConsoleWriteLineColored(InteractiveModeText, ConsoleColor.Cyan);
        checkForInputTask = Task.Run(() => CheckForInput(stopTokenSource.Token), cancellationToken);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (checkForInputTask != null && stopTokenSource != null)
        {
            stopTokenSource.Cancel();
            await checkForInputTask.ConfigureAwait(false);
            stopTokenSource.Dispose();
            stopTokenSource = null;
            checkForInputTask.Dispose();
            checkForInputTask = null;
        }
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">Value indicating if we need to cleanup managed resources.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0045:Do not use blocking call in a sync method (need to make containing method async)", Justification = "Cant make dispose async")]
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            disposedValue = true;
        }
    }

    private static string TrimNewLines(string text)
    {
        while (text.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase))
        {
            text = text[..^Environment.NewLine.Length];
        }

        return text;
    }

    private void CheckForInput(CancellationToken stopToken)
    {
        while (!stopToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var commandAndParameters = ReadLine.Read(">").Split(' ');
                var command = commandAndParameters[0];
                var parameters = commandAndParameters.Skip(1).ToArray();
                ProccesCommand(command, parameters);
            }
        }
    }

    private void ProccesCommand(string command, params string[] parameters)
    {
        var response = commands
            .FirstOrDefault(c => string.Equals(c.Name, command, StringComparison.OrdinalIgnoreCase))?
            .Execute(parameters);

        if (!string.IsNullOrWhiteSpace(response?.Message))
        {
            ConsoleHelper.ConsoleWriteLineColored(TrimNewLines(response.Message), ConsoleColor.Cyan);
        }
    }
}