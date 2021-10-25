using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.TwincatAbstractions;

/// <summary>
/// A implementation of <see cref="ISumSymbolRead"/>.
/// </summary>
internal sealed class SumSymbolReadAbstraction : ISumSymbolRead
{
    private readonly SumSymbolRead backend;

    /// <inheritdoc cref="SumSymbolRead" />
    public SumSymbolReadAbstraction(IAdsConnection connection, IList<ISymbol> symbols)
        => backend = new SumSymbolRead(connection, symbols);

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