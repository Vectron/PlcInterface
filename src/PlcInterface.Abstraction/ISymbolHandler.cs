using System.Collections.Generic;

namespace PlcInterface
{
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
    }
}