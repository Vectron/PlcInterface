using System;
using System.Threading.Tasks;

namespace PlcInterface
{
    public interface IPlcConnection<T> : IPlcConnection
    {
        new IObservable<IConnected<T>> SessionStream
        {
            get;
        }
    }

    public interface IPlcConnection
    {
        IObservable<IConnected> SessionStream
        {
            get;
        }

        object Settings
        {
            get;
        }

        void Connect();

        Task ConnectAsync();

        void Disconnect();

        Task DisconnectAsync();
    }
}