using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PlcInterface;

/// <summary>
/// Extension methods for <see cref="Task"/>.
/// </summary>
internal static partial class TaskExtensions
{
    /// <summary>
    /// Log exceptions async.
    /// </summary>
    /// <param name="task">The task to check for exceptions.</param>
    /// <param name="logger">The <see cref="ILogger"/> to log the error to.</param>
    /// <returns>The original <see cref="Task"/>.</returns>
    public static Task LogExceptionsAsync(this Task task, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(logger);

        return task.ContinueWith(
            t =>
            {
                if (t.Exception != null)
                {
                    var aggregateException = t.Exception.Flatten();
                    for (var i = aggregateException.InnerExceptions.Count - 1; i >= 0; i--)
                    {
                        var exception = aggregateException.InnerExceptions[i];
                        logger.LogException(exception);
                    }
                }
            },
            TaskContinuationOptions.OnlyOnFaulted);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Task Error")]
    private static partial void LogException(this ILogger logger, Exception exception);
}
