﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestUtilities;

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

    private static TypeConverter Converter => new Mock<TypeConverter> { CallBase = true, }.Object;

    [TestMethod]
    public void ConvertConvertsDateTimeToDateTimeOffset()
    {
        // Arrange
        var typeConverter = Converter;
        var dateTime = DateTime.Now;

        // Act
        var actual = typeConverter.Convert<DateTimeOffset>(dateTime);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(DateTimeOffset));
        Assert.AreEqual(new DateTimeOffset(dateTime), actual);
    }

    [TestMethod]
    public void ConvertConvertsDynamicArrayToArray()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = new[]
        {
            new NestedType() { IntValue = 2 },
            new NestedType() { IntValue = 3 },
            new NestedType() { IntValue = 4 },
            new NestedType() { IntValue = 5 },
            new NestedType() { IntValue = 6 },
        };
        var source = new DynamicObject[]
        {
            expected[0].GetDynamicObjectMock(),
            expected[1].GetDynamicObjectMock(),
            expected[2].GetDynamicObjectMock(),
            expected[3].GetDynamicObjectMock(),
            expected[4].GetDynamicObjectMock(),
        };

        // Act
        var actual = typeConverter.Convert<NestedType[]>(source);

        // Assert
        Assert.AreEqual(expected[0].IntValue, actual[0].IntValue);
        Assert.AreEqual(expected[1].IntValue, actual[1].IntValue);
        Assert.AreEqual(expected[2].IntValue, actual[2].IntValue);
        Assert.AreEqual(expected[3].IntValue, actual[3].IntValue);
        Assert.AreEqual(expected[4].IntValue, actual[4].IntValue);
    }

    [TestMethod]
    public void ConvertConvertsDynamicObjectToType()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = TestType.Instance;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(TestType.IntValue), nameof(TestType.IntArray), nameof(TestType.SubType) });
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result)).Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
        {
            value = binder.Name switch
            {
                nameof(TestType.IntValue) => expected.IntValue,
                nameof(TestType.IntArray) => expected.IntArray,
                _ => expected.SubType.GetDynamicObjectMock(),
            };

            return value != null;
        }));

        // Act
        var actual = typeConverter.Convert<TestType>(sourceMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(TestType));
        Assert.AreEqual(expected.IntValue, actual.IntValue);
        CollectionAssert.AreEqual(expected.IntArray, actual.IntArray);

        Assert.IsNotNull(actual.SubType);
        Assert.AreEqual(expected.SubType.IntValue, actual.SubType.IntValue);
    }

    [TestMethod]
    public void ConvertConvertsExpandoToRequestedType()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = TestType.Instance;
        var source = new ExpandoObject() as IDictionary<string, object>;
        source.Add(nameof(TestType.IntValue), expected.IntValue);
        source.Add(nameof(TestType.IntArray), expected.IntArray);
        source.Add(nameof(TestType.SubType), expected.SubType.GetDynamicObjectMock());
        var result = new object();

        // Act
        var actual = typeConverter.Convert<TestType>(source);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(TestType));
        Assert.AreEqual(expected.IntValue, actual.IntValue);
        CollectionAssert.AreEqual(expected.IntArray, actual.IntArray);

        Assert.IsNotNull(actual.SubType);
        Assert.AreEqual(expected.SubType.IntValue, actual.SubType.IntValue);
    }

    [TestMethod]
    public void ConvertConvertsStringToRequestedInt()
    {
        // Arrange
        var typeConverter = Converter;
        var stringObject = "100";

        // Act
        var actual = typeConverter.Convert<int>(stringObject);
        var actual2 = typeConverter.Convert(stringObject, typeof(int));

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
        var typeConverter = Converter;
        var value = 4;

        // Act
        var actual = typeConverter.Convert<TestEnum>(value);
        var actual2 = typeConverter.Convert(value, typeof(TestEnum));

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
        var typeConverter = Converter;
        object expected = new GenericParameterHelper();

        // Act
        var actual = typeConverter.Convert<GenericParameterHelper>(expected);
        var actual2 = typeConverter.Convert(expected, typeof(GenericParameterHelper));

        // Assert
        Assert.IsInstanceOfType(actual, typeof(GenericParameterHelper));
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected, actual2);
    }

    [TestMethod]
    public void ConvertSupportsRecordTypes()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = TestRecordType.Instance;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(new[]
            {
                nameof(TestRecordType.IntValue),
                nameof(TestRecordType.IntArray),
                nameof(TestRecordType.SubType),
            });
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
            .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
        {
            value = binder.Name switch
            {
                nameof(TestRecordType.IntValue) => expected.IntValue,
                nameof(TestRecordType.IntArray) => expected.IntArray,
                nameof(TestRecordType.SubType) => expected.SubType.GetDynamicObjectMock(),
                _ => throw new InvalidOperationException("Unknown member"),
            };

            return value != null;
        }));

        // Act
        var actual = typeConverter.Convert<TestRecordType>(sourceMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(TestRecordType));
        Assert.AreEqual(expected.IntValue, actual.IntValue);
        CollectionAssert.AreEqual(expected.IntArray, actual.IntArray);

        Assert.IsNotNull(actual.SubType);
        Assert.AreEqual(expected.SubType.IntValue, actual.SubType.IntValue);
    }

    [TestMethod]
    public void ConvertThrowsInvalidOperationExceptionWhenDynamicObjectHasNoValue()
    {
        // Arrange
        var typeConverter = Converter;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(TestType.IntValue) });
        var value = new object();
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out value)).Returns(false);

        // Act
        _ = Assert.ThrowsException<InvalidOperationException>(() => typeConverter.Convert<TestType>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsInvalidOperationExceptionWhenPropertyIsNotFound()
    {
        // Arrange
        var typeConverter = Converter;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "IntValue2" });

        // Act
        _ = Assert.ThrowsException<InvalidOperationException>(() => typeConverter.Convert<TestType>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsInvalidOperationExceptionWhenPropertyIsReadOnly()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = TestType.Instance;
        var sourceMock = new Mock<DynamicObject>();
        var expandoSource = new ExpandoObject() as IDictionary<string, object>;
        expandoSource.Add(nameof(NoSetterTestType.IntValue), 10);
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(new[] { nameof(NoSetterTestType.IntValue) });

        // Act Assert
        _ = Assert.ThrowsException<InvalidOperationException>(() => typeConverter.Convert<NoSetterTestType>(sourceMock.Object));
        _ = Assert.ThrowsException<InvalidOperationException>(() => typeConverter.Convert<NoSetterTestType>(expandoSource));
    }

    [TestMethod]
    public void ConvertThrowsNotSupportedExceptionWhenTargetIsArrayButValueIsNotIDynamicValue()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();

        // Act Assert
        _ = Assert.ThrowsException<NotSupportedException>(() => typeConverter.Convert<int[]>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsSymbolExceptionWhenMemberIsNull()
    {
        // Arrange
        var typeConverter = Converter;
        var expandoSource = new ExpandoObject() as IDictionary<string, object>;
        expandoSource.Add(nameof(TestType.IntValue), null!);
        var dynamicObjectsourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = dynamicObjectsourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(new[]
            {
                nameof(TestType.IntValue),
                nameof(TestType.IntArray),
                nameof(TestType.SubType),
            });
        _ = dynamicObjectsourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
            .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
            {
                value = binder.Name switch
                {
                    _ => null!,
                };

                return true;
            }));
        var dynamicObjectsource = dynamicObjectsourceMock.Object;

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<TestType>(dynamicObjectsource));
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<TestType>(expandoSource));
    }

    [TestMethod]
    public void ConvertThrowsSymbolExceptionWhenTypeCantBeConverted()
    {
        // Arrange
        var typeConverter = Converter;
        var source = TestType.Instance;

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<int>(source));
    }

    private sealed class NestedType
    {
        public int IntValue
        {
            get;
            set;
        }

        internal DynamicObject GetDynamicObjectMock()
        {
            var sourceMock = new Mock<DynamicObject>();
            var result = new object();
            _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(IntValue) });
            _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
                .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
                {
                    value = binder.Name switch
                    {
                        _ => IntValue,
                    };

                    return value != null;
                }));

            return sourceMock.Object;
        }
    }

    private sealed class NoSetterTestType
    {
        public int IntValue
        {
            get;
        }
    }

    private sealed class TestType
    {
        public static readonly TestType Instance = new()
        {
            IntValue = 5,
            IntArray = new[] { 6, 7, 8, 9 },
            SubType = new NestedType()
            {
                IntValue = 12,
            },
        };

        public TestType()
        {
            IntArray = Array.Empty<int>();
            SubType = new NestedType();
        }

        public int[] IntArray
        {
            get;
            set;
        }

        public int IntValue
        {
            get;
            set;
        }

        public NestedType SubType
        {
            get;
            set;
        }
    }

    private sealed record TestRecordType(int IntValue, int[] IntArray, NestedRecordType SubType)
    {
        public static readonly TestRecordType Instance = new(5, new[] { 6, 7, 8, 9 }, new NestedRecordType(12));
    }

    private sealed record NestedRecordType(int IntValue)
    {
        internal DynamicObject GetDynamicObjectMock()
        {
            var sourceMock = new Mock<DynamicObject>();
            var result = new object();
            _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(IntValue) });
            _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
                .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
                {
                    value = binder.Name switch
                    {
                        nameof(IntValue) => IntValue,
                        _ => throw new InvalidOperationException("Unknown member"),
                    };

                    return value != null;
                }));

            return sourceMock.Object;
        }
    }
}