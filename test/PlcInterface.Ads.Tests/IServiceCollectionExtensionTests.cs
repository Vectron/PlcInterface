using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Ads.Tests
{
    [TestClass]
    public class IServiceCollectionExtensionTests
    {
        private static ServiceProvider? provider;

        [ClassCleanup]
        public static void CleanDI()
            => provider!.Dispose();

        [ClassInitialize]
        public static void SetupDI(TestContext testContext) =>
            provider = new ServiceCollection()
                .AddOptions()
                .AddSingleton<ILoggerFactory, NullLoggerFactory>()
                .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
                .AddAdsPLC()
                .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        [TestMethod]
        public void AdsTypeConverterIsRegisteredCorrect()
        {
            Assert.IsNotNull(provider);

            var adsTypeConverter = provider.GetRequiredService<IAdsTypeConverter>();
            var typeConverter = provider.GetService<ITypeConverter>();
            var concrete = provider.GetService<AdsTypeConverter>();

            Assert.IsNull(concrete);
            Assert.IsNull(typeConverter);
            Assert.IsInstanceOfType(adsTypeConverter, typeof(AdsTypeConverter));
        }

        [TestMethod]
        public void IfBaseInterfaceRegistrationIsOverwrittenSpecificStillResolvesCorrectly()
        {
            // Arrange
            using var serviceProvider = new ServiceCollection()
                  .AddOptions()
                  .AddSingleton<ILoggerFactory, NullLoggerFactory>()
                  .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
                  .AddAdsPLC()
                  .AddSingleton(x => Mock.Of<IPlcConnection>())
                  .AddSingleton(x => Mock.Of<ISymbolHandler>())
                  .AddSingleton(x => Mock.Of<IMonitor>())
                  .AddSingleton(x => Mock.Of<IReadWrite>())
                  .AddSingleton(x => Mock.Of<IAdsTypeConverter>())
                  .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

            // Act
            var adsPlcConnection = serviceProvider.GetService<IAdsPlcConnection>();
            var adsSymbolHandler = serviceProvider.GetService<IAdsSymbolHandler>();
            var adsReadWrite = serviceProvider.GetService<IAdsReadWrite>();
            var adsMonitor = serviceProvider.GetService<IAdsMonitor>();
            var plcConnection = serviceProvider.GetService<IPlcConnection>();
            var symbolHandler = serviceProvider.GetService<ISymbolHandler>();
            var readWrite = serviceProvider.GetService<IReadWrite>();
            var monitor = serviceProvider.GetService<IMonitor>();

            // Assert
            Assert.IsInstanceOfType(adsPlcConnection, typeof(PlcConnection));
            Assert.IsInstanceOfType(adsSymbolHandler, typeof(SymbolHandler));
            Assert.IsInstanceOfType(adsReadWrite, typeof(ReadWrite));
            Assert.IsInstanceOfType(adsMonitor, typeof(Monitor));
            Assert.AreNotSame(adsPlcConnection, plcConnection);
            Assert.AreNotSame(adsSymbolHandler, symbolHandler);
            Assert.AreNotSame(adsReadWrite, readWrite);
            Assert.AreNotSame(adsMonitor, monitor);
        }

        [TestMethod]
        public void MonitorIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsMonitor = provider.GetRequiredService<IAdsMonitor>();
            var monitor = provider.GetRequiredService<IMonitor>();
            using var concrete = provider.GetService<Monitor>();

            Assert.IsNull(concrete);
            Assert.AreSame(adsMonitor, monitor);
            Assert.IsInstanceOfType(adsMonitor, typeof(Monitor));
        }

        [TestMethod]
        public void PlCConnectionIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsConnection = provider.GetRequiredService<IAdsPlcConnection>();
            var connection = provider.GetRequiredService<IPlcConnection>();
            var genericConnection = provider.GetRequiredService<IPlcConnection<TwinCAT.Ads.IAdsConnection>>();
            using var concrete = provider.GetService<PlcConnection>();

            Assert.IsNull(concrete);
            Assert.AreSame(adsConnection, connection);
            Assert.AreSame(adsConnection, genericConnection);
            Assert.IsInstanceOfType(adsConnection, typeof(PlcConnection));
        }

        [TestMethod]
        public void ReadWriteIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsReadWrite = provider.GetRequiredService<IAdsReadWrite>();
            var readWrite = provider.GetRequiredService<IReadWrite>();
            var concrete = provider.GetService<ReadWrite>();

            Assert.IsNull(concrete);
            Assert.AreSame(adsReadWrite, readWrite);
            Assert.IsInstanceOfType(adsReadWrite, typeof(ReadWrite));
        }

        [TestMethod]
        public void SymbolHandlerIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsSymbolHandler = provider.GetRequiredService<IAdsSymbolHandler>();
            var symbolHandler = provider.GetRequiredService<ISymbolHandler>();
            using var concrete = provider.GetService<SymbolHandler>();

            Assert.IsNull(concrete);
            Assert.AreSame(adsSymbolHandler, symbolHandler);
            Assert.IsInstanceOfType(adsSymbolHandler, typeof(SymbolHandler));
        }
    }
}