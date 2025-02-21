using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// Specialized <see cref="ITypeConverter"/> for Opc Types.
/// </summary>
public interface IOpcTypeConverter : ITypeConverter
{
    /// <summary>
    /// Converts Opc types to System types.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="IOpcSymbolInfo"/>.</param>
    /// <param name="value">The value to fix.</param>
    /// <returns>The converted type.</returns>
    public object Convert(IOpcSymbolInfo symbolInfo, object value);

    /// <summary>
    /// Converts Opc types to System types.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <param name="value">The value to fix.</param>
    /// <returns>The converted type.</returns>
    public object Convert(string symbolName, object value);

    /// <summary>
    /// Create a dynamic type.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="IOpcSymbolInfo"/>.</param>
    /// <param name="valueEnumerator">a <see cref="IEnumerator{T}"/> to enumerate the values.</param>
    /// <returns>A dynamic object representing the <paramref name="symbolInfo"/>.</returns>
    public dynamic CreateDynamic(IOpcSymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator);

    /// <summary>
    /// Create a dynamic type.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <param name="valueEnumerator">a <see cref="IEnumerator{T}"/> to enumerate the values.</param>
    /// <returns>A dynamic object representing the <paramref name="symbolName"/>.</returns>
    public dynamic CreateDynamic(string symbolName, IEnumerator<DataValue> valueEnumerator);

    /// <summary>
    /// Create a <see cref="Variant"/> for the given symbol with value.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="IOpcSymbolInfo"/>.</param>
    /// <param name="value">The value to store.</param>
    /// <returns>The <see cref="Variant"/> type containing the value.</returns>
    public Variant CreateOpcVariant(IOpcSymbolInfo symbolInfo, object value);

    /// <summary>
    /// Create a <see cref="Variant"/> for the given symbol with value.
    /// </summary>
    /// <param name="symbolName">The name of the symbol.</param>
    /// <param name="value">The value to store.</param>
    /// <returns>The <see cref="Variant"/> type containing the value.</returns>
    public Variant CreateOpcVariant(string symbolName, object value);
}
