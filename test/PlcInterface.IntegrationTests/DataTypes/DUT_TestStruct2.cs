using System.Runtime.InteropServices;

namespace PlcInterface.IntegrationTests.DataTypes;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
internal struct DUT_TestStruct2
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
}
