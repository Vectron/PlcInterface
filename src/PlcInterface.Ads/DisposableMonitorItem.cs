using System;

namespace PlcInterface.Ads
{
    internal class DisposableMonitorItem : IDisposable
    {
        private readonly IDisposable stream;

        private bool disposedValue = false;

        public DisposableMonitorItem(IDisposable stream)
        {
            this.stream = stream;
            Subscriptions = 1;
        }

        public int Subscriptions
        {
            get;
            set;
        }

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
                    stream?.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}