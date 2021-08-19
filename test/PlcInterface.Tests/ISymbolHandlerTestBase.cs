using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests
{
    public abstract class ISymbolHandlerTestBase
    {
        [TestMethod]
        [DynamicData(nameof(Settings.GetMonitorData), typeof(Settings), DynamicDataSourceType.Method)]
        public void GetSymbolInfo(string ioName)
        {
            // Arrange
            var symbolHandler = GetSymbolHandler();

            // Act
            var symbol = symbolHandler.GetSymbolinfo(ioName);

            // Assert
            Assert.IsNotNull(symbol);
            Assert.AreEqual(ioName, symbol.Name);
        }

        [TestMethod]
        public void SymbolHandlerGetAll()
        {
            // Arrange
            var symbolHandler = GetSymbolHandler();

            // Act
            var count = symbolHandler.AllSymbols.Count;

            // Assert
            Assert.IsTrue(count > 0);
        }

        protected abstract ISymbolHandler GetSymbolHandler();
    }
}