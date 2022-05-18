using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestUtilities;
using TwinCAT;
using TwinCAT.Ads;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class PlcConnectionTests
{
    [TestMethod]
    public async Task ConnectAsyncCallsConnectOnClientAsync()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        await connection.ConnectAsync();

        // Assert
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsAddress>()), Times.Once);
        adsClientMock.Verify(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsNetId>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public async Task ConnectAsyncWontDoAnythingIfAlreadyConnectedAsync()
    {
        // Arrange
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        _ = adsClientMock.SetupGet(x => x.IsConnected).Returns(true);
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(Mock.Of<ConnectionSettings>()), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        await connection.ConnectAsync();

        // Assert
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsAddress>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsNetId>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void ConnectCallsConnectOnClient()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        connection.Connect();

        // Assert
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsAddress>()), Times.Once);
        adsClientMock.Verify(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsNetId>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void ConnectionStateChangesArePushedToSessionStream()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var adsConnection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);
        var connection = (IPlcConnection)adsConnection;
        var observerMock = new Mock<IObserver<IConnected<IAdsConnection>>>();
        var observerMock2 = new Mock<IObserver<IConnected>>();

        // Act
        adsConnection.Connect();
        using var subscription = adsConnection.SessionStream.Subscribe(observerMock.Object);
        using var subscription2 = connection.SessionStream.Subscribe(observerMock2.Object);
        adsClientMock.Raise(x => x.ConnectionStateChanged += null, new ConnectionStateChangedEventArgs(ConnectionStateChangedReason.Established, ConnectionState.Connected, ConnectionState.Disconnected));

        adsClientMock.Raise(x => x.ConnectionStateChanged += null, new ConnectionStateChangedEventArgs(ConnectionStateChangedReason.Established, ConnectionState.Disconnected, ConnectionState.Disconnected));
        adsClientMock.Raise(x => x.ConnectionStateChanged += null, new ConnectionStateChangedEventArgs(ConnectionStateChangedReason.Established, ConnectionState.Lost, ConnectionState.Disconnected));
        adsClientMock.Raise(x => x.ConnectionStateChanged += null, new ConnectionStateChangedEventArgs(ConnectionStateChangedReason.Established, ConnectionState.None, ConnectionState.Disconnected));

        // Assert
        observerMock.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => x.IsConnected)), Times.Once);
        observerMock2.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => x.IsConnected)), Times.Once);

        observerMock.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => !x.IsConnected)), Times.Exactly(4));
        observerMock2.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => !x.IsConnected)), Times.Exactly(4));
    }

    [TestMethod]
    public void ConnectWontDoAnythingIfAlreadyConnected()
    {
        // Arrange
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        _ = adsClientMock.SetupGet(x => x.IsConnected).Returns(true);
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(Mock.Of<ConnectionSettings>()), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        connection.Connect();

        // Assert
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsAddress>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsNetId>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DisconnectCallsDisconnectOnClient()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        connection.Disconnect();

        // Assert
        adsClientMock.Verify(x => x.Disconnect(), Times.Once);
    }

    [TestMethod]
    public async Task DisconnectCallsDisconnectOnClientAsync()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        await connection.DisconnectAsync();

        // Assert
        adsClientMock.Verify(x => x.Disconnect(), Times.Once);
    }

    [TestMethod]
    public void ExceptionsInSessionStreamDontDestroyTheStream()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var adsConnection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);
        var observerMock = new Mock<IObserver<IConnected<IAdsConnection>>>();

        // Act
        adsConnection.Connect();
        using var subscription2 = adsConnection.SessionStream.Subscribe(observerMock.Object);
        using var subscription = adsConnection.SessionStream.Subscribe(x => throw new NotSupportedException());
        adsClientMock.Raise(x => x.ConnectionStateChanged += null, new ConnectionStateChangedEventArgs(ConnectionStateChangedReason.Established, ConnectionState.Connected, ConnectionState.Disconnected));

        // Assert
        observerMock.Verify(x => x.OnNext(It.IsAny<IConnected<IAdsConnection>>()), Times.Exactly(2));
        observerMock.Verify(x => x.OnError(It.IsAny<Exception>()), Times.Never);
        observerMock.Verify(x => x.OnCompleted(), Times.Never);
    }

    [TestMethod]
    public async Task IfAutoConnectIsTrueConnectionIsMadeImmidiatly()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = true,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        var resetEvent = new TaskCompletionSource<bool>();
        _ = adsClientMock.Setup(x => x.Connect(It.IsAny<AmsAddress>())).Callback((AmsAddress address) => resetEvent.SetResult(true));
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var tokenRegestration = tokenSource.Token.Register(() => resetEvent.SetResult(false));

        // Act
        using var connection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);
        var result = await resetEvent.Task;

        // Assert
        Assert.IsTrue(result);
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsAddress>()), Times.Once);
        adsClientMock.Verify(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<AmsNetId>(), It.IsAny<int>()), Times.Never);
        adsClientMock.Verify(x => x.Connect(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void SesionstreamIsStoppedWhenDisposed()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var adsConnection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);
        var connection = (IPlcConnection)adsConnection;
        var observerMock = new Mock<IObserver<IConnected<IAdsConnection>>>();
        var observerMock2 = new Mock<IObserver<IConnected>>();

        // Act
        using var subscription = adsConnection.SessionStream.Subscribe(observerMock.Object);
        using var subscription2 = connection.SessionStream.Subscribe(observerMock2.Object);
        using (adsConnection)
        {
        }

        // Assert
        observerMock.Verify(x => x.OnCompleted(), Times.Once);
        observerMock2.Verify(x => x.OnCompleted(), Times.Once);

        observerMock.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => !x.IsConnected)), Times.Once);
        observerMock2.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => !x.IsConnected)), Times.Once);
    }

    [TestMethod]
    public void SettingsPropertyContainsPassedInSettings()
    {
        // Arrange
        var settings = new ConnectionSettings()
        {
            AmsNetId = "local",
            Port = 851,
            AutoConnect = false,
        };
        var adsClientMock = new Mock<IAdsDisposableConnection>();
        using var adsConnection = new PlcConnection(MockHelpers.GetOptionsMoq(settings), MockHelpers.GetLoggerMock<PlcConnection>(), adsClientMock.Object);

        // Act
        // Assert
        Assert.AreEqual(settings, adsConnection.Settings);
        Assert.AreSame(settings, adsConnection.Settings);
    }

    [TestMethod]
    public void WhenSubscribingToSessionStreamGetCurrentState()
    {
        // Arrange
        using var adsConnection = new PlcConnection(MockHelpers.GetOptionsMoq(Mock.Of<ConnectionSettings>()), MockHelpers.GetLoggerMock<PlcConnection>(), Mock.Of<IAdsDisposableConnection>());
        var connection = (IPlcConnection)adsConnection;
        var observerMock = new Mock<IObserver<IConnected<IAdsConnection>>>();
        var observerMock2 = new Mock<IObserver<IConnected>>();

        // Act
        using var subscription = adsConnection.SessionStream.Subscribe(observerMock.Object);
        using var subscription2 = connection.SessionStream.Subscribe(observerMock2.Object);

        // Assert
        observerMock.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => x.IsConnected)), Times.Never);
        observerMock2.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => x.IsConnected)), Times.Never);

        observerMock.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => !x.IsConnected)), Times.Once);
        observerMock2.Verify(x => x.OnNext(It.Is<IConnected<IAdsConnection>>(x => !x.IsConnected)), Times.Once);
    }
}