using Microsoft.Extensions.Logging;

namespace PlcInterface.OpcUa;

/// <content>
/// Logging source generator methods.
/// </content>
internal partial class TreeBrowser
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Warning, Message = "Failed to browse symbols (Error: {StatusCode}), changing chunk size {OldSize} -> {NewSize}")]
    private partial void LogBrowseFailed(uint statusCode, int oldSize, int newSize);
}
