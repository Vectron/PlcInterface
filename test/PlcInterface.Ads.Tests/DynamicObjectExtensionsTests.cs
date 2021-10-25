using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlcInterface.Ads.Extensions;
using TestUtilities;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class DynamicObjectExtensionsTests
{
    [TestMethod]
    public void CleanDynamicConvertsDynamicPrimitiveArrayToComplexArray()
    {
        // Arrange
        var nestedValue = new[] { 1, 2, 3, 4, 5 };
        var nestedDummy = nestedValue.Cast<object>();
        var nestedArrayMock = new Mock<DynamicObject>();
        var iDynamicValueNestedArrayMock = nestedArrayMock.As<IDynamicValue>();
        _ = iDynamicValueNestedArrayMock.SetupGet(x => x.DataType).Returns(ComplexArrayType(new int[] { 5 }));
        _ = iDynamicValueNestedArrayMock.Setup(x => x.TryGetArrayElementValues(out nestedDummy)).Returns(true);

        var values = new object[] { 44, 55, 66, 77, nestedArrayMock.Object };
        var dummy = values.Cast<object>();
        var dynamicObjectMock = new Mock<DynamicObject>();
        var iDynamicValueDynamicObjectMock = dynamicObjectMock.As<IDynamicValue>();
        _ = iDynamicValueDynamicObjectMock.SetupGet(x => x.DataType).Returns(ComplexArrayType(new int[] { 5 }));
        _ = iDynamicValueDynamicObjectMock.Setup(x => x.TryGetArrayElementValues(out dummy)).Returns(true);

        // Act
        var expando = dynamicObjectMock.Object.CleanDynamic();

        // Assert
        Assert.IsInstanceOfType(expando, typeof(Array));
        Assert.AreEqual(values[0], expando[0]);
        Assert.AreEqual(values[1], expando[1]);
        Assert.AreEqual(values[2], expando[2]);
        Assert.AreEqual(values[3], expando[3]);
        CollectionAssert.AreEqual(nestedValue, expando[4]);
    }

    [TestMethod]
    public void CleanDynamicConvertsDynamicPrimitiveArrayToComplexArrayLeavesUnusedIndicesAsDefault()
    {
        // Arrange
        var nestedValue = new[] { 1, 2, 3, 4, 5 };
        var nestedDummy = nestedValue.Cast<object>();
        var nestedArrayMock = new Mock<DynamicObject>();
        var iDynamicValueNestedArrayMock = nestedArrayMock.As<IDynamicValue>();
        _ = iDynamicValueNestedArrayMock.SetupGet(x => x.DataType).Returns(ComplexArrayType(new int[] { 5 }));
        _ = iDynamicValueNestedArrayMock.Setup(x => x.TryGetArrayElementValues(out nestedDummy)).Returns(true);

        var values = new object[] { 44, 55, 66, 77, nestedArrayMock.Object };
        var dummy = values.Cast<object>();
        var dynamicObjectMock = new Mock<DynamicObject>();
        var iDynamicValueDynamicObjectMock = dynamicObjectMock.As<IDynamicValue>();
        _ = iDynamicValueDynamicObjectMock.SetupGet(x => x.DataType).Returns(ComplexArrayType(new int[] { 15 }));
        _ = iDynamicValueDynamicObjectMock.Setup(x => x.TryGetArrayElementValues(out dummy)).Returns(true);

        // Act
        var expando = dynamicObjectMock.Object.CleanDynamic();

        // Assert
        Assert.IsInstanceOfType(expando, typeof(Array));
        Assert.AreEqual(15, expando.Length);
        Assert.AreEqual(values[0], expando[0]);
        Assert.AreEqual(values[1], expando[1]);
        Assert.AreEqual(values[2], expando[2]);
        Assert.AreEqual(values[3], expando[3]);
        CollectionAssert.AreEqual(nestedValue, expando[4]);
        for (var i = 5; i < expando.Length; i++)
        {
            Assert.IsNull(expando[i]);
        }
    }

    [TestMethod]
    public void CleanDynamicConvertsDynamicPrimitiveArrayToPrimitiveArray()
    {
        // Arrange
        var values = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var dummy = values.Cast<object>();
        var dynamicObjectMock = new Mock<DynamicObject>();
        var iDynamicValueDynamicObjectMock = dynamicObjectMock.As<IDynamicValue>();
        _ = iDynamicValueDynamicObjectMock.SetupGet(x => x.DataType).Returns(IntArrayType(new int[] { 9 }));
        _ = iDynamicValueDynamicObjectMock.Setup(x => x.TryGetArrayElementValues(out dummy)).Returns(true);

        // Act
        var expando = dynamicObjectMock.Object.CleanDynamic();

        // Assert
        Assert.IsInstanceOfType(expando, typeof(Array));
        CollectionAssert.AreEqual(values, expando);
    }

    [TestMethod]
    public void CleanDynamicConvertsDynamicPrimitiveArrayToPrimitiveArrayLeavesUnusedIndicesAsDefault()
    {
        // Arrange
        var values = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var dummy = values.Cast<object>();
        var dynamicObjectMock = new Mock<DynamicObject>();
        var iDynamicValueDynamicObjectMock = dynamicObjectMock.As<IDynamicValue>();
        _ = iDynamicValueDynamicObjectMock.SetupGet(x => x.DataType).Returns(IntArrayType(new int[] { 15 }));
        _ = iDynamicValueDynamicObjectMock.Setup(x => x.TryGetArrayElementValues(out dummy)).Returns(true);

        // Act
        var expando = dynamicObjectMock.Object.CleanDynamic();

        // Assert
        Assert.IsInstanceOfType(expando, typeof(Array));
        Assert.AreEqual(15, expando.Length);
        for (var i = 0; i < expando.Length; i++)
        {
            if (i < 9)
            {
                Assert.AreEqual(values[i], expando[i]);
            }
            else
            {
                Assert.AreEqual(default(int), expando[i]);
            }
        }
    }

    [TestMethod]
    public void CleanDynamicConvertsStructtypeToExpando()
    {
        // Arrange
        object? dummy = null;
        var dynamicObjectMock = new Mock<DynamicObject>();
        _ = dynamicObjectMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "Property1", "Property2", "Property3", "Property4", "Property5" });
        var iDynamicValueDynamicObjectMock = dynamicObjectMock.As<IDynamicValue>();
        _ = iDynamicValueDynamicObjectMock.SetupGet(x => x.DataType).Returns(Mock.Of<IStructType>());
        _ = iDynamicValueDynamicObjectMock.Setup(x => x.TryGetMemberValue(It.IsAny<string>(), out dummy))
            .Callback(new MockDelegates.OutAction<string, object?>((string name, out object? value)
            => value = name switch
            {
                "Property1" => 255,
                "Property2" => "test",
                "Property3" => false,
                "Property4" => 10L,
                _ => null,
            }))
            .Returns(true);

        // Act
        var expando = dynamicObjectMock.Object.CleanDynamic();

        // Assert
        Assert.AreEqual(255, expando.Property1);
        Assert.AreEqual("test", expando.Property2);
        Assert.AreEqual(false, expando.Property3);
        Assert.AreEqual(10L, expando.Property4);
        Assert.IsNull(expando.Property5);
    }

    [TestMethod]
    public void CleanDynamicStructCanHandleNestedStruct()
    {
        // Arrange
        object? dummy = null;
        var nestedDynamicMock = new Mock<DynamicObject>();
        _ = nestedDynamicMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "Nested1", "Nested2", "Nested3" });
        var nestedDynamicIDynamicValue = nestedDynamicMock.As<IDynamicValue>();
        _ = nestedDynamicIDynamicValue.SetupGet(x => x.DataType).Returns(Mock.Of<IStructType>());
        _ = nestedDynamicIDynamicValue.Setup(x => x.TryGetMemberValue(It.IsAny<string>(), out dummy))
            .Callback(new MockDelegates.OutAction<string, object?>((string name, out object? value)
            => value = name switch
            {
                "Nested1" => 255,
                "Nested2" => "test",
                _ => null,
            }))
            .Returns(true);
        object? nested = nestedDynamicMock.Object;
        var firstDynamic = new Mock<DynamicObject>();
        _ = firstDynamic.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "Property1" });
        var firstIDynamicValue = firstDynamic.As<IDynamicValue>();
        _ = firstIDynamicValue.SetupGet(x => x.DataType).Returns(Mock.Of<IStructType>());
        _ = firstIDynamicValue.Setup(x => x.TryGetMemberValue(It.IsAny<string>(), out nested)).Returns(true);

        // Act
        var expando = firstDynamic.Object.CleanDynamic();

        // Assert
        Assert.AreEqual(255, expando.Property1.Nested1);
        Assert.AreEqual("test", expando.Property1.Nested2);
        Assert.IsNull(expando.Property1.Nested3);
    }

    [TestMethod]
    public void CleanDynamicStructConvertsNestedPlcOpenTypes()
    {
        // Arrange
        var dateMock = new DateTime(2021, 08, 13);
        var timeMock = new TimeSpan(2, 5, 20, 10, 999);
        var lTimeMock = new TimeSpan(111111111);

        object? dummy = null;
        var firstDynamic = new Mock<DynamicObject>();
        _ = firstDynamic.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "Property1", "Property2", "Property3", "Property4" });
        var firstIDynamicValue = firstDynamic.As<IDynamicValue>();
        _ = firstIDynamicValue.SetupGet(x => x.DataType).Returns(Mock.Of<IStructType>());
        _ = firstIDynamicValue.Setup(x => x.TryGetMemberValue(It.IsAny<string>(), out dummy))
            .Returns(new MockDelegates.OutFunction<string, object?, bool>((string name, out object? value) =>
            {
                value = name switch
                {
                    "Property1" => new TwinCAT.PlcOpen.DATE(dateMock),
                    "Property2" => new TwinCAT.PlcOpen.TIME(timeMock),
                    "Property3" => new TwinCAT.PlcOpen.LTIME(lTimeMock),
                    _ => 0,
                };

                return value != null;
            }));

        // Act
        var expando = firstDynamic.Object.CleanDynamic();

        // Assert
        Assert.AreEqual(dateMock, expando.Property1);
        Assert.AreEqual(timeMock, expando.Property2);
        Assert.AreEqual(lTimeMock, expando.Property3);
        Assert.AreEqual(0, expando.Property4);
    }

    [TestMethod]
    public void CleanDynamicStructSkipsUnknownMembers()
    {
        // Arrange
        object? dummy = null;
        var firstDynamic = new Mock<DynamicObject>();
        _ = firstDynamic.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "Property1", "Property2", "Property3", "Property4" });
        var firstIDynamicValue = firstDynamic.As<IDynamicValue>();
        _ = firstIDynamicValue.SetupGet(x => x.DataType).Returns(Mock.Of<IStructType>());
        _ = firstIDynamicValue.Setup(x => x.TryGetMemberValue(It.IsAny<string>(), out dummy))
            .Returns(false);

        // Act
        var expando = firstDynamic.Object.CleanDynamic() as IDictionary<string, object>;

        // Assert
        Assert.IsNotNull(expando);
        Assert.IsFalse(expando.ContainsKey("Property1"));
        Assert.IsFalse(expando.ContainsKey("Property2"));
        Assert.IsFalse(expando.ContainsKey("Property3"));
        Assert.IsFalse(expando.ContainsKey("Property4"));
    }

    [TestMethod]
    public void CleanDynamicThrowsNotSupportedExceptionIfNotAConvertableType()
    {
        // Arrange
        var dynamicObjectMock = Mock.Of<DynamicObject>();
        var iDynamicValueMock = new Mock<DynamicObject>();
        var iDynamicValueDynamicObjectMock = iDynamicValueMock.As<IDynamicValue>();

        // Act
        // Assert
        _ = Assert.ThrowsException<NotSupportedException>(dynamicObjectMock.CleanDynamic);
        _ = Assert.ThrowsException<NotSupportedException>(iDynamicValueMock.Object.CleanDynamic);
    }

    private static ArrayType ComplexArrayType(int[] rank)
    {
        var memberCollection = new MemberCollection()
            {
                new Member("field1", new PrimitiveType("int", typeof(int))),
                new Member("field2", new PrimitiveType("int", typeof(int))),
                new Member("field3", new PrimitiveType("int", typeof(int))),
                new Member("field4", new PrimitiveType("int", typeof(int))),
                new Member("field5", IntArrayType(new[] { 3 })),
            };

        var complexType = new StructType("Data", null, memberCollection);
        var dimensionCollection = new DimensionCollection(rank);
        var arrayType = new ArrayType(complexType, dimensionCollection);
        return arrayType;
    }

    private static ArrayType IntArrayType(int[] rank)
    {
        var intType = new PrimitiveType("int", typeof(int));
        var dimensionCollection = new DimensionCollection(rank);
        var arrayType = new ArrayType(intType, dimensionCollection);
        return arrayType;
    }
}