using Opc.Ua;

namespace PlcInterface.OpcUa
{
    internal class NodeInfo
    {
        public BuiltInType BuiltInType
        {
            get;
            internal set;
        }

        public NodeId DataType
        {
            get;
            internal set;
        }

        public string DataTypeDisplayText
        {
            get;
            internal set;
        }

        public string Description
        {
            get;
            internal set;
        }

        public int ValueRank
        {
            get;
            internal set;
        }
    }
}