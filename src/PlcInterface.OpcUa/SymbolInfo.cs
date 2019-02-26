using Opc.Ua;
using System.Collections.Generic;
using VectronsLibrary;

namespace PlcInterface.OpcUa
{
    internal sealed class SymbolInfo : ObservableObject, ISymbolInfo
    {
        public SymbolInfo(ReferenceDescription symbol, string itemFullName)
        {
            Handle = (NodeId)symbol.NodeId;
            var identifier = symbol.NodeId.Identifier as string;
            Name = itemFullName;
            NameLower = Name.ToLower();
            ShortName = Name.Substring(Name.LastIndexOf(".") + 1);
            ChildSymbols = new List<string>();
            IsArray = Name == null ? false : Name.Contains("[");
            IsBigType = symbol.NodeClass == NodeClass.Object || symbol.NodeClass == NodeClass.ObjectType;
        }

        public IList<string> ChildSymbols
        {
            get;
        }

        public string Comment
        {
            get;
        }

        public NodeId Handle
        {
            get;
        }

        public bool IsArray
        {
            get;
        }

        public bool IsBigType
        {
            get;
        }

        public string Name
        {
            get;
        }

        public string NameLower
        {
            get;
        }

        public string ShortName
        {
            get;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}