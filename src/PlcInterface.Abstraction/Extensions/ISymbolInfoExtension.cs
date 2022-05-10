using System;
using System.Collections.Generic;
using System.Linq;
using PlcInterface.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace PlcInterface;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for <see cref="ISymbolInfo"/>.
/// </summary>
public static class ISymbolInfoExtension
{
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
                object? childValue;
                if (value is Array array)
                {
                    var indices = x.Name.GetIndices();
                    childValue = array.GetValue(indices);
                }
                else
                {
                    var type = value.GetType();
                    var property = type.GetProperty(x.ShortName);
                    childValue = property?.GetValue(value);
                }

                if (childValue == null)
                {
                    return Enumerable.Empty<(ISymbolInfo SymbolInfo, object Value)>();
                }

                return x.FlattenWithValue(symbolHandler, childValue);
            });
    }
}