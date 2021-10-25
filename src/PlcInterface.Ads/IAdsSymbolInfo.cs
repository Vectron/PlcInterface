using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// The Ads implementation of a <see cref="ISymbolInfo"/>.
/// </summary>
public interface IAdsSymbolInfo : ISymbolInfo
{
    /// <summary>
    /// Gets a value indicating whether this symbol represents a array.
    /// </summary>
    bool IsArray
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether this symbol represents a complex type.
    /// </summary>
    bool IsBigType
    {
        get;
    }

    /// <summary>
    /// Gets the PLC symbol this encapsules.
    /// </summary>
    ISymbol Symbol
    {
        get;
    }
}