using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sharp7;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VectronsLibrary.Extensions;

namespace PlcInterface.S7
{
    public class PlcConnection : IPlcConnection<S7Client>, IDisposable
    {
        private readonly BehaviorSubject<IConnected<S7Client>> connectionState = new BehaviorSubject<IConnected<S7Client>>(Connected.No<S7Client>());
        private readonly ILogger<PlcConnection> logger;
        private readonly IOptions<ConnectionSettings> options;

        private bool disposedValue = false;

        public PlcConnection(IOptions<ConnectionSettings> options, ILogger<PlcConnection> logger)
        {
            this.logger = logger;
            this.options = options;

            if (options.Value.AutoConnect)
            {
                _ = ConnectAsync().LogExceptionsAsync(logger);
            }
        }

        public IObservable<IConnected<S7Client>> SessionStream
            => connectionState.AsObservable();

        IObservable<IConnected> IPlcConnection.SessionStream
            => SessionStream.AsObservable();

        public object Settings
            => options.Value;

        public void Connect()
        {
            if (connectionState.TryGetValue(out var lastConnectionState)
                && lastConnectionState.IsConnected == true
                && lastConnectionState.Value?.Connected == true)
            {
                return;
            }

            var settings = options.Value;
            var client = new S7Client();
            var result = client.ConnectTo(settings.Adress, settings.Rack, settings.Slot);
            if (result == 0)
            {
                logger.LogInformation($"Connected to {0}", settings.Adress);
                connectionState.OnNext(Connected.Yes(client));
            }
            else
            {
                var error = client.ErrorText(result);
                logger.LogError("failed to connect to {0}; error: {1}", settings.Adress, error);
                connectionState.OnNext(Connected.No<S7Client>());
            }
        }

        public async Task ConnectAsync()
            => await Task.Run(() => Connect());

        public void Disconnect()
        {
            if (connectionState.TryGetValue(out var lastConnectionState)
                && lastConnectionState.IsConnected == false)
            {
                return;
            }

            connectionState.OnNext(Connected.No<S7Client>());
            if (lastConnectionState == null)
            {
                return;
            }

            var connection = lastConnectionState.Value;
            if (connection == null)
            {
                return;
            }

            var result = connection.Disconnect();
            if (result != 0)
            {
                var error = connection.ErrorText(result);
                logger.LogError("Failed to disconnect from plc; error: {1}", error);
            }
        }

        public async Task DisconnectAsync()
            => await Task.Run(() => Disconnect());

        public void Dispose()
            => Dispose(true);

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
    }
}