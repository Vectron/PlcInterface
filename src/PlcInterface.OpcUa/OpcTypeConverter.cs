using System.Dynamic;
using System.Globalization;
using Opc.Ua;

namespace PlcInterface.OpcUa;

/// <summary>
/// A <see cref="ITypeConverter"/> implementation for OPC types.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OpcTypeConverter"/> class.
/// </remarks>
/// <param name="symbolHandler">A <see cref="ISymbolHandler"/> instance.</param>
public sealed class OpcTypeConverter(IOpcSymbolHandler symbolHandler) : TypeConverter, IOpcTypeConverter
{
    /// <inheritdoc/>
    public object Convert(IOpcSymbolInfo symbolInfo, object value)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime.Ticks == 0)
            {
                return dateTime;
            }

            var specified = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return new DateTimeOffset(specified);
        }

        if (value is Matrix matrix)
        {
            return matrix.ToArray();
        }

        // https://reference.opcfoundation.org/PLCopen/v102/docs/9.2.1
        if (string.Equals(symbolInfo.TypeName, "TIME", StringComparison.OrdinalIgnoreCase)
            || string.Equals(symbolInfo.TypeName, "TOD", StringComparison.OrdinalIgnoreCase))
        {
            var milliSeconds = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            return TimeSpan.FromMilliseconds(milliSeconds);
        }

        if (string.Equals(symbolInfo.TypeName, "LTIME", StringComparison.OrdinalIgnoreCase)
            || string.Equals(symbolInfo.TypeName, "LTOD", StringComparison.OrdinalIgnoreCase))
        {
            var ticks = System.Convert.ToInt64(value, CultureInfo.InvariantCulture) / 100; // ticks are in 100 nano seconds, value is in micro seconds
            return TimeSpan.FromTicks(ticks);
        }

        return value;
    }

    /// <inheritdoc/>
    public override object Convert(object value, Type targetType)
    {
        if (targetType == typeof(DateTimeOffset)
            && value is DateTime dateTime)
        {
            if (dateTime.Ticks == 0)
            {
                return default(DateTimeOffset);
            }

            var specified = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return new DateTimeOffset(specified);
        }

        if (targetType == typeof(TimeSpan)
            && value is not TimeSpan)
        {
            var milliSeconds = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            return TimeSpan.FromMilliseconds(milliSeconds);
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
    public object Convert(string symbolName, object value)
    {
        var symbolInfo = symbolHandler.GetSymbolInfo(symbolName);
        return Convert(symbolInfo, value);
    }

    /// <inheritdoc/>
    public dynamic CreateDynamic(string symbolName, IEnumerator<DataValue> valueEnumerator)
    {
        var symbolInfo = symbolHandler.GetSymbolInfo(symbolName);
        return CreateDynamic(symbolInfo, valueEnumerator);
    }

    /// <inheritdoc/>
    public dynamic CreateDynamic(IOpcSymbolInfo symbolInfo, IEnumerator<DataValue> valueEnumerator)
    {
        if (symbolInfo.ChildSymbols.Count == 0)
        {
            if (!valueEnumerator.MoveNext()
                || !ServiceResult.IsGood(valueEnumerator.Current.StatusCode))
            {
                return new ExpandoObject();
            }

            return Convert(symbolInfo.Name, valueEnumerator.Current.Value);
        }

        if (symbolInfo.IsArray)
        {
            var arrayShape = symbolInfo.ArrayShape;
            var array = Array.CreateInstance(typeof(object), [.. arrayShape.Sizes], [.. arrayShape.LowerBounds]);
            foreach (var childSymbolName in symbolInfo.ChildSymbols)
            {
                var childSymbolInfo = symbolHandler.GetSymbolInfo(childSymbolName);
                var value = CreateDynamic(childSymbolInfo, valueEnumerator);
                var indices = childSymbolInfo.Indices;
                array.SetValue(value, indices);
            }

            return array;
        }

        var collection = new ExpandoObject() as IDictionary<string, object>;
        foreach (var childSymbolName in symbolInfo.ChildSymbols)
        {
            var childSymbolInfo = symbolHandler.GetSymbolInfo(childSymbolName);
            var value = CreateDynamic(childSymbolInfo, valueEnumerator);
            var shortName = childSymbolInfo.ShortName;
            collection.Add(shortName, value);
        }

        return collection;
    }

    /// <inheritdoc/>
    public Variant CreateOpcVariant(IOpcSymbolInfo symbolInfo, object value)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            // Mark it as UTC time so OPC lib won't try and convert it.
            return new Variant(DateTime.SpecifyKind(dateTimeOffset.LocalDateTime, DateTimeKind.Utc));
        }

        if (value is DateTime dateTime)
        {
            // Mark it as UTC time so OPC lib won't try and convert it.
            return new Variant(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
        }

        if (symbolInfo.BuiltInType == BuiltInType.Enumeration)
        {
            return new Variant(System.Convert.ToInt32(value, CultureInfo.InvariantCulture));
        }

        // https://reference.opcfoundation.org/PLCopen/v102/docs/9.2.1
        if (value is TimeSpan timeSpan)
        {
            var useTicks = symbolInfo.TypeName switch
            {
                "Time" => false,
                "LTIME" => true,
                "TOD" => false,
                "LTOD" => true,
                _ => false,
            };

            return (symbolInfo.BuiltInType, useTicks) switch
            {
                (BuiltInType.Int16, true) => new Variant(System.Convert.ToInt16(timeSpan.Ticks * 100, CultureInfo.InvariantCulture)),
                (BuiltInType.UInt16, true) => new Variant(System.Convert.ToUInt16(timeSpan.Ticks * 100, CultureInfo.InvariantCulture)),
                (BuiltInType.Int16, false) => new Variant(System.Convert.ToInt16(timeSpan.TotalMilliseconds, CultureInfo.InvariantCulture)),
                (BuiltInType.UInt16, false) => new Variant(System.Convert.ToUInt16(timeSpan.TotalMilliseconds, CultureInfo.InvariantCulture)),
                (BuiltInType.Int32, true) => new Variant(System.Convert.ToInt32(timeSpan.Ticks * 100, CultureInfo.InvariantCulture)),
                (BuiltInType.UInt32, true) => new Variant(System.Convert.ToUInt32(timeSpan.Ticks * 100, CultureInfo.InvariantCulture)),
                (BuiltInType.Int32, false) => new Variant(System.Convert.ToInt32(timeSpan.TotalMilliseconds, CultureInfo.InvariantCulture)),
                (BuiltInType.UInt32, false) => new Variant(System.Convert.ToUInt32(timeSpan.TotalMilliseconds, CultureInfo.InvariantCulture)),
                (BuiltInType.Int64, true) => new Variant(System.Convert.ToInt64(timeSpan.Ticks * 100, CultureInfo.InvariantCulture)),
                (BuiltInType.UInt64, true) => new Variant(System.Convert.ToUInt64(timeSpan.Ticks * 100, CultureInfo.InvariantCulture)),
                (BuiltInType.Int64, false) => new Variant(System.Convert.ToInt64(timeSpan.TotalMilliseconds, CultureInfo.InvariantCulture)),
                (BuiltInType.UInt64, false) => new Variant(System.Convert.ToUInt64(timeSpan.TotalMilliseconds, CultureInfo.InvariantCulture)),
                (BuiltInType.Double, _) => new Variant(timeSpan.TotalMilliseconds),
                (BuiltInType.Float, _) => new Variant(timeSpan.TotalMilliseconds),
                _ => throw new NotSupportedException($"Can't convert {nameof(TimeSpan)} to {symbolInfo.BuiltInType}"),
            };
        }

        if (value is IArrayWrapper arrayWrapper)
        {
            return new Variant(arrayWrapper.ConvertZeroBased());
        }

        return new Variant(value);
    }

    /// <inheritdoc/>
    public Variant CreateOpcVariant(string symbolName, object value)
    {
        var symbolInfo = symbolHandler.GetSymbolInfo(symbolName);
        return CreateOpcVariant(symbolInfo, value);
    }
}
