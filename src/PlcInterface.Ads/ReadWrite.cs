using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlcInterface.Ads.Extensions;
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
                .Zip(result, (ioName, value) =>
                {
                    var adsSymbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate().Symbol as IValueSymbol;
                    var fixedValue = FixType(value, adsSymbol);
                    return (ioName, fixedValue);
                })
                .ToDictionary(x => x.ioName, x => x.fixedValue);
        }

        public object Read(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            var adsSymbol = symbolInfo.Symbol as IValueSymbol;
            var value = adsSymbol.ReadValue();
            return FixType(value, adsSymbol);
        }

        public T Read<T>(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            var adsSymbol = symbolInfo?.Symbol as IValueSymbol;
            var value = adsSymbol?.ReadValue();
            return FixType<T>(value);
        }

        public Task<object> ReadAsync(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            var adsSymbol = symbolInfo?.Symbol as IValueSymbol;
            return adsSymbol.ReadValueAsync(CancellationToken.None).ContinueWith(x => FixType(x.Result.Value, adsSymbol));
        }

        public Task<T> ReadAsync<T>(string ioName)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate();
            var adsSymbol = symbolInfo?.Symbol as IValueSymbol;
            return adsSymbol.ReadValueAsync(CancellationToken.None).ContinueWith(x => FixType<T>(x.Result.Value));
        }

        public Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames)
        {
            var client = connection.GetConnectedClient();
            var tcSymbols = ioNames
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .Cast<SymbolInfo>()
                .Select(x => x.Symbol)
                .ToList();

            var sumReader = new SumSymbolRead(client, tcSymbols);
            return sumReader.ReadAsync(CancellationToken.None).ContinueWith(t =>
            {
                var dictionary = ioNames
                    .Zip(t.Result.Values, (ioName, value) =>
                    {
                        var adsSymbol = symbolHandler.GetSymbolinfo(ioName).ConvertAndValidate().Symbol as IValueSymbol;
                        var fixedValue = FixType(value, adsSymbol);
                        return (ioName, fixedValue);
                    })
                    .ToDictionary(x => x.ioName, x => x.fixedValue);

                return (IDictionary<string, object>)dictionary;
            });
        }

        public dynamic ReadDynamic(string ioName)
        {
            var value = Read(ioName);
            return value is ExpandoObject
                ? value
                : throw new NotImplementedException("dynamic values are only supported when read mode is set to dynamic");
        }

        public Task<dynamic> ReadDynamicAsync(string ioName)
            => ReadAsync(ioName).ContinueWith(x => x.Result is ExpandoObject
                ? x.Result
                : throw new NotImplementedException("dynamic values are only supported when read mode is set to dynamic"));

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
            var adsSymbol = symbolInfo.Symbol as IValueSymbol;
            adsSymbol.WriteValue(value);
        }

        public Task WriteAsync(IDictionary<string, object> namesValues)
        {
            var client = connection.GetConnectedClient();
            var tcSymbols = namesValues
                .Select(x => symbolHandler.GetSymbolinfo(x.Key))
                .Cast<SymbolInfo>()
                .Select(x => x.Symbol)
                .ToList();
            var sumReader = new SumSymbolWrite(client, tcSymbols);
            return sumReader.WriteAsync(namesValues.Values.ToArray(), CancellationToken.None);
        }

        public Task WriteAsync<T>(string ioName, T value)
        {
            var symbolInfo = symbolHandler.GetSymbolinfo(ioName) as SymbolInfo;
            var adsSymbol = symbolInfo.Symbol as IValueSymbol;
            return adsSymbol.WriteValueAsync(value, CancellationToken.None);
        }

        private object FixType(object value, IValueSymbol valueSymbol)
        {
            if (value is DynamicValue dynamicObject)
            {
                return dynamicObject.CleanDynamic();
            }

            if (valueSymbol.Category == DataTypeCategory.Enum
                && value is short)
            {
                return Convert.ToInt32(value);
            }

            return value;
        }

        private T FixType<T>(object value)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.ToObject(typeof(T), value);
            }

            if (value is T unboxed)
            {
                return unboxed;
            }

            if (value is DynamicObject dynamicObject)
            {
                return (T)dynamicValueConverter.ConvertFrom(dynamicObject, typeof(T));
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}