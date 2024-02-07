using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Common.Tests;

[TestClass]
public class IServiceCollectionExtensionTests
{
    private interface IDummyInterface : IDummyInterface2
    {
    }

    private interface IDummyInterface2
    {
    }

    [TestMethod]
    public void AddSingletonFactoryAddsTheTypesAsSingletons()
    {
        // Arrange
        var provider = new Mock<IServiceCollection>();

        // Act
        _ = provider.Object.AddSingletonFactory<DummyType, IDummyInterface2, IDummyInterface>();

        // Assert
        provider.Verify(
            x => x.Add(It.Is<ServiceDescriptor>(x =>
                x.Lifetime == ServiceLifetime.Singleton
                && ((x.ImplementationType == typeof(DummyType) && x.ServiceType == typeof(IDummyInterface))
                    || (x.ImplementationType == null && x.ServiceType == typeof(IDummyInterface2))))),
            Times.Exactly(2));
    }

    private sealed class DummyType : IDummyInterface
    {
    }
}
