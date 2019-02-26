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
            await Task.Delay(400);
            Assert.IsTrue(results.Count == 4);
            foreach (var result in results)
            {
                Console.Write(result);
                Console.Write(", ");
            }
            results.Clear();
            subscription1.Dispose();

            readWrite.ToggleBool(ioName);
            await Task.Delay(400);
            Assert.IsTrue(results.Count == 3);
            foreach (var result in results)
            {
                Console.Write(result);
                Console.Write(", ");
            }
            results.Clear();
            subscription2.Dispose();

            readWrite.ToggleBool(ioName);
            await Task.Delay(400);
            Assert.IsTrue(results.Count == 2);
            foreach (var result in results)
            {
                Console.Write(result);
                Console.Write(", ");
            }
            results.Clear();
            subscription3.Dispose();

            readWrite.ToggleBool(ioName);
            await Task.Delay(400);
            Assert.IsTrue(results.Count == 1);
            foreach (var result in results)
            {
                Console.Write(result);
                Console.Write(", ");
            }
            results.Clear();
            subscription4.Dispose();

            readWrite.ToggleBool(ioName);
            await Task.Delay(400);
            Assert.IsTrue(results.Count == 0);
            foreach (var result in results)
            {
                Console.Write(result);
                Console.Write(", ");
            }
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
            monitor.SymbolStream.Subscribe(x =>
            {
                if (x.Name == ioName)
                {
                    var value = (bool)x.Value;
                    Assert.AreNotEqual(original, value);
                    done.Set();
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
            monitor.SymbolStream.Subscribe(x =>
            {
                var value = (bool)x.Value;
                originals.TryGetValue(x.Name, out object original);
                Assert.AreNotEqual((bool)original, value);
                hits++;

                if (originals.Count == hits)
                {
                    done.Set();
                }
            });

            monitor.RegisterIO(BooleanVarIONames);

            var valuesToWrite = originals.ToDictionary(x => x.Key, x => (object)!(bool)x.Value);
            readWrite.Write(valuesToWrite);
            var result = done.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            Assert.IsTrue(result, $"Timeout, items processed {hits}");
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
            monitor.SubscribeIO(ioName, !original, () => done.Set());
            readWrite.ToggleBool(ioName);
            var result = done.WaitOne(TimeSpan.FromSeconds(30));

            // Assert
            Assert.IsTrue(result, "Timeout");
        }
    }
}