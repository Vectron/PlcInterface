using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests.DataTypes;
using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests;

public abstract class IReadValueTestBase
{
    private static IEnumerable<object[]> ReadTestData
        =>
        [
            ["BoolValue", true],
            ["ByteValue", byte.MaxValue],
            ["WordValue", ushort.MaxValue],
            ["DWordValue", uint.MaxValue],
            ["LWordValue", ulong.MaxValue],
            ["SIntValue", sbyte.MinValue],
            ["IntValue", short.MinValue],
            ["DIntValue", int.MinValue],
            ["LIntValue", long.MinValue],
            ["USIntValue", byte.MaxValue],
            ["UIntValue", ushort.MaxValue],
            ["UDIntValue", uint.MaxValue],
            ["ULIntValue", ulong.MaxValue],
            ["RealValue", -3.402823E+38F],
            ["LRealValue", -1.79769313486231E+308],
            ["TimeValue", TimeSpan.FromSeconds(1)],
            ["TimeOfDayValue", TimeSpan.FromHours(1)],
            ["LTimeValue", TimeSpan.FromTicks(10)],
            ["DateValue", new DateTimeOffset(2106, 02, 05, 0, 0, 0, TimeSpan.FromHours(1))],
            ["DateAndTimeValue", new DateTimeOffset(2106, 02, 05, 06, 28, 15, TimeSpan.FromHours(1))],
            ["StringValue", "Test String"],
            ["WStringValue", "Test WString"],
            ["EnumValue1", (int)TestEnum.Second],
        ];

    private static IEnumerable<object[]> ReadTestDataExtended
            =>
            [
            ["IntArray", DUT_TestStruct.Default.IntArray],
            ["MultiDimensionArray", DUT_TestStruct.Default.MultiDimensionArray],
            ["ComplexArray", DUT_TestStruct.Default.ComplexArray],
            ["MultiDimensionComplexArray", DUT_TestStruct.Default.MultiDimensionComplexArray],
                ["StructValue", DUT_TestStruct.Default],
                ["StructValue2", DUT_TestClass.Default],
            ];

    [TestMethod]
    public void ReadDynamicReadsAStructure()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var expected = DUT_TestStruct.Default;
        var ioName = $"ReadTestData.{nameof(ReadDynamicReadsAStructure)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = readWrite.ReadDynamic(ioName);

        // Assert
        AssertExtension.DUT_TestStructEquals(expected, actual);
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
        var ioName = $"ReadTestData.{nameof(ReadDynamicReadsAStructureAsync)}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = await readWrite.ReadDynamicAsync(ioName);

        // Assert
        AssertExtension.DUT_TestStructEquals(expected, actual);
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
    [DynamicData(nameof(ReadTestDataExtended))]
    [DataRow("EnumValue2", TestEnum.Second)]
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
            kv => $"ReadTestData.{nameof(ReadMultipleItemsInOneTransaction)}.{kv[0]}",
            kv => kv[1],
            StringComparer.Ordinal);

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = readWrite.Read(data.Keys);

        // Assert
        using var dataEnumerator = data.GetEnumerator();
        using var actualEnumerator = actual.GetEnumerator();

        Assert.AreEqual(data.Count, actual.Count);
        var multiAssert = new MultiAssert();

        while (dataEnumerator.MoveNext()
            && actualEnumerator.MoveNext())
        {
            multiAssert.Check(() => Assert.AreEqual(dataEnumerator.Current.Key, actualEnumerator.Current.Key));
            multiAssert.Check(() => Assert.That.ObjectEquals(dataEnumerator.Current.Value, actualEnumerator.Current.Value, dataEnumerator.Current.Key));
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
            kv => $"ReadTestData.{nameof(ReadMultipleItemsInOneTransactionAsync)}.{kv[0]}",
            kv => kv[1],
            StringComparer.Ordinal);

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = await readWrite.ReadAsync(data.Keys);

        // Assert
        using var dataEnumerator = data.GetEnumerator();
        using var actualEnumerator = actual.GetEnumerator();

        Assert.AreEqual(data.Count, actual.Count);
        var multiAssert = new MultiAssert();

        while (dataEnumerator.MoveNext()
            && actualEnumerator.MoveNext())
        {
            multiAssert.Check(() => Assert.AreEqual(dataEnumerator.Current.Key, actualEnumerator.Current.Key));
            multiAssert.Check(() => Assert.That.ObjectEquals(dataEnumerator.Current.Value, actualEnumerator.Current.Value, dataEnumerator.Current.Key));
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
        var ioName = $"ReadTestData.{nameof(ReadValueReturnsTheExpectedValue)}.{itemName}";

        // Act
        Assert.IsTrue(connection.Connect());
        var actual = readWrite.Read(ioName);

        // Assert
        Assert.That.ObjectEquals(expectedValue, actual, ioName);
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
        var ioName = $"ReadTestData.{nameof(ReadValueReturnsTheExpectedValueAsync)}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = await readWrite.ReadAsync(ioName);

        // Assert
        Assert.That.ObjectEquals(expectedValue, actual, ioName);
    }

    [TestMethod]
    public void WaitForValueThrowsExceptionOnTimeout()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"ReadTestData.{nameof(WaitForValueThrowsExceptionOnTimeout)}";

        // Act assert
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        _ = Assert.ThrowsException<TimeoutException>(() => readWrite.WaitForValue(ioName, filterValue: true, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void WaitForValueThrowsExceptionOnTimeoutAsync()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"ReadTestData.{nameof(WaitForValueThrowsExceptionOnTimeoutAsync)}";

        // Act Assert
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        _ = Assert.ThrowsExceptionAsync<TimeoutException>(async () => await readWrite.WaitForValueAsync(ioName, filterValue: false, TimeSpan.FromSeconds(1)).ConfigureAwait(false));
    }

    [DataTestMethod]
    [DynamicData(nameof(ReadTestData))]
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
        var ioName = $"ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = readWrite.Read<T>(ioName);

        // Assert
        Assert.IsNotNull(expectedValue);
        Assert.IsNotNull(actual);
        Assert.That.ObjectEquals(expectedValue, actual, ioName);
    }

    protected async Task ReadValueGenericAsync<T>(string itemName, T expectedValue, [CallerMemberName] string memberName = "")
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var actual = await readWrite.ReadAsync<T>(ioName).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(expectedValue);
        Assert.IsNotNull(actual);
        Assert.That.ObjectEquals(expectedValue, actual, ioName);
    }

    protected void WaitsForValueToChangeGeneric<T>(string itemName, T readValue, [CallerMemberName] string memberName = "")
        where T : notnull
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"ReadTestData.{memberName}.{itemName}";

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
        var ioName = $"ReadTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        await readWrite.WaitForValueAsync(ioName, readValue, TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);

        // Assert
    }
}
