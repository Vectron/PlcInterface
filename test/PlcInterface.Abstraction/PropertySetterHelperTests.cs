using System.Reflection;
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
        _ = Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            var propertySetterHelper = new PropertySetterHelper(constructor);
            var type = propertySetterHelper.GetType();
        });
    }
}
