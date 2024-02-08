using Microsoft.Extensions.Logging;

namespace PlcInterface.OpcUa;

/// <content>
/// Logging source generator methods.
/// </content>
public partial class Monitor
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Updating subscriptions")]
    private partial void LogUpdatingSubscriptions();

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "{VariableName} is not registered")]
    private partial void LogVariableNotRegistered(string variableName);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Registered IO {VariableName}")]
    private partial void LogVariableRegistered(string variableName);

    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "{VariableName} still has {SubscriptionCount} subscriptions left")]
    private partial void LogVariableStillHasSubscriptions(string variableName, int subscriptionCount);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Unregistered IO {VariableName}")]
    private partial void LogVariableUnregistered(string variableName);
}
