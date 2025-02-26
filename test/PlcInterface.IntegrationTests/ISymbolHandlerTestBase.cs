using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.IntegrationTests;

public abstract class ISymbolHandlerTestBase
{
    [TestMethod]
    public void GetSymbolInfoReturnsTheSymbol()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var symbolHandler = serviceProvider.GetRequiredService<ISymbolHandler>();
        var ioName = $"SymbolTestData.{nameof(GetSymbolInfoReturnsTheSymbol)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var symbol = symbolHandler.GetSymbolInfo(ioName);

        // Assert
        Assert.IsNotNull(symbol);
        Assert.AreEqual(ioName, symbol.Name);
    }

    [TestMethod]
    public void SymbolHandlerGetAll()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var symbolHandler = serviceProvider.GetRequiredService<ISymbolHandler>();

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var count = symbolHandler.AllSymbols.Count;

        // Assert
        Assert.IsTrue(count > 0);
    }

    [TestMethod]
    public void TryGetSymbolInfoReturnsFalseWithInvalidData()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var symbolHandler = serviceProvider.GetRequiredService<ISymbolHandler>();
        var ioName = "ANonExistingSymbol";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var result = symbolHandler.TryGetSymbolInfo(ioName, out var symbol);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(symbol);
    }

    [TestMethod]
    public void TryGetSymbolInfoReturnsTrueWithValidData()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var symbolHandler = serviceProvider.GetRequiredService<ISymbolHandler>();
        var ioName = $"SymbolTestData.{nameof(TryGetSymbolInfoReturnsTrueWithValidData)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var result = symbolHandler.TryGetSymbolInfo(ioName, out var symbol);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(symbol);
        Assert.AreEqual(ioName, symbol.Name);
    }

    protected abstract IServiceProvider GetServiceProvider();
}
