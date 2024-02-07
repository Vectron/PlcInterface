using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using PlcInterface.IntegrationTests.Extension;

namespace PlcInterface.IntegrationTests.DataTypes;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
internal sealed class DUT_TestClass : IEquatable<DUT_TestClass>
{
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
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
    };

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Style cop hasn't caught up yet.")]
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
                { 0501, 0600, 0701, 0801 },
                { 0901, 1000, 1101, 1201 },
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

    [ExcludeFromCodeCoverage]
    public override bool Equals(object? obj) => obj is DUT_TestClass @struct && Equals(@struct);

    public bool Equals(DUT_TestClass? other)
        => other != null
            && BoolValue == other.BoolValue
            && ByteValue == other.ByteValue
            && WordValue == other.WordValue
            && DWordValue == other.DWordValue
            && LWordValue == other.LWordValue
            && ShortValue == other.ShortValue
            && IntValue == other.IntValue
            && DIntValue == other.DIntValue
            && LongValue == other.LongValue
            && UShortValue == other.UShortValue
            && UIntValue == other.UIntValue
            && UDIntValue == other.UDIntValue
            && ULongValue == other.ULongValue
            && FloatValue == other.FloatValue
            && DoubleValue == other.DoubleValue
            && TimeValue.Equals(other.TimeValue)
            && TimeOfDay.Equals(other.TimeOfDay)
            && LTimeValue.Equals(other.LTimeValue)
            && DateValue.Equals(other.DateValue)
            && DateAndTimeValue.Equals(other.DateAndTimeValue)
            && string.Equals(StringValue, other.StringValue, StringComparison.Ordinal)
            && string.Equals(WStringValue, other.WStringValue, StringComparison.Ordinal)
            && Nested != null
            && other.Nested != null
            && EqualityComparer<DUT_TestClass2>.Default.Equals(Nested, other.Nested)
            && IntArray != null
            && other.IntArray != null
            && IntArray.SequenceEqual(other.IntArray)
            && MultiDimensionArray != null
            && other.MultiDimensionArray != null
            && MultiDimensionArray.SequenceEqual<short>(other.MultiDimensionArray)
            && ComplexArray != null
            && other.ComplexArray != null
            && ComplexArray.SequenceEqual(other.ComplexArray)
            && MultiDimensionComplexArray != null
            && other.MultiDimensionComplexArray != null
            && MultiDimensionComplexArray.SequenceEqual<DUT_TestClass2>(other.MultiDimensionComplexArray);

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        var hashCode = 1307849462;
        hashCode = (hashCode * -1521134295) + BoolValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + ByteValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + WordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DWordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + LWordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + ShortValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + IntValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DIntValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + LongValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + UShortValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + UIntValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + UDIntValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + ULongValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + FloatValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DoubleValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + TimeValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + TimeOfDay.GetHashCode();
        hashCode = (hashCode * -1521134295) + LTimeValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DateValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DateAndTimeValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + StringComparer.Ordinal.GetHashCode(StringValue ?? string.Empty);
        hashCode = (hashCode * -1521134295) + StringComparer.Ordinal.GetHashCode(WStringValue ?? string.Empty);
        hashCode = (hashCode * -1521134295) + (Nested == null ? 0 : Nested.GetHashCode());
        hashCode = (hashCode * -1521134295) + (IntArray == null ? 0 : EqualityComparer<short[]>.Default.GetHashCode(IntArray));
        hashCode = (hashCode * -1521134295) + (MultiDimensionArray == null ? 0 : EqualityComparer<short[,,]>.Default.GetHashCode(MultiDimensionArray));
        hashCode = (hashCode * -1521134295) + (ComplexArray == null ? 0 : EqualityComparer<DUT_TestClass2[]>.Default.GetHashCode(ComplexArray));
        hashCode = (hashCode * -1521134295) + (MultiDimensionComplexArray == null ? 0 : EqualityComparer<DUT_TestClass2[,,]>.Default.GetHashCode(MultiDimensionComplexArray));
        return hashCode;
    }
}
