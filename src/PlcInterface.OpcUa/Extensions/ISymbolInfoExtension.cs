#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace PlcInterface.OpcUa;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for <see cref="ISymbolInfo"/>.
/// </summary>
internal static class ISymbolInfoExtension
{
    /// <summary>
    /// Convert the <see cref="ISymbolInfo"/> to <see cref="IOpcSymbolInfo"/> and throw a exception
    /// if the conversion fails.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to change.</param>
    /// <returns>The cast object.</returns>
    /// <exception cref="SymbolException">If the cast fails.</exception>
    public static IOpcSymbolInfo ConvertAndValidate(this ISymbolInfo symbolInfo)
    {
        if (symbolInfo is not IOpcSymbolInfo symbol)
        {
            throw new SymbolException($"symbol is not a {typeof(IOpcSymbolInfo)}");
        }

        return symbol;
    }
}