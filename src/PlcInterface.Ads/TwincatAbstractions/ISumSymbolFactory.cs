using System.Collections.Generic;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A factory for creating SumSymbol commands.
/// </summary>
public interface ISumSymbolFactory
{
    /// <summary>
    /// Creates a <see cref="ISumSymbolRead"/>.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to read.</param>
    /// <returns>The constructed instance.</returns>
    public ISumSymbolRead CreateSumSymbolRead(IAdsConnection connection, IList<ISymbol> symbols);

    /// <summary>
    /// Creates a <see cref="ISumSymbolWrite"/>.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to write.</param>
    /// <returns>The constructed instance.</returns>
    public ISumSymbolWrite CreateSumSymbolWrite(IAdsConnection connection, IList<ISymbol> symbols);
}
