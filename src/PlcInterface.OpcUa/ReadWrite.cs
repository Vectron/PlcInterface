using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using PlcInterface.OpcUa.Dynamic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace PlcInterface.OpcUa
{
    public class ReadWrite : IDisposable, IReadWrite
    {
        private readonly IPlcConnection<Session> client;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ILogger logger;
        private readonly ISymbolHandler symbolHandler;
        private bool disposedValue = false;

        public ReadWrite(IPlcConnection<Session> client, ISymbolHandler symbolHandler, ILogger<ReadWrite> logger)
        {
            this.client = client;
            this.symbolHandler = symbolHandler;
            this.logger = logger;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IDictionary<string, object> Read(IEnumerable<string> ioNames)
        {
            var nodesToRead = ioNames
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Where(x => x is SymbolInfo)
                .Cast<SymbolInfo>()
                .Select(x => x.Handle)
                .ToList();

            var nodesTypes = Enumerable.Repeat(typeof(object), nodesToRead.Count).ToList();

            var session = client.GetConnectedClient();
            session.ReadValues(nodesToRead, nodesTypes, out List<object> values, out List<ServiceResult> errors);

            return ioNames
                .Zip(values, (name, value) => (name, value))
                .Zip(errors, (nv, error) => (nv.name, nv.value, error))
                .Where(x => ServiceResult.IsGood(x.error))
                .ToDictionary(nve => nve.name, nve => nve.value);
        }

        public T Read<T>(string ioName)
        {
            var session = client.GetConnectedClient();
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            return (T)session.ReadValue(symbol.Handle).GetValue(typeof(T));
        }

        public object Read(string ioName)
        {
            var session = client.GetConnectedClient();
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            return session.ReadValue(symbol.Handle).Value;
        }

        public Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
        {
            var querry = ioNames
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Where(x => x is SymbolInfo)
                .Cast<SymbolInfo>()
                .Select(x => new ReadValueId()
                {
                    NodeId = x.Handle,
                    AttributeId = Attributes.Value
                });

            var session = client.GetConnectedClient();

            var nodesToRead = new ReadValueIdCollection(querry);
            var taskCompletionSource = new TaskCompletionSource<IDictionary<string, object>>();

            session.BeginRead(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                ar =>
                {
                    try
                    {
                        var responseHeader = session.EndRead(ar, out DataValueCollection dataValues, out DiagnosticInfoCollection diagnosticInfos);
                        var statusCodes = new StatusCodeCollection(dataValues.Select(x => x.StatusCode));
                        ValidateResponse(nodesToRead, responseHeader, statusCodes, diagnosticInfos, ioNames);

                        var result = ioNames
                                .Zip(dataValues, (name, value) => (name, value.Value, value.StatusCode))
                                .Where(x => ServiceResult.IsGood(x.StatusCode))
                                .ToDictionary(nve => nve.name, nve => nve.Value);

                        taskCompletionSource.TrySetResult(result);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                },
                null);

            return taskCompletionSource.Task;
        }

        public Task<object> ReadAsync(string ioName)
        {
            var session = client.GetConnectedClient();
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId()
                {
                    NodeId = symbol.Handle,
                    AttributeId = Attributes.Value
                }
            };

            var taskCompletionSource = new TaskCompletionSource<object>();
            session.BeginRead(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                ar =>
                {
                    try
                    {
                        var responseHeader = session.EndRead(ar, out DataValueCollection dataValues, out DiagnosticInfoCollection diagnosticInfos);
                        var val = dataValues.FirstOrDefault();
                        var statusCodes = new StatusCodeCollection(dataValues.Select(x => x.StatusCode));
                        ValidateResponse(nodesToRead, responseHeader, statusCodes, diagnosticInfos, new[] { ioName });
                        taskCompletionSource.TrySetResult(val.Value);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                },
                null);

            return taskCompletionSource.Task;
        }

        public Task<T> ReadAsync<T>(string ioName)
        {
            var session = client.GetConnectedClient();
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId()
                {
                    NodeId = symbol.Handle,
                    AttributeId = Attributes.Value
                }
            };

            var taskCompletionSource = new TaskCompletionSource<T>();
            session.BeginRead(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                ar =>
                {
                    var responseHeader = session.EndRead(ar, out DataValueCollection dataValues, out DiagnosticInfoCollection diagnosticInfos);

                    try
                    {
                        var val = dataValues.FirstOrDefault();
                        var statusCodes = new StatusCodeCollection(dataValues.Select(x => x.StatusCode));
                        ValidateResponse(nodesToRead, responseHeader, statusCodes, diagnosticInfos, new[] { ioName });
                        taskCompletionSource.TrySetResult((T)val.GetValue(typeof(T)));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                },
                null);

            return taskCompletionSource.Task;
        }

        public dynamic ReadDynamic(string ioName)
        {
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();

            if (symbol.ChildSymbols.Count > 0)
            {
                var collection = new DynamicCollection();
                foreach (var childSymbol in symbol.ChildSymbols)
                {
                    var value = ReadDynamic(childSymbol);
                    var shortName = symbolHandler.GetSymbolinfo(childSymbol).ShortName;
                    collection.Add(shortName, value);
                }
                return collection;
            }
            else
            {
                var value = Read(ioName);
                if (value is Matrix matrixValue)
                {
                    return matrixValue.ToArray();
                }

                return value;
            }
        }

        public async Task<dynamic> ReadDynamicAsync(string ioName)
            => await Task.Run(() => ReadDynamic(ioName));

        public void ToggleBool(string ioName)
        {
            var previousValue = Read<bool>(ioName);
            Write(ioName, !previousValue);
        }

        public void Write(IDictionary<string, object> namesValues)
        {
            var session = client.GetConnectedClient();
            var querry = namesValues.Select(x => new WriteValue()
            {
                NodeId = symbolHandler.GetSymbolinfo(x.Key).ConvertAndValidate().Handle,
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant(x.Value))
            });

            var nodesToWrite = new WriteValueCollection(querry);

            ResponseHeader responseHeader = session.Write(
                null,
                nodesToWrite,
                out StatusCodeCollection statusCodes,
                out DiagnosticInfoCollection diagnosticInfos);

            ValidateResponse(nodesToWrite, responseHeader, statusCodes, diagnosticInfos, namesValues.Keys);
            Task.Delay(500).Wait();
        }

        public void Write<T>(string ioName, T value)
            => Write(new Dictionary<string, object>() { { ioName, value } });

        public Task WriteAsync(IDictionary<string, object> namesValues)
        {
            var session = client.GetConnectedClient();
            var querry = namesValues.Select(x => new WriteValue()
            {
                NodeId = symbolHandler.GetSymbolinfo(x.Key).ConvertAndValidate().Handle,
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant(x.Value))
            });

            var nodesToWrite = new WriteValueCollection(querry);
            var taskCompletionSource = new TaskCompletionSource<bool>();

            session.BeginWrite(
                null,
                nodesToWrite,
                r =>
                {
                    var responseHeader = session.EndWrite(r, out StatusCodeCollection statusCodes, out DiagnosticInfoCollection diagnosticInfos);
                    try
                    {
                        ValidateResponse(nodesToWrite, responseHeader, statusCodes, diagnosticInfos, namesValues.Keys);
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                },
                null);

            return taskCompletionSource.Task;
        }

        public Task WriteAsync<T>(string ioName, T value)
            => WriteAsync(new Dictionary<string, object>() { { ioName, value } });

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }

                disposedValue = true;
            }
        }

        private void ValidateResponse(IList request, ResponseHeader responseHeader, StatusCodeCollection statusCodes, DiagnosticInfoCollection diagnosticInfos, IEnumerable<string> ioNames)
        {
            ClientBase.ValidateResponse(statusCodes, request);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, request);

            if (!StatusCode.IsGood(responseHeader.ServiceResult))
            {
                throw new ServiceResultException($"Response header is bad");
            }

            if (!statusCodes.TrueForAll(x => StatusCode.IsGood(x)))
            {
                var errors = statusCodes
                    .Zip(ioNames, (statusCode, name) => (statusCode, name))
                    .Where(x => !StatusCode.IsGood(x.statusCode))
                    .Select(x => ServiceResultException.Create(x.statusCode.Code, "{0}: {1}", StatusCode.LookupSymbolicId(x.statusCode.Code), x.name))
                    .ToList();

                throw new AggregateException($"Service result is bad", errors);
            }
        }
    }
}