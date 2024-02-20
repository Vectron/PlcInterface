using TwinCAT.Ads.SumCommand;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A Abstraction layer over <see cref="SumSymbolWrite"/>.
/// </summary>
public interface ISumSymbolWrite
{
    /// <inheritdoc cref="SumSymbolWrite.Write(object[])"/>
    void Write(object[] values);

    /// <inheritdoc cref="SumSymbolWrite.WriteAsync(object[], CancellationToken)"/>
    Task WriteAsync(object[] values, CancellationToken cancel);
}
