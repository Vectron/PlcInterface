using System.Collections.Generic;

namespace PlcInterface;

/// <summary>
/// Represents a type containing information about a PLC symbol.
/// </summary>
public interface ISymbolInfo
{
    /// <summary>
    /// Gets a <see cref="IList{T}"/> of the childsymbols names.
    /// </summary>
    /// <value>
    /// A <see cref="IList{T}"/> containing all child childsymbol names.
    /// </value>
    IList<string> ChildSymbols
    {
        get;
    }

    /// <summary>
    /// Gets the comment for this symbol.
    /// </summary>
    /// <value>
    /// The comment stored in the plc for this symbol.
    /// </value>
    string Comment
    {
        get;
    }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    /// <value>
    /// The Full name of this symbol (Format: container block + . + symbol name) example: Visualisation.L_Display_Deur_1_1.
    /// </value>
    string Name
    {
        get;
    }

    /// <summary>
    /// Gets the name of the symbol. in all lowercase.
    /// </summary>
    /// <value>
    /// The Full name of this symbol.
    /// </value>
    string NameLower
    {
        get;
    }

    /// <summary>
    /// Gets the short name of the symbol in the PLC.
    /// </summary>
    /// <value>
    /// The name of this symbol.
    /// </value>
    string ShortName
    {
        get;
    }
}