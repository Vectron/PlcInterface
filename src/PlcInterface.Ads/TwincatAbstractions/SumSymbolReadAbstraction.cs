using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A implementation of <see cref="ISumSymbolRead"/>.
/// </summary>
/// <inheritdoc cref="SumSymbolRead"/>
internal sealed class SumSymbolReadAbstraction(IAdsConnection connection, IList<ISymbol> symbols) : ISumSymbolRead
{
    private readonly SumSymbolRead backend = new(connection, symbols);

    /// <inheritdoc/>
    public object[] Read()
        => backend.Read();

    /// <inheritdoc/>
    public async Task<object[]?> ReadAsync(CancellationToken cancel)
    {
        var result = await backend.ReadAsync(cancel).ConfigureAwait(false);
        return result.Values;
    }
}