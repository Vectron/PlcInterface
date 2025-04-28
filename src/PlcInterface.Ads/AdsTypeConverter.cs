using System.Dynamic;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// A <see cref="ITypeConverter"/> implementation for ADS types.
/// </summary>
public sealed class AdsTypeConverter : TypeConverter, IAdsTypeConverter
{
    /// <inheritdoc/>
    public object Convert(object value, IDataType? dataType)
    {
        if (dataType is IEnumType)
        {
            return Convert(value, typeof(int));
        }

        if (dataType is IArrayType arrayType)
        {
            var elementType = arrayType.ElementType as IManagedMappableType;
            var managedType = elementType?.ManagedType ?? typeof(object);
            var rank = arrayType.Dimensions.Count;
            var managedArrayType = rank == 1 ? managedType.MakeArrayType() : managedType.MakeArrayType(rank);
            return Convert(value, managedArrayType);
        }

        if (dataType is IStructType)
        {
            return Convert(value, typeof(object));
        }

        if (value is TwinCAT.PlcOpen.DateBase dateBase)
        {
            return new DateTimeOffset(dateBase.Value);
        }

        if (value is TwinCAT.PlcOpen.TimeBase timeBase)
        {
            return timeBase.Time;
        }

        if (value is TwinCAT.PlcOpen.LTimeBase lTimeBase)
        {
            return lTimeBase.Time;
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
        if (value is TwinCAT.PlcOpen.IPlcOpenTimeBase plcOpenTimeBase)
        {
            return ConvertPlcOpenTypes(plcOpenTimeBase, targetType);
        }

        if (value is IStructValue valueObject
            && targetType == typeof(object))
        {
            return SanitizeDynamic(valueObject);
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

    private static object ConvertPlcOpenTypes(TwinCAT.PlcOpen.IPlcOpenTimeBase plcOpenTimeBase, Type targetType)
    {
        if (plcOpenTimeBase is TwinCAT.PlcOpen.DateBase dateBase)
        {
            if (targetType == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(dateBase.Value);
            }

            if (targetType == typeof(DateTime))
            {
                return dateBase.Value;
            }
        }

        if (plcOpenTimeBase is TwinCAT.PlcOpen.TimeBase timeBase)
        {
            if (targetType == typeof(TimeSpan))
            {
                return timeBase.Value;
            }
        }

        if (plcOpenTimeBase is TwinCAT.PlcOpen.LTimeBase lTimeBase)
        {
            if (targetType == typeof(TimeSpan))
            {
                return lTimeBase.Value;
            }
        }

        throw new NotSupportedException($"Can not convert from {plcOpenTimeBase.GetType().FullName} to {targetType.FullName}");
    }

    /// <summary>
    /// This walks the whole dynamic object to make sure all values are non twincat types.
    /// </summary>
    /// <param name="structValue">The structure to sanitize.</param>
    /// <returns>A new dynamic object.</returns>
    /// <exception cref="NotSupportedException">When the data type of the member can not be retrieved.</exception>
    private object SanitizeDynamic(IStructValue structValue)
    {
        var dataType = structValue.DataType as IStructType
            ?? throw new NotSupportedException("Unable to retrieve data type for struct");
        IDictionary<string, object?> expando = new ExpandoObject();
        foreach (var member in dataType.AllMembers)
        {
            var name = member.InstanceName;
            var memberDataType = member.DataType
                ?? throw new NotSupportedException($"Unable to retrieve data type for struct member {name}");
            if (!structValue.TryGetMemberValue(name, out var childValue))
            {
                continue;
            }

            expando[name] = Convert(childValue, memberDataType);
        }

        return expando;
    }
}
