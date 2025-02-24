using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace PlcInterface.OpcUa.Tests;

[TestClass]
public class IServiceCollectionExtensionTest
{
    private static ServiceProvider? provider;

    [ClassCleanup]
    public static void CleanDI()
        => provider?.Dispose();

    [ClassInitialize]
    public static void SetupDI(TestContext testContext)
        => provider = new ServiceCollection()
        .AddOptions()
        .AddSingleton<ILoggerFactory, NullLoggerFactory>()
        .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
        .AddOpcPLC()
        .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

    [TestMethod]
    public void IfBaseInterfaceRegistrationIsOverwrittenSpecificStillResolvesCorrectly()
    {
        // Arrange
        using var serviceProvider = new ServiceCollection()
              .AddOptions()
              .AddSingleton<ILoggerFactory, NullLoggerFactory>()
              .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
              .AddOpcPLC()
              .AddSingleton(x => Mock.Of<IPlcConnection>())
              .AddSingleton(x => Mock.Of<ISymbolHandler>())
              .AddSingleton(x => Mock.Of<IMonitor>())
              .AddSingleton(x => Mock.Of<IReadWrite>())
              .AddSingleton(x => Mock.Of<ITypeConverter>())
              .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        // Act
        var opcPlcConnection = serviceProvider.GetService<IOpcPlcConnection>();
        var opcSymbolHandler = serviceProvider.GetService<IOpcSymbolHandler>();
        var opcReadWrite = serviceProvider.GetService<IOpcReadWrite>();
        var opcMonitor = serviceProvider.GetService<IOpcMonitor>();
        var plcConnection = serviceProvider.GetService<IPlcConnection>();
        var symbolHandler = serviceProvider.GetService<ISymbolHandler>();
        var readWrite = serviceProvider.GetService<IReadWrite>();
        var monitor = serviceProvider.GetService<IMonitor>();

        // Assert
        Assert.IsInstanceOfType<PlcConnection>(opcPlcConnection);
        Assert.IsInstanceOfType<SymbolHandler>(opcSymbolHandler);
        Assert.IsInstanceOfType<ReadWrite>(opcReadWrite);
        Assert.IsInstanceOfType<Monitor>(opcMonitor);
        Assert.AreNotSame(opcPlcConnection, plcConnection);
        Assert.AreNotSame(opcSymbolHandler, symbolHandler);
        Assert.AreNotSame(opcReadWrite, readWrite);
        Assert.AreNotSame(opcMonitor, monitor);
    }

    [TestMethod]
    public void MonitorIsRegisterCorrect()
    {
        Assert.IsNotNull(provider);

        var opcMonitor = provider.GetRequiredService<IOpcMonitor>();
        var monitor = provider.GetRequiredService<IMonitor>();
        using var concrete = provider.GetService<Monitor>();

        Assert.IsNull(concrete);
        Assert.AreSame(opcMonitor, monitor);
        Assert.IsInstanceOfType<Monitor>(opcMonitor);
    }

    [TestMethod]
    public void OpcTypeConverterIsRegisteredCorrect()
    {
        Assert.IsNotNull(provider);

        var opcTypeConverter = provider.GetRequiredService<IOpcTypeConverter>();
        var typeConverter = provider.GetService<ITypeConverter>();
        var concrete = provider.GetService<OpcTypeConverter>();

        Assert.IsNull(concrete);
        Assert.IsNull(typeConverter);
        Assert.IsInstanceOfType<OpcTypeConverter>(opcTypeConverter);
    }

    [TestMethod]
    public void PlCConnectionIsRegisterCorrect()
    {
        Assert.IsNotNull(provider);

        var opcConnection = provider.GetRequiredService<IOpcPlcConnection>();
        var connection = provider.GetRequiredService<IPlcConnection>();
        var genericConnection = provider.GetRequiredService<IPlcConnection<Opc.Ua.Client.ISession>>();
        using var concrete = provider.GetService<PlcConnection>();

        Assert.IsNull(concrete);
        Assert.AreSame(opcConnection, connection);
        Assert.AreSame(opcConnection, genericConnection);
        Assert.IsInstanceOfType<PlcConnection>(opcConnection);
    }

    [TestMethod]
    public void ReadWriteIsRegisterCorrect()
    {
        Assert.IsNotNull(provider);

        var opcReadWrite = provider.GetRequiredService<IOpcReadWrite>();
        var readWrite = provider.GetRequiredService<IReadWrite>();
        using var concrete = provider.GetService<ReadWrite>();

        Assert.IsNull(concrete);
        Assert.AreSame(opcReadWrite, readWrite);
        Assert.IsInstanceOfType<ReadWrite>(opcReadWrite);
    }

    [TestMethod]
    public void SymbolHandlerIsRegisterCorrect()
    {
        Assert.IsNotNull(provider);

        var opcSymbolHandler = provider.GetRequiredService<IOpcSymbolHandler>();
        var symbolHandler = provider.GetRequiredService<ISymbolHandler>();
        using var concrete = provider.GetService<SymbolHandler>();

        Assert.IsNull(concrete);
        Assert.AreSame(opcSymbolHandler, symbolHandler);
        Assert.IsInstanceOfType<SymbolHandler>(opcSymbolHandler);
    }
}
