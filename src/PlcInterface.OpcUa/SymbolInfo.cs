using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using VectronsLibrary;

namespace PlcInterface.OpcUa
{
    internal sealed class SymbolInfo : ObservableObject, ISymbolInfo
    {
        private readonly Lazy<int[]> arrayBounds;
        private readonly NodeInfo nodeInfo;

        public SymbolInfo(ReferenceDescription symbol, string itemFullName, NodeInfo nodeInfo)
        {
            Handle = (NodeId)symbol.NodeId;
            Name = itemFullName;
            this.nodeInfo = nodeInfo;
            NameLower = Name.ToLower();
            ShortName = Name.Substring(Name.LastIndexOf(".") + 1);
            ChildSymbols = new List<string>();
            IsBigType = symbol.NodeClass == NodeClass.Object || symbol.NodeClass == NodeClass.ObjectType;
            Indices = Name.AsSpan().GetIndices();
            arrayBounds = new Lazy<int[]>(CalculateBounds, false);
        }

        public int[] ArrayBounds
            => arrayBounds.Value;

        public BuiltInType BuiltInType
            => nodeInfo.BuiltInType;

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

        public int[] Indices
        {
            get;
        }

        public bool IsArray
            => ChildSymbols.Count > 0 && ChildSymbols[0].AsSpan(Name.Length).IndexOf('[') != -1;

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

        private int[] CalculateBounds()
        {
            if (ChildSymbols.Count == 0)
            {
                return Array.Empty<int>();
            }

            int[] lowerBounds = null;
            int[] upperbounds = null;
            int[] ranks = null;
            var indices = ChildSymbols.Select(x => x.AsSpan(Name.Length).GetIndices());

            foreach (var index in indices)
            {
                if (lowerBounds == null)
                {
                    lowerBounds = new int[index.Length];
                    upperbounds = new int[index.Length];
                    ranks = new int[index.Length];
                }

                for (var i = 0; i < lowerBounds.Length; i++)
                {
                    if (upperbounds[i] < index[i])
                    {
                        upperbounds[i] = index[i];
                    }

                    if (lowerBounds[i] > index[i])
                    {
                        lowerBounds[i] = index[i];
                    }

                    ranks[i] = upperbounds[i] + 1 - lowerBounds[i];
                }
            }

            return ranks;
        }
    }
}