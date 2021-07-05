using System;
using System.Dynamic;
using System.Reflection;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    /// <summary>
    /// A implementation of a <see cref="IDynamicValueConverter"/>.
    /// </summary>
    public class DynamicValueConverter : IDynamicValueConverter
    {
        /// <inheritdoc/>
        public object ConvertFrom(DynamicObject dynamicObject, Type type)
        {
            if (type.IsArray)
            {
                var arrayValue = ConvertArray(dynamicObject, type);
                return arrayValue;
            }

            var destination = Activator.CreateInstance(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                if (!dynamicObject.TryGetMember(new MemberBinder(property.Name, true), out var result))
                {
                    throw new InvalidOperationException($"{property.Name} is not found in the PLC type");
                }

                if (result is DynamicValue resultDynamicObject)
                {
                    if (property.PropertyType.IsArray)
                    {
                        var arrayValue = ConvertArray(resultDynamicObject, property.PropertyType);
                        property.SetValue(destination, arrayValue);
                        continue;
                    }

                    var value = ConvertFrom(resultDynamicObject, property.PropertyType);
                    property.SetValue(destination, value);
                }
                else if (result is TwinCAT.PlcOpen.DateBase dateBase)
                {
                    property.SetValue(destination, dateBase.Date);
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

        private Array ConvertArray(DynamicObject dynamicObject, Type type)
        {
            if (dynamicObject is not DynamicValue valueObject)
            {
                throw new NotSupportedException($"dynamic object is not a {typeof(DynamicValue)}");
            }

            if (valueObject.DataType is not ArrayType dataType)
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

                if (result is DynamicValue resultDynamicObject)
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