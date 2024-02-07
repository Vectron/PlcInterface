using System;
using System.Collections.Generic;
using System.Linq;
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
public abstract class IWriteValueTestBase
{
    protected abstract string DataRoot
    {
        get;
    }

    private static IEnumerable<object[]> WriteTestData
        => new List<object[]>()
        {
            new object[] { "BoolValue", false },
            new object[] { "ByteValue", byte.MinValue },
            new object[] { "WordValue", ushort.MinValue },
            new object[] { "DWordValue", uint.MinValue },
            new object[] { "LWordValue", ulong.MinValue },
            new object[] { "SIntValue", sbyte.MaxValue },
            new object[] { "IntValue", short.MaxValue },
            new object[] { "DIntValue", int.MaxValue },
            new object[] { "LIntValue", long.MaxValue },
            new object[] { "USIntValue", byte.MinValue },
            new object[] { "UIntValue", ushort.MinValue },
            new object[] { "UDIntValue", uint.MinValue },
            new object[] { "ULIntValue", ulong.MinValue },
            new object[] { "RealValue", 3.402823E+38F },
            new object[] { "LRealValue", 1.79769313486231E+308 },
            new object[] { "TimeValue1", TimeSpan.FromSeconds(3) },
            new object[] { "TimeValue2", 3000u, TimeSpan.FromSeconds(3) },
            new object[] { "TimeOfDayValue1", TimeSpan.FromHours(10) },
            new object[] { "TimeOfDayValue2", 36000000u, TimeSpan.FromHours(10) },
            new object[] { "LTimeValue1", TimeSpan.FromTicks(100) },
            new object[] { "LTimeValue2", 10000ul, TimeSpan.FromTicks(100) },
            new object[] { "DateValue1", new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1)) },
            new object[] { "DateValue2", new DateTime(2019, 02, 21), new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1)) },
            new object[] { "DateAndTimeValue1", new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1)) },
            new object[] { "DateAndTimeValue2", new DateTime(2019, 02, 21, 12, 15, 10), new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1)) },
            new object[] { "StringValue", "new Test String" },
            new object[] { "WStringValue", "new Test WString" },
            new object[] { "EnumValue1", TestEnum.Third, (int)TestEnum.Third },
            new object[] { "EnumValue2", (short)TestEnum.Third, (int)TestEnum.Third },
            new object[] { "EnumValue3", (int)TestEnum.Third },
        };

    private static IEnumerable<object[]> WriteTestDataExtended
        => new List<object[]>()
        {
            new object[]
            {
                "IntArray",
                new short[] { 10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 },
            },
            new object[]
            {
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
            },
            new object[]
            {
                "ComplexArray",
                new DUT_TestStruct2[] { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, },
            },
            new object[]
            {
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
            },
            new object[] { "StructValue1", DUT_TestStruct.Write },
            new object[] { "StructValue2", DUT_TestClass.Write },
            new object[] { "Nested1", DUT_TestStruct2.Write },
            new object[] { "Nested2", DUT_TestClass2.Write },
        };

    [TestMethod]
    public void ToggleBool()
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.WriteTestData.ToggleBool";

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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
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
                kv => $"{DataRoot}.WriteTestData.{nameof(WriteMultipleUpdatesTheValueInPlc)}.{kv[0]}",
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
                kv => $"{DataRoot}.WriteTestData.{nameof(WriteMultipleUpdatesTheValueInPlcAsync)}.{kv[0]}",
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
        var ioName = $"{DataRoot}.WriteTestData.{nameof(WriteUpdatesTheValueInPlc)}.{itemName}";

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
        var ioName = $"{DataRoot}.WriteTestData.{nameof(WriteUpdatesTheValueInPlcAsync)}.{itemName}";

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

    protected abstract IServiceProvider GetServiceProvider();

    protected void ResetPLCValues(IReadWrite readWrite, string fieldName, [CallerMemberName] string memberName = "")
    {
        var ioName = $"{DataRoot}.WriteTestData.{memberName}.{fieldName}Reset";
        readWrite.Write(ioName, value: true);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        while (readWrite.Read<bool>(ioName))
        {
            cts.Token.ThrowIfCancellationRequested();
        }
    }

    protected void WriteValueGenericHelper<T1, T2>(string itemName, T1 newValue, T2 readValue, [CallerMemberName] string memberName = "")
        where T1 : notnull
        where T2 : notnull
    {
        // Arrange
        var serviceProvider = GetServiceProvider();
        using var disposable = serviceProvider as IDisposable;
        var connection = serviceProvider.GetRequiredService<IPlcConnection>();
        var readWrite = serviceProvider.GetRequiredService<IReadWrite>();
        var ioName = $"{DataRoot}.WriteTestData.{memberName}.{itemName}";

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
        var ioName = $"{DataRoot}.WriteTestData.{memberName}.{itemName}";

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
