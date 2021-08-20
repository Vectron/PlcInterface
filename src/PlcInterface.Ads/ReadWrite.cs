﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlcInterface.Extensions;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    /// <summary>
    /// Implementation of <see cref="IReadWrite"/>.
    /// </summary>
    public class ReadWrite : IAdsReadWrite
    {
        private readonly IAdsPlcConnection connection;
        private readonly IAdsSymbolHandler symbolHandler;
        private readonly IAdsTypeConverter typeConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWrite"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
        /// <param name="typeConverter">A <see cref="ITypeConverter"/> implementation.</param>
        public ReadWrite(IAdsPlcConnection connection, IAdsSymbolHandler symbolHandler, IAdsTypeConverter typeConverter)
        {
            this.connection = connection;
            this.symbolHandler = symbolHandler;
            this.typeConverter = typeConverter;
        }

        /// <inheritdoc/>
        public IDictionary<string, object> Read(IEnumerable<string> ioNames)
        {
            var client = connection.GetConnectedClient();
            var tcSymbols = ioNames
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Select(x => x.Symbol)
                .ToList();

            var sumReader = new SumSymbolRead(client, tcSymbols);
            var result = sumReader.Read();
            return ioNames
                .Zip(result, (ioName, value) =>
                {
                    if (symbolHandler.GetSymbolinfo(ioName).Symbol is not IValueSymbol adsSymbol)
                    {
                        throw new SymbolException($"{ioName} is not a value symbol.");
                    }

                    var fixedValue = typeConverter.Convert(value, adsSymbol);
                    return (ioName, fixedValue);
                })
                .ToDictionary(x => x.ioName, x => x.fixedValue, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public object Read(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
            var adsSymbol = symbolInfo.Symbol.CastAndValidate();
            var value = adsSymbol.ReadValue().ThrowIfNull();
            return typeConverter.Convert(value, adsSymbol);
        }

        /// <inheritdoc/>
        public T Read<T>(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
            var adsSymbol = symbolInfo.Symbol.CastAndValidate();
            var value = adsSymbol.ReadValue().ThrowIfNull();
            return typeConverter.Convert<T>(value);
        }

        /// <inheritdoc/>
        public async Task<object> ReadAsync(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
            var adsSymbol = symbolInfo.Symbol.CastAndValidate();
            var resultReadValue = await adsSymbol.ReadValueAsync(CancellationToken.None).ConfigureAwait(false);
            return typeConverter.Convert(resultReadValue.Value.ThrowIfNull(), adsSymbol);
        }

        /// <inheritdoc/>
        public async Task<T> ReadAsync<T>(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
            var adsSymbol = symbolInfo.Symbol.CastAndValidate();
            var resultReadValue = await adsSymbol.ReadValueAsync(CancellationToken.None).ConfigureAwait(false);
            return typeConverter.Convert<T>(resultReadValue.Value.ThrowIfNull());
        }

        /// <inheritdoc/>
        public async Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
        {
            var client = await connection.GetConnectedClientAsync().ConfigureAwait(false);
            var tcSymbols = ioNames
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Select(x => x.Symbol)
                .ToList();

            var sumReader = new SumSymbolRead(client, tcSymbols);
            var resultSum = await sumReader.ReadAsync(CancellationToken.None).ConfigureAwait(false);
            var dictionary = ioNames
                    .Zip(resultSum.Values, (ioName, value) =>
                    {
                        var adsSymbol = symbolHandler.GetSymbolinfo(ioName).Symbol.CastAndValidate();
                        var fixedValue = typeConverter.Convert(value, adsSymbol);
                        return (ioName, fixedValue);
                    })
                    .ToDictionary(x => x.ioName, x => x.fixedValue, StringComparer.OrdinalIgnoreCase);
            return dictionary;
        }

        /// <inheritdoc/>
        public dynamic ReadDynamic(string ioName)
        {
            var value = Read(ioName);
            return value is ExpandoObject
                ? value
                : throw new NotSupportedException("dynamic values are only supported when read mode is set to dynamic");
        }

        /// <inheritdoc/>
        public async Task<dynamic> ReadDynamicAsync(string ioName)
        {
            var readResult = await ReadAsync(ioName).ConfigureAwait(false);

            return readResult is ExpandoObject
                ? readResult
                : throw new NotSupportedException("dynamic values are only supported when read mode is set to dynamic");
        }

        /// <inheritdoc/>
        public void ToggleBool(string ioName)
        {
            var current = Read<bool>(ioName);
            Write(ioName, !current);
        }

        /// <inheritdoc/>
        public void Write(IDictionary<string, object> namesValues)
        {
            var client = connection.GetConnectedClient();
            var tcSymbols = namesValues
                .Select(x => symbolHandler.GetSymbolinfo(x.Key))
                .Select(x => x.Symbol)
                .ToList();

            try
            {
                var sumReader = new SumSymbolWrite(client, tcSymbols);
                sumReader.Write(namesValues.Values.ToArray());
            }
            catch (ArgumentException)
            {
                // When a object can't be marshalled then ArgumentException will be thrown, if this happens we try to write all objects individually
                var flattened = namesValues
                    .SelectMany(x => symbolHandler.GetSymbolinfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
                    .Select(x => (x.SymbolInfo.CastAndValidate().Symbol, x.Value));

                var sumReader = new SumSymbolWrite(client, flattened.Select(x => x.Symbol).ToList());
                sumReader.Write(flattened.Select(x => x.Value).ToArray());
            }
        }

        /// <inheritdoc/>
        public void Write<T>(string ioName, T value)
            where T : notnull
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
            var adsSymbol = symbolInfo.Symbol.CastAndValidate();

            try
            {
                adsSymbol.WriteValue(value);
            }
            catch (ArgumentException)
            {
                // When a object can't be marshalled then ArgumentException will be thrown, if this happens we try to write all objects individually
                var flattenItems = symbolInfo.FlattenWithValue(symbolHandler, value).ToDictionary(x => x.SymbolInfo.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
                Write(flattenItems);
            }
        }

        /// <inheritdoc/>
        public async Task WriteAsync(IDictionary<string, object> namesValues)
        {
            var client = await connection.GetConnectedClientAsync().ConfigureAwait(false);
            var tcSymbols = namesValues
                .Select(x => symbolHandler.GetSymbolinfo(x.Key))
                .Select(x => x.Symbol)
                .ToList();
            try
            {
                var sumReader = new SumSymbolWrite(client, tcSymbols);
                _ = await sumReader.WriteAsync(namesValues.Values.ToArray(), CancellationToken.None).ConfigureAwait(false);
            }
            catch (ArgumentException)
            {
                // When a object can't be marshalled then ArgumentException will be thrown, if this happens we try to write all objects individually
                var flattened = namesValues
                    .SelectMany(x => symbolHandler.GetSymbolinfo(x.Key).FlattenWithValue(symbolHandler, x.Value))
                    .Select(x => (x.SymbolInfo.CastAndValidate().Symbol, x.Value));

                var sumReader = new SumSymbolWrite(client, flattened.Select(x => x.Symbol).ToList());
                _ = await sumReader.WriteAsync(flattened.Select(x => x.Value).ToArray(), CancellationToken.None).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task WriteAsync<T>(string ioName, T value)
            where T : notnull
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
            var adsSymbol = symbolInfo.Symbol.CastAndValidate();
            try
            {
                _ = await adsSymbol.WriteValueAsync(value, CancellationToken.None).ConfigureAwait(false);
            }
            catch (ArgumentException)
            {
                // When a object can't be marshalled then ArgumentException will be thrown, if this happens we try to write all objects individually
                var flattenItems = symbolInfo.FlattenWithValue(symbolHandler, value).ToDictionary(x => x.SymbolInfo.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
                await WriteAsync(flattenItems).ConfigureAwait(false);
            }
        }
    }
}