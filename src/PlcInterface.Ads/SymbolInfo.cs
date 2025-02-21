using System.Diagnostics;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// Stores data about a PLC symbol.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SymbolInfo"/> class.
/// </remarks>
/// <param name="symbol">The plc symbol.</param>
/// <param name="rootPath">The root path of the symbol tree.</param>
[DebuggerDisplay("{Name}")]
internal sealed class SymbolInfo(ISymbol symbol, string rootPath) : IAdsSymbolInfo
{
    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Makes it more unreadable")]
    public IList<string> ChildSymbols
        => Symbol.SubSymbols.Select(x => CleanInstancePath(x, rootPath)).ToList();

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
    public string Name { get; } = CleanInstancePath(symbol, rootPath);

    /// <inheritdoc/>
    public string NameLower
        => Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public string ShortName
        => Symbol.InstanceName;

    /// <inheritdoc/>
    public ISymbol Symbol => symbol;

    private static string CleanInstancePath(ISymbol symbol, string rootPath)
    {
        if (string.IsNullOrEmpty(rootPath))
        {
            return symbol.InstancePath;
        }

        return symbol.InstancePath
            .Replace(rootPath, string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim('.');
    }
}
