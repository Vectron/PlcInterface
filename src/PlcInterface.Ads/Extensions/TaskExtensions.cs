using Microsoft.Extensions.Logging;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Extension methods for <see cref="Task"/>.
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// Log exceptions async.
        /// </summary>
        /// <param name="task">The task to check for exceptions.</param>
        /// <param name="logger">The <see cref="ILogger"/> to log the error to.</param>
        /// <returns>The original <see cref="Task"/>.</returns>
        public static Task LogExceptionsAsync(this Task task, ILogger logger)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return task.ContinueWith(
                t =>
                {
                    if (t.Exception != null)
                    {
                        var aggregateException = t.Exception.Flatten();
                        for (var i = aggregateException.InnerExceptions.Count - 1; i >= 0; i--)
                        {
                            var exception = aggregateException.InnerExceptions[i];
                            logger.LogError(exception, "Task Error");
                        }
                    }
                },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}