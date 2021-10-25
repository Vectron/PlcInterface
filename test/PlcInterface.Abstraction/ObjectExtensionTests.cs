using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Extensions;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class ObjectExtensionTests
{
    [TestMethod]
    public void ThrowIfNullReturnsTheOriginalObjectIfNotNull()
    {
        // Arrange
        var expected = new GenericParameterHelper();

        // Act
        var actual = ObjectExtension.ThrowIfNull(expected);

        // Assert
        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void ThrowIfNullThrowsArgumentNullException()
    {
        // Arrange
        GenericParameterHelper? genericParameterHelper = null;

        // Act

        // Assert
        _ = Assert.ThrowsException<ArgumentNullException>(() => ObjectExtension.ThrowIfNull<object>(genericParameterHelper));
    }

    [TestMethod]
    public void YieldReturnsTheGivenItemInAIEnumerable()
    {
        // Arrange
        var data = 5;

        // Act
        var enumerable = data.Yield();

        // Assert
        Assert.AreEqual(1, enumerable.Count());
        Assert.AreEqual(data, enumerable.First());
    }
}