using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PlcInterface.IntegrationTests.DataTypes;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
internal struct DUT_TestStruct2 : System.IEquatable<DUT_TestStruct2>
{
    public static DUT_TestStruct2 Default => new()
    {
        ByteValue = byte.MaxValue,
        WordValue = ushort.MaxValue,
        DWordValue = uint.MaxValue,
        LWordValue = ulong.MaxValue,
    };

    public static DUT_TestStruct2 Write => new()
    {
        ByteValue = byte.MinValue,
        WordValue = ushort.MinValue,
        DWordValue = uint.MinValue,
        LWordValue = ulong.MinValue,
    };

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

    [ExcludeFromCodeCoverage]
    public override readonly bool Equals(object? obj)
        => obj is DUT_TestStruct2 @struct && Equals(@struct);

    public readonly bool Equals(DUT_TestStruct2 other)
        => ByteValue == other.ByteValue
        && WordValue == other.WordValue
        && DWordValue == other.DWordValue
        && LWordValue == other.LWordValue;

    [ExcludeFromCodeCoverage]
    public override readonly int GetHashCode()
    {
        var hashCode = -1110352730;
        hashCode = (hashCode * -1521134295) + ByteValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + WordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DWordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + LWordValue.GetHashCode();
        return hashCode;
    }
}
