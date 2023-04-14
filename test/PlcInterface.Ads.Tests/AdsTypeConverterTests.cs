using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestUtilities;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class AdsTypeConverterTests
{
    [TestMethod]
    public void ConvertCanConvertADynamicIValueSymbol()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var dummy = expected.Cast<object>();
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType(new int[] { 9 }));
        _ = dynamicValue.Setup(x => x.TryGetArrayElementValues(out dummy)).Returns(true);

        // Act
        var actual = typeConverter.Convert(sourceMock.Object, Mock.Of<IValueSymbol>());

        // Assert
        CollectionAssert.AreEqual(expected, (ICollection)actual);
    }

    [TestMethod]
    public void ConvertChangesPlcOpenTypesToDotNetTypes()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new TestType3()
        {
            Date = new DateTime(2021, 08, 13),
            Time = new TimeSpan(2, 5, 20, 10, 999),
            LTime = new TimeSpan(111111111),
            DateTimeOffset = new DateTimeOffset(new DateTime(2021, 08, 13, 00, 00, 00)),
        };
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(TestType3.Date), nameof(TestType3.Time), nameof(TestType3.LTime), nameof(TestType3.DateTimeOffset) });
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result)).Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
        {
            value = binder.Name switch
            {
                nameof(TestType3.Date) => new TwinCAT.PlcOpen.DATE(expected.Date),
                nameof(TestType3.Time) => new TwinCAT.PlcOpen.TIME(expected.Time),
                nameof(TestType3.LTime) => new TwinCAT.PlcOpen.LTIME(expected.LTime),
                _ => new TwinCAT.PlcOpen.DATE(expected.DateTimeOffset),
            };

            return value != null;
        }));

        // Act
        var actual = typeConverter.Convert<TestType3>(sourceMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(TestType3));
        Assert.AreEqual(expected.Date, actual.Date);
        Assert.AreEqual(expected.Time, actual.Time);
        Assert.AreEqual(expected.LTime, actual.LTime);
        Assert.AreEqual(expected.DateTimeOffset, actual.DateTimeOffset);
    }

    [TestMethod]
    public void ConvertConvertsDateTimeToDateTimeOffset()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var dateTime = DateTime.Now;

        // Act
        var actual = typeConverter.Convert<DateTimeOffset>(dateTime);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(DateTimeOffset));
        Assert.AreEqual(new DateTimeOffset(dateTime), actual);
    }

    [TestMethod]
    public void ConvertConvertsDateTimeToDateTimeOfset()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Category).Returns(DataTypeCategory.Struct);
        var source = DateTime.UtcNow;
        var expected = new DateTimeOffset(source);

        // Act
        var actual = typeConverter.Convert(source, valueSymbolMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(DateTimeOffset));
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ConvertConvertsDynamicArrayToArray()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
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
        var typeConverter = new AdsTypeConverter();
        var expected = new TestType()
        {
            IntValue = 5,
            IntArray = new[] { 6, 7, 8, 9 },
            SubType = new NestedType()
            {
                IntValue = 12,
            },
        };
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
    public void ConvertConvertsObjectToRequestedType()
    {
        // Arrange
        var typeConverterMock = new AdsTypeConverter();
        var stringObject = "100";

        // Act
        var actual = typeConverterMock.Convert<int>(stringObject);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(int));
        Assert.AreEqual(100, actual);
    }

    [TestMethod]
    public void ConvertConvertsShortToIntWhenTypeIsEnum()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Category).Returns(DataTypeCategory.Enum);

        // Act
        var actual = typeConverter.Convert((short)1, valueSymbolMock.Object);
        var actual2 = typeConverter.Convert(1L, valueSymbolMock.Object);

        // Assert
        Assert.IsInstanceOfType(actual, typeof(int));
        Assert.AreEqual(1, actual);

        Assert.IsInstanceOfType(actual2, typeof(long));
        Assert.AreEqual(1L, actual2);
    }

    [TestMethod]
    public void ConvertConvertsToComplexArray()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { new NestedType() { IntValue = 2 }, new NestedType() { IntValue = 3 } };
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(ComplexArrayType(new[] { 2 }));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy)).Returns(new MockDelegates.OutFunction<int[], object?, bool>((int[] indices, out object? value) =>
        {
            value = ((NestedType)expected.GetValue(indices)!).GetDynamicObjectMock();
            return true;
        }));

        // Act
        var actual = typeConverter.Convert<NestedType[]>(sourceMock.Object);

        // Assert
        Assert.AreEqual(expected[0].IntValue, actual[0].IntValue);
        Assert.AreEqual(expected[1].IntValue, actual[1].IntValue);
    }

    [TestMethod]
    public void ConvertConvertsToPrimativeArray()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType(new[] { 9 }));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy)).Returns(new MockDelegates.OutFunction<int[], object?, bool>((int[] indices, out object? value) =>
        {
            value = expected.GetValue(indices);
            return true;
        }));

        // Act
        var actual = typeConverter.Convert<int[]>(sourceMock.Object);

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ConvertThrowsInvalidOperationExceptionWhenDynamicObjectHasNoValue()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
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
        var typeConverter = new AdsTypeConverter();
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
        var typeConverter = new AdsTypeConverter();
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns(new[] { nameof(TestType2.IntValue) });

        // Act
        _ = Assert.ThrowsException<InvalidOperationException>(() => typeConverter.Convert<TestType2>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsNotSupportedExceptionWhenTargetIsArrayButValueIsNotIDynamicValue()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();

        // Act Assert
        _ = Assert.ThrowsException<NotSupportedException>(() => typeConverter.Convert<int[]>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsNotSupportedExceptionWhenTargetIsArrayButValueIsNotOfIArrayType()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(Mock.Of<IDataType>());

        // Act Assert
        _ = Assert.ThrowsException<NotSupportedException>(() => typeConverter.Convert<int[]>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsSymbolExceptionWhenElementTypeIsWrong()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType(new[] { expected.Length }));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy)).Returns(true);

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<int[]>(sourceMock.Object));
    }

    [TestMethod]
    public void ConvertThrowsSymbolExceptionWhenIndicesCantBeRead()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType(new[] { expected.Length }));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy)).Returns(false);

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<int[]>(sourceMock.Object));
    }

    private static ArrayType ComplexArrayType(int[] rank)
    {
        var memberCollection = new MemberCollection()
            {
                new Member(nameof(NestedType.IntValue), new PrimitiveType("int", typeof(int))),
            };
        var complexType = new StructType("NestedType", null, memberCollection);
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
            _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result)).Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
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

    private sealed class TestType
    {
        public TestType()
            => IntArray = Array.Empty<int>();

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

        public NestedType? SubType
        {
            get;
            set;
        }
    }

    private sealed class TestType2
    {
        public int IntValue
        {
            get;
        }
    }

    private sealed class TestType3
    {
        public DateTime Date
        {
            get;
            set;
        }

        public DateTimeOffset DateTimeOffset
        {
            get;
            set;
        }

        public TimeSpan LTime
        {
            get;
            set;
        }

        public TimeSpan Time
        {
            get;
            set;
        }
    }
}