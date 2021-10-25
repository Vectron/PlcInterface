using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class IPlcConnectionExtensionTests
{
    [TestMethod]
    public async Task GetConnectedClientAsyncReturnsAVallidValue()
    {
        // Arrange
        var plcConnectionMock = new Mock<IPlcConnection<GenericParameterHelper>>();
        var iConnectedMock = new Mock<IConnected<GenericParameterHelper>>();
        var expected = new GenericParameterHelper();
        _ = iConnectedMock.SetupGet(x => x.IsConnected).Returns(true);
        _ = iConnectedMock.SetupGet(x => x.Value).Returns(expected);
        _ = plcConnectionMock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(iConnectedMock.Object));

        // Act
        var connection = await plcConnectionMock.Object.GetConnectedClientAsync();

        // Assert
        Assert.AreEqual(expected, connection);
    }

    [TestMethod]
    public async Task GetConnectedClientAsyncThrowsArgumentNullException()
    {
        // Arrange
        var plcConnectionMock = new Mock<IPlcConnection<GenericParameterHelper>>();
        var iConnectedMock = new Mock<IConnected<GenericParameterHelper>>();
        _ = iConnectedMock.SetupGet(x => x.IsConnected).Returns(true);
        _ = plcConnectionMock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(iConnectedMock.Object));

        // Act
        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(plcConnectionMock.Object.GetConnectedClientAsync);
        Assert.AreEqual(nameof(IConnected<GenericParameterHelper>.Value), exception.ParamName);
        var exception2 = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => plcConnectionMock.Object.GetConnectedClientAsync(TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(nameof(IConnected<GenericParameterHelper>.Value), exception2.ParamName);
    }

    [TestMethod]
    public async Task GetConnectedClientAsyncThrowsTimeoutExceptionOnTimeOut()
    {
        // Arrange
        var mock = new Mock<IPlcConnection<GenericParameterHelper>>();
        _ = mock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(Mock.Of<IConnected<GenericParameterHelper>>()));

        // Act
        // Assert
        _ = await Assert.ThrowsExceptionAsync<TimeoutException>(() => mock.Object.GetConnectedClientAsync(TimeSpan.FromMilliseconds(10)));
    }

    [TestMethod]
    public void GetConnectedClientReturnsAVallidValue()
    {
        // Arrange
        var plcConnectionMock = new Mock<IPlcConnection<GenericParameterHelper>>();
        var iConnectedMock = new Mock<IConnected<GenericParameterHelper>>();
        var expected = new GenericParameterHelper();
        _ = iConnectedMock.SetupGet(x => x.IsConnected).Returns(true);
        _ = iConnectedMock.SetupGet(x => x.Value).Returns(expected);
        _ = plcConnectionMock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(iConnectedMock.Object));

        // Act
        var connection = plcConnectionMock.Object.GetConnectedClient();

        // Assert
        Assert.AreEqual(expected, connection);
    }

    [TestMethod]
    public void GetConnectedClientThrowsArgumentNullException()
    {
        // Arrange
        var plcConnectionMock = new Mock<IPlcConnection<GenericParameterHelper>>();
        var iConnectedMock = new Mock<IConnected<GenericParameterHelper>>();
        _ = iConnectedMock.SetupGet(x => x.IsConnected).Returns(true);
        _ = plcConnectionMock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(iConnectedMock.Object));

        // Act
        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(plcConnectionMock.Object.GetConnectedClient);
        Assert.AreEqual(nameof(IConnected<GenericParameterHelper>.Value), exception.ParamName);
        var exception2 = Assert.ThrowsException<ArgumentNullException>(() => plcConnectionMock.Object.GetConnectedClient(TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(nameof(IConnected<GenericParameterHelper>.Value), exception2.ParamName);
    }

    [TestMethod]
    public void GetConnectedClientThrowsTimeoutExceptionOnTimeOut()
    {
        // Arrange
        var mock = new Mock<IPlcConnection<GenericParameterHelper>>();
        _ = mock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(Mock.Of<IConnected<GenericParameterHelper>>()));

        // Act
        // Assert
        _ = Assert.ThrowsExceptionAsync<TimeoutException>(() => mock.Object.GetConnectedClientAsync(TimeSpan.FromMilliseconds(10)));
    }
}