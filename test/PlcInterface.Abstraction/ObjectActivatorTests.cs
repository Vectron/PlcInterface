using System.Reflection;
using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class ObjectActivatorTests
{
    [TestMethod]
    public void DeclaringTypeIsNullThrowsArgumentException()
    {
        // Arrange
        var constructorInfo = Mock.Of<ConstructorInfo>(x => x.DeclaringType == null);

        // Act Assert
        _ = Assert.ThrowsExactly<ArgumentException>(() =>
        {
            var objectActivator = new ObjectActivator(constructorInfo);
            var type = objectActivator.GetType();
        });
    }

    [TestMethod]
    public void FailsCreateInstanceIfParameterNameIsNull()
    {
        // Arrange
        var type = MyTypeBuilder.CompileResultType();
        var constructor = type.GetConstructor([typeof(byte)])!;
        var parameterInfo = constructor.GetParameters()[0];
        Assert.IsNull(parameterInfo.Name);
        var activator = new ObjectActivator(constructor!);

        // Act
        var result = activator.TryCreateInstance((name, type) => null, 1, out var instance);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(instance);
    }

    [TestMethod]
    public void NullValueThrowsSymbolException()
    {
        // Arrange
        var constructor = typeof(TestClass)
            .GetConstructors()
            .First(x => x.GetParameters().Length == 1);
        var activator = new ObjectActivator(constructor);
        void Action() => _ = activator.TryCreateInstance((name, type) => null, 1, out var instance);

        // Act Assert
        _ = Assert.ThrowsExactly<SymbolException>(Action);
    }

    private sealed class TestClass
    {
        public TestClass()
            => Value = 99;

        public TestClass(int value)
            => Value = value;

        public int Value
        {
            get;
            set;
        }
    }
}
