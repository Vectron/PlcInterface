using System.Collections.Generic;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace System.Dynamic
{
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
            if (value is not DynamicValue valueObject)
            {
                throw new NotSupportedException($"dynamic object is not a {typeof(DynamicValue)}");
            }

            if (valueObject.DataType is ArrayType arrayType)
            {
                return CleanArray(value, valueObject, arrayType);
            }

            if (valueObject.DataType is StructType)
            {
                return CleanStruct(value, valueObject);
            }

            throw new NotSupportedException($"Data Type is not a {typeof(ArrayType)}");
        }

        private static dynamic CleanArray(DynamicObject value, DynamicValue valueObject, ArrayType arrayType)
        {
            if (valueObject.TryGetArrayElementValues(out var elementValues))
            {
                var dimensionLengts = arrayType.Dimensions.GetDimensionLengths();
                using var ellementEnumerator = elementValues.GetEnumerator();

                if (arrayType.ManagedType != null)
                {
                    var ellementType = arrayType.ManagedType.GetElementType();
                    var destination = Array.CreateInstance(ellementType, dimensionLengts);

                    foreach (var indices in destination.Indices())
                    {
                        if (!ellementEnumerator.MoveNext())
                        {
                            break;
                        }

                        destination.SetValue(ellementEnumerator.Current, indices);
                    }

                    return destination;
                }
                else
                {
                    var destination = Array.CreateInstance(typeof(ExpandoObject), dimensionLengts);

                    foreach (var indices in destination.Indices())
                    {
                        if (!ellementEnumerator.MoveNext())
                        {
                            break;
                        }

                        if (ellementEnumerator.Current is not DynamicObject dynamicObject)
                        {
                            destination.SetValue(ellementEnumerator.Current, indices);
                            continue;
                        }

                        var cleaned = CleanDynamic(dynamicObject) as ExpandoObject;
                        destination.SetValue(cleaned, indices);
                    }

                    return destination;
                }
            }

            return value;
        }

        private static dynamic CleanStruct(DynamicObject value, DynamicValue valueObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var name in value.GetDynamicMemberNames())
            {
                if (!valueObject.TryGetMemberValue(name, out var childValue))
                {
                    continue;
                }

                expando[name] = childValue switch
                {
                    DynamicValue dynamicObject => CleanDynamic(dynamicObject),
                    TwinCAT.PlcOpen.DateBase dateBase => dateBase.Value,
                    TwinCAT.PlcOpen.TimeBase timeBase => timeBase.Time,
                    TwinCAT.PlcOpen.LTimeBase lTimeBase => lTimeBase.Time,
                    _ => childValue,
                };
            }

            return expando;
        }
    }
}