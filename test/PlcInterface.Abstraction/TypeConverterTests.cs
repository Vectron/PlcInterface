using System;
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

    private static IEnumerable<object[]> CanConvertStringsToBaseTypeData
    {
        get
        {
            yield return new object[]
            {
                "09-04-1987 06:10:50",
                typeof(DateTime),
                new DateTime(1987, 04, 09, 06, 10, 50),
            };
        }
    }

    private static TypeConverter Converter => new Mock<TypeConverter> { CallBase = true, }.Object;

    [DataTestMethod]
    [DataRow("false", typeof(bool), false)]
    [DataRow("true", typeof(bool), true)]
    [DataRow("0", typeof(bool), false)]
    [DataRow("1", typeof(bool), true)]
    [DataRow("127", typeof(sbyte), sbyte.MaxValue)]
    [DataRow("255", typeof(byte), byte.MaxValue)]
    [DataRow("32767", typeof(short), short.MaxValue)]
    [DataRow("65535", typeof(ushort), ushort.MaxValue)]
    [DataRow("2147483647", typeof(int), int.MaxValue)]
    [DataRow("4294967295", typeof(uint), uint.MaxValue)]
    [DataRow("9223372036854775807", typeof(long), long.MaxValue)]
    [DataRow("18446744073709551615", typeof(ulong), ulong.MaxValue)]
    [DataRow("1.1", typeof(float), 1.1f)]
    [DataRow("1.1", typeof(double), 1.1)]
    [DataRow("This is text", typeof(string), "This is text")]
    [DataRow("2", typeof(TestEnum), TestEnum.Value2)]
    [DynamicData(nameof(CanConvertStringsToBaseTypeData))]
    public void CanConvertStringsToBaseType(string value, Type targetType, object expectedValue)
    {
        // Arrange
        var typeConverter = Converter;

        // Act
        var type = typeConverter.Convert(value, targetType);

        // Assert
        Assert.IsInstanceOfType(type, targetType);
    }

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
    public void ConvertSupportsRecordValueTypes()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = TestRecordStructType.Instance;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(new[]
            {
                nameof(TestRecordStructType.IntValue),
                nameof(TestRecordStructType.IntArray),
                nameof(TestRecordStructType.SubType),
            });
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
            .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
            {
                value = binder.Name switch
                {
                    nameof(TestRecordStructType.IntValue) => expected.IntValue,
                    nameof(TestRecordStructType.IntArray) => expected.IntArray,
                    nameof(TestRecordStructType.SubType) => expected.SubType.GetDynamicObjectMock(),
                    _ => throw new InvalidOperationException("Unknown member"),
                };

                return value != null;
            }));

        // Act
        var actual = typeConverter.Convert<TestRecordStructType>(sourceMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(TestRecordStructType));
        Assert.AreEqual(expected.IntValue, actual.IntValue);
        CollectionAssert.AreEqual(expected.IntArray, actual.IntArray);

        Assert.IsNotNull(actual.SubType);
        Assert.AreEqual(expected.SubType.IntValue, actual.SubType.IntValue);
    }

    [TestMethod]
    public void ConvertSupportsValueTypes()
    {
        // Arrange
        var typeConverter = Converter;
        var expected = TestValueType.Instance;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(new[]
            {
                nameof(TestValueType.IntValue),
                nameof(TestValueType.IntArray),
                nameof(TestValueType.SubType),
            });
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
            .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
            {
                value = binder.Name switch
                {
                    nameof(TestValueType.IntValue) => expected.IntValue,
                    nameof(TestValueType.IntArray) => expected.IntArray,
                    nameof(TestValueType.SubType) => expected.SubType.GetDynamicObjectMock(),
                    _ => throw new InvalidOperationException("Unknown member"),
                };

                return value != null;
            }));

        // Act
        var actual = typeConverter.Convert<TestValueType>(sourceMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(TestValueType));
        Assert.AreEqual(expected.IntValue, actual.IntValue);
        CollectionAssert.AreEqual(expected.IntArray, actual.IntArray);

        Assert.IsNotNull(actual.SubType);
        Assert.AreEqual(expected.SubType.IntValue, actual.SubType.IntValue);
    }

    [TestMethod]
    public void ConvertSymbolExceptionWhenPropertyIsNotFound()
    {
        // Arrange
        var typeConverter = Converter;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { "IntValue2" });

        // Act
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<TestType>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsNotSupportedExceptionWhenPropertyIsReadOnly()
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
        _ = Assert.ThrowsException<NotSupportedException>(() => typeConverter.Convert<NoSetterTestType>(sourceMock.Object));
        _ = Assert.ThrowsException<NotSupportedException>(() => typeConverter.Convert<NoSetterTestType>(expandoSource));
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
    public void ConvertThrowsSymbolExceptionWhenDynamicObjectHasNoValue()
    {
        // Arrange
        var typeConverter = Converter;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(TestType.IntValue) });
        var value = new object();
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out value)).Returns(false);

        // Act
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<TestType>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsSymbolExceptionWhenMemberIsNull()
    {
        // Arrange
        var typeConverter = Converter;
        var expandoSource = new ExpandoObject() as IDictionary<string, object>;
        expandoSource.Add(nameof(TestType.IntValue), null!);
        expandoSource.Add(nameof(TestType.IntArray), null!);
        expandoSource.Add(nameof(TestType.SubType), null!);
        var dynamicObjectSourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = dynamicObjectSourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(new[]
            {
                nameof(TestType.IntValue),
                nameof(TestType.IntArray),
                nameof(TestType.SubType),
            });
        _ = dynamicObjectSourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
            .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
            {
                value = binder.Name switch
                {
                    _ => null!,
                };

                return true;
            }));
        var dynamicObjectSource = dynamicObjectSourceMock.Object;

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<TestType>(dynamicObjectSource));
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

    private record struct TestRecordStructType(int IntValue, int[] IntArray, NestedRecordStructType SubType)
    {
        public static readonly TestRecordStructType Instance = new(5, new[] { 6, 7, 8, 9 }, new NestedRecordStructType(12));
    }

    private record struct NestedRecordStructType(int IntValue)
    {
        internal readonly DynamicObject GetDynamicObjectMock()
        {
            var intValue = IntValue;
            var sourceMock = new Mock<DynamicObject>();
            var result = new object();
            _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(IntValue) });
            _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
                .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
                {
                    value = binder.Name switch
                    {
                        nameof(IntValue) => intValue,
                        _ => throw new InvalidOperationException("Unknown member"),
                    };

                    return value != null;
                }));

            return sourceMock.Object;
        }
    }

    private struct NestedValueType
    {
        public int IntValue
        {
            get;
            set;
        }

        internal readonly DynamicObject GetDynamicObjectMock()
        {
            var intValue = IntValue;
            var sourceMock = new Mock<DynamicObject>();
            var result = new object();
            _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(IntValue) });
            _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
                .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
                {
                    value = binder.Name switch
                    {
                        _ => intValue,
                    };

                    return value != null;
                }));

            return sourceMock.Object;
        }
    }

    private struct TestValueType
    {
        public static readonly TestValueType Instance = new()
        {
            IntValue = 5,
            IntArray = new[] { 6, 7, 8, 9 },
            SubType = new NestedValueType()
            {
                IntValue = 12,
            },
        };

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

        public NestedValueType SubType
        {
            get;
            set;
        }
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