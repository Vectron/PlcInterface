using System;
using System.Globalization;

namespace PlcInterface;

/// <summary>
/// A base implementation of <see cref="ITypeConverter"/>.
/// </summary>
public abstract class TypeConverter : ITypeConverter
{
    /// <inheritdoc/>
    public virtual T Convert<T>(object value)
    {
        if (value is T targetValue)
        {
            return targetValue;
        }

        return (T)Convert(value, typeof(T));
    }

    /// <inheritdoc/>
    public virtual object Convert(object value, Type targetType)
    {
        if (value.GetType() == targetType)
        {
            return value;
        }

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, value);
        }

        return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}