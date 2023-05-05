using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// A <see cref="ITypeConverter"/> implementation for OPC types.
/// </summary>
public sealed class OpcTypeConverter : TypeConverter, IOpcTypeConverter
{
    private readonly IOpcSymbolHandler symbolHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpcTypeConverter"/> class.
    /// </summary>
    /// <param name="symbolHandler">A <see cref="ISymbolHandler"/> instance.</param>
    public OpcTypeConverter(IOpcSymbolHandler symbolHandler)
        => this.symbolHandler = symbolHandler;

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
        if (targetType == typeof(DateTimeOffset)
            && value is DateTime dateTime)
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

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, value);
        }

        return base.Convert(value, targetType);
    }

    /// <inheritdoc/>
    public dynamic CreateDynamic(string symbolName, IEnumerator<DataValue> valueEnumerator)
    {
        var symbolInfo = symbolHandler.GetSymbolinfo(symbolName).ConvertAndValidate();
        return CreateDynamic(symbolInfo, valueEnumerator);
    }

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

    private dynamic CreateDynamic(SymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator)
    {
        if (symbolInfo.ChildSymbols.Count == 0)
        {
            if (valueEnumerator.MoveNext() && ServiceResult.IsGood(valueEnumerator.Current.StatusCode))
            {
                return Convert(valueEnumerator.Current.Value);
            }
        }

        if (symbolInfo.IsArray)
        {
            var array = Array.CreateInstance(typeof(object), symbolInfo.ArrayBounds);
            foreach (var childSymbolName in symbolInfo.ChildSymbols)
            {
                var childSymbolInfo = symbolHandler.GetSymbolinfo(childSymbolName).ConvertAndValidate();
                var value = CreateDynamic(childSymbolInfo, valueEnumerator);
                var indices = childSymbolInfo.Indices;
                array.SetValue(value, indices);
            }

            return array;
        }

        var collection = new ExpandoObject() as IDictionary<string, object>;
        foreach (var childSymbolName in symbolInfo.ChildSymbols)
        {
            var childSymbolInfo = symbolHandler.GetSymbolinfo(childSymbolName).ConvertAndValidate();
            var value = CreateDynamic(childSymbolInfo, valueEnumerator);
            var shortName = childSymbolInfo.ShortName;
            collection.Add(shortName, value);
        }

        return collection;
    }
}