using System;
using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// Contains extra information about a <see cref="NodeId"/>.
/// </summary>
internal sealed class NodeInfo
{
    private readonly Lazy<BuiltInType> builtInType;
    private readonly Lazy<string> dataTypeDisplayText;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeInfo"/> class.
    /// </summary>
    /// <param name="session">The session to get the data from.</param>
    /// <param name="dataType">The data type of this node.</param>
    /// <param name="description">The description text of this node.</param>
    /// <param name="valueRank">The value rank of this node.</param>
    public NodeInfo(ISession session, NodeId dataType, string description, int valueRank)
    {
        Description = description;
        ValueRank = valueRank;
        DataType = dataType;

        if (NodeId.IsNull(dataType))
        {
            builtInType = new Lazy<BuiltInType>(() => BuiltInType.Null, false);
            dataTypeDisplayText = new Lazy<string>(() => string.Empty, false);
        }
        else
        {
            builtInType = new Lazy<BuiltInType>(() => DataTypes.GetBuiltInType(dataType, session.TypeTree), false);
            dataTypeDisplayText = new Lazy<string>(() => session.NodeCache.GetDisplayText(dataType), false);
        }
    }

    /// <summary>
    /// Gets the build in data type of this node.
    /// </summary>
    public BuiltInType BuiltInType
        => builtInType.Value;

    /// <summary>
    /// Gets the <see cref="NodeId"/> for the data type.
    /// </summary>
    public NodeId DataType
    {
        get;
    }

    /// <summary>
    /// Gets the display text of the <see cref="NodeId"/>.
    /// </summary>
    public string DataTypeDisplayText
        => ValueRank >= 0 ? dataTypeDisplayText.Value + "[]" : dataTypeDisplayText.Value;

    /// <summary>
    /// Gets the description text of the <see cref="NodeId"/>.
    /// </summary>
    public string Description
    {
        get;
    }

    /// <summary>
    /// Gets the rank if this node represents a array.
    /// </summary>
    public int ValueRank
    {
        get;
    }
}