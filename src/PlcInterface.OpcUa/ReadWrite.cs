using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// Implementation of <see cref="IReadWrite"/>.
/// </summary>
public class ReadWrite : IOpcReadWrite, IDisposable
{
    private readonly IOpcPlcConnection connection;
    private readonly CompositeDisposable disposables = new();
    private readonly ILogger logger;
    private readonly IOpcSymbolHandler symbolHandler;
    private readonly IOpcTypeConverter typeConverter;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadWrite"/> class.
    /// </summary>
    /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
    /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
    /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
    public ReadWrite(IOpcPlcConnection connection, IOpcSymbolHandler symbolHandler, IOpcTypeConverter typeConverter, ILogger<ReadWrite> logger)
    {
        this.connection = connection;
        this.symbolHandler = symbolHandler;
        this.typeConverter = typeConverter;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IDictionary<string, object> Read(IEnumerable<string> ioNames)
    {
        var nodesToRead = ioNames
            .SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler))
            .Where(x => x is SymbolInfo)
            .Cast<SymbolInfo>()
            .Select(x => x.Handle)
            .ToList();

        var nodesTypes = Enumerable.Repeat(typeof(object), nodesToRead.Count).ToList();

        var session = connection.GetConnectedClient();
        session.ReadValues(nodesToRead, nodesTypes, out var values, out var errors);

        using var valueEnumerator = values.Zip(errors, (value, error) => new DataValue(error.StatusCode) { Value = value }).GetEnumerator();
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var ioName in ioNames)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            var value = typeConverter.CreateDynamic(symbolInfo, valueEnumerator, symbolHandler);
            result.Add(ioName, typeConverter.Convert(value));
        }

        return result;
    }

    /// <inheritdoc/>
    public T Read<T>(string ioName)
    {
        var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
        if (symbol.ChildSymbols.Count > 0)
        {
            var dynamic = ReadDynamic(ioName) as object;
            return typeConverter.Convert<T>(dynamic);
        }

        var session = connection.GetConnectedClient();
        var dataValue = session.ReadValue(symbol.Handle);
        return typeConverter.Convert<T>(dataValue.Value);
    }

    /// <inheritdoc/>
    public object Read(string ioName)
    {
        var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
        if (symbol.ChildSymbols.Count > 0)
        {
            return ReadDynamic(ioName);
        }

        var session = connection.GetConnectedClient();
        var value = session.ReadValue(symbol.Handle);
        return typeConverter.Convert(value.Value);
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
    {
        var querry = ioNames
            .SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler))
            .Where(x => x.ChildSymbols.Count == 0 && x is SymbolInfo)
            .Cast<SymbolInfo>()
            .Select(x => new ReadValueId()
            {
                NodeId = x.Handle,
                AttributeId = Attributes.Value,
            });

#pragma warning disable IDISP001 // Dispose created
        var session = await connection.GetConnectedClientAsync().ConfigureAwait(false);
#pragma warning restore IDISP001 // Dispose created
        var nodesToRead = new ReadValueIdCollection(querry);
        var taskCompletionSource = new TaskCompletionSource<IDictionary<string, object>>();

        _ = session.BeginRead(
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

                    using var valueEnumerator = dataValues.GetEnumerator() as IEnumerator<DataValue>;
                    var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var ioName in ioNames)
                    {
                        var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
                        var value = typeConverter.CreateDynamic(symbolInfo, valueEnumerator, symbolHandler);
                        result.Add(ioName, typeConverter.Convert(value));
                    }

                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            },
            null);

        return await taskCompletionSource.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<object> ReadAsync(string ioName)
    {
        var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
        if (symbol.ChildSymbols.Count > 0)
        {
            return await ReadDynamicAsync(ioName).ConfigureAwait(false);
        }
#pragma warning disable IDISP001 // Dispose created
        var session = await connection.GetConnectedClientAsync().ConfigureAwait(false);
#pragma warning restore IDISP001 // Dispose created
        var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId()
                {
                    NodeId = symbol.Handle,
                    AttributeId = Attributes.Value,
                },
            };

        var taskCompletionSource = new TaskCompletionSource<object>();
        _ = session.BeginRead(
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
                    ValidateResponse(nodesToRead, responseHeader, statusCodes, diagnosticInfos, new[] { ioName });
                    var val = dataValues.FirstOrDefault().ThrowIfNull();
                    taskCompletionSource.SetResult(typeConverter.Convert(val.Value));
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            },
            null);

        return await taskCompletionSource.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<T> ReadAsync<T>(string ioName)
    {
        var symbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
        if (symbol.ChildSymbols.Count > 0)
        {
            var value = await ReadDynamicAsync(ioName).ConfigureAwait(false) as object;
            return typeConverter.Convert<T>(value);
        }
#pragma warning disable IDISP001 // Dispose created
        var session = await connection.GetConnectedClientAsync().ConfigureAwait(false);
#pragma warning restore IDISP001 // Dispose created
        var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId()
                {
                    NodeId = symbol.Handle,
                    AttributeId = Attributes.Value,
                },
            };

        var taskCompletionSource = new TaskCompletionSource<T>();
        _ = session.BeginRead(
            null,
            0,
            TimestampsToReturn.Neither,
            nodesToRead,
            ar =>
            {
                var responseHeader = session.EndRead(ar, out var dataValues, out var diagnosticInfos);

                try
                {
                    var statusCodes = new StatusCodeCollection(dataValues.Select(x => x.StatusCode));
                    ValidateResponse(nodesToRead, responseHeader, statusCodes, diagnosticInfos, new[] { ioName });
                    var val = dataValues.FirstOrDefault().ThrowIfNull();
                    taskCompletionSource.SetResult(typeConverter.Convert<T>(val.Value));
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            },
            null);

        return await taskCompletionSource.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public dynamic ReadDynamic(string ioName)
        => ReadDynamicAsync(ioName).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public async Task<dynamic> ReadDynamicAsync(string ioName)
    {
        var value = await ReadAsync(new[] { ioName }).ConfigureAwait(false);
        return value.Values.First();
    }

    /// <inheritdoc/>
    public void ToggleBool(string ioName)
    {
        var previousValue = Read<bool>(ioName);
        Write(ioName, !previousValue);
    }

    /// <inheritdoc/>
    public void Write(IDictionary<string, object> namesValues)
    {
        var session = connection.GetConnectedClient();
        var querry = namesValues
            .SelectMany(x => symbolHandler.GetSymbolinfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
            .Select(x => (x.SymbolInfo.ConvertAndValidate(), x.Value))
            .Select(x => new WriteValue()
            {
                NodeId = x.Item1.Handle,
                AttributeId = Attributes.Value,
                Value = new DataValue(ConvertToOpcType(x.Value, x.Item1.BuiltInType)),
            });

        var nodesToWrite = new WriteValueCollection(querry);

        var responseHeader = session.Write(
            null,
            nodesToWrite,
            out var statusCodes,
            out var diagnosticInfos);

        ValidateResponse(nodesToWrite, responseHeader, statusCodes, diagnosticInfos, namesValues.Keys);
    }

    /// <inheritdoc/>
    public void Write<T>(string ioName, T value)
        where T : notnull
        => Write(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { ioName, value } });

    /// <inheritdoc/>
    public async Task WriteAsync(IDictionary<string, object> namesValues)
    {
#pragma warning disable IDISP001 // Dispose created
        var session = await connection.GetConnectedClientAsync().ConfigureAwait(false);
#pragma warning restore IDISP001 // Dispose created
        var querry = namesValues
             .SelectMany(x => symbolHandler.GetSymbolinfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
             .Select(x => (x.SymbolInfo.ConvertAndValidate(), x.Value))
             .Select(x => new WriteValue()
             {
                 NodeId = x.Item1.Handle,
                 AttributeId = Attributes.Value,
                 Value = new DataValue(ConvertToOpcType(x.Value, x.Item1.BuiltInType)),
             });

        var nodesToWrite = new WriteValueCollection(querry);
        var taskCompletionSource = new TaskCompletionSource<bool>();

        _ = session.BeginWrite(
            null,
            nodesToWrite,
            r =>
            {
                var responseHeader = session.EndWrite(r, out var statusCodes, out var diagnosticInfos);
                try
                {
                    ValidateResponse(nodesToWrite, responseHeader, statusCodes, diagnosticInfos, namesValues.Keys);
                    taskCompletionSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            },
            null);

        _ = await taskCompletionSource.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task WriteAsync<T>(string ioName, T value)
        where T : notnull
        => WriteAsync(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { ioName, value } });

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
                disposables.Dispose();
            }

            disposedValue = true;
        }
    }

    private static Variant ConvertToOpcType(object value, BuiltInType builtInType)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            // Mark it as UTC time so OPC lib won't try and convert it.
            return new Variant(DateTime.SpecifyKind(dateTimeOffset.LocalDateTime, DateTimeKind.Utc));
        }

        if (value is DateTime dateTime)
        {
            // Mark it as UTC time so OPC lib won't try and convert it.
            return new Variant(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
        }

        if (builtInType == BuiltInType.Enumeration)
        {
            return new Variant(Convert.ToInt32(value, CultureInfo.InvariantCulture));
        }

        if (value is TimeSpan timeSpan)
        {
            var ticks = timeSpan.Ticks * 100;
            return builtInType switch
            {
                BuiltInType.Int32 => new Variant(timeSpan.TotalMilliseconds),
                BuiltInType.UInt32 => new Variant((uint)timeSpan.TotalMilliseconds),
                BuiltInType.Int64 => new Variant(ticks),
                BuiltInType.UInt64 => new Variant((ulong)ticks),
                BuiltInType.Float => new Variant((float)timeSpan.TotalSeconds),
                BuiltInType.Double => new Variant(timeSpan.TotalSeconds),
                BuiltInType.Null => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Boolean => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.SByte => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Byte => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Int16 => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.UInt16 => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.String => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.DateTime => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Guid => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.ByteString => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.XmlElement => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.NodeId => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.ExpandedNodeId => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.StatusCode => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.QualifiedName => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.LocalizedText => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.ExtensionObject => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.DataValue => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Variant => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.DiagnosticInfo => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Number => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Integer => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.UInteger => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                BuiltInType.Enumeration => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
                _ => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {builtInType}"),
            };
        }

        return new Variant(value);
    }

    private static void ValidateResponse(IList request, ResponseHeader responseHeader, StatusCodeCollection statusCodes, DiagnosticInfoCollection diagnosticInfos, IEnumerable<string> ioNames)
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