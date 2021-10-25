namespace PlcInterface.OpcUa.Extensions;

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
}