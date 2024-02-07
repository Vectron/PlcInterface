using System;
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
        Assert.IsInstanceOfType(connected, typeof(IConnected));
        Assert.IsInstanceOfType(connected, typeof(IConnected<GenericParameterHelper>));

        // Assert
        Assert.IsInstanceOfType(connected, typeof(IConnected));
        Assert.IsInstanceOfType(connected, typeof(IConnected<GenericParameterHelper>));
    }

    [TestMethod]
    public void ConnectedNoReturnsAValidIConnected()
    {
        // Arrange

        // Act
        var notConnected = Connected.No<GenericParameterHelper>();

        // Assert
        Assert.IsInstanceOfType(notConnected, typeof(IConnected));
        Assert.IsInstanceOfType(notConnected, typeof(IConnected<GenericParameterHelper>));
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
        Assert.IsInstanceOfType(connected, typeof(IConnected));
        Assert.IsInstanceOfType(connected, typeof(IConnected<GenericParameterHelper>));
        Assert.IsTrue(connected.IsConnected);
        Assert.AreSame(expectedValue, connected.Value);
    }
}
