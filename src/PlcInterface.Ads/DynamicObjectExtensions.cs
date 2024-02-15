using System;
using System.Collections.Generic;
using System.Dynamic;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads;

/// <summary>
/// Extension methods for <see cref="DynamicObject"/>.
/// </summary>
internal static class DynamicObjectExtensions
{
    /// <summary>
    /// Clean the dynamic object so it only contains system types.
    /// </summary>
    /// <param name="value">The <see cref="DynamicObject"/> to clean.</param>
    /// <returns>The cleaned object.</returns>
    public static dynamic CleanDynamic(this DynamicObject value)
    {
        if (value is not IDynamicValue dynamicValue)
        {
            throw new NotSupportedException($"dynamic object is not a {typeof(IDynamicValue)}");
        }

        if (TryCleanArray(dynamicValue, out var dynamicArray))
        {
            return dynamicArray!;
        }

        if (TryCleanStruct(dynamicValue, out var dynamicStruct))
        {
            return dynamicStruct!;
        }

        throw new NotSupportedException($"Data Type is not a {typeof(IArrayType)} or {typeof(IStructType)}");
    }

    private static bool TryCleanArray(IDynamicValue arrayValue, out dynamic? array)
    {
        if (arrayValue.DataType is not ArrayType arrayType
            || !arrayValue.TryGetArrayElementValues(out var elementValues))
        {
            array = null;
            return false;
        }

        var dimensionLengths = arrayType.Dimensions.GetDimensionLengths();
        using var elementEnumerator = elementValues.GetEnumerator();

        if (arrayType.ManagedType != null)
        {
            var elementType = arrayType.ManagedType.GetElementType()
                ?? throw new NotSupportedException($"Unable to retrieve element type");

            var destination = Array.CreateInstance(elementType, dimensionLengths);

            foreach (var indices in IndicesHelper.GetIndices(destination))
            {
                if (!elementEnumerator.MoveNext())
                {
                    break;
                }

                destination.SetValue(elementEnumerator.Current, indices);
            }

            array = destination;
            return true;
        }
        else
        {
            var destination = Array.CreateInstance(typeof(object), dimensionLengths);

            foreach (var indices in IndicesHelper.GetIndices(destination))
            {
                if (!elementEnumerator.MoveNext())
                {
                    break;
                }

                if (elementEnumerator.Current is not DynamicObject dynamicObject)
                {
                    destination.SetValue(elementEnumerator.Current, indices);
                    continue;
                }

                var cleaned = dynamicObject.CleanDynamic();
                destination.SetValue(cleaned, indices);
            }

            array = destination;
            return true;
        }
    }

    private static bool TryCleanStruct(IDynamicValue structValue, out dynamic? dynamicStruct)
    {
        if (structValue.DataType is not IStructType)
        {
            dynamicStruct = null;
            return false;
        }

        IDictionary<string, object?> expando = new ExpandoObject();
        foreach (var name in ((DynamicObject)structValue).GetDynamicMemberNames())
        {
            if (!structValue.TryGetMemberValue(name, out var childValue))
            {
                continue;
            }

            expando[name] = childValue switch
            {
                DynamicObject dynamicObject => dynamicObject.CleanDynamic(),
                TwinCAT.PlcOpen.DateBase dateBase => dateBase.Value,
                TwinCAT.PlcOpen.TimeBase timeBase => timeBase.Time,
                TwinCAT.PlcOpen.LTimeBase lTimeBase => lTimeBase.Time,
                _ => childValue,
            };
        }

        dynamicStruct = expando;
        return true;
    }
}
