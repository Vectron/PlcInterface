using TwinCAT.Ads;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A implementation of <see cref="ISumSymbolRead"/>.
/// </summary>
/// <inheritdoc cref="SumSymbolRead"/>
internal sealed class SumSymbolWriteAbstraction(IAdsConnection connection, IList<ISymbol> symbols) : ISumSymbolWrite
{
    private readonly SumSymbolWrite backend = new(connection, symbols);

    /// <inheritdoc/>
    public void Write(object[] values)
        => backend.Write(values);

    /// <inheritdoc/>
    public Task WriteAsync(object[] values, CancellationToken cancel)
        => backend.WriteAsync(values, cancel);
}
