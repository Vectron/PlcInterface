using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlcInterface.Tests
{
    [TestClass]
    public abstract class IMonitorTestBase : ConnectionBase
    {
        protected abstract IEnumerable<string> BooleanVarIONames
        {
            get;
        }

        [TestMethod]
        public async Task MultipleSubscriptions()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            var ioName = BooleanVarIONames.Last();
            var results = new List<bool>();

            // Act
            var subscription1 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
            var subscription2 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
            var subscription3 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
            var subscription4 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));

            // Assert
            readWrite.ToggleBool(ioName);
            await SubscribeCheckAsync(results, 4);
            subscription1.Dispose();

            readWrite.ToggleBool(ioName);
            await SubscribeCheckAsync(results, 3);
            subscription2.Dispose();

            readWrite.ToggleBool(ioName);
            await SubscribeCheckAsync(results, 2);
            subscription3.Dispose();

            readWrite.ToggleBool(ioName);
            await SubscribeCheckAsync(results, 1);
            subscription4.Dispose();

            readWrite.ToggleBool(ioName);
            await SubscribeCheckAsync(results, 0);
        }

        [TestMethod]
        public void RegisterUnregisterIO()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            var done = new ManualResetEvent(false);
            var ioName = BooleanVarIONames.First();

            // Act
            var original = readWrite.Read<bool>(ioName);
            var subscription = monitor.SymbolStream.Subscribe(x =>
              {
                  if (x.Name == ioName)
                  {
                      var value = (bool)x.Value;
                      Assert.AreNotEqual(original, value);
                      _ = done.Set();
                  }
              });

            monitor.RegisterIO(ioName, 1000);
            readWrite.ToggleBool(ioName);
            var result = done.WaitOne(TimeSpan.FromSeconds(30));

            // Assert
            Assert.IsTrue(result, "Timeout");
        }

        [TestMethod]
        public void RegisterUnregisterMultipleIO()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            var done = new ManualResetEvent(false);
            var hits = 0;

            // Act
            var originals = readWrite.Read(BooleanVarIONames);
            var subscription = monitor.SymbolStream.Subscribe(x =>
            {
                var value = (bool)x.Value;
                _ = originals.TryGetValue(x.Name, out object original);
                Assert.AreNotEqual((bool)original, value);
                hits++;

                if (originals.Count == hits)
                {
                    _ = done.Set();
                }
            });

            monitor.RegisterIO(BooleanVarIONames);

            var valuesToWrite = originals.ToDictionary(x => x.Key, x => (object)!(bool)x.Value);
            readWrite.Write(valuesToWrite);
            var result = done.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            Assert.IsTrue(result, $"Timeout, items processed {hits}/{originals.Count}");
        }

        [TestMethod]
        public void SubscribeIO()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            var done = new ManualResetEvent(false);
            var ioName = BooleanVarIONames.Last();

            // Act
            var original = readWrite.Read<bool>(ioName);
            var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
            readWrite.ToggleBool(ioName);
            var result = done.WaitOne(TimeSpan.FromSeconds(30));

            // Assert
            Assert.IsTrue(result, "Timeout");
        }

        private async Task SubscribeCheckAsync(IList<bool> results, int expectedCount)
        {
            await Task.Delay(500);
            Assert.AreEqual(expectedCount, results.Count);
            foreach (var result in results)
            {
                Console.Write(result);
                Console.Write(", ");
            }
            Console.WriteLine();
            results.Clear();
        }
    }
}