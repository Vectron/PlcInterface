using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    public class ReadWrite : IReadWrite
    {
        private readonly IPlcConnection<IAdsConnection> connection;
        private readonly IDynamicValueConverter dynamicValueConverter;
        private readonly ILogger<ReadWrite> logger;
        private readonly ISymbolHandler symbolHandler;

        public ReadWrite(IPlcConnection<IAdsConnection> connection, ISymbolHandler symbolHandler, IDynamicValueConverter dynamicValueConverter, ILogger<ReadWrite> logger)
        {
            this.connection = connection;
            this.symbolHandler = symbolHandler;
            this.dynamicValueConverter = dynamicValueConverter;
            this.logger = logger;
        }

        public IDictionary<string, object> Read(IEnumerable<string> ioNames)
        {
            var client = connection.GetConnectedClient();
            var tcSymbols = ioNames
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Cast<SymbolInfo>()
                .Select(x => x.Symbol)
                .ToList();

            var sumReader = new SumSymbolRead(client, tcSymbols);
            var result = sumReader.Read();
            return ioNames
                .Zip(result, (ioName, value) => (ioName, value))
                .ToDictionary(x => x.ioName, x => x.value);
        }

        public object Read(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            var adsSymbol = symbolInfo?.Symbol as IValueSymbol;
            return adsSymbol?.ReadValue();
        }

        public T Read<T>(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            var adsSymbol = symbolInfo?.Symbol as ISymbol;

            if (adsSymbol.Category == DataTypeCategory.Enum
                && typeof(T).IsEnum)
            {
                return (T)Enum.ToObject(typeof(T), Read(ioName));
            }

            if (adsSymbol?.IsPrimitiveType == true)
            {
                return (T)Read(ioName);
            }

            if (symbolInfo?.Symbol is DynamicSymbol dynamicSymbol)
            {
                var value = dynamicSymbol.ReadValue();

                if (value is DynamicObject dynamicObject)
                {
                    return (T)dynamicValueConverter.ConvertFrom(dynamicObject, typeof(T));
                }
                else if (value is T requestedType)
                {
                    return requestedType;
                }
                else
                {
                    throw new SymbolException($"Unable to convert type {value}");
                }
            }

            if (symbolInfo?.Symbol is IValueAnySymbol valueAnySymbol)
            {
                var destination = Activator.CreateInstance(typeof(T));
                valueAnySymbol.UpdateAnyValue(ref destination);
                return (T)destination;
            }

            throw new NotSupportedException($"Unable to read {ioName} as {typeof(T)}");
        }

        public Task<object> ReadAsync(string ioName)
            => Task.Run(() => Read(ioName));

        public Task<T> ReadAsync<T>(string ioName)
            => Task.Run(() => Read<T>(ioName));

        public Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
            => Task.Run(() => Read(ioNames));

        public dynamic ReadDynamic(string ioName)
        {
            var value = Read(ioName);

            if (value is DynamicObject dynamicSymbol)
            {
                return value;
            }

            throw new NotImplementedException("dynamic values are only supported when read mode is set to dynamic");
        }

        public Task<dynamic> ReadDynamicAsync(string ioName)
            => Task.Run(() => ReadDynamic(ioName));

        public void ToggleBool(string ioName)
        {
            var current = Read<bool>(ioName);
            Write(ioName, !current);
        }

        public void Write(IDictionary<string, object> namesValues)
        {
            var client = connection.GetConnectedClient();
            var tcSymbols = namesValues
                .Select(x => symbolHandler.GetSymbolinfo(x.Key))
                .Cast<SymbolInfo>()
                .Select(x => x.Symbol)
                .ToList();

            var sumReader = new SumSymbolWrite(client, tcSymbols);
            sumReader.Write(namesValues.Values.ToArray());
        }

        public void Write<T>(string ioName, T value)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            var adsSymbol = symbolInfo?.Symbol as IValueSymbol;
            adsSymbol.WriteValue(value);
        }

        public Task WriteAsync(IDictionary<string, object> namesValues)
            => Task.Run(() => Write(namesValues));

        public Task WriteAsync<T>(string ioName, T value)
            => Task.Run(() => Write(ioName, value));
    }
}