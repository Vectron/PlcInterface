using System.Reactive.Linq;
using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class IPlcConnectionExtensionTests
{
    [TestMethod]
    public async Task GetConnectedClientAsyncReturnsAValidValueAsync()
    {
        // Arrange
        var plcConnectionMock = new Mock<IPlcConnection<GenericParameterHelper>>();
        var iConnectedMock = new Mock<IConnected<GenericParameterHelper>>();
        var expected = new GenericParameterHelper();
        _ = iConnectedMock.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = iConnectedMock.SetupGet(x => x.Value).Returns(expected);
        _ = plcConnectionMock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(iConnectedMock.Object));

        // Act
        var connection = await plcConnectionMock.Object.GetConnectedClientAsync();

        // Assert
        Assert.AreEqual(expected, connection);
    }

    [TestMethod]
    public async Task GetConnectedClientAsyncThrowsTimeoutExceptionOnTimeOutAsync()
    {
        // Arrange
        var mock = new Mock<IPlcConnection<GenericParameterHelper>>();
        _ = mock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(Mock.Of<IConnected<GenericParameterHelper>>()));

        // Act Assert
        _ = await Assert.ThrowsExactlyAsync<TimeoutException>(() => mock.Object.GetConnectedClientAsync(TimeSpan.FromMilliseconds(10)));
    }

    [TestMethod]
    public void GetConnectedClientReturnsAValidValue()
    {
        // Arrange
        var plcConnectionMock = new Mock<IPlcConnection<GenericParameterHelper>>();
        var iConnectedMock = new Mock<IConnected<GenericParameterHelper>>();
        var expected = new GenericParameterHelper();
        _ = iConnectedMock.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = iConnectedMock.SetupGet(x => x.Value).Returns(expected);
        _ = plcConnectionMock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(iConnectedMock.Object));

        // Act
        var connection = plcConnectionMock.Object.GetConnectedClient();

        // Assert
        Assert.AreEqual(expected, connection);
    }

    [TestMethod]
    public void GetConnectedClientThrowsTimeoutExceptionOnTimeOut()
    {
        // Arrange
        var mock = new Mock<IPlcConnection<GenericParameterHelper>>();
        _ = mock.SetupGet(x => x.SessionStream).Returns(Observable.Repeat(Mock.Of<IConnected<GenericParameterHelper>>()));

        // Act Assert
        _ = Assert.ThrowsExactlyAsync<TimeoutException>(() => mock.Object.GetConnectedClientAsync(TimeSpan.FromMilliseconds(10)));
    }
}
