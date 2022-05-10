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
/// Implementation of <see cref="IPlcConnection{T}"/> for the <see cref="Session"/>.
/// </summary>
public class PlcConnection : IOpcPlcConnection, IDisposable
{
    private readonly BehaviorSubject<IConnected<Session>> connectionState = new(Connected.No<Session>());
    private readonly CompositeDisposable disposables = new();
    private readonly ILogger logger;
    private readonly IOptions<OPCSettings> options;
    private readonly TimeSpan reconnectDelay = TimeSpan.FromSeconds(1);
    private bool disposedValue;
    private Session? session;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcConnection"/> class.
    /// </summary>
    /// <param name="options">A <see cref="IOptions{TOptions}"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public PlcConnection(IOptions<OPCSettings> options, ILogger<PlcConnection> logger)
    {
        this.options = options;
        this.logger = logger;

        if (options.Value.AutoConnect)
        {
            _ = Task.Run(ConnectAsync, CancellationToken.None).ContinueWith(
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

    /// <inheritdoc/>
    public IObservable<IConnected<Session>> SessionStream
        => connectionState.AsObservable();

    /// <inheritdoc/>
    IObservable<IConnected> IPlcConnection.SessionStream
         => SessionStream;

    /// <inheritdoc/>
    public object Settings
        => options.Value;

    /// <inheritdoc/>
    public void Connect()
        => ConnectAsync().GetAwaiter().GetResult();

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0051:Method is too long", Justification = "Unable to make it shorter.")]
    public async Task ConnectAsync()
    {
        try
        {
            if (session != null
                && session.Connected)
            {
                return;
            }

            var settings = options.Value;
            var config = settings.ApplicationConfiguration ?? throw new InvalidOperationException("No vallid application configuration given.");
            Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);
            logger.LogInformation("Opening connection to {Adress}", settings.Address);
            logger.LogDebug("Creating an Application Configuration.");
            await config.Validate(ApplicationType.Client).ConfigureAwait(false);
            var usesSecurity = SetupSecurity(config);
            logger.LogDebug("Discover endpoints of {DiscoveryAdress}.", settings.DiscoveryAdress);
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(settings.DiscoveryAdress.ToString(), usesSecurity, 15000);
            logger.LogDebug("Selected endpoint uses: {Security}", selectedEndpoint.SecurityPolicyUri[(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1)..]);
            logger.LogDebug("Create a session with OPC UA server.");
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            var identity = GetUserIdentity(settings, selectedEndpoint);
            session?.Dispose();
            session = await Session.Create(config, endpoint, false, config.ApplicationName, 60000, identity, null).ConfigureAwait(false);

            if (!session.Connected)
            {
                logger.LogError("Failed to connect to {Endpoint}", endpoint);
                session.Dispose();
                session = null;
                return;
            }

            logger.LogInformation("Connected to {Endpoint}", endpoint);
            disposables.Add(Observable.FromEventPattern<KeepAliveEventHandler, Session, KeepAliveEventArgs>(
                h => session.KeepAlive += h,
                h => session.KeepAlive -= h)
                .Where(x => ServiceResult.IsBad(x.EventArgs.Status))
                .Subscribe(KeepAliveSubscription));

            disposables.Add(Observable.FromEventPattern(
                 h => session.SessionClosing += h,
                 h => session.SessionClosing -= h)
                 .Subscribe(_ => connectionState.OnNext(Connected.No<Session>())));

            connectionState.OnNext(Connected.Yes(session));
        }
        catch (ServiceResultException ex)
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
                    logger.LogError(ex, "Unproccesed error {BrowseName}", StatusCodes.GetBrowseName(ex.Result.StatusCode.Code));
                    break;
            }
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

        connectionState.OnNext(Connected.No<Session>());
        if (session.Connected)
        {
            var closeSessionResponse = await session.CloseSessionAsync(null, false, CancellationToken.None).ConfigureAwait(false);
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
        Dispose(true);
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

    private static void UpdateAplicationUri(ApplicationConfiguration applicationConfiguration)
    {
        var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;
        if (applicationCertificate.Certificate != null)
        {
            applicationConfiguration.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(applicationCertificate.Certificate);
        }
    }

    private void CreateCertificate(ApplicationConfiguration applicationConfiguration)
    {
        var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;

        logger.LogDebug("Creating new application certificate for: {ApplicationName}", applicationConfiguration.ApplicationName);

        using var certificate = CertificateFactory.CreateCertificate(applicationConfiguration.ApplicationUri, applicationConfiguration.ApplicationName, applicationCertificate.SubjectName, null)
             .SetNotBefore(DateTime.UtcNow - TimeSpan.FromDays(1))
             .SetNotAfter((DateTime.UtcNow - TimeSpan.FromDays(1)).AddMonths(CertificateFactory.DefaultLifeTime))
             .SetHashAlgorithm(X509Utils.GetRSAHashAlgorithmName(CertificateFactory.DefaultHashSize))
             .SetRSAKeySize(CertificateFactory.DefaultKeySize)
             .CreateForRSA()
             .AddToStore(applicationCertificate.StoreType, applicationCertificate.StorePath);
    }

    private UserIdentity GetUserIdentity(OPCSettings settings, EndpointDescription endpoint)
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

    private void KeepAliveSubscription(EventPattern<Session, KeepAliveEventArgs> x)
    {
        connectionState.OnNext(Connected.No<Session>());
        logger.LogError("{Status}, Reconnecting to {Endpoint}", x.EventArgs.Status, x.Sender?.ConfiguredEndpoint);
        var sessionReconnectHandler = new SessionReconnectHandler();
        sessionReconnectHandler.BeginReconnect(x.Sender, (int)reconnectDelay.TotalMilliseconds, (s, e) =>
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
                logger.LogDebug("Accepted Certificate: {CertificateSubject} (errorcode: {BrowseName})", certificate.Subject, StatusCodes.GetBrowseName(eventArgs.Error.Code));
                eventArgs.Accept = true;
                return;
            }

            logger.LogDebug("Rejected Certificate: {CertificateSubject} (errorcode: {BrowseName})", certificate.Subject, StatusCodes.GetBrowseName(eventArgs.Error.Code));
        }));
    }

    private bool SetupSecurity(ApplicationConfiguration config)
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
            UpdateAplicationUri(config);
            return true;
        }

        if (!settings.AutoGenCertificate)
        {
            logger.LogWarning("Missing application certificate, using unsecure connection.");
            return false;
        }

        CreateCertificate(config);
        UpdateAplicationUri(config);
        return true;
    }
}