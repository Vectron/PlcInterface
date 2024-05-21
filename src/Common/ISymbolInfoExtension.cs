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
        => symbolInfo.ChildSymbols.Count == 0 ? [symbolInfo] : symbolInfo.ChildSymbols.SelectMany(x => symbolHandler.GetSymbolInfo(x).Flatten(symbolHandler));

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
            return [(symbolInfo, value)];
        }

        return symbolInfo.ChildSymbols
            .Select(symbolHandler.GetSymbolInfo)
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
                    return [];
                }

                return x.FlattenWithValue(symbolHandler, childValue);
            });
    }
}
