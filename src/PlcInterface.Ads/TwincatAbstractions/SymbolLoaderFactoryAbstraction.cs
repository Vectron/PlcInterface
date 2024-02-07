using TwinCAT;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.TwinCATAbstractions;

/// <summary>
/// A implementation of <see cref="ISymbolLoaderFactory"/>.
/// </summary>
public class SymbolLoaderFactoryAbstraction : ISymbolLoaderFactory
{
    /// <inheritdoc/>
    public ISymbolLoader Create(IConnection connection, ISymbolLoaderSettings settings)
        => SymbolLoaderFactory.Create(connection, settings);
}
