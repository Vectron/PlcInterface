using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// Implementation of <see cref="IPlcConnection{T}"/> for the <see cref="ISession"/>.
/// </summary>
public class PlcConnection : IOpcPlcConnection, IDisposable
{
    private readonly BehaviorSubject<IConnected<ISession>> connectionState = new(Connected.No<ISession>());

    private readonly CompositeDisposable disposables = [];

    private readonly ILogger logger;
    private readonly IOptions<OpcPlcConnectionOptions> options;
    private readonly TimeSpan reconnectDelay = TimeSpan.FromSeconds(1);
    private bool disposedValue;
    private ISession? session;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcConnection"/> class.
    /// </summary>
    /// <param name="options">A <see cref="IOptions{TOptions}"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public PlcConnection(IOptions<OpcPlcConnectionOptions> options, ILogger<PlcConnection> logger)
    {
        this.options = options;
        this.logger = logger;

        if (options.Value.AutoConnect)
        {
            _ = ConnectAsync().LogExceptionsAsync(logger);
        }
    }

    /// <inheritdoc/>
    public bool IsConnected
        => session?.Connected ?? false;

    /// <inheritdoc/>
    public IObservable<IConnected<ISession>> SessionStream
        => connectionState.AsObservable();

    /// <inheritdoc/>
    IObservable<IConnected> IPlcConnection.SessionStream
         => SessionStream;

    /// <inheritdoc/>
    public object Settings
        => options.Value;

    /// <inheritdoc/>
    public bool Connect()
        => ConnectAsync().GetAwaiter().GetResult();

    /// <inheritdoc/>
    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (IsConnected)
            {
                return true;
            }

            var settings = options.Value;
            var config = settings.ApplicationConfiguration ?? throw new InvalidOperationException("No valid application configuration given.");
            Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);
            logger.LogInformation("Opening connection to {Address}", settings.Address);
            logger.LogDebug("Creating an Application Configuration.");
            await config.Validate(ApplicationType.Client).ConfigureAwait(false);
            var usesSecurity = await SetupSecurityAsync(config).ConfigureAwait(false);
            logger.LogDebug("Discover endpoints of {DiscoveryAddress}.", settings.DiscoveryAddress);
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(settings.DiscoveryAddress.ToString(), usesSecurity, 15000);
            logger.LogDebug("Selected endpoint uses: {Security}", selectedEndpoint.SecurityPolicyUri[(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1)..]);
            logger.LogDebug("Create a session with OPC UA server.");
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(collection: null, selectedEndpoint, endpointConfiguration);
            var identity = GetUserIdentity(settings, selectedEndpoint);
            session?.Dispose();
            session = await Session.Create(config, endpoint, updateBeforeConnect: false, config.ApplicationName, 60000, identity, preferredLocales: null).ConfigureAwait(false);

            if (!IsConnected)
            {
                logger.LogError("Failed to connect to {Endpoint}", endpoint);
                session.Dispose();
                session = null;
                return false;
            }

            logger.LogInformation("Connected to {Endpoint}", endpoint);
            disposables.Add(Observable.FromEventPattern<KeepAliveEventHandler, ISession, KeepAliveEventArgs>(
                h => session.KeepAlive += h,
                h => session.KeepAlive -= h)
                .Where(x => ServiceResult.IsBad(x.EventArgs.Status))
                .Subscribe(KeepAliveSubscription));

            disposables.Add(Observable.FromEventPattern(
                 h => session.SessionClosing += h,
                 h => session.SessionClosing -= h)
                 .Subscribe(_ => connectionState.OnNext(Connected.No<ISession>())));

            connectionState.OnNext(Connected.Yes(session));
            return true;
        }
        catch (ServiceResultException ex)
        {
            LogConnectionException(ex);
            return false;
        }
    }

    /// <inheritdoc/>
    public void Disconnect()
        => DisconnectAsync().GetAwaiter().GetResult();

    /// <inheritdoc/>
    public async Task DisconnectAsync()
    {
        if (session == null)
        {
            return;
        }

        connectionState.OnNext(Connected.No<ISession>());
        if (session.Connected)
        {
            var closeSessionResponse = await session.CloseSessionAsync(requestHeader: null, deleteSubscriptions: false, CancellationToken.None).ConfigureAwait(false);
            if (ServiceResult.IsBad(closeSessionResponse.ResponseHeader.ServiceResult))
            {
                logger.LogError("Failed to close session {StatusCode}", closeSessionResponse.ResponseHeader.ServiceResult);
            }
        }

        disposables.Dispose();
        session?.Dispose();
        session = null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">Value indicating if we need to cleanup managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            Disconnect();
            connectionState.OnCompleted();
            disposables.Dispose();
            connectionState.Dispose();

            session?.Dispose();
        }

        disposedValue = true;
    }

    private static void UpdateApplicationUri(ApplicationConfiguration applicationConfiguration)
    {
        var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;
        if (applicationCertificate.Certificate != null)
        {
            applicationConfiguration.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(applicationCertificate.Certificate);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don't ignore created IDisposable", Justification = "already marked for disposing.")]
    private async Task CreateCertificate(ApplicationConfiguration applicationConfiguration)
    {
        var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;

        logger.LogDebug("Creating new application certificate for: {ApplicationName}", applicationConfiguration.ApplicationName);

        using var certificate = CertificateFactory.CreateCertificate(applicationConfiguration.ApplicationUri, applicationConfiguration.ApplicationName, applicationCertificate.SubjectName, domainNames: null)
             .SetNotBefore(DateTime.UtcNow - TimeSpan.FromDays(1))
             .SetNotAfter((DateTime.UtcNow - TimeSpan.FromDays(1)).AddMonths(CertificateFactory.DefaultLifeTime))
             .SetHashAlgorithm(X509Utils.GetRSAHashAlgorithmName(CertificateFactory.DefaultHashSize))
             .SetRSAKeySize(CertificateFactory.DefaultKeySize)
             .CreateForRSA();
        _ = await certificate.AddToStoreAsync(applicationCertificate.StoreType, applicationCertificate.StorePath, ct: CancellationToken.None)
            .ConfigureAwait(false);
    }

    private UserIdentity GetUserIdentity(OpcPlcConnectionOptions settings, EndpointDescription endpoint)
    {
        var tokenType = endpoint.UserIdentityTokens.FirstOrDefault(x => x.TokenType == UserTokenType.UserName);
        if (!string.IsNullOrEmpty(settings.UserName)
            && tokenType != null)
        {
            logger.LogDebug("Logging in with user: {UserName}", settings.UserName);
            return new UserIdentity(settings.UserName, settings.Password);
        }

        return new UserIdentity(new AnonymousIdentityToken());
    }

    private void KeepAliveSubscription(EventPattern<ISession, KeepAliveEventArgs> x)
    {
        connectionState.OnNext(Connected.No<ISession>());
        logger.LogError("{Status}, Reconnecting to {Endpoint}", x.EventArgs.Status, x.Sender?.ConfiguredEndpoint);
        var sessionReconnectHandler = new SessionReconnectHandler();
        _ = sessionReconnectHandler.BeginReconnect(x.Sender, (int)reconnectDelay.TotalMilliseconds, (s, e) =>
        {
            // ignore callbacks from discarded objects.
            if (!ReferenceEquals(s, sessionReconnectHandler))
            {
                return;
            }

            var sender = s as SessionReconnectHandler;
            logger.LogInformation("Connected to {Endpoint}", sender?.Session.ConfiguredEndpoint);
            connectionState.OnNext(Connected.Yes(sessionReconnectHandler.Session));
            sessionReconnectHandler.Dispose();
        });
    }

    private void LogConnectionException(ServiceResultException ex)
    {
        switch (ex.Result.StatusCode.Code)
        {
            case StatusCodes.BadNotConnected:
                logger.LogError(ex, "Failed to connect to Opc, check network connection and Opc settings");
                break;

            case StatusCodes.BadSecurityChecksFailed:
                logger.LogError(ex, "Security Checks Failed, Check if certificate is trusted");
                break;

            case StatusCodes.BadRequestTimeout:
                logger.LogError(ex, "Request Timed Out");
                break;

            default:
                logger.LogError(ex, "Unprocessed error {BrowseName}", StatusCodes.GetBrowseName(ex.Result.StatusCode.Code));
                break;
        }
    }

    private void SetupCertificateSigning(ApplicationConfiguration applicationConfiguration)
    {
        var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;
        var certificateValidationStream = Observable.FromEventPattern<CertificateValidationEventHandler, CertificateValidationEventArgs>(
            handler => handler.Invoke,
            h => applicationConfiguration.CertificateValidator.CertificateValidation += h,
            h => applicationConfiguration.CertificateValidator.CertificateValidation -= h);

        disposables.Add(certificateValidationStream.Subscribe(e =>
        {
            var eventArgs = e.EventArgs;
            var certificate = e.EventArgs.Certificate;

            if (applicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                logger.LogDebug("Accepted Certificate: {CertificateSubject} (error code: {BrowseName})", certificate.Subject, StatusCodes.GetBrowseName(eventArgs.Error.Code));
                eventArgs.Accept = true;
                return;
            }

            logger.LogDebug("Rejected Certificate: {CertificateSubject} (error code: {BrowseName})", certificate.Subject, StatusCodes.GetBrowseName(eventArgs.Error.Code));
        }));
    }

    private async Task<bool> SetupSecurityAsync(ApplicationConfiguration config)
    {
        var settings = options.Value;
        if (!settings.UseSecurity)
        {
            logger.LogWarning("Security turned off, using unsecure connection.");
            return false;
        }

        SetupCertificateSigning(config);

        var applicationCertificate = config.SecurityConfiguration.ApplicationCertificate;
        if (applicationCertificate.Certificate != null)
        {
            UpdateApplicationUri(config);
            return true;
        }

        if (!settings.AutoGenCertificate)
        {
            logger.LogWarning("Missing application certificate, using unsecure connection.");
            return false;
        }

        await CreateCertificate(config).ConfigureAwait(false);
        UpdateApplicationUri(config);
        return true;
    }
}
