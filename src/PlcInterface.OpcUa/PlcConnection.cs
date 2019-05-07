using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace PlcInterface.OpcUa
{
    public class PlcConnection : IPlcConnection<Session>, IDisposable
    {
        private readonly BehaviorSubject<IConnected<Session>> connectionState = new BehaviorSubject<IConnected<Session>>(Connected.No<Session>());
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ILogger logger;
        private readonly TimeSpan reconnectDelay = TimeSpan.FromSeconds(1);
        private readonly IOptions<OPCSettings> settings;
        private bool disposedValue = false;

        public PlcConnection(IOptions<OPCSettings> settings, ILogger<PlcConnection> logger)
        {
            this.settings = settings;
            this.logger = logger;

            if (settings.Value.AutoConnect)
            {
                _ = Task.Run(ConnectAsync).LogExceptionsAsync(logger);
            }
        }

        public IObservable<IConnected<Session>> SessionStream
            => connectionState.AsObservable();

        IObservable<IConnected> IPlcConnection.SessionStream
             => SessionStream;

        public object Settings
            => settings.Value;

        public void Connect()
            => ConnectAsync().GetAwaiter().GetResult();

        public async Task ConnectAsync()
        {
            try
            {
                if (connectionState.TryGetValue(out IConnected<Session> lastConnectionState) &&
                    lastConnectionState.IsConnected == true &&
                    lastConnectionState.Value?.Connected == true)
                {
                    return;
                }

                var settings = this.settings.Value;
                Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);
                logger.LogInformation("Opening connection to {0}", settings.Address);
                logger.LogDebug("Creating an Application Configuration.");
                var config = new ApplicationConfiguration()
                {
                    ApplicationName = settings.ApplicationName,
                    ApplicationType = ApplicationType.Client,
                    ApplicationUri = "urn:" + Utils.GetHostName() + ":" + settings.ApplicationName,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier
                        {
                            StoreType = "X509Store",
                            StorePath = "CurrentUser\\My",
                            SubjectName = settings.ApplicationName
                        },
                        TrustedPeerCertificates = new CertificateTrustList
                        {
                            StoreType = "Directory",
                            StorePath = "OPC Foundation/CertificateStores/UA Applications",
                        },
                        TrustedIssuerCertificates = new CertificateTrustList
                        {
                            StoreType = "Directory",
                            StorePath = "OPC Foundation/CertificateStores/UA Certificate Authorities",
                        },
                        RejectedCertificateStore = new CertificateTrustList
                        {
                            StoreType = "Directory",
                            StorePath = "OPC Foundation/CertificateStores/RejectedCertificates",
                        },
                        NonceLength = 32,
                        AutoAcceptUntrustedCertificates = true
                    },
                    TransportConfigurations = new TransportConfigurationCollection(),
                    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000, }
                };

                // TODO: Option to get data from application settings section
                // var config = await ApplicationConfiguration.Load(settings.ConfigSection, ApplicationType.Client);
                await config.Validate(ApplicationType.Client);
                bool haveAppCertificate = config.SecurityConfiguration.ApplicationCertificate.Certificate != null;

                if (!haveAppCertificate && settings.AutoGenCertificate)
                {
                    logger.LogDebug($"Creating new application certificate: {config.ApplicationName}");
                    var certificate = CertificateFactory.CreateCertificate(
                        config.SecurityConfiguration.ApplicationCertificate.StoreType,
                        config.SecurityConfiguration.ApplicationCertificate.StorePath,
                        null,
                        config.ApplicationUri,
                        config.ApplicationName,
                        config.SecurityConfiguration.ApplicationCertificate.SubjectName,
                        null,
                        CertificateFactory.defaultKeySize,
                        DateTime.UtcNow - TimeSpan.FromDays(1),
                        CertificateFactory.defaultLifeTime,
                        CertificateFactory.defaultHashSize,
                        false,
                        null,
                        null);

                    config.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;
                }

                haveAppCertificate = config.SecurityConfiguration.ApplicationCertificate.Certificate != null;

                if (haveAppCertificate)
                {
                    config.ApplicationUri = Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);

                    if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                    {
                        var certificateValidationStream = Observable.FromEventPattern<CertificateValidationEventHandler, CertificateValidationEventArgs>(
                            handler => handler.Invoke,
                            h => config.CertificateValidator.CertificateValidation += h,
                            h => config.CertificateValidator.CertificateValidation -= h);

                        disposables.Add(certificateValidationStream.Subscribe(e =>
                        {
                            logger.LogDebug($"Accepted Certificate: {e.EventArgs.Certificate.Subject}");
                            e.EventArgs.Accept = e.EventArgs.Error.StatusCode == StatusCodes.BadCertificateUntrusted;
                        }));
                    }
                }
                else
                {
                    logger.LogWarning("Missing application certificate, using unsecure connection.");
                }

                logger.LogDebug($"Discover endpoints of {settings.DiscoveryAdress}.");
                var selectedEndpoint = CoreClientUtils.SelectEndpoint(settings.DiscoveryAdress.ToString(), haveAppCertificate, 15000);
                logger.LogDebug($"Selected endpoint uses: {selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1)}");

                logger.LogDebug("Create a session with OPC UA server.");
                var endpointConfiguration = EndpointConfiguration.Create(config);
                var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

                var tokenType = selectedEndpoint.UserIdentityTokens.Where(x => x.TokenType == UserTokenType.UserName).FirstOrDefault();
                UserIdentity identity;

                if (!string.IsNullOrEmpty(settings.UserName) && tokenType != null)
                {
                    logger.LogDebug($"Logging in with user: {settings.UserName}");
                    identity = new UserIdentity(settings.UserName, settings.Password);
                }
                else
                {
                    identity = new UserIdentity(new AnonymousIdentityToken());
                }

                var session = await Session.Create(config, endpoint, false, settings.ApplicationName, 60000, identity, null);

                if (session.Connected)
                {
                    logger.LogInformation($"Connected to {endpoint}");
                }
                else
                {
                    logger.LogError($"Failed to connect to {endpoint}");
                }

                disposables.Add(Observable.FromEventPattern<KeepAliveEventHandler, Session, KeepAliveEventArgs>(
                    h => session.KeepAlive += h,
                    h => session.KeepAlive -= h)
                    .Where(x => ServiceResult.IsBad(x.EventArgs.Status))
                    .Subscribe(x =>
                    {
                        connectionState.OnNext(Connected.No<Session>());
                        logger.LogInformation($"{x.EventArgs.Status}, Reconnecting to {x.Sender.ConfiguredEndpoint}");
                        var sessionReconnectHandler = new SessionReconnectHandler();
                        sessionReconnectHandler.BeginReconnect(x.Sender, (int)reconnectDelay.TotalMilliseconds, (s, e) =>
                        {
                            // ignore callbacks from discarded objects.
                            if (!ReferenceEquals(s, sessionReconnectHandler))
                            {
                                return;
                            }
                            var sender = s as SessionReconnectHandler;
                            logger.LogInformation($"Connected to {sender.Session.ConfiguredEndpoint}");
                            connectionState.OnNext(Connected.Yes(sessionReconnectHandler.Session));
                            sessionReconnectHandler.Dispose();
                        });
                    }));

                var sessionClosingStream = Observable.FromEventPattern(
                     h => session.SessionClosing += h,
                     h => session.SessionClosing -= h);

                disposables.Add(sessionClosingStream.Subscribe(_ => connectionState.OnNext(Connected.No<Session>())));

                connectionState.OnNext(Connected.Yes(session));
            }
            catch (ServiceResultException ex)
            {
                switch (ex.Result.StatusCode.Code)
                {
                    case StatusCodes.BadNotConnected:
                        logger.LogError("Failed to connect to Opc, check network connection and Opc settings");
                        logger.LogTrace(ex, "Failed to connect to Opc, check network connection and Opc settings");
                        break;

                    case StatusCodes.BadSecurityChecksFailed:
                        logger.LogError("Security Checks Failed, Check if certificate is trusted");
                        logger.LogTrace(ex, "Security Checks Failed, Check if certificate is trusted");
                        break;

                    case StatusCodes.BadRequestTimeout:
                        logger.LogError("Request Timed Out");
                        logger.LogTrace(ex, "Request Timed Out");
                        break;

                    default:
                        logger.LogError($"Unproccesed error {StatusCodes.GetBrowseName(ex.Result.StatusCode.Code)}");
                        logger.LogTrace(ex, $"Unproccesed error {StatusCodes.GetBrowseName(ex.Result.StatusCode.Code)}");
                        break;
                }
            }
        }

        public void Disconnect()
        {
            if (connectionState.TryGetValue(out var lastConnectionState)
                && lastConnectionState.IsConnected == true
                && lastConnectionState.Value?.Connected == true)
            {
                connectionState.OnNext(Connected.No<Session>());
                _ = lastConnectionState.Value.CloseSession(null, false);
            }
        }

        public Task DisconnectAsync()
            => Task.Run(() => Disconnect());

        public void Dispose()
            => Dispose(true);

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
                connectionState?.Dispose();
            }

            disposedValue = true;
        }
    }
}