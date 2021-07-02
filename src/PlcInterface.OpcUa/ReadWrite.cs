using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

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
                .SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler))
                .Where(x => x is SymbolInfo)
                .Cast<SymbolInfo>()
                .Select(x => x.Handle)
                .ToList();

            var nodesTypes = Enumerable.Repeat(typeof(object), nodesToRead.Count).ToList();

            var session = client.GetConnectedClient();
            session.ReadValues(nodesToRead, nodesTypes, out var values, out var errors);

            var valueEnumerator = values.Zip(errors, (value, error) => new DataValue(error.StatusCode) { Value = value }).GetEnumerator();
            var result = new Dictionary<string, object>();
            foreach (var ioName in ioNames)
            {
                var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
                var value = CreateDynamic(symbolInfo, valueEnumerator);
                result.Add(ioName, value);
            }

            return result;
        }

        public T Read<T>(string ioName)
        {
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            if (symbol.ChildSymbols.Count > 0)
            {
                var dynamic = ReadDynamic(ioName);
                return (T)FixType(dynamic, typeof(T));
            }

            var session = client.GetConnectedClient();
            var value = session.ReadValue(symbol.Handle);
            return (T)FixType(value.Value, typeof(T));
        }

        public object Read(string ioName)
        {
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            if (symbol.ChildSymbols.Count > 0)
            {
                return ReadDynamic(ioName);
            }

            var session = client.GetConnectedClient();
            var value = session.ReadValue(symbol.Handle);
            return FixType(value.Value);
        }

        public Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
        {
            var querry = ioNames
                .SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler))
                .Where(x => x.ChildSymbols.Count == 0)
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
                        var responseHeader = session.EndRead(ar, out var dataValues, out var diagnosticInfos);
                        var statusCodes = new StatusCodeCollection(dataValues.Select(x => x.StatusCode));
                        ValidateResponse(nodesToRead, responseHeader, statusCodes, diagnosticInfos, ioNames);

                        var valueEnumerator = dataValues.GetEnumerator() as IEnumerator<DataValue>;
                        var result = new Dictionary<string, object>();
                        foreach (var ioName in ioNames)
                        {
                            var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
                            var value = CreateDynamic(symbolInfo, valueEnumerator);
                            result.Add(ioName, value);
                        }

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
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            if (symbol.ChildSymbols.Count > 0)
            {
                return ReadDynamicAsync(ioName);
            }

            var session = client.GetConnectedClient();
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
                        taskCompletionSource.SetResult(FixType(FixType(val.Value)));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(ex);
                    }
                },
                null);

            return taskCompletionSource.Task;
        }

        public Task<T> ReadAsync<T>(string ioName)
        {
            var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            if (symbol.ChildSymbols.Count > 0)
            {
                return ReadDynamicAsync(ioName).ContinueWith(x => (T)FixType(x.Result, typeof(T)));
            }

            var session = client.GetConnectedClient();
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
                        taskCompletionSource.SetResult((T)FixType(val.Value, typeof(T)));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(ex);
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
                if (symbol.IsArray)
                {
                    var array = Array.CreateInstance(typeof(ExpandoObject), symbol.ArrayBounds);
                    foreach (var childSymbol in symbol.ChildSymbols)
                    {
                        var value = ReadDynamic(childSymbol);
                        var indices = symbolHandler.GetSymbolinfo(childSymbol).ConvertAndValidate().Indices;
                        array.SetValue(value, indices);
                    }

                    return array;
                }

                var collection = new ExpandoObject() as IDictionary<string, object>;
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
                return value;
            }
        }

        public Task<dynamic> ReadDynamicAsync(string ioName)
            => Task.Run(() => ReadDynamic(ioName));

        public void ToggleBool(string ioName)
        {
            var previousValue = Read<bool>(ioName);
            Write(ioName, !previousValue);
        }

        public void Write(IDictionary<string, object> namesValues)
        {
            var session = client.GetConnectedClient();
            var querry = namesValues
                .SelectMany(x => symbolHandler.GetSymbolinfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
                .Select(x => (x.SymbolInfo.ConvertAndValidate(), x.Value))
                .Select(x => new WriteValue()
                {
                    NodeId = x.Item1.Handle,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(ConvertToOpcType(x.Value, x.Item1.BuiltInType))
                });

            var nodesToWrite = new WriteValueCollection(querry);

            ResponseHeader responseHeader = session.Write(
                null,
                nodesToWrite,
                out StatusCodeCollection statusCodes,
                out DiagnosticInfoCollection diagnosticInfos);

            ValidateResponse(nodesToWrite, responseHeader, statusCodes, diagnosticInfos, namesValues.Keys);
        }

        public void Write<T>(string ioName, T value)
            => Write(new Dictionary<string, object>() { { ioName, value } });

        public Task WriteAsync(IDictionary<string, object> namesValues)
        {
            var session = client.GetConnectedClient();
            var querry = namesValues
                 .SelectMany(x => symbolHandler.GetSymbolinfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
                 .Select(x => (x.SymbolInfo.ConvertAndValidate(), x.Value))
                 .Select(x => new WriteValue()
                 {
                     NodeId = x.Item1.Handle,
                     AttributeId = Attributes.Value,
                     Value = new DataValue(ConvertToOpcType(x.Value, x.Item1.BuiltInType))
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

        private static object FixType(object value)
        {
            if (value is DateTime dateTime)
            {
                var specified = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                return new DateTimeOffset(specified);
            }

            if (value is Matrix matrix)
            {
                return matrix.ToArray();
            }

            return value;
        }

        private Variant ConvertToOpcType(object value, BuiltInType builtInType)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                var specified = DateTime.SpecifyKind(dateTimeOffset.DateTime, DateTimeKind.Utc);
                return new Variant(specified);
            }
            else if (value is TimeSpan timeSpan)
            {
                var ticks = timeSpan.Ticks;
                var seconds = timeSpan.TotalSeconds;
                switch (builtInType)
                {
                    case BuiltInType.Int32:
                        return new Variant((int)seconds);

                    case BuiltInType.UInt32:
                        return new Variant((uint)seconds);

                    case BuiltInType.Int64:
                        return new Variant(ticks);

                    case BuiltInType.UInt64:
                        return new Variant((ulong)ticks);

                    case BuiltInType.Float:
                        return new Variant((float)seconds);

                    case BuiltInType.Double:
                        return new Variant(seconds);

                    case BuiltInType.Null:
                    case BuiltInType.Boolean:
                    case BuiltInType.SByte:
                    case BuiltInType.Byte:
                    case BuiltInType.Int16:
                    case BuiltInType.UInt16:
                    case BuiltInType.String:
                    case BuiltInType.DateTime:
                    case BuiltInType.DataValue:
                    case BuiltInType.Enumeration:
                    case BuiltInType.Number:
                    case BuiltInType.Integer:
                    case BuiltInType.UInteger:
                    case BuiltInType.Guid:
                    case BuiltInType.ByteString:
                    case BuiltInType.XmlElement:
                    case BuiltInType.NodeId:
                    case BuiltInType.ExpandedNodeId:
                    case BuiltInType.StatusCode:
                    case BuiltInType.QualifiedName:
                    case BuiltInType.LocalizedText:
                    case BuiltInType.ExtensionObject:
                    case BuiltInType.Variant:
                    case BuiltInType.DiagnosticInfo:
                    default:
                        break;
                }
            }
            else if (builtInType == BuiltInType.Enumeration)
            {
                return new Variant(Convert.ToInt32(value));
            }
            return new Variant(value);
        }

        private object CreateDynamic(SymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator)
        {
            if (symbolInfo.ChildSymbols.Count == 0)
            {
                if (valueEnumerator.MoveNext() && ServiceResult.IsGood(valueEnumerator.Current.StatusCode))
                {
                    if (valueEnumerator.Current.Value is Matrix matrixValue)
                    {
                        return matrixValue.ToArray();
                    }

                    return valueEnumerator.Current.Value;
                }
            }

            if (symbolInfo.IsArray)
            {
                var array = Array.CreateInstance(typeof(ExpandoObject), symbolInfo.ArrayBounds);
                foreach (var childSymbolName in symbolInfo.ChildSymbols)
                {
                    var childSymbolInfo = symbolHandler.GetSymbolinfo(childSymbolName).ConvertAndValidate();
                    var value = CreateDynamic(childSymbolInfo, valueEnumerator);
                    var indices = childSymbolInfo.Indices;
                    array.SetValue(value, indices);
                }

                return array;
            }

            var collection = new ExpandoObject() as IDictionary<string, object>;
            foreach (var childSymbolName in symbolInfo.ChildSymbols)
            {
                var childSymbolInfo = symbolHandler.GetSymbolinfo(childSymbolName).ConvertAndValidate();
                var value = CreateDynamic(childSymbolInfo, valueEnumerator);
                var shortName = childSymbolInfo.ShortName;
                collection.Add(shortName, value);
            }
            return collection;
        }

        private object FixType(object value, Type targetType)
        {
            if (value == null)
            {
                return value;
            }

            if (value.GetType() == targetType)
            {
                return value;
            }

            if (targetType == typeof(DateTimeOffset) && value is DateTime dateTime)
            {
                var specified = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                return new DateTimeOffset(specified);
            }

            if (targetType == typeof(TimeSpan))
            {
                if (value.GetType() == typeof(ulong))
                {
                    var ticks = Convert.ToInt64(value) / 100; // ticks are in 100 nano seconds, value is in micro seconds
                    return TimeSpan.FromTicks(ticks);
                }

                var miliSeconds = Convert.ToDouble(value);
                return TimeSpan.FromMilliseconds(miliSeconds);
            }

            if (value is Matrix matrix)
            {
                return matrix.ToArray();
            }

            if (value.GetType().IsArray && targetType.IsArray)
            {
                var array = value as Array;
                var upperBoundsRank = new int[array.Rank];

                for (var dimension = 0; dimension < array.Rank; dimension++)
                {
                    upperBoundsRank[dimension] = array.GetLength(dimension);
                }

                var elementType = targetType.GetElementType();
                var typedArray = Array.CreateInstance(elementType, upperBoundsRank);
                var indices = new int[array.Rank];
                indices[indices.Length - 1]--;

                while (array.IncrementIndices(indices))
                {
                    var item = array.GetValue(indices);
                    var fixedObject = FixType(item, elementType);
                    typedArray.SetValue(fixedObject, indices);
                }

                return typedArray;
            }

            if (value is ExpandoObject keyValues)
            {
                var instance = Activator.CreateInstance(targetType);
                foreach (var keyValue in keyValues)
                {
                    var property = targetType.GetProperty(keyValue.Key);

                    if (property == null)
                    {
                        logger.LogError("No property found with name: {0} on object of type: {1}", keyValue.Key, targetType.Name);
                        continue;
                    }

                    var fixedObject = FixType(keyValue.Value, property.PropertyType);
                    property.SetValue(instance, fixedObject);
                }

                return instance;
            }

            return Convert.ChangeType(value, targetType);
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