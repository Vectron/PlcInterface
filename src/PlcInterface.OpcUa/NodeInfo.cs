using Opc.Ua;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// Contains extra information about a <see cref="NodeId"/>.
    /// </summary>
    internal sealed class NodeInfo
    {
        /// <summary>
        /// Gets or sets the build in data type of this node.
        /// </summary>
        public BuiltInType BuiltInType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the <see cref="NodeId"/> for the data type.
        /// </summary>
        public NodeId? DataType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the display text of the <see cref="NodeId"/>.
        /// </summary>
        public string? DataTypeDisplayText
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the description text of the <see cref="NodeId"/>.
        /// </summary>
        public string? Description
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the rank if this node represents a array.
        /// </summary>
        public int ValueRank
        {
            get;
            internal set;
        }
    }
}