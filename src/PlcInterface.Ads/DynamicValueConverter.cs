using System;
using System.Dynamic;
using System.Reflection;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    public class DynamicValueConverter : IDynamicValueConverter
    {
        public object ConvertFrom(DynamicObject dynamicObject, Type type)
        {
            var destination = Activator.CreateInstance(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                if (!dynamicObject.TryGetMember(new MemberBinder(property.Name, true), out object result))
                {
                    throw new InvalidOperationException($"{property.Name} is not found in the PLC type");
                }

                if (!(result is DynamicObject resultDynamicObject))
                {
                    property.SetValue(destination, result);
                    continue;
                }

                if (property.PropertyType.IsArray)
                {
                    var arrayValue = ConvertArray(resultDynamicObject, property.PropertyType);
                    property.SetValue(destination, arrayValue);
                    continue;
                }

                var value = ConvertFrom(resultDynamicObject, property.PropertyType);
                property.SetValue(destination, value);
            }
            return destination;
        }

        private Array ConvertArray(DynamicObject dynamicObject, Type type)
        {
            if (!(dynamicObject is DynamicValue valueObject))
            {
                throw new NotSupportedException($"dynamic object is not a {typeof(DynamicValue)}");
            }

            if (!(valueObject.DataType is ArrayType dataType))
            {
                throw new NotSupportedException($"Data Type is not a {typeof(ArrayType)}");
            }

            var dimensionLengts = dataType.Dimensions.GetDimensionLengths();
            var ellementType = type.GetElementType();
            var destination = Array.CreateInstance(ellementType, dimensionLengts);
            var indices = new int[dataType.ElementCount][];
            int[] previous = null;

            for (int i = 0; i < dataType.ElementCount; i++)
            {
                var current = new int[dataType.DimensionCount];
                previous?.CopyTo(current, 0);

                for (int j = dataType.DimensionCount - 1; j >= 0; j--)
                {
                    var dimensionInfo = dataType.Dimensions[j];

                    if (previous == null)
                    {
                        current[j] = dimensionInfo.LowerBound;
                        continue;
                    }

                    var upperBound = dimensionInfo.LowerBound + dimensionInfo.ElementCount;
                    current[j] += 1;

                    if (current[j] >= upperBound)
                    {
                        current[j] = dimensionInfo.LowerBound;
                        continue;
                    }

                    break;
                }

                indices[i] = current;
                previous = current;
            }

            foreach (var index in indices)
            {
                valueObject.TryGetIndexValue(index, out object result);

                if (result is DynamicValue resultDynamicObject)
                {
                    var value = ConvertFrom(resultDynamicObject, ellementType);
                    destination.SetValue(value, index);
                }
            }

            return destination;
        }

        private class MemberBinder : GetMemberBinder
        {
            public MemberBinder(string name, bool ignoreCase)
                : base(name, ignoreCase)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }
    }
}