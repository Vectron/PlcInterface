using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.Tests.Extension;

namespace PlcInterface.Tests;

[DoNotParallelize]
public abstract class IMonitorTestBase
{
    [TestMethod]
    public void MonitorBeforeConnectDoesntMatter()
    {
        // Arrange
        var connection = GetPLCConnection();
        var monitor = GetMonitor();
        var readWrite = GetReadWrite();
        using var done = new ManualResetEvent(false);
        var ioName = "MonitorTestData.BoolValue8";

        // Act
        var original = readWrite.Read<bool>(ioName);
        connection.Disconnect();
        using var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
        connection.Connect();
        readWrite.ToggleBool(ioName);
        var result = done.WaitOne(TimeSpan.FromSeconds(5));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    [TestMethod]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP017:Prefer using.", Justification = "Need to dispose for test flow")]
    public async Task MultipleSubscriptions()
    {
        // Arrange
        var monitor = GetMonitor();
        var readWrite = GetReadWrite();
        var ioName = "MonitorTestData.BoolValue8";
        var results = new List<bool>();

        // Act
        using var subscription1 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
        using var subscription2 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
        using var subscription3 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));
        using var subscription4 = monitor.SubscribeIO<bool>(ioName, x => results.Add(x));

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
        using var done = new ManualResetEvent(false);
        var ioName = "MonitorTestData.BoolValue1";
        var results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        monitor.RegisterIO(ioName, 1000);

        // Act
        var originals = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { ioName, readWrite.Read<bool>(ioName) } };
        using var subscription = monitor.SymbolStream.Subscribe(x =>
          {
              if (string.Equals(x.Name, ioName, StringComparison.OrdinalIgnoreCase))
              {
                  results.Add(x.Name, x.Value);
                  _ = done.Set();
              }
          });

        readWrite.ToggleBool(ioName);
        var timeoutResult = done.WaitOne(TimeSpan.FromSeconds(30));
        monitor.UnregisterIO(ioName);

        // Assert
        Assert.IsTrue(timeoutResult, "Timeout");
        Assert.AreEqual(originals.Count, results.Count);
        using var originalsEnumerator = originals.OrderBy(x => x.Key).GetEnumerator();
        using var resultsEnumerator = results.OrderBy(x => x.Key).GetEnumerator();

        while (originalsEnumerator.MoveNext() && resultsEnumerator.MoveNext())
        {
            var original = originalsEnumerator.Current;
            var result = resultsEnumerator.Current;
            Assert.AreEqual(original.Key, result.Key);
            Assert.AreNotEqual(original.Value, result.Value);
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
        var results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        monitor.RegisterIO(variables);

        // Act
        var originals = readWrite.Read(variables);
        using var subscription = monitor.SymbolStream.Subscribe(x =>
        {
            results.Add(x.Name, x.Value);
            hits++;

            if (originals.Count == hits)
            {
                _ = done.Set();
            }
        });

        var valuesToWrite = originals.ToDictionary(x => x.Key, x => (object)!(bool)x.Value, StringComparer.OrdinalIgnoreCase);
        readWrite.Write(valuesToWrite);
        var timeoutResult = done.WaitOne(TimeSpan.FromSeconds(5));
        monitor.UnregisterIO(variables);

        // Assert
        Assert.IsTrue(timeoutResult, FormattableString.Invariant($"Timeout, items processed {hits}/{originals.Count}"));
        Assert.AreEqual(originals.Count, results.Count);
        using var originalsEnumerator = originals.OrderBy(x => x.Key).GetEnumerator();
        using var resultsEnumerator = results.OrderBy(x => x.Key).GetEnumerator();

        while (originalsEnumerator.MoveNext() && resultsEnumerator.MoveNext())
        {
            var original = originalsEnumerator.Current;
            var result = resultsEnumerator.Current;
            Assert.AreEqual(original.Key, result.Key);
            Assert.AreNotEqual(original.Value, result.Value);
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
        using var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
        readWrite.ToggleBool(ioName);
        var result = done.WaitOne(TimeSpan.FromSeconds(5));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    [DataTestMethod]
    [DynamicData(nameof(Settings.GetMonitorData2), typeof(Settings), DynamicDataSourceType.Method)]
    public void SubscribeIOShouldTriggerOnSubscribe(string ioName, Type instanceType)
    {
        var method = typeof(IMonitorTestBase)
            .GetMethod(nameof(MonitorValueGenericHelper), BindingFlags.NonPublic | BindingFlags.Instance)
            .MakeGenericMethod(instanceType);
        _ = method.InvokeUnwrappedException(this, new object[] { ioName });
    }

    [TestMethod]
    public void SubscriptionsPersistReconnects()
    {
        // Arrange
        var connection = GetPLCConnection();
        var monitor = GetMonitor();
        var readWrite = GetReadWrite();
        using var done = new ManualResetEvent(false);
        var ioName = "MonitorTestData.BoolValue8";

        // Act
        var original = readWrite.Read<bool>(ioName);
        using var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
        connection.Disconnect();
        connection.Connect();
        readWrite.ToggleBool(ioName);
        var result = done.WaitOne(TimeSpan.FromSeconds(5));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    protected abstract IMonitor GetMonitor();

    protected abstract IPlcConnection GetPLCConnection();

    protected abstract IReadWrite GetReadWrite();

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