using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.OpcUa.Tests
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
               .AddOpcPLC()
               .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        [TestMethod]
        public void MonitorIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var opcMonitor = provider.GetRequiredService<IOpcMonitor>();
            var monitor = provider.GetRequiredService<IMonitor>();
            using var concrete = provider.GetService<Monitor>();

            Assert.IsNull(concrete);
            Assert.AreSame(opcMonitor, monitor);
        }

        [TestMethod]
        public void PlCConnectionIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var opcConnection = provider.GetRequiredService<IOpcPlcConnection>();
            var connection = provider.GetRequiredService<IPlcConnection>();
            var genericConnection = provider.GetRequiredService<IPlcConnection<Opc.Ua.Client.Session>>();
            using var concrete = provider.GetService<PlcConnection>();

            Assert.IsNull(concrete);
            Assert.AreSame(opcConnection, connection);
            Assert.AreSame(opcConnection, genericConnection);
        }

        [TestMethod]
        public void ReadWriteIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var opcReadWrite = provider.GetRequiredService<IOpcReadWrite>();
            var readWrite = provider.GetRequiredService<IReadWrite>();
            using var concrete = provider.GetService<ReadWrite>();

            Assert.IsNull(concrete);
            Assert.AreSame(opcReadWrite, readWrite);
        }

        [TestMethod]
        public void SymbolHandlerIsregisterCorrect()
        {
            Assert.IsNotNull(provider);

            var opcSymbolHandler = provider.GetRequiredService<IOpcSymbolHandler>();
            var symbolHandler = provider.GetRequiredService<ISymbolHandler>();
            using var concrete = provider.GetService<SymbolHandler>();

            Assert.IsNull(concrete);
            Assert.AreSame(opcSymbolHandler, symbolHandler);
        }
    }
}