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
    public ArrayShape ArrayShape
    {
        get;
    }

    /// <summary>
    /// Gets the built in type.
    /// </summary>
    public BuiltInType BuiltInType
    {
        get;
    }

    /// <summary>
    /// Gets the PLC symbol this encapsules.
    /// </summary>
    public NodeId Handle
    {
        get;
    }

    /// <summary>
    /// Gets the indices of this array item.
    /// </summary>
    public int[] Indices
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether this symbol represents a array.
    /// </summary>
    public bool IsArray
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether this symbol represents a complex type.
    /// </summary>
    public bool IsBigType
    {
        get;
    }

    /// <summary>
    /// Gets the display name of the type.
    /// </summary>
    public string TypeName
    {
        get;
    }
}
