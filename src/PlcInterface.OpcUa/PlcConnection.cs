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
using VectronsLibrary.Extensions;

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
                var config = settings.ApplicationConfiguration;
                Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);
                logger.LogInformation("Opening connection to {0}", settings.Address);
                logger.LogDebug("Creating an Application Configuration.");
                await config.Validate(ApplicationType.Client);
                var usesSecurity = SetupSecurity();
                logger.LogDebug($"Discover endpoints of {settings.DiscoveryAdress}.");
                var selectedEndpoint = CoreClientUtils.SelectEndpoint(settings.DiscoveryAdress.ToString(), usesSecurity, 15000);
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

                var session = await Session.Create(config, endpoint, false, settings.ApplicationConfiguration.ApplicationName, 60000, identity, null);

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
                        logger.LogError(ex, $"Unproccesed error {StatusCodes.GetBrowseName(ex.Result.StatusCode.Code)}");
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

        private void CreateCertificate(ApplicationConfiguration applicationConfiguration)
        {
            var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;

            logger.LogDebug($"Creating new application certificate for: {applicationConfiguration.ApplicationName}");
            applicationCertificate.Certificate = CertificateFactory.CreateCertificate(
                applicationCertificate.StoreType,
                applicationCertificate.StorePath,
                null,
                applicationConfiguration.ApplicationUri,
                applicationConfiguration.ApplicationName,
                applicationCertificate.SubjectName,
                null,
                CertificateFactory.defaultKeySize,
                DateTime.UtcNow - TimeSpan.FromDays(1),
                CertificateFactory.defaultLifeTime,
                CertificateFactory.defaultHashSize,
                false,
                null,
                null);
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
                    logger.LogDebug("Accepted Certificate: {0} (errorcode: {1})", certificate.Subject, StatusCodes.GetBrowseName(eventArgs.Error.Code));
                    eventArgs.Accept = true;
                    return;
                }

                logger.LogDebug("Rejected Certificate: {0} (errorcode: {1})", certificate.Subject, StatusCodes.GetBrowseName(eventArgs.Error.Code));
            }));
        }

        private bool SetupSecurity()
        {
            var settings = this.settings.Value;
            if (!settings.UseSecurity)
            {
                logger.LogWarning("Security turned off, using unsecure connection.");
                return false;
            }

            var applicationConfiguration = settings.ApplicationConfiguration;
            SetupCertificateSigning(applicationConfiguration);

            var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;
            if (applicationCertificate.Certificate != null)
            {
                UpdateAplicationUri(applicationConfiguration);
                return true;
            }

            if (!settings.AutoGenCertificate)
            {
                logger.LogWarning("Missing application certificate, using unsecure connection.");
                return false;
            }

            CreateCertificate(applicationConfiguration);
            UpdateAplicationUri(applicationConfiguration);
            return true;
        }

        private void UpdateAplicationUri(ApplicationConfiguration applicationConfiguration)
        {
            var applicationCertificate = applicationConfiguration.SecurityConfiguration.ApplicationCertificate;
            if (applicationCertificate.Certificate != null)
            {
                applicationConfiguration.ApplicationUri = Utils.GetApplicationUriFromCertificate(applicationCertificate.Certificate);
            }
        }
    }
}