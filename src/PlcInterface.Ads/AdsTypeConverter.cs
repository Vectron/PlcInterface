using System;
using System.Dynamic;
using System.Globalization;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
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
            if (value is DynamicObject dynamicObject)
            {
                return ConvertFrom(dynamicObject, targetType);
            }

            if (value is DateTime dateTime && targetType == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(dateTime);
            }

            return base.Convert(value, targetType);
        }

        private Array ConvertArray(DynamicObject dynamicObject, Type type)
        {
            if (dynamicObject is not IDynamicValue valueObject)
            {
                throw new NotSupportedException($"dynamic object is not a {typeof(IDynamicValue)}");
            }

            if (valueObject.DataType is not IArrayType dataType)
            {
                throw new NotSupportedException($"Data Type is not a {typeof(ArrayType)}");
            }

            var ellementType = type.GetElementType();
            var dimensionLengts = dataType.Dimensions.GetDimensionLengths();
            var destination = Array.CreateInstance(ellementType, dimensionLengts);

            foreach (var indices in destination.Indices())
            {
                if (!valueObject.TryGetIndexValue(indices, out var result))
                {
                    throw new SymbolException($"No value found at index {indices}");
                }

                if (result is DynamicObject resultDynamicObject)
                {
                    var value = ConvertFrom(resultDynamicObject, ellementType);
                    destination.SetValue(value, indices);
                }
                else if (result.GetType() == ellementType)
                {
                    destination.SetValue(result, indices);
                }
                else
                {
                    throw new SymbolException($"Unable to convert type {result}");
                }
            }

            return destination;
        }

        private object ConvertFrom(DynamicObject dynamicObject, Type type)
        {
            if (type.IsArray)
            {
                return ConvertArray(dynamicObject, type);
            }

            var destination = Activator.CreateInstance(type);

            foreach (var memberName in dynamicObject.GetDynamicMemberNames())
            {
                var property = type.GetProperty(memberName);
                if (property == null)
                {
                    throw new InvalidOperationException($"{memberName} not found as a property");
                }

                if (!property.CanWrite)
                {
                    throw new InvalidOperationException($"{property.Name} is not writable");
                }

                if (!dynamicObject.TryGetMember(new MemberBinder(property.Name, true), out var result))
                {
                    throw new InvalidOperationException($"{property.Name} is not found in the PLC type");
                }

                if (result is DynamicObject resultDynamicObject)
                {
                    var value = ConvertFrom(resultDynamicObject, property.PropertyType);
                    property.SetValue(destination, value);
                }
                else if (result is TwinCAT.PlcOpen.DateBase dateBase)
                {
                    if (property.PropertyType == typeof(DateTimeOffset))
                    {
                        property.SetValue(destination, new DateTimeOffset(dateBase.Value));
                    }
                    else
                    {
                        property.SetValue(destination, dateBase.Value);
                    }
                }
                else if (result is TwinCAT.PlcOpen.TimeBase timeBase)
                {
                    property.SetValue(destination, timeBase.Time);
                }
                else if (result is TwinCAT.PlcOpen.LTimeBase lTimeBase)
                {
                    property.SetValue(destination, lTimeBase.Time);
                }
                else
                {
                    property.SetValue(destination, result);
                    continue;
                }
            }

            return destination;
        }

        private sealed class MemberBinder : GetMemberBinder
        {
            public MemberBinder(string name, bool ignoreCase)
                : base(name, ignoreCase)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
                => throw new NotSupportedException();
        }
    }
}