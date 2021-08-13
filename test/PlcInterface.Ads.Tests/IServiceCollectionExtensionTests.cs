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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public Api")]
        public static void SetupDI(TestContext testContext)
            => provider = new ServiceCollection()
            .AddOptions()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddAdsPLC()
            .AddSingleton(Mock.Of<IPlcConnection>())
            .AddSingleton(Mock.Of<ISymbolHandler>())
            .AddSingleton(Mock.Of<IMonitor>())
            .AddSingleton(Mock.Of<IReadWrite>())
            .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        [TestMethod]
        public void MonitorIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsMonitor = provider.GetRequiredService<IAdsMonitor>();
            using var concrete = provider.GetService<Monitor>();

            Assert.IsNull(concrete);
            Assert.IsInstanceOfType(adsMonitor, typeof(Monitor));
        }

        [TestMethod]
        public void PlCConnectionIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsConnection = provider.GetRequiredService<IAdsPlcConnection>();
            var genericConnection = provider.GetRequiredService<IPlcConnection<TwinCAT.Ads.IAdsConnection>>();
            using var concrete = provider.GetService<PlcConnection>();

            Assert.IsNull(concrete);
            Assert.IsInstanceOfType(adsConnection, typeof(PlcConnection));
            Assert.AreSame(adsConnection, genericConnection);
        }

        [TestMethod]
        public void ReadWriteIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsReadWrite = provider.GetRequiredService<IAdsReadWrite>();
            var concrete = provider.GetService<ReadWrite>();

            Assert.IsNull(concrete);
            Assert.IsInstanceOfType(adsReadWrite, typeof(ReadWrite));
        }

        [TestMethod]
        public void SymbolHandlerIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsSymbolHandler = provider.GetRequiredService<IAdsSymbolHandler>();
            using var concrete = provider.GetService<SymbolHandler>();

            Assert.IsNull(concrete);
            Assert.IsInstanceOfType(adsSymbolHandler, typeof(SymbolHandler));
        }
    }
}