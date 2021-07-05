using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests
{
    public abstract class IPlcConnectionTestBase : ConnectionBase
    {
        [TestMethod]
        public void OpenCloseConnection()
        {
            // Arrange
            var connection = GetPLCConnection();
            using var disposable = connection as IDisposable;

            // Act
            connection.Connect();
            var result = connection
                .SessionStream
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(5))
                .Wait();

            // Assert
            Assert.IsTrue(result.IsConnected);

            // Act
            connection.Disconnect();
            result = connection
                .SessionStream
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(5))
                .Wait();

            // Assert
            Assert.IsFalse(result.IsConnected);
            connection.Disconnect();
        }

        [TestMethod]
        public async Task OpenCloseConnectionAsync()
        {
            // Arrange
            var connection = GetPLCConnection();
            using var disposable = connection as IDisposable;

            // Act
            await connection.ConnectAsync();
            var result = await connection.SessionStream.FirstAsync();

            // Assert
            Assert.IsTrue(result.IsConnected);

            // Act
            await connection.DisconnectAsync();
            result = await connection.SessionStream.FirstAsync();

            // Assert
            Assert.IsFalse(result.IsConnected);
            await connection.DisconnectAsync();
        }

        protected override IMonitor GetMonitor()
            => throw new NotSupportedException();

        protected override IReadWrite GetReadWrite()
            => throw new NotSupportedException();

        protected override ISymbolHandler GetSymbolHandler()
            => throw new NotSupportedException();
    }
}