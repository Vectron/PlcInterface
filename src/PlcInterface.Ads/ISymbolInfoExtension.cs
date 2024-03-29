namespace PlcInterface.Ads;

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
    public static IAdsSymbolInfo CastAndValidate(this ISymbolInfo symbolInfo)
    {
        if (symbolInfo is not IAdsSymbolInfo symbol)
        {
            throw new SymbolException($"Symbol is not a {typeof(IAdsSymbolInfo)}");
        }

        return symbol;
    }
}
