using System.Collections.Generic;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A abstraction for creating SumSymbol commands.
/// </summary>
public class SumSymbolFactory : ISumSymbolFactory
{
    /// <inheritdoc/>
    public ISumSymbolRead CreateSumSymbolRead(IAdsConnection connection, IList<ISymbol> symbols)
        => new SumSymbolReadAbstraction(connection, symbols);

    /// <inheritdoc/>
    public ISumSymbolWrite CreateSumSymbolWrite(IAdsConnection connection, IList<ISymbol> symbols)
        => new SumSymbolWriteAbstraction(connection, symbols);
}
