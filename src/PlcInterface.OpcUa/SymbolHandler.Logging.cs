using Microsoft.Extensions.Logging;

namespace PlcInterface.OpcUa;

/// <content>
/// Logging source generator methods.
/// </content>
public partial class SymbolHandler
{
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Symbols updated in {Time} ms, found {Amount} symbols")]
    private partial void LogSymbolsUpdated(long time, int amount);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Updating symbols")]
    private partial void LogUpdatingSymbols();

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Updating symbols failed")]
    private partial void LogUpdatingSymbolsFailed(Exception exception);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "{VariableName} does not exist")]
    private partial void LogVariableDoesNotExist(string variableName);
}
