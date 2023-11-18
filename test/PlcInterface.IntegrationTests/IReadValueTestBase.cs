using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests.DataTypes;
using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests;

public abstract class IReadValueTestBase
{
    protected abstract string DataRoot
    {
        get;
    }

    private static IEnumerable<object[]> ReadTestData
        => new List<object[]>()
        {
            new object[] { "BoolValue", true },
            new object[] { "ByteValue", byte.MaxValue },
            new object[] { "WordValue", ushort.MaxValue },
            new object[] { "DWordValue", uint.MaxValue },
            new object[] { "LWordValue", ulong.MaxValue },
            new object[] { "SIntValue", sbyte.MinValue },
            new object[] { "IntValue", short.MinValue },
            new object[] { "DIntValue", int.MinValue },
            new object[] { "LIntValue", long.MinValue },
            new object[] { "USIntValue", byte.MaxValue },
            new object[] { "UIntValue", ushort.MaxValue },
            new object[] { "UDIntValue", uint.MaxValue },
            new object[] { "ULIntValue", ulong.MaxValue },
            new object[] { "RealValue", -3.402823E+38F },
            new object[] { "LRealValue", -1.79769313486231E+308 },
            new object[] { "TimeValue", TimeSpan.FromSeconds(1) },
            new object[] { "TimeOfDayValue", TimeSpan.FromHours(1) },
            new object[] { "LTimeValue", TimeSpan.FromTicks(10) },
            new object[] { "DateValue", new DateTimeOffset(2106, 02, 05, 0, 0, 0, TimeSpan.FromHours(1)) },
            new object[] { "DateAndTimeValue", new DateTimeOffset(2106, 02, 05, 06, 28, 15, TimeSpan.FromHours(1)) },
            new object[] { "StringValue", "Test String" },
            new object[] { "WStringValue", "Test WString" },
            new object[] { "EnumValue1", (int)TestEnum.Second },
        };

    private static IEnumerable<object[]> ReadTestDataExtended
            => new List<object[]>()
            {
                new object[]
                {
                    "IntArray",
                    new short[]
                    {
                        1000,
                        1001,
                        1002,
                        1003,
                        1004,
                        1005,
                        1006,
                        1007,
                        1008,
                        1009,
                        1010,
                    },
                },
                new object[]
                {
                    "MultiDimensionArray",
                    new short[,,]
                    {
                        {
                            { 0100, 0200, 0300, 0400 },
                            { 0500, 0600, 0700, 0800 },
                            { 0900, 1000, 1100, 1200 },
                        },
                        {
                            { 1300, 1400, 1500, 1600 },
                            { 1700, 1800, 1900, 2000 },
                            { 2100, 2200, 2300, 2400 },
                        },
                    },
                },
                new object[]
                {
                    "ComplexArray",
                    new DUT_TestStruct2[]
                    {
                        DUT_TestStruct2.Default,
                        DUT_TestStruct2.Default,
                        DUT_TestStruct2.Default,
                    },
                },
                new object[]
                {
                    "MultiDimensionComplexArray",
                    new DUT_TestStruct2[,,]
                    {
                        {
                            {
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                            },
                            {
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                            },
                            {
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                            },
                        },
                        {
                            {
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                            },
                            {
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                            },
                            {
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                                DUT_TestStruct2.Default,
                            },
                        },
                    },
                },
                new object[] { "StructValue", DUT_TestStruct.Default },
                new object[] { "StructValue2", DUT_TestClass.Default },
            };

    [TestMethod]
    public void ReadDynamicReadsAStructure()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var expected = DUT_TestStruct.Default;
        var ioName = $"{DataRoot}.ReadTestData.{nameof(ReadDynamicReadsAStructure)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var structValue = readWrite.ReadDynamic(ioName);

        // Assert
        AssertExtension.DUT_TestStructEquals(expected, structValue);
    }

    [TestMethod]
    public async Task ReadDynamicReadsAStructureAsync()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var expected = DUT_TestStruct.Default;
        var ioName = $"{DataRoot}.ReadTestData.{nameof(ReadDynamicReadsAStructureAsync)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var structValue = await readWrite.ReadDynamicAsync(ioName);

        // Assert
        AssertExtension.DUT_TestStructEquals(expected, structValue);
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [DynamicData(nameof(ReadTestDataExtended))]
    [DataRow("EnumValue2", TestEnum.Second)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public void ReadGenericReturnsTheRequestedType(string ioName, object value)
    {
        var instanceType = value.GetType();
        var method = typeof(IReadValueTestBase)
            .GetMethod(nameof(ReadValueGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(instanceType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {nameof(ReadValueGeneric)}.");

        _ = method.InvokeUnwrappedException(this, [ioName, value, nameof(ReadGenericReturnsTheRequestedType)]);
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [DynamicData(nameof(ReadTestDataExtended))]
    [DataRow("EnumValue2", TestEnum.Second)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public async Task ReadGenericReturnsTheRequestedTypeAsync(string ioName, object value)
    {
        var instanceType = value.GetType();
        var method = typeof(IReadValueTestBase)
            .GetMethod(nameof(ReadValueGenericAsync), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(instanceType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {nameof(ReadValueGenericAsync)}.");

        await method.InvokeAsyncUnwrappedException(this, [ioName, value, nameof(ReadGenericReturnsTheRequestedTypeAsync)]);
    }

    [TestMethod]
    public void ReadMultipleItemsInOneTransaction()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var data = ReadTestData.Concat(ReadTestDataExtended).ToDictionary(
            kv => $"{DataRoot}.ReadTestData.{nameof(ReadMultipleItemsInOneTransaction)}.{kv[0]}",
            kv => kv[1],
            StringComparer.Ordinal);

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var value = readWrite.Read(data.Keys);

        // Assert
        using var dataEnumerator = data.GetEnumerator();
        using var valueEnumerator = value.GetEnumerator();

        Assert.AreEqual(data.Count, value.Count);
        var multiAssert = new MultiAssert();

        while (dataEnumerator.MoveNext()
            && valueEnumerator.MoveNext())
        {
            multiAssert.Check(() => Assert.AreEqual(dataEnumerator.Current.Key, valueEnumerator.Current.Key));
            multiAssert.Check(() => Assert.That.ObjectEquals(dataEnumerator.Current.Value, valueEnumerator.Current.Value, dataEnumerator.Current.Key));
        }

        multiAssert.Assert();
    }

    [TestMethod]
    public async Task ReadMultipleItemsInOneTransactionAsync()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var data = ReadTestData.Concat(ReadTestDataExtended).ToDictionary(
            kv => $"{DataRoot}.ReadTestData.{nameof(ReadMultipleItemsInOneTransactionAsync)}.{kv[0]}",
            kv => kv[1],
            StringComparer.Ordinal);

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var value = await readWrite.ReadAsync(data.Keys);

        // Assert
        using var dataEnumerator = data.GetEnumerator();
        using var valueEnumerator = value.GetEnumerator();

        Assert.AreEqual(data.Count, value.Count);
        var multiAssert = new MultiAssert();

        while (dataEnumerator.MoveNext()
            && valueEnumerator.MoveNext())
        {
            multiAssert.Check(() => Assert.AreEqual(dataEnumerator.Current.Key, valueEnumerator.Current.Key));
            multiAssert.Check(() => Assert.That.ObjectEquals(dataEnumerator.Current.Value, valueEnumerator.Current.Value, dataEnumerator.Current.Key));
        }

        multiAssert.Assert();
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [DynamicData(nameof(ReadTestDataExtended))]
    public void ReadValueReturnsTheExpectedValue(string itemName, object expectedValue)
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{nameof(ReadValueReturnsTheExpectedValue)}.{itemName}";

        // Act
        Assert.IsTrue(connection.Connect());
        var value = readWrite.Read(ioName);

        // Assert
        Assert.That.ObjectEquals(expectedValue, value, ioName);
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [DynamicData(nameof(ReadTestDataExtended))]
    public async Task ReadValueReturnsTheExpectedValueAsync(string itemName, object expectedValue)
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{nameof(ReadValueReturnsTheExpectedValueAsync)}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var value = await readWrite.ReadAsync(ioName);

        // Assert
        Assert.That.ObjectEquals(expectedValue, value, ioName);
    }

    [TestMethod]
    public void WaitForValueThrowsExceptionOnTimeout()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{nameof(WaitForValueThrowsExceptionOnTimeout)}";

        // Act assert
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        _ = Assert.ThrowsException<TimeoutException>(() => readWrite.WaitForValue(ioName, true, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void WaitForValueThrowsExceptionOnTimeoutAsync()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{nameof(WaitForValueThrowsExceptionOnTimeoutAsync)}";

        // Act Assert
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        _ = Assert.ThrowsExceptionAsync<TimeoutException>(async () => await readWrite.WaitForValueAsync(ioName, false, TimeSpan.FromSeconds(1)).ConfigureAwait(false));
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public void WaitsForValueToChange(string ioName, object readValue)
    {
        var instanceType = readValue.GetType();
        var method = typeof(IReadValueTestBase)
            .GetMethod(nameof(WaitsForValueToChangeGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(instanceType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {nameof(WaitsForValueToChangeGeneric)}.");

        _ = method.InvokeUnwrappedException(this, [ioName, readValue, nameof(WaitsForValueToChange)]);
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
    public async Task WaitsForValueToChangeAsync(string ioName, object readValue)
    {
        var instanceType = readValue.GetType();
        var method = typeof(IReadValueTestBase)
            .GetMethod(nameof(WaitsForValueToChangeGenericAsync), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(instanceType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {nameof(WaitsForValueToChangeGenericAsync)}.");

        await method.InvokeAsyncUnwrappedException(this, [ioName, readValue, nameof(WaitsForValueToChangeAsync)]);
    }

    protected abstract IServiceProvider GetServiceProvider();

    protected void ReadValueGeneric<T>(string itemName, T expectedValue, [CallerMemberName] string memberName = "")
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var value = readWrite.Read<T>(ioName);

        // Assert
        Assert.IsNotNull(expectedValue);
        Assert.IsNotNull(value);
        Assert.That.ObjectEquals(expectedValue, value, ioName);
    }

    protected async Task ReadValueGenericAsync<T>(string itemName, T expectedValue, [CallerMemberName] string memberName = "")
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var value = await readWrite.ReadAsync<T>(ioName).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(expectedValue);
        Assert.IsNotNull(value);
        Assert.That.ObjectEquals(expectedValue, value, ioName);
    }

    protected void WaitsForValueToChangeGeneric<T>(string itemName, T readValue, [CallerMemberName] string memberName = "")
        where T : notnull
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        readWrite.WaitForValue(ioName, readValue, TimeSpan.FromMilliseconds(1000));

        // Assert
    }

    protected async Task WaitsForValueToChangeGenericAsync<T>(string itemName, T readValue, [CallerMemberName] string memberName = "")
        where T : notnull
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        await readWrite.WaitForValueAsync(ioName, readValue, TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);

        // Assert
    }
}