using System;
using System.Collections.Generic;
using System.Linq;
using PlcInterface.OpcUa;

namespace PlcInterface
{
    internal static class ISymbolInfoExtension
    {
        public static SymbolInfo ConvertAndValidate(this ISymbolInfo symbolInfo)
        {
            if (!(symbolInfo is SymbolInfo symbol))
            {
                throw new SymbolException($"symbol is not a {typeof(SymbolInfo)}");
            }

            return symbol;
        }

        public static IEnumerable<ISymbolInfo> Flatten(this ISymbolInfo symbolInfo, ISymbolHandler symbolHandler)
            => symbolInfo.ChildSymbols.Count == 0 ? symbolInfo.Yield() : symbolInfo.ChildSymbols.SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler));

        public static IEnumerable<(ISymbolInfo SymbolInfo, object Value)> FlattenWithValue(this ISymbolInfo symbolInfo, ISymbolHandler symbolHandler, object value)
        {
            if (symbolInfo.ChildSymbols.Count == 0)
            {
                return (symbolInfo, value).Yield();
            }

            return symbolInfo.ChildSymbols
                .Select(x => symbolHandler.GetSymbolinfo(x))
                .SelectMany(x =>
                {
                    object childValue;
                    if (value is Array array && x is SymbolInfo symbol)
                    {
                        childValue = array.GetValue(symbol.Indices);
                    }
                    else
                    {
                        var type = value.GetType();
                        var property = type.GetProperty(x.ShortName);
                        childValue = property.GetValue(value);
                    }
                    return x.FlattenWithValue(symbolHandler, childValue);
                });
        }
    }
}