using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests.DataTypes;
using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests;

[DoNotParallelize]
public abstract class IWriteValueTestBase
{
    private static IEnumerable<object[]> WriteTestData
        =>
        [
            ["BoolValue", false],
            ["ByteValue", byte.MinValue],
            ["WordValue", ushort.MinValue],
            ["DWordValue", uint.MinValue],
            ["LWordValue", ulong.MinValue],
            ["SIntValue", sbyte.MaxValue],
            ["IntValue", short.MaxValue],
            ["DIntValue", int.MaxValue],
            ["LIntValue", long.MaxValue],
            ["USIntValue", byte.MinValue],
            ["UIntValue", ushort.MinValue],
            ["UDIntValue", uint.MinValue],
            ["ULIntValue", ulong.MinValue],
            ["RealValue", 3.402823E+38F],
            ["LRealValue", 1.79769313486231E+308],
            ["TimeValue1", TimeSpan.FromSeconds(3)],
            ["TimeValue2", 3000u, TimeSpan.FromSeconds(3)],
            ["TimeOfDayValue1", TimeSpan.FromHours(10)],
            ["TimeOfDayValue2", 36000000u, TimeSpan.FromHours(10)],
            ["LTimeValue1", TimeSpan.FromTicks(100)],
            ["LTimeValue2", 10000ul, TimeSpan.FromTicks(100)],
            ["DateValue1", new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1))],
            ["DateValue2", new DateTime(2019, 02, 21), new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1))],
            ["DateAndTimeValue1", new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1))],
            ["DateAndTimeValue2", new DateTime(2019, 02, 21, 12, 15, 10), new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1))],
            ["StringValue", "new Test String"],
            ["WStringValue", "new Test WString"],
            ["EnumValue1", TestEnum.Third, (int)TestEnum.Third],
            ["EnumValue2", (short)TestEnum.Third, (int)TestEnum.Third],
            ["EnumValue3", (int)TestEnum.Third],
        ];

    private static IEnumerable<object[]> WriteTestDataExtended
        =>
        [
            [
                "IntArray",
                new short[] { 10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 },
            ],
            [
                "MultiDimensionArray",
                new short[,,]
                {
                    {
                        { 01000, 02000, 03000, 04000 },
                        { 05000, 06000, 07000, 08000 },
                        { 09000, 10000, 11000, 12000 },
                    },
                    {
                        { 13000, 14000, 15000, 16000 },
                        { 17000, 18000, 19000, 20000 },
                        { 21000, 22000, 23000, 24000 },
                    },
                },
            ],
            [
                "ComplexArray",
                new DUT_TestStruct2[] { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
            ],
            [
                "MultiDimensionComplexArray",
                new DUT_TestStruct2[,,]
                {
                    {
                        { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
                        { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
                        { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
                    },
                    {
                        { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
                        { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
                        { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
                    },
                },
            ],
            ["StructValue1", DUT_TestStruct.Write],
            ["StructValue2", DUT_TestClass.Write],
            ["Nested1", DUT_TestStruct2.Write],
            ["Nested2", DUT_TestClass2.Write],
        ];

    [TestMethod]
    public void ToggleBool()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = "WriteTestData.ToggleBool";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        var original = readWrite.Read<bool>(ioName);
        readWrite.ToggleBool(ioName);
        var newValue = readWrite.Read<bool>(ioName);

        // Assert
        Assert.AreNotEqual(original, newValue);
    }

    [DataTestMethod]
    [DynamicData(nameof(WriteTestData))]
    [DynamicData(nameof(WriteTestDataExtended))]
    public void WriteGenericUpdatesTheValueInPlc(string itemName, object newValue, object? readValue = null)
    {
        var writeType = newValue.GetType();
        var readType = readValue == null ? writeType : readValue.GetType();

        var method = GetType()
            .GetMethod(nameof(WriteValueGenericHelper), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(writeType, readType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {newValue.GetType().Name}.");
        _ = method.InvokeUnwrappedException(this, [itemName, newValue, readValue ?? newValue, nameof(WriteGenericUpdatesTheValueInPlc)]);
    }

    [DataTestMethod]
    [DynamicData(nameof(WriteTestData))]
    [DynamicData(nameof(WriteTestDataExtended))]
    public async Task WriteGenericUpdatesTheValueInPlcAsync(string itemName, object newValue, object? readValue = null)
    {
        var writeType = newValue.GetType();
        var readType = readValue == null ? writeType : readValue.GetType();

        var method = GetType()
            .GetMethod(nameof(WriteValueGenericHelperAsync), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(writeType, readType)
            ?? throw new InvalidOperationException($"Unable to create the generic method {newValue.GetType().Name}.");
        await method.InvokeAsyncUnwrappedException(this, [itemName, newValue, readValue ?? newValue, nameof(WriteGenericUpdatesTheValueInPlcAsync)]);
    }

    [TestMethod]
    public void WriteMultipleUpdatesTheValueInPlc()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var writeData = WriteTestData
            .Concat(WriteTestDataExtended)
            .ToDictionary(
                kv => $"WriteTestData.{nameof(WriteMultipleUpdatesTheValueInPlc)}.{kv[0]}",
                kv => kv[1],
                StringComparer.Ordinal);

        var expectedData = WriteTestData
            .Concat(WriteTestDataExtended)
            .Select(kv => kv.Length == 3 ? kv[2] : kv[1])
            .ToList();

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        ResetPLCValues(readWrite, string.Empty);
        var originalData = readWrite.Read(writeData.Keys);
        readWrite.Write(writeData);
        var newValueRead = readWrite.Read(writeData.Keys);

        // Assert
        using var writeDataEnumerator = writeData.GetEnumerator();
        using var originalEnumerator = originalData.GetEnumerator();
        using var newValueEnumerator = newValueRead.GetEnumerator();
        using var expectedDataEnumerator = expectedData.GetEnumerator();

        Assert.AreEqual(writeData.Count, originalData.Count);
        Assert.AreEqual(writeData.Count, newValueRead.Count);
        var multiAssert = new MultiAssert();

        while (writeDataEnumerator.MoveNext()
                && newValueEnumerator.MoveNext()
                && originalEnumerator.MoveNext()
                && expectedDataEnumerator.MoveNext())
        {
            multiAssert.Check(() => Assert.AreEqual(writeDataEnumerator.Current.Key, newValueEnumerator.Current.Key));
            multiAssert.Check(() => Assert.AreEqual(writeDataEnumerator.Current.Key, originalEnumerator.Current.Key));
            multiAssert.Check(() => Assert.That.ObjectNotEquals(writeDataEnumerator.Current.Value, originalEnumerator.Current.Value, "Reset values in PLC"));
            multiAssert.Check(() => Assert.That.ObjectEquals(expectedDataEnumerator.Current, newValueEnumerator.Current.Value, writeDataEnumerator.Current.Key));
        }

        multiAssert.Assert();
    }

    [TestMethod]
    public async Task WriteMultipleUpdatesTheValueInPlcAsync()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var writeData = WriteTestData
            .Concat(WriteTestDataExtended)
            .ToDictionary(
                kv => $"WriteTestData.{nameof(WriteMultipleUpdatesTheValueInPlcAsync)}.{kv[0]}",
                kv => kv[1],
                StringComparer.Ordinal);

        var expectedData = WriteTestData
            .Concat(WriteTestDataExtended)
            .Select(kv => kv.Length == 3 ? kv[2] : kv[1])
            .ToList();

        // Act
        var connected = await connection.ConnectAsync();
        Assert.IsTrue(connected, "Plc could not connect");
        ResetPLCValues(readWrite, string.Empty);
        var originalData = await readWrite.ReadAsync(writeData.Keys);
        await readWrite.WriteAsync(writeData);
        var newValueRead = await readWrite.ReadAsync(writeData.Keys);

        // Assert
        using var writeDataEnumerator = writeData.GetEnumerator();
        using var originalEnumerator = originalData.GetEnumerator();
        using var newValueEnumerator = newValueRead.GetEnumerator();
        using var expectedDataEnumerator = expectedData.GetEnumerator();

        Assert.AreEqual(writeData.Count, originalData.Count);
        Assert.AreEqual(writeData.Count, newValueRead.Count);
        var multiAssert = new MultiAssert();

        while (writeDataEnumerator.MoveNext()
            && newValueEnumerator.MoveNext()
            && originalEnumerator.MoveNext()
            && expectedDataEnumerator.MoveNext())
        {
            multiAssert.Check(() => Assert.AreEqual(writeDataEnumerator.Current.Key, newValueEnumerator.Current.Key));
            multiAssert.Check(() => Assert.AreEqual(writeDataEnumerator.Current.Key, originalEnumerator.Current.Key));
            multiAssert.Check(() => Assert.That.ObjectNotEquals(writeDataEnumerator.Current.Value, originalEnumerator.Current.Value, "Reset values in PLC"));
            multiAssert.Check(() => Assert.That.ObjectEquals(expectedDataEnumerator.Current, newValueEnumerator.Current.Value, writeDataEnumerator.Current.Key));
        }

        multiAssert.Assert();
    }

    [DataTestMethod]
    [DynamicData(nameof(WriteTestData))]
    [DynamicData(nameof(WriteTestDataExtended))]
    public void WriteUpdatesTheValueInPlc(string itemName, object newValue, object? readValue = null)
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"WriteTestData.{nameof(WriteUpdatesTheValueInPlc)}.{itemName}";

        // Act
        Assert.IsTrue(connection.Connect());
        ResetPLCValues(readWrite, itemName);
        var original = readWrite.Read(ioName);
        readWrite.Write(ioName, newValue);
        var newValueRead = readWrite.Read(ioName);

        // Assert
        Assert.That.ObjectNotEquals(readValue ?? newValue, original, "Reset values in PLC");
        Assert.That.ObjectEquals(readValue ?? newValue, newValueRead, ioName);
    }

    [DataTestMethod]
    [DynamicData(nameof(WriteTestData))]
    [DynamicData(nameof(WriteTestDataExtended))]
    public async Task WriteUpdatesTheValueInPlcAsync(string itemName, object newValue, object? readValue = null)
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"WriteTestData.{nameof(WriteUpdatesTheValueInPlcAsync)}.{itemName}";

        // Act
        Assert.IsTrue(connection.Connect());
        ResetPLCValues(readWrite, itemName);
        var original = await readWrite.ReadAsync(ioName);
        await readWrite.WriteAsync(ioName, newValue);
        var newValueRead = await readWrite.ReadAsync(ioName);

        // Assert
        Assert.That.ObjectNotEquals(readValue ?? newValue, original, "Reset values in PLC");
        Assert.That.ObjectEquals(readValue ?? newValue, newValueRead, ioName);
    }

    protected static void ResetPLCValues(IReadWrite readWrite, string fieldName, [CallerMemberName] string memberName = "")
    {
        var ioName = $"WriteTestData.{memberName}.{fieldName}Reset";
        readWrite.Write(ioName, value: true);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        while (readWrite.Read<bool>(ioName))
        {
            cts.Token.ThrowIfCancellationRequested();
        }
    }

    protected abstract IServiceProvider GetServiceProvider();

    protected void WriteValueGenericHelper<T1, T2>(string itemName, T1 newValue, T2 readValue, [CallerMemberName] string memberName = "")
        where T1 : notnull
        where T2 : notnull
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"WriteTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        ResetPLCValues(readWrite, itemName, memberName);
        var original = readWrite.Read<T2>(ioName);
        readWrite.Write(ioName, newValue);

        var newValueRead = readWrite.Read<T2>(ioName);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (newValueRead.Equals(original))
        {
            Assert.IsFalse(cts.IsCancellationRequested, "Timeout reading");
            newValueRead = readWrite.Read<T2>(ioName);
        }

        // Assert
        Assert.That.ObjectNotEquals(readValue, original, "Reset values in PLC");
        Assert.That.ObjectEquals(readValue, newValueRead, ioName);
    }

    protected async Task WriteValueGenericHelperAsync<T1, T2>(string itemName, T1 newValue, T2 readValue, [CallerMemberName] string memberName = "")
        where T1 : notnull
        where T2 : notnull
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"WriteTestData.{memberName}.{itemName}";

        // Act
        var connected = connection.Connect();
        Assert.IsTrue(connected, "Plc could not connect");
        ResetPLCValues(readWrite, itemName, memberName);
        var original = await readWrite.ReadAsync<T2>(ioName).ConfigureAwait(false);
        await readWrite.WriteAsync(ioName, newValue).ConfigureAwait(false);

        var newValueRead = await readWrite.ReadAsync<T2>(ioName).ConfigureAwait(false);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (newValueRead.Equals(original))
        {
            Assert.IsFalse(cts.IsCancellationRequested, "Timeout reading");
            newValueRead = await readWrite.ReadAsync<T2>(ioName).ConfigureAwait(false);
        }

        // Assert
        Assert.That.ObjectNotEquals(readValue, original, "Reset values in PLC");
        Assert.That.ObjectEquals(readValue, newValueRead, ioName);
    }
}
