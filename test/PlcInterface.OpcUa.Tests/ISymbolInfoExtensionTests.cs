using Moq;

namespace PlcInterface.OpcUa.Tests;

[TestClass]
public class ISymbolInfoExtensionTests
{
    [TestMethod]
    public void CastAndValidateReturnsSymbolInfo()
    {
        // Arrange
        ISymbolInfo symbolInfo = Mock.Of<IOpcSymbolInfo>();

        // Act
        var actual = symbolInfo.ConvertAndValidate();

        // Assert
        Assert.IsInstanceOfType<IOpcSymbolInfo>(actual);
        Assert.IsInstanceOfType<ISymbolInfo>(actual);
        Assert.AreSame(symbolInfo, actual);
    }

    [TestMethod]
    public void CastAndValidateThrowsSymbolExceptionWhenNotAdsSymbol()
    {
        // Arrange
        var symbolMock = Mock.Of<ISymbolInfo>();

        // Act Assert
        _ = Assert.ThrowsExactly<SymbolException>(() => symbolMock.ConvertAndValidate());
    }
}
