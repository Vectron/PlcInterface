using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// Stores data about a PLC symbol.
/// </summary>
[DebuggerDisplay("{Name}")]
internal sealed class SymbolInfo : IOpcSymbolInfo
{
    private readonly Lazy<ArrayShape> arrayShape;
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
        ChildSymbols = [];
        IsBigType = symbol.NodeClass is NodeClass.Object or NodeClass.ObjectType;
        Indices = IndicesHelper.GetIndices(Name);
        arrayShape = new(CalculateArrayShape, isThreadSafe: true);
        Comment = string.Empty;
    }

    /// <inheritdoc/>
    public ArrayShape ArrayShape
        => arrayShape.Value;

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

    /// <inheritdoc/>
    public string TypeName
        => nodeInfo.DataTypeDisplayText;

    private ArrayShape CalculateArrayShape()
    {
        if (ChildSymbols.Count == 0)
        {
            return default;
        }

        var indices = ChildSymbols.Select(x => IndicesHelper.GetIndices(x.AsSpan(Name.Length)));
        var rank = indices.First().Length;
        var lowerBounds = Enumerable.Repeat(int.MaxValue, rank).ToArray();
        var upperBounds = Enumerable.Repeat(0, rank).ToArray();

        foreach (var index in indices)
        {
            for (var i = 0; i < index.Length; i++)
            {
                if (upperBounds[i] < index[i])
                {
                    upperBounds[i] = index[i];
                }

                if (lowerBounds[i] > index[i])
                {
                    lowerBounds[i] = index[i];
                }
            }
        }

        var sizes = Enumerable.Repeat(0, rank).ToArray();
        for (var i = 0; i < rank; i++)
        {
            sizes[i] = upperBounds[i] + 1 - lowerBounds[i];
        }

        return new ArrayShape(rank, ImmutableArray.Create(sizes), ImmutableArray.Create(lowerBounds));
    }
}
