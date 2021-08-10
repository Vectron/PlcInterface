using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Abstraction.Tests
{
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
    }
}