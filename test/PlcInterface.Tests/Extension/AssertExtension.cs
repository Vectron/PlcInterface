using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests.DataTypes;

namespace PlcInterface.Tests
{
    internal static class AssertExtension
    {
        public static void DUT_TestStructEquals(DUT_TestStruct expected, dynamic current)
        {
            Assert.AreEqual(expected.BoolValue, current.BoolValue);

            Assert.AreEqual(expected.ByteValue, current.ByteValue);
            Assert.AreEqual(expected.WordValue, current.WordValue);
            Assert.AreEqual(expected.DWordValue, current.DWordValue);
            Assert.AreEqual(expected.LWordValue, current.LWordValue);

            Assert.AreEqual(expected.ShortValue, current.ShortValue);
            Assert.AreEqual(expected.IntValue, current.IntValue);
            Assert.AreEqual(expected.DIntValue, current.DIntValue);
            Assert.AreEqual(expected.LongValue, current.LongValue);

            Assert.AreEqual(expected.UShortValue, current.UShortValue);
            Assert.AreEqual(expected.UIntValue, current.UIntValue);
            Assert.AreEqual(expected.UDIntValue, current.UDIntValue);
            Assert.AreEqual(expected.ULongValue, current.ULongValue);

            Assert.AreEqual(expected.FloatValue, current.FloatValue);
            Assert.AreEqual(expected.DoubleValue, current.DoubleValue);
            Assert.AreEqual(expected.TimeValue, current.TimeValue);
            Assert.AreEqual(expected.TimeOfDay, current.TimeOfDay);
            Assert.AreEqual(expected.LTimeValue, current.LTimeValue);
            Assert.AreEqual(expected.DateValue, current.DateValue);
            Assert.AreEqual(expected.DateAndTimeValue, current.DateAndTimeValue);

            Assert.AreEqual(expected.StringValue, current.StringValue);
            Assert.AreEqual(expected.WStringValue, current.WStringValue);

            CollectionAssert.AreEqual(expected.IntArray, current.IntArray);
            CollectionAssert.AreEqual(expected.MultiDimensionArray, current.MultiDimensionArray);

            for (var i = 0; i < expected.ComplexArray.Length - 1; i++)
            {
                Assert.AreEqual(expected.ComplexArray[i].ByteValue, current.ComplexArray[i].ByteValue);
                Assert.AreEqual(expected.ComplexArray[i].WordValue, current.ComplexArray[i].WordValue);
                Assert.AreEqual(expected.ComplexArray[i].DWordValue, current.ComplexArray[i].DWordValue);
                Assert.AreEqual(expected.ComplexArray[i].LWordValue, current.ComplexArray[i].LWordValue);
            }

            for (var i = 0; i < expected.MultiDimensionComplexArray.GetLength(0); i++)
            {
                for (var j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                {
                    for (var k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                    {
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].ByteValue, current.MultiDimensionComplexArray[i, j, k].ByteValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].WordValue, current.MultiDimensionComplexArray[i, j, k].WordValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].DWordValue, current.MultiDimensionComplexArray[i, j, k].DWordValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].LWordValue, current.MultiDimensionComplexArray[i, j, k].LWordValue);
                    }
                }
            }

            Assert.AreEqual(expected.Nested.ByteValue, current.Nested.ByteValue);
            Assert.AreEqual(expected.Nested.WordValue, current.Nested.WordValue);
            Assert.AreEqual(expected.Nested.DWordValue, current.Nested.DWordValue);
            Assert.AreEqual(expected.Nested.LWordValue, current.Nested.LWordValue);
        }

        public static void DUT_TestStructEquals(DUT_TestClass expected, dynamic current)
        {
            Assert.AreEqual(expected.BoolValue, current.BoolValue);

            Assert.AreEqual(expected.ByteValue, current.ByteValue);
            Assert.AreEqual(expected.WordValue, current.WordValue);
            Assert.AreEqual(expected.DWordValue, current.DWordValue);
            Assert.AreEqual(expected.LWordValue, current.LWordValue);

            Assert.AreEqual(expected.ShortValue, current.ShortValue);
            Assert.AreEqual(expected.IntValue, current.IntValue);
            Assert.AreEqual(expected.DIntValue, current.DIntValue);
            Assert.AreEqual(expected.LongValue, current.LongValue);

            Assert.AreEqual(expected.UShortValue, current.UShortValue);
            Assert.AreEqual(expected.UIntValue, current.UIntValue);
            Assert.AreEqual(expected.UDIntValue, current.UDIntValue);
            Assert.AreEqual(expected.ULongValue, current.ULongValue);

            Assert.AreEqual(expected.FloatValue, current.FloatValue);
            Assert.AreEqual(expected.DoubleValue, current.DoubleValue);
            Assert.AreEqual(expected.TimeValue, current.TimeValue);
            Assert.AreEqual(expected.TimeOfDay, current.TimeOfDay);
            Assert.AreEqual(expected.LTimeValue, current.LTimeValue);
            Assert.AreEqual(expected.DateValue, current.DateValue);
            Assert.AreEqual(expected.DateAndTimeValue, current.DateAndTimeValue);

            Assert.AreEqual(expected.StringValue, current.StringValue);
            Assert.AreEqual(expected.WStringValue, current.WStringValue);

            CollectionAssert.AreEqual(expected.IntArray, current.IntArray);
            CollectionAssert.AreEqual(expected.MultiDimensionArray, current.MultiDimensionArray);

            for (var i = 0; i < expected.ComplexArray.Length - 1; i++)
            {
                Assert.AreEqual(expected.ComplexArray[i].ByteValue, current.ComplexArray[i].ByteValue);
                Assert.AreEqual(expected.ComplexArray[i].WordValue, current.ComplexArray[i].WordValue);
                Assert.AreEqual(expected.ComplexArray[i].DWordValue, current.ComplexArray[i].DWordValue);
                Assert.AreEqual(expected.ComplexArray[i].LWordValue, current.ComplexArray[i].LWordValue);
            }

            for (var i = 0; i < expected.MultiDimensionComplexArray.GetLength(0); i++)
            {
                for (var j = 0; j < expected.MultiDimensionComplexArray.GetLength(1); j++)
                {
                    for (var k = 0; k < expected.MultiDimensionComplexArray.GetLength(2); k++)
                    {
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].ByteValue, current.MultiDimensionComplexArray[i, j, k].ByteValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].WordValue, current.MultiDimensionComplexArray[i, j, k].WordValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].DWordValue, current.MultiDimensionComplexArray[i, j, k].DWordValue);
                        Assert.AreEqual(expected.MultiDimensionComplexArray[i, j, k].LWordValue, current.MultiDimensionComplexArray[i, j, k].LWordValue);
                    }
                }
            }

            Assert.AreEqual(expected.Nested.ByteValue, current.Nested.ByteValue);
            Assert.AreEqual(expected.Nested.WordValue, current.Nested.WordValue);
            Assert.AreEqual(expected.Nested.DWordValue, current.Nested.DWordValue);
            Assert.AreEqual(expected.Nested.LWordValue, current.Nested.LWordValue);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public Api")]
        public static void ObjectEquals(this Assert assert, object expectedValue, object value, string message = null)
        {
            if (expectedValue is System.Collections.ICollection expectedCollection
                && value is System.Collections.ICollection valueCollection)
            {
                CollectionAssert.AreEqual(expectedCollection, valueCollection, message);
            }
            else
            {
                Assert.AreEqual(expectedValue, value, message);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public Api")]
        public static void ObjectNotEquals(this Assert assert, object expectedValue, object value, string message = null)
        {
            if (expectedValue is System.Collections.ICollection expectedCollection
                && value is System.Collections.ICollection valueCollection)
            {
                CollectionAssert.AreNotEqual(expectedCollection, valueCollection, message);
            }
            else
            {
                Assert.AreNotEqual(expectedValue, value, message);
            }
        }
    }
}