using Microsoft.Extensions.Logging;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcInterface.S7
{
    internal class ReadWrite : IReadWrite
    {
        private readonly IPlcConnection<S7Client> connection;
        private readonly ILogger<ReadWrite> logger;
        private readonly ISymbolHandler symbolHandler;

        public ReadWrite(IPlcConnection<S7Client> connection, ILogger<ReadWrite> logger, ISymbolHandler symbolHandler)
        {
            this.connection = connection;
            this.logger = logger;
            this.symbolHandler = symbolHandler;
        }

        public IDictionary<string, object> Read(IEnumerable<string> ioNames) => throw new NotImplementedException();

        public object Read(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            var client = connection.GetConnectedClient();
            var buffer = new byte[1000];

            var result = client.ReadArea((int)symbolInfo.AreaType, symbolInfo.DBNumber, symbolInfo.StartAdress, 1, (int)symbolInfo.SymbolType, buffer);

            throw new NotImplementedException();
        }

        public T Read<T>(string ioName) => throw new NotImplementedException();

        public Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames) => throw new NotImplementedException();

        public Task<object> ReadAsync(string ioName) => throw new NotImplementedException();

        public Task<T> ReadAsync<T>(string ioName) => throw new NotImplementedException();

        public dynamic ReadDynamic(string ioName) => throw new NotImplementedException();

        public Task<dynamic> ReadDynamicAsync(string ioName) => throw new NotImplementedException();

        public void ToggleBool(string ioName) => throw new NotImplementedException();

        public void Write(IDictionary<string, object> namesValues) => throw new NotImplementedException();

        public void Write<T>(string ioName, T value) => throw new NotImplementedException();

        public Task WriteAsync(IDictionary<string, object> namesValues) => throw new NotImplementedException();

        public Task WriteAsync<T>(string ioName, T value) => throw new NotImplementedException();
    }
}