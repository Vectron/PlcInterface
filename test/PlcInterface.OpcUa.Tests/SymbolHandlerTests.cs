using Moq;
using Opc.Ua.Client;
using TestUtilities;

namespace PlcInterface.OpcUa.Tests;

[TestClass]
public class SymbolHandlerTests
{
    [TestMethod]
    public void GetSymbolInfoThrowsSymbolExceptionWhenNoSymbolsAreLoaded()
    {
        // Arrange
        var ioName = "DummyVar1";
        var plcConnection = new Mock<IOpcPlcConnection>();
        _ = plcConnection.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<ISession>>>());
        _ = plcConnection.SetupGet(x => x.IsConnected).Returns(value: true);
        var symbolHandlerSettings = new OpcSymbolHandlerOptions();
        using var symbolHandler = new SymbolHandler(plcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>());

        // Act

        // Assert
        var exception = Assert.ThrowsException<SymbolException>(() => symbolHandler.GetSymbolInfo(ioName));
        Assert.AreEqual(exception.Message, $"{ioName} Does not exist in the PLC");
    }

    [TestMethod]
    public void SymbolExceptionThrownWhenNoPlcConnected()
    {
        // Arrange
        var ioName = "DummyVar1";
        var plcConnection = new Mock<IOpcPlcConnection>();
        _ = plcConnection.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<ISession>>>());
        _ = plcConnection.SetupGet(x => x.IsConnected).Returns(value: false);
        var symbolHandlerSettings = new OpcSymbolHandlerOptions();
        using var symbolHandler = new SymbolHandler(plcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>());

        // Act

        // Assert
        var exception = Assert.ThrowsException<SymbolException>(() => symbolHandler.GetSymbolInfo(ioName));
        Assert.AreEqual("PLC not connected", exception.Message);
    }
}
