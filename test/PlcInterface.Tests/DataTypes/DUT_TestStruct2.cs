using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests.DataTypes
{
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

        public override readonly bool Equals(object obj)
        {
            if (obj is not DUT_TestStruct2)
            {
                return false;
            }

            var other = (DUT_TestStruct2)obj;
            Assert.AreEqual(ByteValue, other.ByteValue);
            Assert.AreEqual(WordValue, other.WordValue);
            Assert.AreEqual(DWordValue, other.DWordValue);
            Assert.AreEqual(LWordValue, other.LWordValue);

            return true;
        }

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
}