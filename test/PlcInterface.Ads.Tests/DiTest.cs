using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Ads.Tests
{
    [TestClass]
    public class DiTest
    {
        private static ServiceProvider? provider;

        [ClassCleanup]
        public static void CleanDI()
            => provider?.Dispose();

        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public Api")]
        public static void SetupDI(TestContext testContext)
            => provider = new ServiceCollection()
               .AddOptions()
               .AddSingleton<ILoggerFactory, NullLoggerFactory>()
               .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
               .AddAdsPLC()
               .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        [TestMethod]
        public void MonitorIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var adsMonitor = provider.GetRequiredService<IAdsMonitor>();
            var monitor = provider.GetRequiredService<IMonitor>();
            using var concrete = provider.GetService<Monitor>();

            Assert.IsNull(concrete);
            Assert.AreSame(adsMonitor, monitor);
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
        }
    }
}