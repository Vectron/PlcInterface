using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace PlcInterface.S7
{
    public class Monitor : IMonitor
    {
        private PlcConnection connection;
        private ILogger<Monitor> logger;
        private SymbolHandler symbolHandler;

        public Monitor(PlcConnection connection, SymbolHandler symbolHandler, ILogger<Monitor> logger)
        {
            this.connection = connection;
            this.symbolHandler = symbolHandler;
            this.logger = logger;
        }

        public IObservable<IMonitorResult> SymbolStream
        {
            get;
        }

        public void RegisterIO(IEnumerable<string> ioNames, int updateInterval = 1000) => throw new NotImplementedException();

        public void RegisterIO(string ioName, int updateInterval = 1000) => throw new NotImplementedException();

        public void UnregisterIO(IEnumerable<string> ioNames) => throw new NotImplementedException();

        public void UnregisterIO(string ioName) => throw new NotImplementedException();
    }
}