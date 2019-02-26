using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PlcInterface.Tests
{
    public abstract class ISymbolHandlerTestBase : ConnectionBase
    {
        public abstract IEnumerable<object[]> Data
        {
            get;
        }

        [TestMethod]
        public void GetSymbolInfo()
        {
            foreach (var item in Data)
            {
                // Arrange
                var symbolHandler = GetSymbolHandler();
                var ioName = item[0] as string;

                // Act
                var symbol = symbolHandler.GetSymbolinfo(ioName);

                // Assert
                Assert.IsNotNull(symbol);
                Assert.AreEqual(ioName, symbol.Name);
            }
        }

        [TestMethod]
        public void SymbolHandlerGetAll()
        {
            // Arrange
            var symbolHandler = GetSymbolHandler();

            // Act

            // Assert
            Assert.IsTrue(symbolHandler.AllSymbols.Count > 0);
        }

        protected override IMonitor GetMonitor()
        {
            throw new System.NotImplementedException();
        }

        protected override IReadWrite GetReadWrite()
        {
            throw new System.NotImplementedException();
        }
    }
}