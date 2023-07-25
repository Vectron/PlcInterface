using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace PlcInterface.Sandbox;

/// <summary>
/// A custom formatter for pretty console entries.
/// </summary>
public sealed class CustomConsoleFormatter : ConsoleFormatter, IDisposable
{
    private const string LogLevelPadding = ": ";
    private static readonly string MessagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LogLevelPadding.Length);
    private static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;
    private readonly IDisposable? optionsReloadToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomConsoleFormatter"/> class.
    /// </summary>
    /// <param name="options">Options for setting up this formatter.</param>
    public CustomConsoleFormatter(IOptionsMonitor<CustomConsoleFormatterOptions> options)
        : base("Custom")
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Gets or sets the formatter options.
    /// </summary>
    internal CustomConsoleFormatterOptions FormatterOptions
    {
        get;
        set;
    }

    private static bool IsAndroidOrAppleMobile => OperatingSystem.IsAndroid() || OperatingSystem.IsTvOS() || OperatingSystem.IsIOS(); // returns true on MacCatalyst

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken?.Dispose();

    /// <inheritdoc/>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        WriteTime(textWriter);
        WriteLogLevel(logEntry, textWriter);
        textWriter.Write(logEntry.Category);
        WriteEventId(logEntry, textWriter);
        WriteScopeInformation(textWriter, scopeProvider);
        WriteMessage(textWriter, message);

        // Example: System.InvalidOperationException at Namespace.Class.Function() in File:line X
        var exception = logEntry.Exception;
        if (exception != null)
        {
            // exception message
            textWriter.WriteLine();
            textWriter.Write(MessagePadding);
            WriteMessage(textWriter, exception.ToString());
        }

        textWriter.WriteLine();
    }

    private static string GetLogLevelConsoleColors(LogLevel logLevel)
    {
        // We shouldn't be outputting color codes for Android/Apple mobile platforms, they have no
        // shell (adb shell is not meant for running apps) and all the output gets redirected to
        // some log file.
        var disableColors = IsAndroidOrAppleMobile;
        if (disableColors)
        {
            return string.Empty;
        }

        // We must explicitly set the background color if we are setting the foreground color, since
        // just setting one can look bad on the users console.
        return logLevel switch
        {
            LogLevel.Trace => "\u001b[37m\u001b[40m",
            LogLevel.Debug => "\u001b[37m\u001b[40m",
            LogLevel.Information => "\u001b[32m\u001b[40m",
            LogLevel.Warning => "\u001b[1m\u001b[33m\u001b[40m",
            LogLevel.Error => "\u001b[30m\u001b[41m",
            LogLevel.Critical => "\u001b[1m\u001b[37m\u001b[41m",
            LogLevel.None => string.Empty,
            _ => string.Empty,
        };
    }

    private static string GetLogLevelString(LogLevel logLevel)
            => logLevel switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "FAIL",
                LogLevel.Critical => "CRIT",
                LogLevel.None => throw new ArgumentOutOfRangeException(nameof(logLevel)),
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
            };

    private static void WriteEventId<TState>(LogEntry<TState> logEntry, TextWriter textWriter)
    {
        textWriter.Write('[');
        var eventId = logEntry.EventId.Id;
        Span<char> span = stackalloc char[10];
        if (eventId.TryFormat(span, out var charsWritten, default, CultureInfo.CurrentCulture))
        {
            textWriter.Write(span[..charsWritten]);
        }
        else
        {
            textWriter.Write(eventId.ToString(CultureInfo.CurrentCulture));
        }

        textWriter.Write(']');
        textWriter.Write(' ');
    }

    private static void WriteLogLevel<TState>(LogEntry<TState> logEntry, TextWriter textWriter)
    {
        var logLevel = logEntry.LogLevel;
        var logLevelColors = GetLogLevelConsoleColors(logLevel);
        var logLevelString = GetLogLevelString(logLevel);
        if (logLevelString != null)
        {
            textWriter.Write(logLevelColors);
            textWriter.Write(logLevelString);
            textWriter.Write("\u001b[39m\u001b[22m\u001b[49m");
            textWriter.Write(' ');
        }
    }

    private static void WriteMessage(TextWriter textWriter, string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            var newMessage = message.Replace(Environment.NewLine, NewLineWithMessagePadding, StringComparison.Ordinal);
            textWriter.Write(newMessage);
        }
    }

    private DateTimeOffset GetCurrentDateTime()
        => FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(CustomConsoleFormatterOptions options)
        => FormatterOptions = options;

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider)
    {
        if (!FormatterOptions.IncludeScopes || scopeProvider == null)
        {
            return;
        }

        var paddingNeeded = false;
        scopeProvider.ForEachScope(
            (scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(MessagePadding);
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }

                state.Write(scope);
            },
            textWriter);
    }

    private void WriteTime(TextWriter textWriter)
    {
        string? timestamp = null;
        var timestampFormat = FormatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            var dateTimeOffset = GetCurrentDateTime();
            timestamp = dateTimeOffset.ToString(timestampFormat, CultureInfo.CurrentCulture);
        }

        if (timestamp != null)
        {
            textWriter.Write(timestamp);
            textWriter.Write(' ');
        }
    }
}