using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class StructActivatorTests
{
    [TestMethod]
    public void FailsToCreateTypeWhenThereAreNotEnoughProperties()
    {
        // Arrange
        var activator = new StructActivator(typeof(TestValueType));

        // Act
        var result = activator.TryCreateInstance((name, type) => null, 10, out var instance);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(instance);
    }

    [TestMethod]
    public void NullValueThrowsSymbolException()
    {
        // Arrange
        var activator = new StructActivator(typeof(TestValueType));
        void Action() => _ = activator.TryCreateInstance((name, type) => null, 1, out var instance);

        // Act Assert
        _ = Assert.ThrowsException<SymbolException>(Action);
    }

    private struct TestValueType
    {
        public int Value
        {
            get;
            set;
        }
    }
}