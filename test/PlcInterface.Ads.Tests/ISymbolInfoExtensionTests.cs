using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests
{
    [TestClass]
    public class ISymbolInfoExtensionTests
    {
        [TestMethod]
        public void CastAndValidateReturnsSymbolInfo()
        {
            // Arrange
            ISymbolInfo symbolInfo = new SymbolInfo(Mock.Of<ISymbol>());

            // Act
            var actual = symbolInfo.CastAndValidate();

            // Assert
            Assert.IsInstanceOfType(actual, typeof(SymbolInfo));
            Assert.AreSame(symbolInfo, actual);
        }

        [TestMethod]
        public void CastAndValidateThrowsSymbolExceptionWhenNotAdsSymbol()
        {
            // Arrange
            var symbolMock = Mock.Of<ISymbolInfo>();

            // Act
            // Assert
            _ = Assert.ThrowsException<SymbolException>(symbolMock.CastAndValidate);
        }
    }
}