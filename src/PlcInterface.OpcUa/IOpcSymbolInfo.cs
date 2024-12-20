using System.Reflection.Metadata;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// The OPC implementation of a <see cref="ISymbolInfo"/>.
/// </summary>
public interface IOpcSymbolInfo : ISymbolInfo
{
    /// <summary>
    /// Gets get the shape of the array.
    /// </summary>
    ArrayShape ArrayShape
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

    /// <summary>
    /// Gets the display name of the type.
    /// </summary>
    string TypeName
    {
        get;
    }
}
