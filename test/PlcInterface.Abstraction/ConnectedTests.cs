using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class ConnectedTests
{
    [TestMethod]
    public void ConnectedImplementsIConnected()
    {
        // Arrange
        var connected = new Connected<GenericParameterHelper>();

        // Act
        Assert.IsInstanceOfType<IConnected>(connected);
        Assert.IsInstanceOfType<IConnected<GenericParameterHelper>>(connected);

        // Assert
        Assert.IsInstanceOfType<IConnected>(connected);
        Assert.IsInstanceOfType<IConnected<GenericParameterHelper>>(connected);
    }

    [TestMethod]
    public void ConnectedNoReturnsAValidIConnected()
    {
        // Arrange

        // Act
        var notConnected = Connected.No<GenericParameterHelper>();

        // Assert
        Assert.IsInstanceOfType<IConnected>(notConnected);
        Assert.IsInstanceOfType<IConnected<GenericParameterHelper>>(notConnected);
        Assert.IsFalse(notConnected.IsConnected);
        _ = Assert.ThrowsException<InvalidOperationException>(() => notConnected.Value);
    }

    [TestMethod]
    public void ConnectedYesReturnsAValidIConnected()
    {
        // Arrange
        var expectedValue = new GenericParameterHelper();

        // Act
        var connected = Connected.Yes(expectedValue);

        // Assert
        Assert.IsInstanceOfType<IConnected>(connected);
        Assert.IsInstanceOfType<IConnected<GenericParameterHelper>>(connected);
        Assert.IsTrue(connected.IsConnected);
        Assert.AreSame(expectedValue, connected.Value);
    }
}
