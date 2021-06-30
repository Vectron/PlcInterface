using System.Collections.Generic;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace System.Dynamic
{
    internal static class DynamicObjectExtensions
    {
        public static dynamic CleanDynamic(this DynamicObject value)
        {
            if (!(value is DynamicValue valueObject))
            {
                throw new NotSupportedException($"dynamic object is not a {typeof(DynamicValue)}");
            }

            if (valueObject.DataType is ArrayType arrayType)
            {
                if (valueObject.TryGetArrayElementValues(out var elementValues))
                {
                    bool incArray(Array a, int[] ind)
                    {
                        var rank = a.Rank;
                        ind[rank - 1]++;
                        for (var i = rank - 1; i >= 0; i--)
                        {
                            if (ind[i] > a.GetUpperBound(i))
                            {
                                if (i == 0)
                                {
                                    return false;
                                }

                                for (var j = i; j < rank; j++)
                                {
                                    ind[j] = 0;
                                }

                                ind[i - 1]++;
                            }
                        }

                        return true;
                    }
                    var dimensionLengts = arrayType.Dimensions.GetDimensionLengths();
                    var indices = new int[dimensionLengts.Length];
                    indices[indices.Length - 1]--;
                    var ellementEnumerator = elementValues.GetEnumerator();

                    if (arrayType.ManagedType != null)
                    {
                        var ellementType = arrayType.ManagedType.GetElementType();
                        var destination = Array.CreateInstance(ellementType, dimensionLengts);

                        while (incArray(destination, indices) && ellementEnumerator.MoveNext())
                        {
                            destination.SetValue(ellementEnumerator.Current, indices);
                        }

                        return destination;
                    }
                    else
                    {
                        var destination = Array.CreateInstance(typeof(ExpandoObject), dimensionLengts);

                        while (incArray(destination, indices) && ellementEnumerator.MoveNext())
                        {
                            var dynamicObject = ellementEnumerator.Current as DynamicObject;
                            var cleaned = CleanDynamic(dynamicObject) as ExpandoObject;
                            destination.SetValue(cleaned, indices);
                        }
                    }
                }
                else
                {
                }

                return value;
            }
            else if (valueObject.DataType is StructType)
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (var name in value.GetDynamicMemberNames())
                {
                    if (!valueObject.TryGetMemberValue(name, out var childValue))
                    {
                        continue;
                    }

                    if (childValue is DynamicValue dynamicObject)
                    {
                        expando[name] = CleanDynamic(dynamicObject);
                    }
                    else if (childValue is TwinCAT.PlcOpen.DateBase dateBase)
                    {
                        expando[name] = dateBase.Date;
                    }
                    else if (childValue is TwinCAT.PlcOpen.TimeBase timeBase)
                    {
                        expando[name] = timeBase.Time;
                    }
                    else if (childValue is TwinCAT.PlcOpen.LTimeBase lTimeBase)
                    {
                        expando[name] = lTimeBase.Time;
                    }
                    else
                    {
                        expando[name] = childValue;
                    }
                }
                return expando;
            }

            throw new NotSupportedException($"Data Type is not a {typeof(ArrayType)}");
        }
    }
}