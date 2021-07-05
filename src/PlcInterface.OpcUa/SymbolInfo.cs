using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Opc.Ua;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// Stores data about a PLC symbol.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal sealed class SymbolInfo : ISymbolInfo
    {
        private readonly Lazy<int[]> arrayBounds;
        private readonly NodeInfo nodeInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolInfo"/> class.
        /// </summary>
        /// <param name="symbol">The <see cref="ReferenceDescription"/> to describe.</param>
        /// <param name="itemFullName">The fullname of the symbol.</param>
        /// <param name="nodeInfo">Extra info for this symbol.</param>
        public SymbolInfo(ReferenceDescription symbol, string itemFullName, NodeInfo nodeInfo)
        {
            Handle = (NodeId)symbol.NodeId;
            Name = itemFullName;
            this.nodeInfo = nodeInfo;
            NameLower = Name.ToLower(CultureInfo.InvariantCulture);
            ShortName = Name.Substring(Name.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1);
            ChildSymbols = new List<string>();
            IsBigType = symbol.NodeClass is NodeClass.Object or NodeClass.ObjectType;
            Indices = Name.AsSpan().GetIndices();
            arrayBounds = new Lazy<int[]>(CalculateBounds, false);
            Comment = string.Empty;
        }

        /// <summary>
        /// Gets the bounds of the array.
        /// </summary>
        public int[] ArrayBounds
            => arrayBounds.Value;

        /// <summary>
        /// Gets the builtin type.
        /// </summary>
        public BuiltInType BuiltInType
            => nodeInfo.BuiltInType;

        /// <inheritdoc/>
        public IList<string> ChildSymbols
        {
            get;
        }

        /// <inheritdoc/>
        public string Comment
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
            => ChildSymbols.Count > 0 && ChildSymbols[0].AsSpan(Name.Length).IndexOf('[') != -1;

        /// <summary>
        /// Gets a value indicating whether this symbol represents a complex type.
        /// </summary>
        public bool IsBigType
        {
            get;
        }

        /// <inheritdoc/>
        public string Name
        {
            get;
        }

        /// <inheritdoc/>
        public string NameLower
        {
            get;
        }

        /// <inheritdoc/>
        public string ShortName
        {
            get;
        }

        private int[] CalculateBounds()
        {
            if (ChildSymbols.Count == 0)
            {
                return Array.Empty<int>();
            }

            var indices = ChildSymbols.Select(x => x.AsSpan(Name.Length).GetIndices());
            var length = indices.First().Length;
            var lowerBounds = new int[length];
            var upperbounds = new int[length];
            var ranks = new int[length];

            foreach (var index in indices)
            {
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