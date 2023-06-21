using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// The OPC implementation of a <see cref="ISymbolInfo"/>.
/// </summary>
public interface IOpcSymbolInfo : ISymbolInfo
{
    /// <summary>
    /// Gets the bounds of the array.
    /// </summary>
    int[] ArrayBounds
    {
        get;
    }

    /// <summary>
    /// Gets the built in type.
    /// </summary>
    BuiltInType BuiltInType
    {
        get;
    }

    /// <summary>
    /// Gets the PLC symbol this encapsules.
    /// </summary>
    NodeId Handle
    {
        get;
    }

    /// <summary>
    /// Gets the indices of this array item.
    /// </summary>
    int[] Indices
    {
        get;
    }

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
}