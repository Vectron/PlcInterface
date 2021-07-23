﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests
{
    [TestClass]
    public abstract class IMonitorTestBase : ConnectionBase
    {
        [TestMethod]
        public async Task MultipleSubscriptions()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            var ioName = "MonitorTestData.BoolValue8";
            var results = new List<bool>();

            // Act
            var subscription1 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
            var subscription2 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
            var subscription3 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
            var subscription4 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));

            // Assert
            using (subscription1)
            using (subscription2)
            using (subscription3)
            using (subscription4)
            {
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
        }

        [TestMethod]
        public void RegisterUnregisterIO()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            using var done = new ManualResetEvent(false);
            var ioName = "MonitorTestData.BoolValue1";

            // Act
            var original = readWrite.Read<bool>(ioName);
            var subscription = monitor.SymbolStream.Subscribe(x =>
              {
                  if (string.Equals(x.Name, ioName, StringComparison.OrdinalIgnoreCase))
                  {
                      var value = (bool)x.Value;
                      Assert.AreNotEqual(original, value);
                      _ = done.Set();
                  }
              });

            using (subscription)
            {
                monitor.RegisterIO(ioName, 1000);
                readWrite.ToggleBool(ioName);
                var result = done.WaitOne(TimeSpan.FromSeconds(30));

                // Assert
                Assert.IsTrue(result, "Timeout");
            }
        }

        [TestMethod]
        public void RegisterUnregisterMultipleIO()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            using var done = new ManualResetEvent(false);
            var hits = 0;
            var variables = Settings.GetMonitorMultiple();

            // Act
            var originals = readWrite.Read(variables);
            var subscription = monitor.SymbolStream.Subscribe(x =>
            {
                var value = (bool)x.Value;
                _ = originals.TryGetValue(x.Name, out var original);
                Assert.AreNotEqual((bool)original, value);
                hits++;

                if (originals.Count == hits)
                {
                    _ = done.Set();
                }
            });

            using (subscription)
            {
                monitor.RegisterIO(variables);

                var valuesToWrite = originals.ToDictionary(x => x.Key, x => (object)!(bool)x.Value, StringComparer.OrdinalIgnoreCase);
                readWrite.Write(valuesToWrite);
                var result = done.WaitOne(TimeSpan.FromSeconds(5));

                // Assert
                Assert.IsTrue(result, FormattableString.Invariant($"Timeout, items processed {hits}/{originals.Count}"));
            }
        }

        [TestMethod]
        public void SubscribeIO()
        {
            // Arrange
            var monitor = GetMonitor();
            var readWrite = GetReadWrite();
            using var done = new ManualResetEvent(false);
            var ioName = "MonitorTestData.BoolValue8";

            // Act
            var original = readWrite.Read<bool>(ioName);
            var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
            using (subscription)
            {
                readWrite.ToggleBool(ioName);
                var result = done.WaitOne(TimeSpan.FromSeconds(5));

                // Assert
                Assert.IsTrue(result, "Timeout");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(Settings.GetMonitorData2), typeof(Settings), DynamicDataSourceType.Method)]
        public void SubscribeIOShouldTriggerOnSubscribe(string ioName, Type instanceType)
        {
            var method = typeof(IMonitorTestBase)
                .GetMethod(nameof(MonitorValueGenericHelper), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(instanceType);
            try
            {
                _ = method.Invoke(this, new object[] { ioName });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        protected void MonitorValueGenericHelper<T>(string ioName)
        {
            // Arrange
            var monitor = GetMonitor();
            using var done = new ManualResetEvent(false);

            // Act
            using var subscription = monitor.SubscribeIO<T>(ioName, _ => done.Set());
            var result = done.WaitOne(TimeSpan.FromMilliseconds(2000));

            // Assert
            Assert.IsTrue(result, "Timeout");
        }

        private static async Task SubscribeCheckAsync(IList<bool> results, int expectedCount)
        {
            await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
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