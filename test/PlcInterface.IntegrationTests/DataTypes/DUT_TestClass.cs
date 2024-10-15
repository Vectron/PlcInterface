using System.Runtime.InteropServices;

namespace PlcInterface.IntegrationTests.DataTypes;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
internal sealed class DUT_TestClass
{
    public static DUT_TestClass Default => new()
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
        Nested = DUT_TestClass2.Default,
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
        ComplexArray = [DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default],
        MultiDimensionComplexArray = new DUT_TestClass2[,,]
        {
            {
                { DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default },
                { DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default },
                { DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default },
            },
            {
                { DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default },
                { DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default },
                { DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default, DUT_TestClass2.Default },
            },
        },
        IntArray2 = CreateNonZeroBoundIntArrayRead(),
        MultiDimensionArray2 = CreateNonZeroBoundMultiDimensionArrayRead(),
        ComplexArray2 = CreateNonZeroBoundComplexArrayRead(),
        MultiDimensionComplexArray2 = CreateNonZeroBoundMultiDimensionComplexArrayRead(),
    };

    public static DUT_TestClass Write => new()
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

        Nested = DUT_TestClass2.Write,
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
        ComplexArray = [DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write],
        MultiDimensionComplexArray = new DUT_TestClass2[,,]
        {
            {
                { DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write },
                { DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write },
                { DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write },
            },
            {
                { DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write },
                { DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write },
                { DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write, DUT_TestClass2.Write },
            },
        },
        IntArray2 = CreateNonZeroBoundIntArrayWrite(),
        MultiDimensionArray2 = CreateNonZeroBoundMultiDimensionArrayWrite(),
        ComplexArray2 = CreateNonZeroBoundComplexArrayWrite(),
        MultiDimensionComplexArray2 = CreateNonZeroBoundMultiDimensionComplexArrayWrite(),
    };

    public bool BoolValue
    {
        get;
        set;
    }

    public byte ByteValue
    {
        get;
        set;
    }

    public ushort WordValue
    {
        get;
        set;
    }

    public uint DWordValue
    {
        get;
        set;
    }

    public ulong LWordValue
    {
        get;
        set;
    }

    public sbyte ShortValue
    {
        get;
        set;
    }

    public short IntValue
    {
        get;
        set;
    }

    public int DIntValue
    {
        get;
        set;
    }

    public long LongValue
    {
        get;
        set;
    }

    public byte UShortValue
    {
        get;
        set;
    }

    public ushort UIntValue
    {
        get;
        set;
    }

    public uint UDIntValue
    {
        get;
        set;
    }

    public ulong ULongValue
    {
        get;
        set;
    }

    public float FloatValue
    {
        get;
        set;
    }

    public double DoubleValue
    {
        get;
        set;
    }

    public TimeSpan TimeValue
    {
        get;
        set;
    }

    public TimeSpan TimeOfDay
    {
        get;
        set;
    }

    public TimeSpan LTimeValue
    {
        get;
        set;
    }

    public DateTimeOffset DateValue
    {
        get;
        set;
    }

    public DateTimeOffset DateAndTimeValue
    {
        get;
        set;
    }

    public string? StringValue
    {
        get;
        set;
    }

    public string? WStringValue
    {
        get;
        set;
    }

    public DUT_TestClass2? Nested
    {
        get;
        set;
    }

    public short[]? IntArray
    {
        get;
        set;
    }

    public short[,,]? MultiDimensionArray
    {
        get;
        set;
    }

    public DUT_TestClass2[]? ComplexArray
    {
        get;
        set;
    }

    public DUT_TestClass2[,,]? MultiDimensionComplexArray
    {
        get;
        set;
    }

    public NonZeroBasedArray<short> IntArray2
    {
        get;
        set;
    }

    public NonZeroBasedArray<short> MultiDimensionArray2
    {
        get;
        set;
    }

    public NonZeroBasedArray<DUT_TestClass2> ComplexArray2
    {
        get;
        set;
    }

    public NonZeroBasedArray<DUT_TestClass2> MultiDimensionComplexArray2
    {
        get;
        set;
    }

    private static NonZeroBasedArray<DUT_TestClass2> CreateNonZeroBoundComplexArrayRead()
    {
        var array = new NonZeroBasedArray<DUT_TestClass2>([3], [1]);
        array.SetValue(DUT_TestClass2.Default, [1]);
        array.SetValue(DUT_TestClass2.Default, [2]);
        array.SetValue(DUT_TestClass2.Default, [3]);
        return array;
    }

    private static NonZeroBasedArray<DUT_TestClass2> CreateNonZeroBoundComplexArrayWrite()
    {
        var array = new NonZeroBasedArray<DUT_TestClass2>([3], [1]);
        array.SetValue(DUT_TestClass2.Write, [1]);
        array.SetValue(DUT_TestClass2.Write, [2]);
        array.SetValue(DUT_TestClass2.Write, [3]);
        return array;
    }

    private static NonZeroBasedArray<short> CreateNonZeroBoundIntArrayRead()
    {
        var array = new NonZeroBasedArray<short>([11], [1]);
        array.SetValue(1000, [01]);
        array.SetValue(1001, [02]);
        array.SetValue(1002, [03]);
        array.SetValue(1003, [04]);
        array.SetValue(1004, [05]);
        array.SetValue(1005, [06]);
        array.SetValue(1006, [07]);
        array.SetValue(1007, [08]);
        array.SetValue(1008, [09]);
        array.SetValue(1009, [10]);
        array.SetValue(1010, [11]);
        return array;
    }

    private static NonZeroBasedArray<short> CreateNonZeroBoundIntArrayWrite()
    {
        var array = new NonZeroBasedArray<short>([11], [1]);
        array.SetValue(10000, [01]);
        array.SetValue(10001, [02]);
        array.SetValue(10002, [03]);
        array.SetValue(10003, [04]);
        array.SetValue(10004, [05]);
        array.SetValue(10005, [06]);
        array.SetValue(10006, [07]);
        array.SetValue(10007, [08]);
        array.SetValue(10008, [09]);
        array.SetValue(10009, [10]);
        array.SetValue(10010, [11]);
        return array;
    }

    private static NonZeroBasedArray<short> CreateNonZeroBoundMultiDimensionArrayRead()
    {
        var array = new NonZeroBasedArray<short>([2, 3, 4], [1, 1, 1]);
        array.SetValue(0100, [1, 1, 1]);
        array.SetValue(0200, [1, 1, 2]);
        array.SetValue(0300, [1, 1, 3]);
        array.SetValue(0400, [1, 1, 4]);

        array.SetValue(0500, [1, 2, 1]);
        array.SetValue(0600, [1, 2, 2]);
        array.SetValue(0700, [1, 2, 3]);
        array.SetValue(0800, [1, 2, 4]);

        array.SetValue(0900, [1, 3, 1]);
        array.SetValue(1000, [1, 3, 2]);
        array.SetValue(1100, [1, 3, 3]);
        array.SetValue(1200, [1, 3, 4]);

        array.SetValue(1300, [2, 1, 1]);
        array.SetValue(1400, [2, 1, 2]);
        array.SetValue(1500, [2, 1, 3]);
        array.SetValue(1600, [2, 1, 4]);

        array.SetValue(1700, [2, 2, 1]);
        array.SetValue(1800, [2, 2, 2]);
        array.SetValue(1900, [2, 2, 3]);
        array.SetValue(2000, [2, 2, 4]);

        array.SetValue(2100, [2, 3, 1]);
        array.SetValue(2200, [2, 3, 2]);
        array.SetValue(2300, [2, 3, 3]);
        array.SetValue(2400, [2, 3, 4]);
        return array;
    }

    private static NonZeroBasedArray<short> CreateNonZeroBoundMultiDimensionArrayWrite()
    {
        var array = new NonZeroBasedArray<short>([2, 3, 4], [1, 1, 1]);
        array.SetValue(01000, [1, 1, 1]);
        array.SetValue(02000, [1, 1, 2]);
        array.SetValue(03000, [1, 1, 3]);
        array.SetValue(04000, [1, 1, 4]);

        array.SetValue(05000, [1, 2, 1]);
        array.SetValue(06000, [1, 2, 2]);
        array.SetValue(07000, [1, 2, 3]);
        array.SetValue(08000, [1, 2, 4]);

        array.SetValue(09000, [1, 3, 1]);
        array.SetValue(10000, [1, 3, 2]);
        array.SetValue(11000, [1, 3, 3]);
        array.SetValue(12000, [1, 3, 4]);

        array.SetValue(13000, [2, 1, 1]);
        array.SetValue(14000, [2, 1, 2]);
        array.SetValue(15000, [2, 1, 3]);
        array.SetValue(16000, [2, 1, 4]);

        array.SetValue(17000, [2, 2, 1]);
        array.SetValue(18000, [2, 2, 2]);
        array.SetValue(19000, [2, 2, 3]);
        array.SetValue(20000, [2, 2, 4]);

        array.SetValue(21000, [2, 3, 1]);
        array.SetValue(22000, [2, 3, 2]);
        array.SetValue(23000, [2, 3, 3]);
        array.SetValue(24000, [2, 3, 4]);
        return array;
    }

    private static NonZeroBasedArray<DUT_TestClass2> CreateNonZeroBoundMultiDimensionComplexArrayRead()
    {
        var array = new NonZeroBasedArray<DUT_TestClass2>([2, 3, 4], [1, 1, 1]);
        array.SetValue(DUT_TestClass2.Default, [1, 1, 1]);
        array.SetValue(DUT_TestClass2.Default, [1, 1, 2]);
        array.SetValue(DUT_TestClass2.Default, [1, 1, 3]);
        array.SetValue(DUT_TestClass2.Default, [1, 1, 4]);

        array.SetValue(DUT_TestClass2.Default, [1, 2, 1]);
        array.SetValue(DUT_TestClass2.Default, [1, 2, 2]);
        array.SetValue(DUT_TestClass2.Default, [1, 2, 3]);
        array.SetValue(DUT_TestClass2.Default, [1, 2, 4]);

        array.SetValue(DUT_TestClass2.Default, [1, 3, 1]);
        array.SetValue(DUT_TestClass2.Default, [1, 3, 2]);
        array.SetValue(DUT_TestClass2.Default, [1, 3, 3]);
        array.SetValue(DUT_TestClass2.Default, [1, 3, 4]);

        array.SetValue(DUT_TestClass2.Default, [2, 1, 1]);
        array.SetValue(DUT_TestClass2.Default, [2, 1, 2]);
        array.SetValue(DUT_TestClass2.Default, [2, 1, 3]);
        array.SetValue(DUT_TestClass2.Default, [2, 1, 4]);

        array.SetValue(DUT_TestClass2.Default, [2, 2, 1]);
        array.SetValue(DUT_TestClass2.Default, [2, 2, 2]);
        array.SetValue(DUT_TestClass2.Default, [2, 2, 3]);
        array.SetValue(DUT_TestClass2.Default, [2, 2, 4]);

        array.SetValue(DUT_TestClass2.Default, [2, 3, 1]);
        array.SetValue(DUT_TestClass2.Default, [2, 3, 2]);
        array.SetValue(DUT_TestClass2.Default, [2, 3, 3]);
        array.SetValue(DUT_TestClass2.Default, [2, 3, 4]);
        return array;
    }

    private static NonZeroBasedArray<DUT_TestClass2> CreateNonZeroBoundMultiDimensionComplexArrayWrite()
    {
        var array = new NonZeroBasedArray<DUT_TestClass2>([2, 3, 4], [1, 1, 1]);
        array.SetValue(DUT_TestClass2.Write, [1, 1, 1]);
        array.SetValue(DUT_TestClass2.Write, [1, 1, 2]);
        array.SetValue(DUT_TestClass2.Write, [1, 1, 3]);
        array.SetValue(DUT_TestClass2.Write, [1, 1, 4]);

        array.SetValue(DUT_TestClass2.Write, [1, 2, 1]);
        array.SetValue(DUT_TestClass2.Write, [1, 2, 2]);
        array.SetValue(DUT_TestClass2.Write, [1, 2, 3]);
        array.SetValue(DUT_TestClass2.Write, [1, 2, 4]);

        array.SetValue(DUT_TestClass2.Write, [1, 3, 1]);
        array.SetValue(DUT_TestClass2.Write, [1, 3, 2]);
        array.SetValue(DUT_TestClass2.Write, [1, 3, 3]);
        array.SetValue(DUT_TestClass2.Write, [1, 3, 4]);

        array.SetValue(DUT_TestClass2.Write, [2, 1, 1]);
        array.SetValue(DUT_TestClass2.Write, [2, 1, 2]);
        array.SetValue(DUT_TestClass2.Write, [2, 1, 3]);
        array.SetValue(DUT_TestClass2.Write, [2, 1, 4]);

        array.SetValue(DUT_TestClass2.Write, [2, 2, 1]);
        array.SetValue(DUT_TestClass2.Write, [2, 2, 2]);
        array.SetValue(DUT_TestClass2.Write, [2, 2, 3]);
        array.SetValue(DUT_TestClass2.Write, [2, 2, 4]);

        array.SetValue(DUT_TestClass2.Write, [2, 3, 1]);
        array.SetValue(DUT_TestClass2.Write, [2, 3, 2]);
        array.SetValue(DUT_TestClass2.Write, [2, 3, 3]);
        array.SetValue(DUT_TestClass2.Write, [2, 3, 4]);
        return array;
    }
}
