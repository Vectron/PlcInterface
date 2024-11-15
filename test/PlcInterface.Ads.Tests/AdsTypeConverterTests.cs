using System.Collections;
using System.Dynamic;
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
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType([9]));
        _ = dynamicValue.Setup(x => x.TryGetArrayElementValues(out dummy)).Returns(value: true);

        // Act
        var actual = typeConverter.Convert(sourceMock.Object, Mock.Of<IValueSymbol>());

        // Assert
        Assert.IsInstanceOfType<ICollection>(actual);
        CollectionAssert.AreEqual(expected, (ICollection)actual);
    }

    [TestMethod]
    public void ConvertChangesPlcOpenTypesToDotNetTypes()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = TimesTestType.Default;
        var sourceMock = new Mock<DynamicObject>();
        var result = new object();
        _ = sourceMock.Setup(x => x.GetDynamicMemberNames())
            .Returns(
            [
                nameof(TimesTestType.Date),
                nameof(TimesTestType.Time),
                nameof(TimesTestType.LTime),
                nameof(TimesTestType.DateTimeOffset),
            ]);
        _ = sourceMock.Setup(x => x.TryGetMember(It.IsAny<GetMemberBinder>(), out result))
            .Returns(new MockDelegates.OutFunction<GetMemberBinder, object, bool>((GetMemberBinder binder, out object value) =>
            {
                value = binder.Name switch
                {
                    nameof(TimesTestType.Date) => new TwinCAT.PlcOpen.DATE(expected.Date),
                    nameof(TimesTestType.Time) => new TwinCAT.PlcOpen.TIME(expected.Time),
                    nameof(TimesTestType.LTime) => new TwinCAT.PlcOpen.LTIME(expected.LTime),
                    nameof(TimesTestType.DateTimeOffset) => new TwinCAT.PlcOpen.DATE(expected.DateTimeOffset),
                    _ => throw new NotSupportedException("Unknown type mapping."),
                };

                return value != null;
            }));

        // Act
        var actual = typeConverter.Convert<TimesTestType>(sourceMock.Object);

        // Assert
        Assert.IsInstanceOfType<TimesTestType>(actual);
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
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Category).Returns(DataTypeCategory.Struct);
        var source = DateTime.UtcNow;
        var expected = new DateTimeOffset(source);

        // Act
        var actual = typeConverter.Convert(source, valueSymbolMock.Object);

        // Assert
        Assert.IsInstanceOfType<DateTimeOffset>(actual);
        Assert.AreEqual(expected, actual);
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
        Assert.IsInstanceOfType<int>(actual);
        Assert.AreEqual(1, actual);

        Assert.IsInstanceOfType<long>(actual2);
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
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(ComplexArrayType([2]));
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
    public void ConvertConvertsToPrimitiveArray()
    {
        // Arrange
        var typeConverter = new AdsTypeConverter();
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var sourceMock = new Mock<DynamicObject>();
        var dynamicValue = sourceMock.As<IDynamicValue>();
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType([9]));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy))
            .Returns(new MockDelegates.OutFunction<int[], object?, bool>((int[] indices, out object? value) =>
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
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType([expected.Length]));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy)).Returns(value: true);

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
        _ = dynamicValue.SetupGet(x => x.DataType).Returns(IntArrayType([expected.Length]));
        var dummy = new object();
        _ = dynamicValue.Setup(x => x.TryGetIndexValue(It.IsAny<int[]>(), out dummy)).Returns(value: false);

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(() => typeConverter.Convert<int[]>(sourceMock.Object));
    }

    private static ArrayType ComplexArrayType(int[] rank)
    {
        var memberCollection = new MemberCollection()
        {
            new Member(nameof(NestedType.IntValue), new PrimitiveType("int", typeof(int))),
        };
        var complexType = new StructType("NestedType", baseType: null, memberCollection);
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
            _ = sourceMock.Setup(x => x.GetDynamicMemberNames()).Returns([nameof(IntValue)]);
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

    private sealed class TimesTestType
    {
        public static TimesTestType Default => new()
        {
            Date = new DateTime(2021, 08, 13),
            Time = new TimeSpan(2, 5, 20, 10, 999),
            LTime = new TimeSpan(111111111),
            DateTimeOffset = new DateTimeOffset(new DateTime(2021, 08, 13, 00, 00, 00)),
        };

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
