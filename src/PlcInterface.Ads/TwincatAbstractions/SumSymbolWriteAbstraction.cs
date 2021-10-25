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
internal sealed class SumSymbolWriteAbstraction : ISumSymbolWrite
{
    private readonly SumSymbolWrite backend;

    /// <inheritdoc cref="SumSymbolRead" />
    public SumSymbolWriteAbstraction(IAdsConnection connection, IList<ISymbol> symbols)
        => backend = new SumSymbolWrite(connection, symbols);

    /// <inheritdoc/>
    public void Write(object[] values)
        => backend.Write(values);

    /// <inheritdoc/>
    public Task WriteAsync(object[] values, CancellationToken cancel)
        => backend.WriteAsync(values, cancel);
}