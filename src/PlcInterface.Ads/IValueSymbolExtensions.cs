using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// Extension methods for <see cref="IValueSymbol"/>.
/// </summary>
internal static class IValueSymbolExtensions
{
    /// <summary>
    /// Convert the <see cref="ISymbolInfo"/> to <see cref="SymbolInfo"/> and throw a exception if the conversion fails.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="ISymbolInfo"/> to change.</param>
    /// <returns>The cast object.</returns>
    /// <exception cref="SymbolException">If the cast fails.</exception>
    public static IValueSymbol CastAndValidate(this ISymbol symbolInfo)
    {
        if (symbolInfo is not IValueSymbol symbol)
        {
            throw new SymbolException($"Symbol is not a {typeof(IValueSymbol)}");
        }

        return symbol;
    }
}
