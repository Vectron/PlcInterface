using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <content>
/// Logging source generator methods.
/// </content>
public partial class PlcConnection
{
    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Failed to connect to Opc, check network connection and Opc settings")]
    private partial void LogBadNoConnection(Exception exception);

    [LoggerMessage(EventId = 21, Level = LogLevel.Information, Message = "Accepted Certificate: {CertificateSubject} (error code: {StatusCode} from {BrowseName})")]
    private partial void LogCertificateAccepted(string certificateSubject, StatusCode statusCode, string browseName);

    [LoggerMessage(EventId = 25, Level = LogLevel.Error, Message = "Rejected Certificate: {CertificateSubject} (error code: {StatusCode} from {BrowseName})")]
    private partial void LogCertificateRejected(string certificateSubject, StatusCode statusCode, string browseName);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Failed to close connection with error {StatusCode}")]
    private partial void LogCloseConnectionFailed(StatusCode statusCode);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Connected to {Endpoint}")]
    private partial void LogConnected(ConfiguredEndpoint endpoint);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Failed to connect to {Endpoint}")]
    private partial void LogConnectingFailed(ConfiguredEndpoint endpoint);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Opening connection to {Address}")]
    private partial void LogConnectingTo(Uri address);

    [LoggerMessage(EventId = 20, Level = LogLevel.Trace, Message = "Creating new application certificate for: {ApplicationName}")]
    private partial void LogCreateCertificate(string applicationName);

    [LoggerMessage(EventId = 13, Level = LogLevel.Trace, Message = "Create a session with OPC UA server")]
    private partial void LogCreateSession();

    [LoggerMessage(EventId = 10, Level = LogLevel.Trace, Message = "Creating an Application Configuration")]
    private partial void LogCreatingAppConfiguration();

    [LoggerMessage(EventId = 11, Level = LogLevel.Trace, Message = "Discover endpoints of {DiscoveryAddress}")]
    private partial void LogFindingEndpoint(Uri discoveryAddress);

    [LoggerMessage(EventId = 30, Level = LogLevel.Error, Message = "Server not responding, status: {ServiceResult}, Reconnecting to {Endpoint}")]
    private partial void LogKeepAliveFailed(ServiceResult serviceResult, ConfiguredEndpoint endpoint);

    [LoggerMessage(EventId = 14, Level = LogLevel.Debug, Message = "Logging in with user: {UserName}")]
    private partial void LogLoggingIn(string userName);

    [LoggerMessage(EventId = 24, Level = LogLevel.Warning, Message = "Missing application certificate, using unsecure connection")]
    private partial void LogMissingCertificate();

    [LoggerMessage(EventId = 31, Level = LogLevel.Error, Message = "Request Timed Out")]
    private partial void LogRequestTimeout(Exception exception);

    [LoggerMessage(EventId = 22, Level = LogLevel.Error, Message = "Security Checks Failed, Check if certificate is trusted")]
    private partial void LogSecurityCheckFailed(Exception exception);

    [LoggerMessage(EventId = 12, Level = LogLevel.Trace, Message = "Selected endpoint uses: {SecurityPolicy}")]
    private partial void LogSelectedSecurity(string securityPolicy);

    [LoggerMessage(EventId = 32, Level = LogLevel.Error, Message = "Unprocessed error {StatusCode} from {BrowseName}")]
    private partial void LogUnknownServiceResult(Exception exception, StatusCode statusCode, string browseName);

    [LoggerMessage(EventId = 23, Level = LogLevel.Warning, Message = "Security turned off, using unsecure connection")]
    private partial void LogUnsecureConnection();
}
