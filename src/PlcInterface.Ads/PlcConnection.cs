﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwinCAT.Ads;

namespace PlcInterface.Ads
{
    public class PlcConnection : IPlcConnection<IAdsConnection>, IDisposable
    {
        private readonly BehaviorSubject<IConnected<AdsClient>> connectionState = new BehaviorSubject<IConnected<AdsClient>>(Connected.No<AdsClient>());
        private readonly ILogger<PlcConnection> logger;
        private readonly IOptions<ConnectionSettings> settings;

        private bool disposedValue = false;

        public PlcConnection(IOptions<ConnectionSettings> settings, ILogger<PlcConnection> logger)
        {
            this.settings = settings;
            this.logger = logger;

            if (settings.Value.AutoConnect)
            {
                ConnectAsync().LogExceptionsAsync(logger);
            }
        }

        public IObservable<IConnected<IAdsConnection>> SessionStream
            => connectionState.AsObservable();

        IObservable<IConnected> IPlcConnection.SessionStream
            => SessionStream;

        public object Settings
            => settings.Value;

        public void Connect()
        {
            if (connectionState.TryGetValue(out IConnected<AdsClient> lastConnectionState)
                && lastConnectionState.IsConnected == true
                && lastConnectionState.Value?.IsConnected == true)
            {
                return;
            }

            var adsClient = new AdsClient();

            adsClient.RouterStateChanged += AdsClient_RouterStateChanged;
            var Address = new AmsAddress(settings.Value.AmsNetId, settings.Value.Port);
            adsClient.Connect(Address);

            if (adsClient.IsConnected
                && adsClient.TryReadState(out StateInfo plcState) == AdsErrorCode.NoError
                && (plcState.AdsState == AdsState.Run || plcState.AdsState == AdsState.Stop))
            {
                logger.LogInformation($"Connected to {Address}");
                connectionState.OnNext(Connected.Yes(adsClient));
            }
            else
            {
                connectionState.OnNext(Connected.No<AdsClient>());
            }
        }

        public async Task ConnectAsync()
            => await Task.Run(() => Connect());

        public void Disconnect()
        {
            if (connectionState.TryGetValue(out IConnected<AdsClient> lastConnectionState)
                && lastConnectionState.IsConnected == false)
            {
                return;
            }

            connectionState.OnNext(Connected.No<AdsClient>());
            if (lastConnectionState == null)
            {
                return;
            }

            var connection = lastConnectionState.Value;
            if (connection == null)
            {
                return;
            }

            connection.Disconnect();

            var disposableConnection = connection as IDisposable;
            if (connection == null)
            {
                return;
            }

            disposableConnection.Dispose();
        }

        public Task DisconnectAsync()
            => Task.Run(() => Disconnect());

        public void Dispose()
        {
            Dispose(true);
        }

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

        private void AdsClient_RouterStateChanged(object sender, AmsRouterNotificationEventArgs e)
        {
            // TODO, this might indicate that we need to reconnect
            Console.WriteLine($"Ads Router Notification gotten {e.State}");
        }
    }
}