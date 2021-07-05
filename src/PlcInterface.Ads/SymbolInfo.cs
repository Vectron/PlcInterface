using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    /// <summary>
    /// Stores data about a PLC symbol.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal sealed class SymbolInfo : ISymbolInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolInfo"/> class.
        /// </summary>
        /// <param name="symbol">The plc symbol.</param>
        public SymbolInfo(ISymbol symbol)
            => Symbol = symbol;

        /// <inheritdoc/>
        public IList<string> ChildSymbols
            => Symbol.SubSymbols.Select(x => x.InstancePath).ToList();

        /// <inheritdoc/>
        public string Comment
            => Symbol.Comment;

        /// <summary>
        /// Gets a value indicating whether this symbol represents a array.
        /// </summary>
        public bool IsArray
            => Symbol.DataType?.Category == DataTypeCategory.Array;

        /// <summary>
        /// Gets a value indicating whether this symbol represents a complex type.
        /// </summary>
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

        /// <summary>
        /// Gets the PLC symbol this encapsules.
        /// </summary>
        public ISymbol Symbol
        {
            get;
        }
    }
}