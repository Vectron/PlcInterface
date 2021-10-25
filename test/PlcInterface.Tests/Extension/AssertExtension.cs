using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests.DataTypes;

namespace PlcInterface.Tests;

internal static class AssertExtension
{
    public static void DUT_TestStructEquals(DUT_TestStruct2 expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.ByteValue)}"),
            () => Assert.AreEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.WordValue)}"),
            () => Assert.AreEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.DWordValue)}"),
            () => Assert.AreEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.LWordValue)}"));

    public static void DUT_TestStructEquals(DUT_TestClass2 expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.ByteValue)}"),
            () => Assert.AreEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.WordValue)}"),
            () => Assert.AreEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.DWordValue)}"),
            () => Assert.AreEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.LWordValue)}"));

    public static void DUT_TestStructEquals(DUT_TestStruct expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreEqual(expected.BoolValue, current.BoolValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.BoolValue)}"),
            () => Assert.AreEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ByteValue)}"),
            () => Assert.AreEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.WordValue)}"),
            () => Assert.AreEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DWordValue)}"),
            () => Assert.AreEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.LWordValue)}"),
            () => Assert.AreEqual(expected.ShortValue, current.ShortValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ShortValue)}"),
            () => Assert.AreEqual(expected.IntValue, current.IntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.IntValue)}"),
            () => Assert.AreEqual(expected.DIntValue, current.DIntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DIntValue)}"),
            () => Assert.AreEqual(expected.LongValue, current.LongValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.LongValue)}"),
            () => Assert.AreEqual(expected.UShortValue, current.UShortValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.UShortValue)}"),
            () => Assert.AreEqual(expected.UIntValue, current.UIntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.UIntValue)}"),
            () => Assert.AreEqual(expected.UDIntValue, current.UDIntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.UDIntValue)}"),
            () => Assert.AreEqual(expected.ULongValue, current.ULongValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ULongValue)}"),
            () => Assert.AreEqual(expected.FloatValue, current.FloatValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.FloatValue)}"),
            () => Assert.AreEqual(expected.DoubleValue, current.DoubleValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DoubleValue)}"),
            () => Assert.AreEqual(expected.TimeValue, current.TimeValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.TimeValue)}"),
            () => Assert.AreEqual(expected.TimeOfDay, current.TimeOfDay, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.TimeOfDay)}"),
            () => Assert.AreEqual(expected.LTimeValue, current.LTimeValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.LTimeValue)}"),
            () => Assert.AreEqual(expected.DateValue, current.DateValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DateValue)}"),
            () => Assert.AreEqual(expected.DateAndTimeValue, current.DateAndTimeValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DateAndTimeValue)}"),
            () => Assert.AreEqual(expected.StringValue, current.StringValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.StringValue)}"),
            () => Assert.AreEqual(expected.WStringValue, current.WStringValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.WStringValue)}"),
            () => CollectionAssert.AreEqual(expected.IntArray, current.IntArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.IntArray)}"),
            () => CollectionAssert.AreEqual(expected.MultiDimensionArray, current.MultiDimensionArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.MultiDimensionArray)}"),
            () => Assert.IsNotNull(current.ComplexArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.ComplexArray.Length - 1; i++)
                {
                    DUT_TestStructEquals(expected.ComplexArray[i], current.ComplexArray[i]);
                }
            },
            () => Assert.IsNotNull(current.MultiDimensionComplexArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.MultiDimensionComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.MultiDimensionComplexArray.GetLength(0); i++)
                {
                    for (var j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                    {
                        for (var k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                        {
                            DUT_TestStructEquals(expected.MultiDimensionComplexArray[i, j, k], current.MultiDimensionComplexArray[i, j, k]);
                        }
                    }
                }
            },
            () => DUT_TestStructEquals(expected.Nested, current.Nested));

    public static void DUT_TestStructEquals(DUT_TestClass expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreEqual(expected.BoolValue, current.BoolValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.BoolValue)}"),
            () => Assert.AreEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ByteValue)}"),
            () => Assert.AreEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.WordValue)}"),
            () => Assert.AreEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DWordValue)}"),
            () => Assert.AreEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.LWordValue)}"),
            () => Assert.AreEqual(expected.ShortValue, current.ShortValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ShortValue)}"),
            () => Assert.AreEqual(expected.IntValue, current.IntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.IntValue)}"),
            () => Assert.AreEqual(expected.DIntValue, current.DIntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DIntValue)}"),
            () => Assert.AreEqual(expected.LongValue, current.LongValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.LongValue)}"),
            () => Assert.AreEqual(expected.UShortValue, current.UShortValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.UShortValue)}"),
            () => Assert.AreEqual(expected.UIntValue, current.UIntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.UIntValue)}"),
            () => Assert.AreEqual(expected.UDIntValue, current.UDIntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.UDIntValue)}"),
            () => Assert.AreEqual(expected.ULongValue, current.ULongValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ULongValue)}"),
            () => Assert.AreEqual(expected.FloatValue, current.FloatValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.FloatValue)}"),
            () => Assert.AreEqual(expected.DoubleValue, current.DoubleValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DoubleValue)}"),
            () => Assert.AreEqual(expected.TimeValue, current.TimeValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.TimeValue)}"),
            () => Assert.AreEqual(expected.TimeOfDay, current.TimeOfDay, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.TimeOfDay)}"),
            () => Assert.AreEqual(expected.LTimeValue, current.LTimeValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.LTimeValue)}"),
            () => Assert.AreEqual(expected.DateValue, current.DateValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DateValue)}"),
            () => Assert.AreEqual(expected.DateAndTimeValue, current.DateAndTimeValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DateAndTimeValue)}"),
            () => Assert.AreEqual(expected.StringValue, current.StringValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.StringValue)}"),
            () => Assert.AreEqual(expected.WStringValue, current.WStringValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.WStringValue)}"),
            () => CollectionAssert.AreEqual(expected.IntArray, current.IntArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.IntArray)}"),
            () => CollectionAssert.AreEqual(expected.MultiDimensionArray, current.MultiDimensionArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.MultiDimensionArray)}"),
            () => Assert.IsNotNull(current.ComplexArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.ComplexArray!.Length - 1; i++)
                {
                    DUT_TestStructEquals(expected.ComplexArray[i], current.ComplexArray[i]);
                }
            },
            () => Assert.IsNotNull(current.MultiDimensionComplexArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.MultiDimensionComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.MultiDimensionComplexArray!.GetLength(0); i++)
                {
                    for (var j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                    {
                        for (var k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                        {
                            DUT_TestStructEquals(expected.MultiDimensionComplexArray[i, j, k], current.MultiDimensionComplexArray[i, j, k]);
                        }
                    }
                }
            },
            () => DUT_TestStructEquals(expected.Nested, current.Nested));

    public static void DUT_TestStructNotEquals(DUT_TestStruct2 expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreNotEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.ByteValue)}"),
            () => Assert.AreNotEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.WordValue)}"),
            () => Assert.AreNotEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.DWordValue)}"),
            () => Assert.AreNotEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestStruct2)}.{nameof(DUT_TestStruct2.LWordValue)}"));

    public static void DUT_TestStructNotEquals(DUT_TestClass2 expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreNotEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.ByteValue)}"),
            () => Assert.AreNotEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.WordValue)}"),
            () => Assert.AreNotEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.DWordValue)}"),
            () => Assert.AreNotEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestClass2)}.{nameof(DUT_TestClass2.LWordValue)}"));

    public static void DUT_TestStructNotEquals(DUT_TestStruct expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreNotEqual(expected.BoolValue, current.BoolValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.BoolValue)}"),
            () => Assert.AreNotEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ByteValue)}"),
            () => Assert.AreNotEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.WordValue)}"),
            () => Assert.AreNotEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DWordValue)}"),
            () => Assert.AreNotEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.LWordValue)}"),
            () => Assert.AreNotEqual(expected.ShortValue, current.ShortValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ShortValue)}"),
            () => Assert.AreNotEqual(expected.IntValue, current.IntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.IntValue)}"),
            () => Assert.AreNotEqual(expected.DIntValue, current.DIntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DIntValue)}"),
            () => Assert.AreNotEqual(expected.LongValue, current.LongValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.LongValue)}"),
            () => Assert.AreNotEqual(expected.UShortValue, current.UShortValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.UShortValue)}"),
            () => Assert.AreNotEqual(expected.UIntValue, current.UIntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.UIntValue)}"),
            () => Assert.AreNotEqual(expected.UDIntValue, current.UDIntValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.UDIntValue)}"),
            () => Assert.AreNotEqual(expected.ULongValue, current.ULongValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ULongValue)}"),
            () => Assert.AreNotEqual(expected.FloatValue, current.FloatValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.FloatValue)}"),
            () => Assert.AreNotEqual(expected.DoubleValue, current.DoubleValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DoubleValue)}"),
            () => Assert.AreNotEqual(expected.TimeValue, current.TimeValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.TimeValue)}"),
            () => Assert.AreNotEqual(expected.TimeOfDay, current.TimeOfDay, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.TimeOfDay)}"),
            () => Assert.AreNotEqual(expected.LTimeValue, current.LTimeValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.LTimeValue)}"),
            () => Assert.AreNotEqual(expected.DateValue, current.DateValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DateValue)}"),
            () => Assert.AreNotEqual(expected.DateAndTimeValue, current.DateAndTimeValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.DateAndTimeValue)}"),
            () => Assert.AreNotEqual(expected.StringValue, current.StringValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.StringValue)}"),
            () => Assert.AreNotEqual(expected.WStringValue, current.WStringValue, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.WStringValue)}"),
            () => CollectionAssert.AreNotEqual(expected.IntArray, current.IntArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.IntArray)}"),
            () => CollectionAssert.AreNotEqual(expected.MultiDimensionArray, current.MultiDimensionArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.MultiDimensionArray)}"),
            () => Assert.IsNotNull(current.ComplexArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.ComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.ComplexArray.Length - 1; i++)
                {
                    DUT_TestStructNotEquals(expected.ComplexArray[i], current.ComplexArray[i]);
                }
            },
            () => Assert.IsNotNull(current.MultiDimensionComplexArray, $"{nameof(DUT_TestStruct)}.{nameof(DUT_TestStruct.MultiDimensionComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.MultiDimensionComplexArray.GetLength(0); i++)
                {
                    for (var j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                    {
                        for (var k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                        {
                            DUT_TestStructNotEquals(expected.MultiDimensionComplexArray[i, j, k], current.MultiDimensionComplexArray[i, j, k]);
                        }
                    }
                }
            },
            () => DUT_TestStructNotEquals(expected.Nested, current.Nested));

    public static void DUT_TestStructNotEquals(DUT_TestClass expected, dynamic current)
        => MultiAssert.Aggregate(
            () => Assert.AreNotEqual(expected.BoolValue, current.BoolValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.BoolValue)}"),
            () => Assert.AreNotEqual(expected.ByteValue, current.ByteValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ByteValue)}"),
            () => Assert.AreNotEqual(expected.WordValue, current.WordValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.WordValue)}"),
            () => Assert.AreNotEqual(expected.DWordValue, current.DWordValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DWordValue)}"),
            () => Assert.AreNotEqual(expected.LWordValue, current.LWordValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.LWordValue)}"),
            () => Assert.AreNotEqual(expected.ShortValue, current.ShortValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ShortValue)}"),
            () => Assert.AreNotEqual(expected.IntValue, current.IntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.IntValue)}"),
            () => Assert.AreNotEqual(expected.DIntValue, current.DIntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DIntValue)}"),
            () => Assert.AreNotEqual(expected.LongValue, current.LongValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.LongValue)}"),
            () => Assert.AreNotEqual(expected.UShortValue, current.UShortValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.UShortValue)}"),
            () => Assert.AreNotEqual(expected.UIntValue, current.UIntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.UIntValue)}"),
            () => Assert.AreNotEqual(expected.UDIntValue, current.UDIntValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.UDIntValue)}"),
            () => Assert.AreNotEqual(expected.ULongValue, current.ULongValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ULongValue)}"),
            () => Assert.AreNotEqual(expected.FloatValue, current.FloatValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.FloatValue)}"),
            () => Assert.AreNotEqual(expected.DoubleValue, current.DoubleValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DoubleValue)}"),
            () => Assert.AreNotEqual(expected.TimeValue, current.TimeValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.TimeValue)}"),
            () => Assert.AreNotEqual(expected.TimeOfDay, current.TimeOfDay, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.TimeOfDay)}"),
            () => Assert.AreNotEqual(expected.LTimeValue, current.LTimeValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.LTimeValue)}"),
            () => Assert.AreNotEqual(expected.DateValue, current.DateValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DateValue)}"),
            () => Assert.AreNotEqual(expected.DateAndTimeValue, current.DateAndTimeValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.DateAndTimeValue)}"),
            () => Assert.AreNotEqual(expected.StringValue, current.StringValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.StringValue)}"),
            () => Assert.AreNotEqual(expected.WStringValue, current.WStringValue, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.WStringValue)}"),
            () => CollectionAssert.AreNotEqual(expected.IntArray, current.IntArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.IntArray)}"),
            () => CollectionAssert.AreNotEqual(expected.MultiDimensionArray, current.MultiDimensionArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.MultiDimensionArray)}"),
            () => Assert.IsNotNull(current.ComplexArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.ComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.ComplexArray!.Length - 1; i++)
                {
                    DUT_TestStructNotEquals(expected.ComplexArray[i], current.ComplexArray[i]);
                }
            },
            () => Assert.IsNotNull(current.MultiDimensionComplexArray, $"{nameof(DUT_TestClass)}.{nameof(DUT_TestClass.MultiDimensionComplexArray)}"),
            () =>
            {
                for (var i = 0; i < expected.MultiDimensionComplexArray!.GetLength(0); i++)
                {
                    for (var j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                    {
                        for (var k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                        {
                            DUT_TestStructNotEquals(expected.MultiDimensionComplexArray[i, j, k], current.MultiDimensionComplexArray[i, j, k]);
                        }
                    }
                }
            },
            () => DUT_TestStructNotEquals(expected.Nested, current.Nested));

    public static void ObjectEquals(this Assert assert, object expectedValue, object value, string message = "")
    {
        if (value is System.Dynamic.IDynamicMetaObjectProvider dynamic)
        {
            if (expectedValue is DUT_TestStruct testStruct)
            {
                DUT_TestStructEquals(testStruct, dynamic);
                return;
            }

            if (expectedValue is DUT_TestStruct2 testStruct2)
            {
                DUT_TestStructEquals(testStruct2, dynamic);
                return;
            }

            if (expectedValue is DUT_TestClass testClass)
            {
                DUT_TestStructEquals(testClass, dynamic);
                return;
            }

            if (expectedValue is DUT_TestClass2 testClass2)
            {
                DUT_TestStructEquals(testClass2, dynamic);
                return;
            }

            Assert.Fail("Unknown type");
        }
        else if (expectedValue is System.Collections.ICollection expectedCollection
            && value is System.Collections.ICollection valueCollection)
        {
            if (expectedValue.GetType() == value.GetType())
            {
                CollectionAssert.AreEqual(expectedCollection, valueCollection, message);
                return;
            }

            var expectedEnumerator = expectedCollection.GetEnumerator();
            var valueEnumerator = valueCollection.GetEnumerator();
            while (expectedEnumerator.MoveNext() && valueEnumerator.MoveNext())
            {
                assert.ObjectEquals(expectedEnumerator.Current, valueEnumerator.Current, message);
            }

            return;
        }

        Assert.AreEqual(expectedValue, value, message);
    }

    public static void ObjectNotEquals(this Assert assert, object expectedValue, object value, string message = "")
    {
        if (value is System.Dynamic.IDynamicMetaObjectProvider dynamic)
        {
            if (expectedValue is DUT_TestStruct testStruct)
            {
                DUT_TestStructNotEquals(testStruct, dynamic);
                return;
            }

            if (expectedValue is DUT_TestStruct2 testStruct2)
            {
                DUT_TestStructNotEquals(testStruct2, dynamic);
                return;
            }

            if (expectedValue is DUT_TestClass testClass)
            {
                DUT_TestStructNotEquals(testClass, dynamic);
                return;
            }

            if (expectedValue is DUT_TestClass2 testClass2)
            {
                DUT_TestStructNotEquals(testClass2, dynamic);
                return;
            }

            Assert.Fail("Unknown type");
        }
        else if (expectedValue is System.Collections.ICollection expectedCollection
            && value is System.Collections.ICollection valueCollection)
        {
            if (expectedValue.GetType() == value.GetType())
            {
                CollectionAssert.AreNotEqual(expectedCollection, valueCollection, message);
                return;
            }

            var expectedEnumerator = expectedCollection.GetEnumerator();
            var valueEnumerator = valueCollection.GetEnumerator();
            while (expectedEnumerator.MoveNext() && valueEnumerator.MoveNext())
            {
                assert.ObjectNotEquals(expectedEnumerator.Current, valueEnumerator.Current, message);
            }

            return;
        }

        Assert.AreNotEqual(expectedValue, value, message);
    }
}