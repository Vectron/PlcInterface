using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests
{
    internal static class AssertExtension
    {
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