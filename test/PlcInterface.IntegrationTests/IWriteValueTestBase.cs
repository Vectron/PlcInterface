using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests.DataTypes;
using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests;

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
            ["IntArray", DUT_TestStruct.Write.IntArray],
            ["MultiDimensionArray", DUT_TestStruct.Write.MultiDimensionArray],
            ["ComplexArray", DUT_TestStruct.Write.ComplexArray],
            ["MultiDimensionComplexArray", DUT_TestStruct.Write.MultiDimensionComplexArray],
            ["IntArray2", DUT_TestStruct.Write.IntArray2],
            ["MultiDimensionArray2", DUT_TestStruct.Write.MultiDimensionArray2],
            ["ComplexArray2", DUT_TestStruct.Write.ComplexArray2],
            ["MultiDimensionComplexArray2", DUT_TestStruct.Write.MultiDimensionComplexArray2],
            ["StructValue1", DUT_TestStruct.Write],
            ["StructValue2", DUT_TestClass.Write],
            ["Nested1", DUT_TestStruct2.Write],
            ["Nested2", DUT_TestClass2.Write],
            ["IntArray2", new short[] { 11000, 11001, 11002, 11003, 11004, 11005, 11006, 11007, 11008, 11009, 11010 }],
            ["MultiDimensionArray2", new short[,,]
            {
                {
                    { 7101, 7201, 7301, 7401 },
                    { 7501, 7600, 7701, 7801 },
                    { 7901, 8000, 8101, 8201 },
                },
                {
                    { 8301, 8401, 8501, 8601 },
                    { 8701, 8801, 8901, 9001 },
                    { 8101, 8201, 8301, 9401 },
                },
            },]
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
            .DistinctBy(kv => kv[0])
            .ToDictionary(
                kv => $"WriteTestData.{nameof(WriteMultipleUpdatesTheValueInPlc)}.{kv[0]}",
                kv => kv[1],
                StringComparer.Ordinal);

        var expectedData = WriteTestData
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
            multiAssert.Check(() => AssertObjectValue.ValuesAreNotEqual(writeDataEnumerator.Current.Value, originalEnumerator.Current.Value, writeDataEnumerator.Current.Key));
            multiAssert.Check(() => AssertObjectValue.ValuesAreEqual(expectedDataEnumerator.Current, newValueEnumerator.Current.Value, writeDataEnumerator.Current.Key));
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
            .DistinctBy(kv => kv[0])
            .ToDictionary(
                kv => $"WriteTestData.{nameof(WriteMultipleUpdatesTheValueInPlcAsync)}.{kv[0]}",
                kv => kv[1],
                StringComparer.Ordinal);

        var expectedData = WriteTestData
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
            multiAssert.Check(() => AssertObjectValue.ValuesAreNotEqual(writeDataEnumerator.Current.Value, originalEnumerator.Current.Value, writeDataEnumerator.Current.Key));
            multiAssert.Check(() => AssertObjectValue.ValuesAreEqual(expectedDataEnumerator.Current, newValueEnumerator.Current.Value, writeDataEnumerator.Current.Key));
        }

        multiAssert.Assert();
    }

    [DataTestMethod]
    [DynamicData(nameof(WriteTestData))]
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
        AssertObjectValue.ValuesAreNotEqual(readValue ?? newValue, original, ioName);
        AssertObjectValue.ValuesAreEqual(readValue ?? newValue, newValueRead, ioName);
    }

    [DataTestMethod]
    [DynamicData(nameof(WriteTestData))]
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
        AssertObjectValue.ValuesAreNotEqual(readValue ?? newValue, original, ioName);
        AssertObjectValue.ValuesAreEqual(readValue ?? newValue, newValueRead, ioName);
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
        AssertObjectValue.ValuesAreNotEqual(readValue, original, ioName);
        AssertObjectValue.ValuesAreEqual(readValue, newValueRead, ioName);
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
        AssertObjectValue.ValuesAreNotEqual(readValue, original, ioName);
        AssertObjectValue.ValuesAreEqual(readValue, newValueRead, ioName);
    }
}
