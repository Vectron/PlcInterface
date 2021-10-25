using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class IValueSymbolExtensionsTests
{
    [TestMethod]
    public void CastAndValidateReturnsSymbolInfo()
    {
        // Arrange
        var symbolMock = new Mock<ISymbol>();
        _ = symbolMock.As<IValueSymbol>();

        // Act
        var actual = symbolMock.Object.CastAndValidate();

        // Assert
        Assert.IsInstanceOfType(actual, typeof(IValueSymbol));
        Assert.AreSame(symbolMock.Object, actual);
    }

    [TestMethod]
    public void CastAndValidateThrowsSymbolExceptionWhenNotAdsSymbol()
    {
        // Arrange
        var symbolMock = Mock.Of<ISymbol>();

        // Act
        // Assert
        _ = Assert.ThrowsException<SymbolException>(symbolMock.CastAndValidate);
    }
}