using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class TypeConverterTests
{
    private enum TestEnum
    {
        Value0,
        Value1,
        Value2,
        Value3,
        Value4,
        Value5,
    }

    [TestMethod]
    public void ConvertConvertsObjectToRequestedType()
    {
        // Arrange
        var typeConverterMock = new Mock<TypeConverter>
        {
            CallBase = true,
        };
        var stringObject = "100";

        // Act
        var actual = typeConverterMock.Object.Convert<int>(stringObject);
        var actual2 = typeConverterMock.Object.Convert(stringObject, typeof(int));

        // Assert
        Assert.IsInstanceOfType(actual, typeof(int));
        Assert.AreEqual(100, actual);
        Assert.IsInstanceOfType(actual2, typeof(int));
        Assert.AreEqual(100, actual2);
    }

    [TestMethod]
    public void ConvertConvertsToEnum()
    {
        // Arrange
        var typeConverterMock = new Mock<TypeConverter>
        {
            CallBase = true,
        };

        var value = 4;

        // Act
        var actual = typeConverterMock.Object.Convert<TestEnum>(value);
        var actual2 = typeConverterMock.Object.Convert(value, typeof(TestEnum));

        // Assert
        Assert.AreEqual(TestEnum.Value4, actual);
        Assert.IsInstanceOfType(actual, typeof(TestEnum));
        Assert.AreEqual(TestEnum.Value4, actual2);
        Assert.IsInstanceOfType(actual2, typeof(TestEnum));
    }

    [TestMethod]
    public void ConvertReturnsSourceObjectIfAlreadyTheRightType()
    {
        // Arrange
        var typeConverterMock = new Mock<TypeConverter>
        {
            CallBase = true,
        };
        object expected = new GenericParameterHelper();

        // Act
        var actual = typeConverterMock.Object.Convert<GenericParameterHelper>(expected);
        var actual2 = typeConverterMock.Object.Convert(expected, typeof(GenericParameterHelper));

        // Assert
        Assert.IsInstanceOfType(actual, typeof(GenericParameterHelper));
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected, actual2);
    }
}