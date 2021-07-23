using System.Collections.Generic;
using Opc.Ua;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// Specialized <see cref="ITypeConverter"/> for Opc Types.
    /// </summary>
    public interface IOpcTypeConverter : ITypeConverter
    {
        /// <summary>
        /// Converts Opc types to System types.
        /// </summary>
        /// <param name="value">The value to fix.</param>
        /// <returns>The converted type.</returns>
        object Convert(object value);

        /// <summary>
        /// Create a dynamic type.
        /// </summary>
        /// <param name="symbolInfo"><see cref="ISymbolInfo"/> of the type to convert.</param>
        /// <param name="valueEnumerator">a <see cref="IEnumerator{T}"/> to enumerate the values.</param>
        /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> to get type info from.</param>
        /// <returns>A dynamic object representing the <paramref name="symbolInfo"/>.</returns>
        dynamic CreateDynamic(ISymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator, ISymbolHandler symbolHandler);
    }
}