using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// Stores data about a PLC symbol.
/// </summary>
[DebuggerDisplay("{Name}")]
internal sealed class SymbolInfo : IOpcSymbolInfo
{
    private readonly Lazy<int[]> arrayBounds;
    private readonly NodeInfo nodeInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolInfo"/> class.
    /// </summary>
    /// <param name="symbol">The <see cref="ReferenceDescription"/> to describe.</param>
    /// <param name="itemFullName">The full name of the symbol.</param>
    /// <param name="nodeInfo">Extra info for this symbol.</param>
    public SymbolInfo(ReferenceDescription symbol, string itemFullName, NodeInfo nodeInfo)
    {
        Handle = (NodeId)symbol.NodeId;
        Name = itemFullName;
        this.nodeInfo = nodeInfo;
        NameLower = Name.ToLower(CultureInfo.InvariantCulture);
        ShortName = Name[(Name.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1)..];
        ChildSymbols = new List<string>();
        IsBigType = symbol.NodeClass is NodeClass.Object or NodeClass.ObjectType;
        Indices = IndicesHelper.GetIndices(Name);
        arrayBounds = new Lazy<int[]>(CalculateBounds, isThreadSafe: false);
        Comment = string.Empty;
    }

    /// <inheritdoc/>
    public int[] ArrayBounds
        => arrayBounds.Value;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public NodeId Handle
    {
        get;
    }

    /// <inheritdoc/>
    public int[] Indices
    {
        get;
    }

    /// <inheritdoc/>
    public bool IsArray
        => ChildSymbols.Count > 0 && ChildSymbols[0].AsSpan(Name.Length).IndexOf('[') != -1;

    /// <inheritdoc/>
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
            return [];
        }

        var indices = ChildSymbols.Select(x => IndicesHelper.GetIndices(x.AsSpan(Name.Length)));
        var length = indices.First().Length;
        var lowerBounds = new int[length];
        var upperBounds = new int[length];
        var ranks = new int[length];

        foreach (var index in indices)
        {
            for (var i = 0; i < lowerBounds.Length; i++)
            {
                if (upperBounds[i] < index[i])
                {
                    upperBounds[i] = index[i];
                }

                if (lowerBounds[i] > index[i])
                {
                    lowerBounds[i] = index[i];
                }

                ranks[i] = upperBounds[i] + 1 - lowerBounds[i];
            }
        }

        return ranks;
    }
}
