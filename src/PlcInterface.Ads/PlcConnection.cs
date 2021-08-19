﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcInterface.Extensions;
using TwinCAT.Ads;

namespace PlcInterface.Ads
{
    /// <summary>
    /// Implementation of <see cref="IPlcConnection{T}"/> for the <see cref="IAdsConnection"/>.
    /// </summary>
    public class PlcConnection : IAdsPlcConnection, IDisposable
    {
        private readonly IAdsDisposableConnection adsDisposableConnection;
        private readonly BehaviorSubject<IConnected<IAdsDisposableConnection>> connectionState = new(Connected.No<IAdsDisposableConnection>());
        private readonly IOptions<ConnectionSettings> settings;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlcConnection"/> class.
        /// </summary>
        /// <param name="settings">A <see cref="IOptions{TOptions}"/> implementation.</param>
        /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
        /// <param name="adsDisposableConnection">The ads client used for connecting.</param>
        public PlcConnection(IOptions<ConnectionSettings> settings, ILogger<PlcConnection> logger, IAdsDisposableConnection adsDisposableConnection)
        {
            this.settings = settings;
            this.adsDisposableConnection = adsDisposableConnection;
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
                if (adsDisposableConnection.IsConnected)
                {
                    return;
                }

                var address = new AmsAddress(settings.Value.AmsNetId, settings.Value.Port);
                adsDisposableConnection.ConnectionStateChanged += AdsDisposableConnection_ConnectionStateChanged;
                adsDisposableConnection.Connect(address);
            });

        /// <inheritdoc/>
        public void Disconnect()
            => DisconnectAsync().GetAwaiter().GetResult();

        /// <inheritdoc/>
        public Task DisconnectAsync()
            => Task.Run(() =>
            {
                _ = adsDisposableConnection.Disconnect();
                adsDisposableConnection.ConnectionStateChanged -= AdsDisposableConnection_ConnectionStateChanged;
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
                    connectionState.Dispose();
                }

                disposedValue = true;
            }
        }

        private void AdsDisposableConnection_ConnectionStateChanged(object sender, TwinCAT.ConnectionStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case TwinCAT.ConnectionState.Connected:
                    connectionState.OnNext(Connected.Yes((IAdsDisposableConnection)sender));
                    break;

                case TwinCAT.ConnectionState.None:
                case TwinCAT.ConnectionState.Disconnected:
                case TwinCAT.ConnectionState.Lost:
                default:
                    connectionState.OnNext(Connected.No<IAdsDisposableConnection>());
                    break;
            }
        }
    }
}