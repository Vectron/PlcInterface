using System;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class IMonitorExtensionTests
{
    [TestMethod]
    public void SubscribeIOOnlyCallsActionForTheGivenName()
    {
        // Arrange
        var ioName = "test.var";
        var monitorMock = new Mock<IMonitor>();
        var typeConverterMock = new Mock<ITypeConverter>();
        _ = typeConverterMock.Setup(x => x.Convert<bool>(It.IsAny<object>())).Returns<object>(x => (bool)x);
        using var subject = new Subject<IMonitorResult>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        _ = monitorMock.SetupGet(x => x.TypeConverter).Returns(typeConverterMock.Object);
        var monitorResultMock = new Mock<IMonitorResult>();

        var observerMock = new Mock<Action<bool>>();

        // Act
        using var subscription = monitorMock.Object.SubscribeIO(ioName, observerMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns($"{ioName}1");
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(0);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns($"{ioName}2");
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(1u);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns($"{ioName}3");
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: false);
        subject.OnNext(monitorResultMock.Object);

        // Assert
        observerMock.Verify(x => x(It.IsAny<bool>()), Times.Once);
    }

    [TestMethod]
    public void SubscribeIOOnlyCallsActionWhenFilterIsMet()
    {
        // Arrange
        var ioName = "test.var";
        var monitorMock = new Mock<IMonitor>();
        var typeConverterMock = new Mock<ITypeConverter>();
        _ = typeConverterMock.Setup(x => x.Convert<bool>(It.IsAny<object>())).Returns<object>(x => (bool)x);
        using var subject = new Subject<IMonitorResult>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        _ = monitorMock.SetupGet(x => x.TypeConverter).Returns(typeConverterMock.Object);
        var monitorResultMock = new Mock<IMonitorResult>();

        var observerMock = new Mock<Action>();

        // Act
        using var subscription = monitorMock.Object.SubscribeIO(ioName, filterValue: true, observerMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: false);
        subject.OnNext(monitorResultMock.Object);

        // Assert
        observerMock.Verify(x => x(), Times.Once);
    }

    [TestMethod]
    public void SubscribeIOOnlyReturnsValuesFromTheGivenName()
    {
        // Arrange
        var ioName = "test.var";
        var monitorMock = new Mock<IMonitor>();
        var typeConverterMock = new Mock<ITypeConverter>();
        _ = typeConverterMock.Setup(x => x.Convert<bool>(It.IsAny<object>())).Returns<object>(x => (bool)x);
        using var subject = new Subject<IMonitorResult>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        _ = monitorMock.SetupGet(x => x.TypeConverter).Returns(typeConverterMock.Object);
        var monitorResultMock = new Mock<IMonitorResult>();

        var observerMock = new Mock<IObserver<bool>>();

        // Act
        using var subscription = monitorMock.Object.SubscribeIO<bool>(ioName).Subscribe(observerMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns($"{ioName}1");
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns($"{ioName}2");
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns($"{ioName}3");
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);

        // Assert
        observerMock.Verify(x => x.OnNext(It.IsAny<bool>()), Times.Once);
    }

    [TestMethod]
    public void SubscribeIOOnlyReturnsValuesWhenFilterIsMet()
    {
        // Arrange
        var ioName = "test.var";
        var monitorMock = new Mock<IMonitor>();
        var typeConverterMock = new Mock<ITypeConverter>();
        _ = typeConverterMock.Setup(x => x.Convert<bool>(It.IsAny<object>())).Returns<object>(x => (bool)x);
        using var subject = new Subject<IMonitorResult>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        _ = monitorMock.SetupGet(x => x.TypeConverter).Returns(typeConverterMock.Object);
        var monitorResultMock = new Mock<IMonitorResult>();

        var observerMock = new Mock<IObserver<bool>>();

        // Act
        using var subscription = monitorMock.Object.SubscribeIO(ioName, filterValue: true).Subscribe(observerMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: true);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(value: false);
        subject.OnNext(monitorResultMock.Object);

        // Assert
        observerMock.Verify(x => x.OnNext(It.IsAny<bool>()), Times.Once);
    }

    [TestMethod]
    public void SubscribeIOOnlyReturnsValuesWhenNotNull()
    {
        // Arrange
        var ioName = "test.var";
        var monitorMock = new Mock<IMonitor>();
        var typeConverterMock = new Mock<ITypeConverter>();
        _ = typeConverterMock.Setup(x => x.Convert<string>(It.IsAny<object>())).Returns<string>(x => x);
        using var subject = new Subject<IMonitorResult>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        _ = monitorMock.SetupGet(x => x.TypeConverter).Returns(typeConverterMock.Object);
        var monitorResultMock = new Mock<IMonitorResult>();

        var observerMock = new Mock<IObserver<string>>();

        // Act
        using var subscription = monitorMock.Object.SubscribeIO(ioName, "Test").Subscribe(observerMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns(null!);
        subject.OnNext(monitorResultMock.Object);
        _ = monitorResultMock.SetupGet(x => x.Name).Returns(ioName);
        _ = monitorResultMock.SetupGet(x => x.Value).Returns("Test");
        subject.OnNext(monitorResultMock.Object);

        // Assert
        observerMock.Verify(x => x.OnNext(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void SubscribeIORegistersIOForUpdates()
    {
        // Arrange
        var ioName = "test.var";
        using var subject = new Subject<IMonitorResult>();
        var monitorMock = new Mock<IMonitor>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        var observerMock = new Mock<IObserver<bool>>();

        // Act
        using var subscription = monitorMock.Object.SubscribeIO<bool>(ioName).Subscribe(observerMock.Object);

        // Assert
        monitorMock.Verify(x => x.RegisterIO(It.Is<string>(x => ioName.Equals(x, StringComparison.Ordinal)), It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void SubscribeIOUnRegistersIOWhenDisposed()
    {
        // Arrange
        var ioName = "test.var";
        using var subject = new Subject<IMonitorResult>();
        var monitorMock = new Mock<IMonitor>();
        _ = monitorMock.SetupGet(x => x.SymbolStream).Returns(subject);
        var observerMock = new Mock<IObserver<bool>>();
        using (var subscription = monitorMock.Object.SubscribeIO<bool>(ioName).Subscribe(observerMock.Object))
        {
        }

        // Assert
        monitorMock.Verify(x => x.UnregisterIO(It.Is<string>(x => ioName.Equals(x, StringComparison.Ordinal))), Times.Once);
    }
}
