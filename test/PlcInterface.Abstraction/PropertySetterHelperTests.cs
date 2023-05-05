using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class PropertySetterHelperTests
{
    [TestMethod]
    public void WhenDeclaringTypeIsNullThrowsNotSupportedException()
    {
        // Arrange
        var constructor = Mock.Of<PropertyInfo>(x => x.DeclaringType == null);

        // Act Assert
        _ = Assert.ThrowsException<NotSupportedException>(() => new PropertySetterHelper(constructor));
    }
}