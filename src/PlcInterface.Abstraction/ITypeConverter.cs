namespace PlcInterface;

/// <summary>
/// A converter that can be used to convert PLC types to system types.
/// </summary>
public interface ITypeConverter
{
    /// <summary>
    /// Converts from object to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="value">The object to convert.</param>
    /// <returns>The resulting <typeparamref name="T"/>.</returns>
    public T Convert<T>(object value);

    /// <summary>
    /// Converts from object to <paramref name="targetType"/>.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <param name="targetType">The <see cref="Type"/> to convert to.</param>
    /// <returns>The converted object.</returns>
    public object Convert(object value, Type targetType);
}
