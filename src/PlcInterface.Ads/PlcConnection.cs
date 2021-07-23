using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwinCAT.Ads;

namespace PlcInterface.Ads
{
    /// <summary>
    /// Implementation of <see cref="IPlcConnection{T}"/> for the <see cref="IAdsConnection"/>.
    /// </summary>
    public class PlcConnection : IPlcConnection<IAdsConnection>, IDisposable
    {
        private readonly BehaviorSubject<IConnected<AdsClient>> connectionState = new(Connected.No<AdsClient>());
        private readonly ILogger logger;
        private readonly IOptions<ConnectionSettings> settings;
        private AdsClient? adsClient;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlcConnection"/> class.
        /// </summary>
        /// <param name="settings">A <see cref="IOptions{TOptions}"/> implementation.</param>
        /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
        public PlcConnection(IOptions<ConnectionSettings> settings, ILogger<PlcConnection> logger)
        {
            this.settings = settings;
            this.logger = logger;

            if (settings.Value.AutoConnect)
            {
                _ = ConnectAsync().LogExceptionsAsync(logger);
            }
        }

        /// <inheritdoc/>
        public IObservable<IConnected<IAdsConnection>> SessionStream
            => connectionState.AsObservable();

        /// <inheritdoc/>
        IObservable<IConnected> IPlcConnection.SessionStream
            => SessionStream;

        /// <inheritdoc/>
        public object Settings
            => settings.Value;

        /// <inheritdoc/>
        public void Connect()
            => ConnectAsync().GetAwaiter().GetResult();

        /// <inheritdoc/>
        public Task ConnectAsync()
            => Task.Run(() =>
            {
                if (adsClient != null
                   && adsClient.IsConnected)
                {
                    return;
                }

                adsClient?.Dispose();
                adsClient = new AdsClient(logger);
                adsClient.RouterStateChanged += AdsClient_RouterStateChanged;
                var address = new AmsAddress(settings.Value.AmsNetId, settings.Value.Port);
                adsClient.Connect(address);

                if (adsClient.IsConnected
                    && adsClient.TryReadState(out var plcState) == AdsErrorCode.NoError
                    && (plcState.AdsState == AdsState.Run || plcState.AdsState == AdsState.Stop))
                {
                    logger.LogInformation($"Connected to {address}");
                    connectionState.OnNext(Connected.Yes(adsClient));
                }
                else
                {
                    connectionState.OnNext(Connected.No<AdsClient>());
                }
            });

        /// <inheritdoc/>
        public void Disconnect()
            => DisconnectAsync().GetAwaiter().GetResult();

        /// <inheritdoc/>
        public Task DisconnectAsync()
            => Task.Run(() =>
            {
                if (adsClient == null)
                {
                    return;
                }

                connectionState.OnNext(Connected.No<AdsClient>());
                if (adsClient.IsConnected)
                {
                    _ = adsClient.Disconnect();
                }

                adsClient.Dispose();
                adsClient = null;
            });

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
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();
                    connectionState.OnCompleted();
                    connectionState?.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO, this might indicate that we need to reconnect
        private void AdsClient_RouterStateChanged(object sender, AmsRouterNotificationEventArgs e) =>
            logger.LogDebug("Ads Router Notification gotten {0}", e.State);
    }
}