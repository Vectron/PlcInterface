using System;
using System.Collections.Generic;
using System.Linq;
using PlcInterface.OpcUa;

namespace PlcInterface
{
    /// <summary>
    /// Extension methods for <see cref="ISymbolInfo"/>.
    /// </summary>
    internal static class ISymbolInfoExtension
    {
        /// <summary>
        /// Convert the <see cref="ISymbolInfo"/> to <see cref="SymbolInfo"/> and throw a exception if the conversion fails.
        /// </summary>
        /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to change.</param>
        /// <returns>The cast object.</returns>
        /// <exception cref="SymbolException">If the cast fails.</exception>
        public static SymbolInfo ConvertAndValidate(this ISymbolInfo symbolInfo)
        {
            if (symbolInfo is not SymbolInfo symbol)
            {
                throw new SymbolException($"symbol is not a {typeof(SymbolInfo)}");
            }

            return symbol;
        }

        /// <summary>
        /// Flatten the type hierarchy.
        /// </summary>
        /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to flatten.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of all child symbols.</returns>
        public static IEnumerable<ISymbolInfo> Flatten(this ISymbolInfo symbolInfo, ISymbolHandler symbolHandler)
            => symbolInfo.ChildSymbols.Count == 0 ? symbolInfo.Yield() : symbolInfo.ChildSymbols.SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler));

        /// <summary>
        /// Flatten the type hierarchy.
        /// </summary>
        /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to flatten.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
        /// <param name="value">The <see cref="object"/> to flatten.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of all child symbols and their value.</returns>
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