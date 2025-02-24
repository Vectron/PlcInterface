using System.Reactive.Subjects;
using Moq;
using TestUtilities;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class MonitorTests
{
    [TestMethod]
    public void AllTagsAreRegisteredWhenPlcConnects()
    {
        // Arrange
        var ioTags = new[] { "TestVar1", "TestVar2", "TestVar3", "TestVar1" };
        var connectionMock = new Mock<IAdsPlcConnection>();
        using var session = new Subject<IConnected<IAdsConnection>>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(session);
        var twinCATConnectionMock = new Mock<IConnection>();
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolInfo = symbolInfoMock.Object;
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.TryGetSymbolInfo(It.IsAny<string>(), out symbolInfo)).Returns(value: true);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());
        var connectedMock = new Mock<IConnected<IAdsConnection>>();
        _ = connectedMock.Setup(x => x.IsConnected).Returns(value: true);

        // Act
        monitor.RegisterIO(ioTags);
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Never);
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        session.OnNext(connectedMock.Object);

        // Assert
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Exactly(3));
    }

    [TestMethod]
    public void MultipleIOCanBeRegisteredAtOnce()
    {
        // Arrange
        var ioTags = new[] { "TestVar1", "TestVar2", "TestVar3", "TestVar1" };
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var twinCATConnectionMock = new Mock<IConnection>();
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolInfo = symbolInfoMock.Object;
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.TryGetSymbolInfo(It.IsAny<string>(), out symbolInfo)).Returns(value: true);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());

        // Act
        monitor.RegisterIO(ioTags);

        // Assert
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Exactly(3));
    }

    [TestMethod]
    public void MultipleIOCanBeUnregisteredAtOnce()
    {
        // Arrange
        var ioTags = new[] { "TestVar1", "TestVar2", "TestVar3", "TestVar1" };
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var twinCATConnectionMock = new Mock<IConnection>();
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.GetSymbolInfo(It.IsAny<string>())).Returns(symbolInfoMock.Object);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());

        // Act
        monitor.UnregisterIO(ioTags);

        // Assert
        valueSymbolMock.VerifyRemove(x => x.ValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Never);
    }

    [TestMethod]
    public void RegisterSubscribesToValueUpdateEventsOnlyOnce()
    {
        // Arrange
        var ioTag = "TestVar";
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var twinCATConnectionMock = new Mock<IConnection>();
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolInfo = symbolInfoMock.Object;
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.TryGetSymbolInfo(It.IsAny<string>(), out symbolInfo)).Returns(value: true);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());

        // Act
        monitor.RegisterIO(ioTag);
        monitor.RegisterIO(ioTag);

        // Assert
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
    }

    [TestMethod]
    public void TypeConverterPropertyContainsPassedInTypeConverter()
    {
        // Arrange
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var symbolHandler = new Mock<IAdsSymbolHandler>();
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandler.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());

        // Act Assert
        Assert.AreSame(typeConverterMock.Object, monitor.TypeConverter);
    }

    [TestMethod]
    public void UnregisterANotRegisteredTagDoesNotDoAnything()
    {
        // Arrange
        var ioTag = "TestVar";
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var twinCATConnectionMock = new Mock<IConnection>();
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.GetSymbolInfo(It.Is<string>(x => x.Equals(ioTag, StringComparison.Ordinal)))).Returns(symbolInfoMock.Object);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());

        // Act
        monitor.UnregisterIO(ioTag);

        // Assert
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Never);
        valueSymbolMock.VerifyRemove(x => x.ValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Never);
    }

    [TestMethod]
    public void UnregisterDoesNotUnregisterEventUntilAllRegistrationsAreUnregistered()
    {
        // Arrange
        var ioTag = "TestVar";
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var twinCATConnectionMock = new Mock<IConnection>();
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolInfo = symbolInfoMock.Object;
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.TryGetSymbolInfo(It.IsAny<string>(), out symbolInfo)).Returns(value: true);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());

        // Act Assert
        monitor.RegisterIO(ioTag);
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
        monitor.RegisterIO(ioTag);
        valueSymbolMock.VerifyAdd(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
        monitor.UnregisterIO(ioTag);
        valueSymbolMock.VerifyRemove(x => x.ValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Never);
        monitor.UnregisterIO(ioTag);
        valueSymbolMock.VerifyRemove(x => x.ValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
    }

    [TestMethod]
    public void ValueChangedEventsArePostedOnTheStream()
    {
        // Arrange
        var ioTag = "TestVar";
        var connectionMock = new Mock<IAdsPlcConnection>();
        _ = connectionMock.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var twinCATConnectionMock = new Mock<IConnection>();
        _ = twinCATConnectionMock.SetupGet(x => x.IsConnected).Returns(value: true);
        var valueSymbolMock = new Mock<IValueSymbol>();
        _ = valueSymbolMock.SetupGet(x => x.Connection).Returns(twinCATConnectionMock.Object);
        var symbolInfoMock = new Mock<IAdsSymbolInfo>();
        _ = symbolInfoMock.SetupGet(x => x.Symbol).Returns(valueSymbolMock.Object);
        var symbolInfo = symbolInfoMock.Object;
        var symbolHandlerMock = new Mock<IAdsSymbolHandler>();
        _ = symbolHandlerMock.Setup(x => x.TryGetSymbolInfo(It.IsAny<string>(), out symbolInfo)).Returns(value: true);
        var typeConverterMock = new Mock<IAdsTypeConverter>();
        _ = typeConverterMock.Setup(x => x.Convert(It.IsAny<object>(), It.IsAny<IValueSymbol>())).Returns<object, IValueSymbol>((o, v) => o);
        using var monitor = new Monitor(connectionMock.Object, symbolHandlerMock.Object, typeConverterMock.Object, MockHelpers.GetLoggerMock<Monitor>());
        var connectedMock = new Mock<IConnected<IAdsConnection>>();
        _ = connectedMock.Setup(x => x.IsConnected).Returns(value: true);
        var observerMock = new Mock<IObserver<IMonitorResult>>();

        // Act
        using var subscription = monitor.SymbolStream.Subscribe(observerMock.Object);
        monitor.RegisterIO(ioTag);
        valueSymbolMock.Raise(x => x.ValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), new ValueChangedEventArgs(valueSymbolMock.Object, value: true, DateTimeOffset.UtcNow));

        // Assert
        observerMock.Verify(x => x.OnNext(It.Is<IMonitorResult>(x => x.Name.Equals(ioTag, StringComparison.Ordinal) && ((bool)x.Value))), Times.Once);
    }
}
