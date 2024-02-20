using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PlcInterface.IntegrationTests.DataTypes;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
internal sealed class DUT_TestClass2 : IEquatable<DUT_TestClass2?>
{
    public static DUT_TestClass2 Default => new()
    {
        ByteValue = byte.MaxValue,
        WordValue = ushort.MaxValue,
        DWordValue = uint.MaxValue,
        LWordValue = ulong.MaxValue,
    };

    public static DUT_TestClass2 Write => new()
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

    public override bool Equals(object? obj)
        => Equals(obj as DUT_TestClass2);

    public bool Equals(DUT_TestClass2? other)
        => other != null
            && ByteValue == other.ByteValue
            && WordValue == other.WordValue
            && DWordValue == other.DWordValue
            && LWordValue == other.LWordValue;

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        var hashCode = -1110352730;
        hashCode = (hashCode * -1521134295) + ByteValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + WordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + DWordValue.GetHashCode();
        hashCode = (hashCode * -1521134295) + LWordValue.GetHashCode();
        return hashCode;
    }
}
