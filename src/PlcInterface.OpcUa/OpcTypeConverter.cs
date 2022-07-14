using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using Opc.Ua;
using PlcInterface.Extensions;

namespace PlcInterface.OpcUa;

/// <summary>
/// A <see cref="ITypeConverter"/> implementation for OPC types.
/// </summary>
public sealed class OpcTypeConverter : TypeConverter, IOpcTypeConverter
{
    /// <inheritdoc/>
    public object Convert(object value)
    {
        if (value is DateTime dateTime)
        {
            var specified = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return new DateTimeOffset(specified);
        }

        if (value is Matrix matrix)
        {
            return matrix.ToArray();
        }

        return value;
    }

    /// <inheritdoc/>
    public override object Convert(object value, Type targetType)
    {
        if (targetType == typeof(DateTimeOffset) && value is DateTime dateTime)
        {
            var specified = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return new DateTimeOffset(specified);
        }

        if (targetType == typeof(TimeSpan))
        {
            return CreateTimeSpan(value);
        }

        if (value is Matrix matrix)
        {
            return matrix.ToArray();
        }

        if (value.GetType().IsArray
            && targetType.IsArray
            && value is Array array)
        {
            return CreateArray(targetType, array);
        }

        if (value is ExpandoObject keyValues)
        {
            var instance = Activator.CreateInstance(targetType) ?? throw new NotSupportedException($"Unable to create a instance for type: {targetType}");
            foreach (var keyValue in keyValues)
            {
                var property = targetType.GetProperty(keyValue.Key);

                if (property == null)
                {
                    Debug.Assert(property != null, "No property found with name: {0} on object of type: {1}", keyValue.Key, targetType.Name);
                    continue;
                }

                var fixedObject = keyValue.Value == null ? null : Convert(keyValue.Value, property.PropertyType);
                property.SetValue(instance, fixedObject);
            }

            return instance;
        }

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, value);
        }

        return base.Convert(value, targetType);
    }

    /// <inheritdoc/>
    public dynamic CreateDynamic(ISymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator, ISymbolHandler symbolHandler)
        => CreateDynamic(symbolInfo.ConvertAndValidate(), valueEnumerator, symbolHandler);

    private static TimeSpan CreateTimeSpan(object value)
    {
        if (value.GetType() == typeof(ulong))
        {
            var ticks = System.Convert.ToInt64(value, CultureInfo.InvariantCulture) / 100; // ticks are in 100 nano seconds, value is in micro seconds
            return TimeSpan.FromTicks(ticks);
        }

        var miliSeconds = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
        return TimeSpan.FromMilliseconds(miliSeconds);
    }

    private Array CreateArray(Type targetType, Array array)
    {
        var upperBoundsRank = new int[array.Rank];
        for (var dimension = 0; dimension < array.Rank; dimension++)
        {
            upperBoundsRank[dimension] = array.GetLength(dimension);
        }

        var elementType = targetType.GetElementType() ?? throw new NotSupportedException("Unable to get element type");
        var typedArray = Array.CreateInstance(elementType, upperBoundsRank);

        foreach (var indices in typedArray.Indices())
        {
            var item = array.GetValue(indices);
            if (item == null)
            {
                continue;
            }

            var fixedObject = Convert(item, elementType);
            typedArray.SetValue(fixedObject, indices);
        }

        return typedArray;
    }

    private dynamic CreateDynamic(SymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator, ISymbolHandler symbolHandler)
    {
        if (symbolInfo.ChildSymbols.Count == 0)
        {
            if (valueEnumerator.MoveNext() && ServiceResult.IsGood(valueEnumerator.Current.StatusCode))
            {
                if (valueEnumerator.Current.Value is Matrix matrixValue)
                {
                    return matrixValue.ToArray();
                }

                return Convert(valueEnumerator.Current.Value);
            }
        }

        if (symbolInfo.IsArray)
        {
            var array = Array.CreateInstance(typeof(ExpandoObject), symbolInfo.ArrayBounds);
            foreach (var childSymbolName in symbolInfo.ChildSymbols)
            {
                var childSymbolInfo = symbolHandler.GetSymbolinfo(childSymbolName).ConvertAndValidate();
                var value = ((IOpcTypeConverter)this).CreateDynamic(childSymbolInfo, valueEnumerator, symbolHandler);
                var indices = childSymbolInfo.Indices;
                array.SetValue(value, indices);
            }

            return array;
        }

        var collection = new ExpandoObject() as IDictionary<string, object>;
        foreach (var childSymbolName in symbolInfo.ChildSymbols)
        {
            var childSymbolInfo = symbolHandler.GetSymbolinfo(childSymbolName).ConvertAndValidate();
            var value = ((IOpcTypeConverter)this).CreateDynamic(childSymbolInfo, valueEnumerator, symbolHandler);
            var shortName = childSymbolInfo.ShortName;
            collection.Add(shortName, value);
        }

        return collection;
    }
}