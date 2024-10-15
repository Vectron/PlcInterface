using System.Dynamic;
using System.Globalization;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// A <see cref="ITypeConverter"/> implementation for ADS types.
/// </summary>
public sealed class AdsTypeConverter : TypeConverter, IAdsTypeConverter
{
    /// <inheritdoc/>
    public object Convert(object value, IValueSymbol valueSymbol)
    {
        if (value is DynamicObject dynamicObject)
        {
            return dynamicObject.CleanDynamic();
        }

        if (valueSymbol.Category == DataTypeCategory.Enum
            && value is short)
        {
            return System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        if (value is DateTime dateTime)
        {
            return new DateTimeOffset(dateTime);
        }

        return value;
    }

    /// <inheritdoc/>
    public override object Convert(object value, Type targetType)
    {
        if (value is TwinCAT.PlcOpen.DateBase dateBase)
        {
            if (targetType == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(dateBase.Value);
            }

            return dateBase.Value;
        }

        if (value is TwinCAT.PlcOpen.TimeBase timeBase)
        {
            return timeBase.Time;
        }

        if (value is TwinCAT.PlcOpen.LTimeBase lTimeBase)
        {
            return lTimeBase.Time;
        }

        if (value is IDynamicValue valueObject && valueObject.DataType is IArrayType arrayType)
        {
            return ConvertDynamicValueArray(valueObject, arrayType, targetType);
        }

        return base.Convert(value, targetType);
    }

    /// <inheritdoc/>
    public object ConvertToPLCType(object value)
    {
        if (value is IArrayWrapper arrayWrapper)
        {
            return arrayWrapper.BackingArray;
        }

        return value;
    }

    private Array ConvertDynamicValueArray(IDynamicValue valueObject, IArrayType arrayType, Type targetType)
    {
        var elementType = targetType.GetElementType()
            ?? throw new NotSupportedException($"Unable to retrieve element type");
        var dimensionLengths = arrayType.Dimensions.GetDimensionLengths();
        var destination = Array.CreateInstance(elementType, dimensionLengths);

        foreach (var indices in IndicesHelper.GetIndices(destination))
        {
            if (!valueObject.TryGetIndexValue(indices, out var memberValue))
            {
                throw new SymbolException($"No value found at index {string.Join(';', indices)}");
            }

            destination.SetValue(Convert(memberValue, elementType), indices);
        }

        return destination;
    }
}
