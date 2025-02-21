using TwinCAT;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// An abstraction layer over the static class <see cref="SymbolLoaderFactory"/>.
/// </summary>
public interface ISymbolLoaderFactory
{
    /// <inheritdoc cref="SymbolLoaderFactory.Create(IConnection, ISymbolLoaderSettings)"/>
    public ISymbolLoader Create(IConnection connection, ISymbolLoaderSettings settings);
}
