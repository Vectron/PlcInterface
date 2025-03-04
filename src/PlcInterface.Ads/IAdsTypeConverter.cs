using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// Specialized <see cref="ITypeConverter"/> for Ads Types.
/// </summary>
public interface IAdsTypeConverter : ITypeConverter
{
    /// <summary>
    /// Converts Ads types to System types.
    /// </summary>
    /// <param name="value">The value to fix.</param>
    /// <param name="valueSymbol">The symbol containing type information.</param>
    /// <returns>The converted type.</returns>
    public object Convert(object value, IValueSymbol valueSymbol);

    /// <summary>
    /// Conver the given object to a type that can be written to the PLC.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>The object that can be written to the PLC.</returns>
    public object ConvertToPLCType(object value);
}
