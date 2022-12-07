﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcInterface;

/// <summary>
/// Extension methods for <see cref="ISymbolInfo"/>.
/// </summary>
internal static class ISymbolInfoExtension
{
    /// <summary>
    /// Flatten the type hierarchy.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to flatten.</param>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> implementation.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all child symbols.</returns>
    public static IEnumerable<ISymbolInfo> Flatten(this ISymbolInfo symbolInfo, ISymbolHandler symbolHandler)
        => symbolInfo.ChildSymbols.Count == 0 ? new[] { symbolInfo } : symbolInfo.ChildSymbols.SelectMany(x => symbolHandler.GetSymbolinfo(x).Flatten(symbolHandler));

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
            return new[] { (symbolInfo, value) };
        }

        return symbolInfo.ChildSymbols
            .Select(symbolHandler.GetSymbolinfo)
            .SelectMany(x =>
            {
                object? childValue;
                if (value is Array array)
                {
                    var indices = IndicesHelper.GetIndices(x.Name);
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