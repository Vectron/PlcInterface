using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Extensions;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void GetIndicesReturnsTheValueFromString()
    {
        // Arrange
        var name = "ArrayObject[5,3]";
        var expected = new[] { 5, 3 };

        // Act
        var indeces1 = name.GetIndices();
        var indeces2 = name.AsSpan().GetIndices();

        // Assert
        CollectionAssert.AreEqual(expected, indeces1);
        CollectionAssert.AreEqual(indeces1, indeces2);
    }
}