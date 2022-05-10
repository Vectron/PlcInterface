using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests;

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

    [TestMethod]
    public void TryGetSymbolInfoReturnsFalseWithInvallidData()
    {
        // Arrange
        var symbolHandler = GetSymbolHandler();
        var ioName = "ANonExcistingSymbol";

        // Act
        var result = symbolHandler.TryGetSymbolinfo(ioName, out var symbol);

        // Assert
        Assert.AreEqual(false, result);
        Assert.IsNull(symbol);
    }

    [TestMethod]
    [DynamicData(nameof(Settings.GetMonitorData), typeof(Settings), DynamicDataSourceType.Method)]
    public void TryGetSymbolInfoReturnsTrueWithVallidData(string ioName)
    {
        // Arrange
        var symbolHandler = GetSymbolHandler();

        // Act
        var result = symbolHandler.TryGetSymbolinfo(ioName, out var symbol);

        // Assert
        Assert.AreEqual(true, result);
        Assert.IsNotNull(symbol);
        Assert.AreEqual(ioName, symbol.Name);
    }

    protected abstract ISymbolHandler GetSymbolHandler();
}