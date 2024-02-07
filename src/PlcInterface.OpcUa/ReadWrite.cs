using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// Implementation of <see cref="IReadWrite"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ReadWrite"/> class.
/// </remarks>
/// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
/// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
/// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
/// <param name="logger">A <see cref="ILogger"/> implementation.</param>
public class ReadWrite(IOpcPlcConnection connection, IOpcSymbolHandler symbolHandler, IOpcTypeConverter typeConverter, ILogger<ReadWrite> logger) : IOpcReadWrite, IDisposable
{
    private readonly CompositeDisposable disposables = [];

    private readonly ILogger logger = logger;
    private bool disposedValue;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IDictionary<string, object> Read(IEnumerable<string> ioNames)
    {
        var nodesToRead = ioNames
          .SelectMany(x => symbolHandler.GetSymbolInfo(x).Flatten(symbolHandler))
          .Where(x => x.ChildSymbols.Count == 0 && x is IOpcSymbolInfo)
          .Cast<IOpcSymbolInfo>()
          .Select(x => x.Handle)
          .ToList();

#pragma warning disable IDISP001 // Dispose created
        var session = connection.GetConnectedClient();
#pragma warning restore IDISP001 // Dispose created
        session.ReadValues(nodesToRead, out var values, out var serviceResults);

        using var valueEnumerator = values.GetEnumerator() as IEnumerator<DataValue>;
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        return ioNames.ToDictionary(
            name => name,
            name => typeConverter.CreateDynamic(name, valueEnumerator),
            StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public T Read<T>(string ioName)
    {
        var value = Read(ioName);
        return typeConverter.Convert<T>(value);
    }

    /// <inheritdoc/>
    public object Read(string ioName)
    {
        var symbol = symbolHandler.GetSymbolInfo(ioName);
        if (symbol.ChildSymbols.Count > 0)
        {
            return ReadDynamic(ioName);
        }

        var session = connection.GetConnectedClient();
        var readResponse = session.ReadValue(symbol.Handle);
        ValidateDataValue(ioName, readResponse);
        return typeConverter.Convert(readResponse.Value);
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
    {
        var nodesToRead = ioNames
            .SelectMany(x => symbolHandler.GetSymbolInfo(x).Flatten(symbolHandler))
            .Where(x => x.ChildSymbols.Count == 0 && x is IOpcSymbolInfo)
            .Cast<IOpcSymbolInfo>()
            .Select(x => x.Handle)
            .ToList();

#pragma warning disable IDISP001 // Dispose created
        var session = await connection.GetConnectedClientAsync().ConfigureAwait(false);
#pragma warning restore IDISP001 // Dispose created
        var (dataValues, serviceResult) = await session.ReadValuesAsync(nodesToRead, CancellationToken.None)
            .ConfigureAwait(false);

        using var valueEnumerator = dataValues.GetEnumerator() as IEnumerator<DataValue>;
        return ioNames.ToDictionary(
            name => name,
            name => typeConverter.CreateDynamic(name, valueEnumerator),
            StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public async Task<object> ReadAsync(string ioName)
    {
        var symbol = symbolHandler.GetSymbolInfo(ioName);
        if (symbol.ChildSymbols.Count > 0)
        {
            return await ReadDynamicAsync(ioName).ConfigureAwait(false);
        }

#pragma warning disable IDISP001 // Dispose created
        var session = await connection.GetConnectedClientAsync().ConfigureAwait(false);
#pragma warning restore IDISP001 // Dispose created
        var readResponse = await session.ReadValueAsync(symbol.Handle, CancellationToken.None)
            .ConfigureAwait(false);
        ValidateDataValue(ioName, readResponse);

        return typeConverter.Convert(readResponse.Value);
    }

    /// <inheritdoc/>
    public async Task<T> ReadAsync<T>(string ioName)
    {
        var value = await ReadAsync(ioName).ConfigureAwait(false);
        return typeConverter.Convert<T>(value);
    }

    /// <inheritdoc/>
    public dynamic ReadDynamic(string ioName)
    {
        var value = Read(new[] { ioName });
        return value.Values.First();
    }

    /// <inheritdoc/>
    public async Task<dynamic> ReadDynamicAsync(string ioName)
    {
        var value = await ReadAsync(new[] { ioName })
            .ConfigureAwait(false);
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
        var query = namesValues
            .SelectMany(x => symbolHandler.GetSymbolInfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
            .Select(x => (x.SymbolInfo.ConvertAndValidate(), x.Value))
            .Select(x => new WriteValue()
            {
                NodeId = x.Item1.Handle,
                AttributeId = Attributes.Value,
                Value = new DataValue(ConvertToOpcType(x.Value, x.Item1.BuiltInType)),
            });

        var nodesToWrite = new WriteValueCollection(query);
        var responseHeader = session.Write(
            requestHeader: null,
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
        var query = namesValues
             .SelectMany(x => symbolHandler.GetSymbolInfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
             .Select(x => (x.SymbolInfo.ConvertAndValidate(), x.Value))
             .Select(x => new WriteValue()
             {
                 NodeId = x.Item1.Handle,
                 AttributeId = Attributes.Value,
                 Value = new DataValue(ConvertToOpcType(x.Value, x.Item1.BuiltInType)),
             });

        var nodesToWrite = new WriteValueCollection(query);

        var writeResult = await session.WriteAsync(
            requestHeader: null,
            nodesToWrite,
            CancellationToken.None)
            .ConfigureAwait(false);

        ValidateResponse(nodesToWrite, writeResult.ResponseHeader, writeResult.Results, writeResult.DiagnosticInfos, namesValues.Keys);
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

    private static void ValidateDataValue(string ioName, DataValue readResponse)
    {
        if (StatusCode.IsNotGood(readResponse.StatusCode))
        {
            throw ServiceResultException.Create(
                readResponse.StatusCode.Code,
                "{0}: {1}",
                StatusCode.LookupSymbolicId(readResponse.StatusCode.Code),
                ioName);
        }

        if (readResponse.Value == null)
        {
            ThrowHelper.ThrowInvalidOperationException_FailedToRead(ioName);
        }
    }

    private static void ValidateResponse(IList request, ResponseHeader responseHeader, StatusCodeCollection statusCodes, DiagnosticInfoCollection diagnosticInfos, IEnumerable<string> ioNames)
    {
        ClientBase.ValidateResponse(statusCodes, request);
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, request);

        if (!StatusCode.IsGood(responseHeader.ServiceResult))
        {
            throw new ServiceResultException($"Response header is bad");
        }

        if (!statusCodes.TrueForAll(StatusCode.IsGood))
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
