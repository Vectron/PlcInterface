using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests.DataTypes;
using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests;

[DoNotParallelize]
public abstract class IMonitorTestBase
{
    protected abstract string DataRoot
    {
        get;
    }

    [DataTestMethod]
    [DataRow("BoolValue", typeof(bool))]
    [DataRow("ByteValue", typeof(byte))]
    [DataRow("WordValue", typeof(ushort))]
    [DataRow("DWordValue", typeof(uint))]
    [DataRow("LWordValue", typeof(ulong))]
    [DataRow("SIntValue", typeof(sbyte))]
    [DataRow("IntValue", typeof(short))]
    [DataRow("DIntValue", typeof(int))]
    [DataRow("LIntValue", typeof(long))]
    [DataRow("USIntValue", typeof(byte))]
    [DataRow("UIntValue", typeof(ushort))]
    [DataRow("UDIntValue", typeof(uint))]
    [DataRow("ULIntValue", typeof(ulong))]
    [DataRow("RealValue", typeof(float))]
    [DataRow("LRealValue", typeof(double))]
    [DataRow("TimeValue", typeof(TimeSpan))]
    [DataRow("TimeOfDayValue", typeof(TimeSpan))]
    [DataRow("LTimeValue", typeof(TimeSpan))]
    [DataRow("DateValue", typeof(DateTimeOffset))]
    [DataRow("DateAndTimeValue", typeof(DateTimeOffset))]
    [DataRow("StringValue", typeof(string))]
    [DataRow("WStringValue", typeof(string))]
    [DataRow("EnumValue1", typeof(int))]
    [DataRow("EnumValue2", typeof(TestEnum))]
    public void CanSubscribeToAllPlcTypes(string ioName, Type instanceType)
    {
        var method = typeof(IMonitorTestBase)
            .GetMethod(nameof(MonitorValueGenericHelper), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(instanceType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {nameof(MonitorValueGenericHelper)}.");

        _ = method.InvokeUnwrappedException(this, [ioName, nameof(CanSubscribeToAllPlcTypes)]);
    }

    [TestMethod]
    public void MonitorBeforeConnectDoesNotMatter()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        using var done = new ManualResetEvent(initialState: false);
        var ioName = $"{DataRoot}.MonitorTestData.{nameof(MonitorBeforeConnectDoesNotMatter)}";
        var original = false;

        // Act
        using var subscription = monitor.SubscribeIO<bool>(ioName, x =>
        {
            if (x != original)
            {
                _ = done.Set();
            }
        });

        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        original = readWrite.Read<bool>(ioName);
        readWrite.ToggleBool(ioName);
        var result = done.WaitOne(TimeSpan.FromSeconds(5));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    [TestMethod]
    public void RegisterUnregisterIO()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        using var done = new ManualResetEvent(initialState: false);
        var ioName = $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterIO)}";
        var results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        monitor.RegisterIO(ioName, 1000);
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
        using var originalsEnumerator = originals.OrderBy(x => x.Key, StringComparer.Ordinal).GetEnumerator();
        using var resultsEnumerator = results.OrderBy(x => x.Key, StringComparer.Ordinal).GetEnumerator();

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
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        using var done = new ManualResetEvent(initialState: false);
        var variables = new List<string>()
        {
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}1",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}2",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}3",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}4",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}5",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}6",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}7",
            $"{DataRoot}.MonitorTestData.{nameof(RegisterUnregisterMultipleIO)}8",
        };
        var results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        monitor.RegisterIO(variables);
        var originals = readWrite.Read(variables);
        using var subscription = monitor.SymbolStream.Subscribe(x =>
        {
            results.Add(x.Name, x.Value);

            if (originals.Count == results.Count)
            {
                _ = done.Set();
            }
        });

        var valuesToWrite = originals.ToDictionary(x => x.Key, x => (object)!(bool)x.Value, StringComparer.OrdinalIgnoreCase);
        readWrite.Write(valuesToWrite);
        var timeoutResult = done.WaitOne(TimeSpan.FromSeconds(10));
        monitor.UnregisterIO(variables);

        // Assert
        Assert.IsTrue(timeoutResult, string.Create(CultureInfo.InvariantCulture, $"Timeout, items processed {results.Count}/{originals.Count}"));
        Assert.AreEqual(originals.Count, results.Count);
        using var originalsEnumerator = originals.OrderBy(x => x.Key, StringComparer.Ordinal).GetEnumerator();
        using var resultsEnumerator = results.OrderBy(x => x.Key, StringComparer.Ordinal).GetEnumerator();

        while (originalsEnumerator.MoveNext() && resultsEnumerator.MoveNext())
        {
            var original = originalsEnumerator.Current;
            var result = resultsEnumerator.Current;
            Assert.AreEqual(original.Key, result.Key);
            Assert.AreNotEqual(original.Value, result.Value);
        }
    }

    [TestMethod]
    public void SubscribeIOShouldTriggerOnSubscribe()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        using var done = new ManualResetEvent(initialState: false);
        var ioName = $"{DataRoot}.MonitorTestData.{nameof(SubscribeIOShouldTriggerOnSubscribe)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var original = readWrite.Read<bool>(ioName);
        using var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
        readWrite.Write(ioName, !original);
        var result = done.WaitOne(TimeSpan.FromSeconds(1));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    [TestMethod]
    public void SubscribeIOUpdatesWhenValueChanges()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        using var done = new ManualResetEvent(initialState: false);
        var ioName = $"{DataRoot}.MonitorTestData.{nameof(SubscribeIOUpdatesWhenValueChanges)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var original = readWrite.Read<bool>(ioName);
        using var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
        readWrite.ToggleBool(ioName);
        var result = done.WaitOne(TimeSpan.FromSeconds(5));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    [TestMethod]
    public void SubscriptionsPersistReconnects()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        using var done = new ManualResetEvent(initialState: false);
        var ioName = $"{DataRoot}.MonitorTestData.{nameof(SubscriptionsPersistReconnects)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var original = readWrite.Read<bool>(ioName);
        using var subscription = monitor.SubscribeIO(ioName, !original, () => done.Set());
        connection.Disconnect();
        _ = connection.Connect();
        readWrite.ToggleBool(ioName);
        var result = done.WaitOne(TimeSpan.FromSeconds(5));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "IDisposableAnalyzers.Correctness",
        "IDISP017:Prefer using",
        Justification = "We want to specifically dispose the subscription")]
    public async Task SubscriptionsWillAllTriggerOnUpdate()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.MonitorTestData.{nameof(SubscriptionsWillAllTriggerOnUpdate)}";
        var results = new List<bool>();

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        using var subscription1 = monitor.SubscribeIO<bool>(ioName, results.Add, 100);
        using var subscription2 = monitor.SubscribeIO<bool>(ioName, results.Add, 100);
        using var subscription3 = monitor.SubscribeIO<bool>(ioName, results.Add, 100);
        using var subscription4 = monitor.SubscribeIO<bool>(ioName, results.Add, 100);
        await SubscribeCheckAsync(results, 4);

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

    protected abstract IServiceProvider GetServiceProvider();

    protected void MonitorValueGenericHelper<T>(string itemName, [CallerMemberName] string memberName = "")
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var monitor = serviceProvider.GetRequiredService<IMonitor>();
        using var done = new ManualResetEvent(initialState: false);
        var ioName = $"{DataRoot}.MonitorTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        using var subscription = monitor.SubscribeIO<T>(ioName, _ => done.Set(), 100);
        var result = done.WaitOne(TimeSpan.FromSeconds(10));

        // Assert
        Assert.IsTrue(result, "Timeout");
    }

    private static async Task SubscribeCheckAsync(List<bool> results, int expectedCount)
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
