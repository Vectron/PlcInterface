using System;
using System.Collections.Generic;

namespace PlcInterface
{
    public interface IMonitor
    {
        IObservable<IMonitorResult> SymbolStream
        {
            get;
        }

        void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000);

        void RegisterIO(string ioName, int updateInterval = 1000);

        void UnregisterIO(IEnumerable<string> ioNames);

        void UnregisterIO(string ioName);
    }
}