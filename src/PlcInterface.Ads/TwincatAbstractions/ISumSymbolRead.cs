using TwinCAT.Ads.SumCommand;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A Abstraction layer over <see cref="SumSymbolRead"/>.
/// </summary>
public interface ISumSymbolRead
{
    /// <inheritdoc cref="SumSymbolRead.Read"/>
    public object[] Read();

    /// <inheritdoc cref="SumSymbolRead.ReadAsync(CancellationToken)"/>
    Task<object[]?> ReadAsync(CancellationToken cancel);
}
