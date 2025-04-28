using System.Diagnostics.CodeAnalysis;

namespace PlcInterface;

/// <summary>
/// Represents a type used to store PLC symbols.
/// </summary>
public interface ISymbolHandler
{
    /// <summary>
    /// An event that is raised when the symbols are updated.
    /// </summary>
    public event EventHandler SymbolsUpdated;

    /// <summary>
    /// Gets a collection of all symbols in the PLC.
    /// </summary>
    public IReadOnlyCollection<ISymbolInfo> AllSymbols
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="ISymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <returns>The found <see cref="ISymbolInfo"/>.</returns>
    public ISymbolInfo GetSymbolInfo(string ioName);

    /// <summary>
    /// Try to get the <see cref="ISymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <param name="symbolInfo">The found <see cref="ISymbolInfo"/>.</param>
    /// <returns><see langword="true"/> when the symbol was found else <see langword="false"/>.</returns>
    public bool TryGetSymbolInfo(string ioName, [MaybeNullWhen(false)] out ISymbolInfo symbolInfo);
}
