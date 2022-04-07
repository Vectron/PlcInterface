using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlcInterface.Ads.TwincatAbstractions;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class ReadWriteTests
{
    [TestMethod]
    public async Task ReadDynamicReturnsAExpandoObjectAsync()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = new ExpandoObject();
        var connection = new Mock<IAdsPlcConnection>();

        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var symbolInfo = CreateSymbolInfoMock(ioTag, dummyValue, out var valueSymbolMock);

        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(symbolInfo);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        var value1 = readWrite.ReadDynamic(ioTag);
        var value2 = await readWrite.ReadDynamicAsync(ioTag);

        // Assert
        Assert.AreSame(dummyValue, value1);
        Assert.AreSame(dummyValue, value2);
    }

    [TestMethod]
    public void ReadDynamicThrowsNotSupportedExceptionIfPlcConnectionIsNotInDynamicTreeMode()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = 9;
        var connection = new Mock<IAdsPlcConnection>();

        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var symbolInfo = CreateSymbolInfoMock(ioTag, dummyValue, out var valueSymbolMock);

        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(symbolInfo);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        // Assert
        _ = Assert.ThrowsException<NotSupportedException>(() => _ = readWrite.ReadDynamic(ioTag));
        _ = Assert.ThrowsExceptionAsync<NotSupportedException>(() => _ = readWrite.ReadDynamicAsync(ioTag));
    }

    [TestMethod]
    public async Task ReadValueReturnsTheExpectedValueAsync()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = 9;
        var connection = new Mock<IAdsPlcConnection>();

        var symbolHandler = new Mock<IAdsSymbolHandler>();
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(CreateSymbolInfoMock(ioTag, dummyValue, out _));
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        var value = readWrite.Read(ioTag);
        var value2 = readWrite.Read<int>(ioTag);
        var value3 = await readWrite.ReadAsync(ioTag);
        var value4 = await readWrite.ReadAsync<int>(ioTag);

        // Assert
        Assert.AreEqual(dummyValue, value);
        Assert.AreEqual(dummyValue, value2);
        Assert.AreEqual(dummyValue, value3);
        Assert.AreEqual(dummyValue, value4);
    }

    [TestMethod]
    public async Task ReadValuesReturnsTheExpectedValuesAsync()
    {
        // Arrange
        var ioTags = new[] { "dummyVar1", "dummyVar2", "dummyVar3", "dummyVar4", };
        var dummyValues = new[] { 9, 521, 456, 456, };
        var symbolMocks = new[]
        {
            CreateSymbolInfoMock(ioTags[0], dummyValues[0], out _),
            CreateSymbolInfoMock(ioTags[1], dummyValues[1], out _),
            CreateSymbolInfoMock(ioTags[2], dummyValues[2], out _),
            CreateSymbolInfoMock(ioTags[3], dummyValues[3], out _),
        };

        var connection = new Mock<IAdsPlcConnection>();
        using var subject = new BehaviorSubject<IConnected<IAdsConnection>>(Connected.Yes(Mock.Of<IAdsConnection>()));
        _ = connection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandler = new Mock<IAdsSymbolHandler>();
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.Is<string>(x => x.Equals(ioTags[0], StringComparison.Ordinal)))).Returns(symbolMocks[0]);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.Is<string>(x => x.Equals(ioTags[1], StringComparison.Ordinal)))).Returns(symbolMocks[1]);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.Is<string>(x => x.Equals(ioTags[2], StringComparison.Ordinal)))).Returns(symbolMocks[2]);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.Is<string>(x => x.Equals(ioTags[3], StringComparison.Ordinal)))).Returns(symbolMocks[3]);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolMock = new Mock<ISumSymbolRead>();
        var returnValue = dummyValues.Cast<object>().ToArray();
        _ = sumSymbolMock.Setup(x => x.Read()).Returns(returnValue);
        _ = sumSymbolMock.Setup(x => x.ReadAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult<object[]?>(returnValue));
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        _ = sumSymbolFactory.Setup(x => x.CreateSumSymbolRead(It.IsAny<IAdsConnection>(), It.IsAny<IList<ISymbol>>())).Returns(sumSymbolMock.Object);
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        var value = readWrite.Read(ioTags);
        var value2 = await readWrite.ReadAsync(ioTags);

        // Assert
        Assert.AreEqual(dummyValues[0], value[ioTags[0]]);
        Assert.AreEqual(dummyValues[1], value[ioTags[1]]);
        Assert.AreEqual(dummyValues[2], value[ioTags[2]]);
        Assert.AreEqual(dummyValues[3], value[ioTags[3]]);
        Assert.AreEqual(dummyValues[0], value2[ioTags[0]]);
        Assert.AreEqual(dummyValues[1], value2[ioTags[1]]);
        Assert.AreEqual(dummyValues[2], value2[ioTags[2]]);
        Assert.AreEqual(dummyValues[3], value2[ioTags[3]]);
    }

    [TestMethod]
    public void ReadValueThrowsArgumentNullExceptionWhenNullIsReturned()
    {
        // Arrange
        var ioTag = "dummyVar";
        object? dummyValue = null;
        var connection = new Mock<IAdsPlcConnection>();

        var symbolHandler = new Mock<IAdsSymbolHandler>();
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(CreateSymbolInfoMock(ioTag, dummyValue, out _));
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        // Assert
        _ = Assert.ThrowsException<ArgumentNullException>(() => _ = readWrite.Read(ioTag));
        _ = Assert.ThrowsException<ArgumentNullException>(() => _ = readWrite.Read<int>(ioTag));
        _ = Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _ = readWrite.ReadAsync(ioTag));
        _ = Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _ = readWrite.ReadAsync<int>(ioTag));
    }

    [TestMethod]
    public void ToggleBoolInvertsTheValue()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = true;

        var connection = new Mock<IAdsPlcConnection>();
        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var symbol = CreateSymbolInfoMock(ioTag, dummyValue, out var valueSymbolMock);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(symbol);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        readWrite.ToggleBool(ioTag);

        // Assert
        valueSymbolMock.Verify(x => x.WriteValue(It.Is<bool>(x => dummyValue != x)), Times.Once);
    }

    [TestMethod]
    public async Task WriteCallsUnderlyingWriteMethodAsync()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = 9;

        var connection = new Mock<IAdsPlcConnection>();
        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var symbolInfo = CreateSymbolInfoMock(ioTag, dummyValue, out var valueSymbolMock);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(symbolInfo);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        readWrite.Write(ioTag, dummyValue);
        await readWrite.WriteAsync(ioTag, dummyValue);

        // Assert
        valueSymbolMock.Verify(x => x.WriteValue(It.Is<int>(x => dummyValue == x)), Times.Once);
        valueSymbolMock.Verify(x => x.WriteValueAsync(It.Is<int>(x => dummyValue == x), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task WriteMultipleCallsUnderlyingWriteMethodAsync()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = 9;

        var connection = new Mock<IAdsPlcConnection>();
        using var subject = new BehaviorSubject<IConnected<IAdsConnection>>(Connected.Yes(Mock.Of<IAdsConnection>()));
        _ = connection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var symbolInfo = CreateSymbolInfoMock(ioTag, dummyValue, out _);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(symbolInfo);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolMock = new Mock<ISumSymbolWrite>();
        _ = sumSymbolMock.Setup(x => x.WriteAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()));
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        _ = sumSymbolFactory.Setup(x => x.CreateSumSymbolWrite(It.IsAny<IAdsConnection>(), It.IsAny<IList<ISymbol>>())).Returns(sumSymbolMock.Object);
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        var writeData = new Dictionary<string, object>(StringComparer.Ordinal) { { ioTag, dummyValue } };
        readWrite.Write(writeData);
        await readWrite.WriteAsync(writeData);

        // Assert
        sumSymbolMock.Verify(x => x.Write(It.IsAny<object[]>()), Times.Once);
        sumSymbolMock.Verify(x => x.WriteAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task WriteMultipleWhenArgumentExceptionIsThrownFlattenTheHirachyAndWriteAgainAsync()
    {
        // Arrange
        var ioTag = "dummyVar";
        var dummyValue = 9;

        var connection = new Mock<IAdsPlcConnection>();
        using var subject = new BehaviorSubject<IConnected<IAdsConnection>>(Connected.Yes(Mock.Of<IAdsConnection>()));
        _ = connection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var symbolInfo = CreateSymbolInfoMock(ioTag, dummyValue, out _);
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns(symbolInfo);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolMock = new Mock<ISumSymbolWrite>();
        _ = sumSymbolMock.SetupSequence(x => x.WriteAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Throws<ArgumentException>().Returns(Task.CompletedTask);
        _ = sumSymbolMock.SetupSequence(x => x.Write(It.IsAny<object[]>())).Throws<ArgumentException>();
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        _ = sumSymbolFactory.Setup(x => x.CreateSumSymbolWrite(It.IsAny<IAdsConnection>(), It.IsAny<IList<ISymbol>>())).Returns(sumSymbolMock.Object);
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        var writeData = new Dictionary<string, object>(StringComparer.Ordinal) { { ioTag, dummyValue } };
        readWrite.Write(writeData);
        await readWrite.WriteAsync(writeData);

        // Assert
        sumSymbolMock.Verify(x => x.Write(It.IsAny<object[]>()), Times.Exactly(2));
        sumSymbolMock.Verify(x => x.WriteAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task WriteSingleWhenArgumentExceptionIsThrownFlattenTheHirachyAndWriteAgainAsync()
    {
        // Arrange
        var ioTag = "dummyVar1";
        var value = 9;
        var symbol = CreateSymbolInfoMock(ioTag, value, out var valueSymbol);
        _ = valueSymbol.SetupSequence(x => x.WriteValueAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Throws<ArgumentException>().Returns(Task.FromResult(new ResultWriteAccess(0)));
        _ = valueSymbol.SetupSequence(x => x.WriteValue(It.IsAny<object>())).Throws<ArgumentException>();

        var connection = new Mock<IAdsPlcConnection>();
        using var subject = new BehaviorSubject<IConnected<IAdsConnection>>(Connected.Yes(Mock.Of<IAdsConnection>()));
        _ = connection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandler = new Mock<IAdsSymbolHandler>();

        var genericSymbolHandler = symbolHandler.As<ISymbolHandler>();
        _ = symbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns<string>(x => symbol);
        _ = genericSymbolHandler.Setup(x => x.GetSymbolinfo(It.IsAny<string>())).Returns<string>(x => symbol);
        var typeConverter = CreateTypeConverterMock();
        var sumSymbolMock = new Mock<ISumSymbolWrite>();
        _ = sumSymbolMock.Setup(x => x.WriteAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var sumSymbolFactory = new Mock<ISumSymbolFactory>();
        _ = sumSymbolFactory.Setup(x => x.CreateSumSymbolWrite(It.IsAny<IAdsConnection>(), It.IsAny<IList<ISymbol>>())).Returns(sumSymbolMock.Object);
        var readWrite = new ReadWrite(connection.Object, symbolHandler.Object, typeConverter, sumSymbolFactory.Object);

        // Act
        readWrite.Write(ioTag, value);
        await readWrite.WriteAsync(ioTag, value);

        // Assert
        valueSymbol.Verify(x => x.WriteValue(It.Is<int>(x => value == x)), Times.Once);
        valueSymbol.Verify(x => x.WriteValueAsync(It.Is<int>(x => value == x), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static IAdsSymbolInfo CreateSymbolInfoMock<T>(string name, T? value, out Mock<IValueSymbol> valueSymbolMock)
    {
        valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.Setup(x => x.ReadValue()).Returns(value!);
        _ = valueSymbolMock.Setup(x => x.ReadValue(It.IsAny<int>())).Returns(value!);
        _ = valueSymbolMock.Setup(x => x.ReadValueAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ResultReadValueAccess(value, 0)));
        _ = valueSymbolMock.Setup(x => x.WriteValueAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ResultWriteAccess(0)));
        var symbolMock = new Mock<IAdsSymbolInfo>();
        _ = symbolMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        _ = symbolMock.SetupGet(x => x.Name).Returns(name);
        _ = symbolMock.SetupGet(x => x.ChildSymbols).Returns(new List<string>());
        var splitNames = name.Split('.');
        var shortName = splitNames[splitNames.Length - 1];
        _ = symbolMock.SetupGet(x => x.ShortName).Returns(shortName);
        return symbolMock.Object;
    }

    private static IAdsTypeConverter CreateTypeConverterMock()
    {
        var typeConverter = new Mock<IAdsTypeConverter>();
        _ = typeConverter.Setup(x => x.Convert(It.IsAny<object>(), It.IsAny<IValueSymbol>())).Returns<object, IValueSymbol>((o, v) => o);
        _ = typeConverter.Setup(x => x.Convert(It.IsAny<object>(), It.IsAny<Type>())).Returns<object, Type>((o, t) => o);
        _ = typeConverter.Setup(x => x.Convert<int>(It.IsAny<object>())).Returns<object>(o => (int)o);
        _ = typeConverter.Setup(x => x.Convert<bool>(It.IsAny<object>())).Returns<object>(o => (bool)o);
        return typeConverter.Object;
    }
}