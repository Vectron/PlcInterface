using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PlcInterface;

/// <summary>
/// Represents a type used to store PLC symbols.
/// </summary>
public interface ISymbolHandler
{
    /// <summary>
    /// Gets a collection of all symbols in the PLC.
    /// </summary>
    IReadOnlyCollection<ISymbolInfo> AllSymbols
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="ISymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <returns>The found <see cref="ISymbolInfo"/>.</returns>
    ISymbolInfo GetSymbolinfo(string ioName);

    /// <summary>
    /// Try to get the <see cref="ISymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <param name="symbolInfo">The found <see cref="ISymbolInfo"/>.</param>
    /// <returns><see langword="true"/> when the symbol was found else <see langword="false"/>.</returns>
    bool TryGetSymbolinfo(string ioName, [MaybeNullWhen(false)] out ISymbolInfo symbolInfo);
}