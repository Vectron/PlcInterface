using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PlcInterface.IntegrationTests.DataTypes;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
internal struct DUT_TestStruct
{
    public static DUT_TestStruct Default => new()
    {
        BoolValue = true,
        ByteValue = byte.MaxValue,
        WordValue = ushort.MaxValue,
        DWordValue = uint.MaxValue,
        LWordValue = ulong.MaxValue,
        ShortValue = sbyte.MinValue,
        IntValue = short.MinValue,
        DIntValue = int.MinValue,
        LongValue = long.MinValue,
        UShortValue = byte.MaxValue,
        UIntValue = ushort.MaxValue,
        UDIntValue = uint.MaxValue,
        ULongValue = ulong.MaxValue,
        FloatValue = -3.402823E+38F,
        DoubleValue = -1.79769313486231E+308,
        TimeValue = TimeSpan.FromSeconds(1),
        TimeOfDay = TimeSpan.FromHours(1),
        LTimeValue = TimeSpan.FromTicks(10),
        DateValue = new DateTimeOffset(2106, 02, 05, 0, 0, 0, TimeSpan.FromHours(1)),
        DateAndTimeValue = new DateTimeOffset(2106, 02, 05, 06, 28, 15, TimeSpan.FromHours(1)),
        StringValue = "Test String",
        WStringValue = "Test WString",
        Nested = DUT_TestStruct2.Default,
        IntArray = [1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010],
        MultiDimensionArray = new short[,,]
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
        ComplexArray = [DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default],
        MultiDimensionComplexArray = new DUT_TestStruct2[,,]
        {
            {
                { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
            },
            {
                { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
                { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
            },
        },
    };

    public static DUT_TestStruct Write => new()
    {
        BoolValue = false,
        ByteValue = byte.MinValue,
        WordValue = ushort.MinValue,
        DWordValue = uint.MinValue,
        LWordValue = ulong.MinValue,
        ShortValue = sbyte.MaxValue,
        IntValue = short.MaxValue,
        DIntValue = int.MaxValue,
        LongValue = long.MaxValue,
        UShortValue = byte.MinValue,
        UIntValue = ushort.MinValue,
        UDIntValue = uint.MinValue,
        ULongValue = ulong.MinValue,
        FloatValue = float.MaxValue,
        DoubleValue = double.MaxValue,
        TimeValue = TimeSpan.FromSeconds(3),
        TimeOfDay = TimeSpan.FromHours(10),
        LTimeValue = TimeSpan.FromTicks(100),
        DateValue = new DateTimeOffset(2019, 02, 21, 00, 00, 00, TimeSpan.FromHours(1)),
        DateAndTimeValue = new DateTimeOffset(2019, 02, 21, 12, 15, 10, TimeSpan.FromHours(1)),
        StringValue = "new Test String",
        WStringValue = "new Test WString",
        Nested = DUT_TestStruct2.Write,
        IntArray = [10000, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010],
        MultiDimensionArray = new short[,,]
        {
            {
                { 0101, 0201, 0301, 0401 },
                { 0501, 0601, 0701, 0801 },
                { 0901, 1001, 1101, 1201 },
            },
            {
                { 1301, 1401, 1501, 1601 },
                { 1701, 1801, 1901, 2001 },
                { 2101, 2201, 2301, 2401 },
            },
        },
        ComplexArray = [DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write],
        MultiDimensionComplexArray = new DUT_TestStruct2[,,]
        {
            {
                { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
            },
            {
                { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
                { DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write, DUT_TestStruct2.Write },
            },
        },
    };

    public bool BoolValue
    {
        get;
        set;
    }

    public byte ByteValue
    {
        get; set;
    }

    public ushort WordValue
    {
        get; set;
    }

    public uint DWordValue
    {
        get; set;
    }

    public ulong LWordValue
    {
        get; set;
    }

    public sbyte ShortValue
    {
        get; set;
    }

    public short IntValue
    {
        get; set;
    }

    public int DIntValue
    {
        get; set;
    }

    public long LongValue
    {
        get; set;
    }

    public byte UShortValue
    {
        get; set;
    }

    public ushort UIntValue
    {
        get; set;
    }

    public uint UDIntValue
    {
        get; set;
    }

    public ulong ULongValue
    {
        get; set;
    }

    public float FloatValue
    {
        get; set;
    }

    public double DoubleValue
    {
        get; set;
    }

    public TimeSpan TimeValue
    {
        get; set;
    }

    public TimeSpan TimeOfDay
    {
        get; set;
    }

    public TimeSpan LTimeValue
    {
        get; set;
    }

    public DateTimeOffset DateValue
    {
        get; set;
    }

    public DateTimeOffset DateAndTimeValue
    {
        get; set;
    }

    public string StringValue
    {
        get; set;
    }

    public string WStringValue
    {
        get; set;
    }

    public DUT_TestStruct2 Nested
    {
        get; set;
    }

    public short[] IntArray
    {
        get;
        set;
    }

    public short[,,] MultiDimensionArray
    {
        get;
        set;
    }

    public DUT_TestStruct2[] ComplexArray
    {
        get;
        set;
    }

    public DUT_TestStruct2[,,] MultiDimensionComplexArray
    {
        get;
        set;
    }
}
