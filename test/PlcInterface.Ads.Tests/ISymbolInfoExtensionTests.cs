using Moq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class ISymbolInfoExtensionTests
{
    [TestMethod]
    public void CastAndValidateReturnsSymbolInfo()
    {
        // Arrange
        ISymbolInfo symbolInfo = new SymbolInfo(Mock.Of<ISymbol>(), string.Empty);

        // Act
        var actual = symbolInfo.CastAndValidate();

        // Assert
        Assert.IsInstanceOfType<IAdsSymbolInfo>(actual);
        Assert.IsInstanceOfType<ISymbolInfo>(actual);
        Assert.AreSame(symbolInfo, actual);
    }

    [TestMethod]
    public void CastAndValidateThrowsSymbolExceptionWhenNotAdsSymbol()
    {
        // Arrange
        var symbolMock = Mock.Of<ISymbolInfo>();

        // Act
        // Assert
        _ = Assert.ThrowsExactly<SymbolException>(() => symbolMock.CastAndValidate());
    }
}
