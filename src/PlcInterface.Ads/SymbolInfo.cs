using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// Stores data about a PLC symbol.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SymbolInfo"/> class.
/// </remarks>
/// <param name="symbol">The plc symbol.</param>
[DebuggerDisplay("{Name}")]
internal sealed class SymbolInfo(ISymbol symbol) : IAdsSymbolInfo
{
    /// <inheritdoc/>
    public IList<string> ChildSymbols
        => Symbol.SubSymbols.Select(x => x.InstancePath).ToList();

    /// <inheritdoc/>
    public string Comment
        => Symbol.Comment;

    /// <inheritdoc/>
    public bool IsArray
        => Symbol.DataType?.Category == DataTypeCategory.Array;

    /// <inheritdoc/>
    public bool IsBigType
        => Symbol.DataType?.Category == DataTypeCategory.Struct;

    /// <inheritdoc/>
    public string Name
        => Symbol.InstancePath;

    /// <inheritdoc/>
    public string NameLower
        => Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public string ShortName
        => Symbol.InstanceName;

    /// <inheritdoc/>
    public ISymbol Symbol => symbol;
}