using System.Diagnostics.CodeAnalysis;

namespace PlcInterface.Ads;

/// <summary>
/// The Ads implementation of a <see cref="ISymbolHandler"/>.
/// </summary>
public interface IAdsSymbolHandler : ISymbolHandler
{
    /// <summary>
    /// Gets the <see cref="ISymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <returns>The found <see cref="ISymbolInfo"/>.</returns>
    new IAdsSymbolInfo GetSymbolInfo(string ioName);

    /// <summary>
    /// Try to get the <see cref="IAdsSymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <param name="symbolInfo">The found <see cref="IAdsSymbolInfo"/>.</param>
    /// <returns><see langword="true"/> when the symbol was found else <see langword="false"/>.</returns>
    bool TryGetSymbolInfo(string ioName, [MaybeNullWhen(false)] out IAdsSymbolInfo symbolInfo);
}
