using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests.DataTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal sealed class DUT_TestClass2
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

        public override bool Equals(object obj)
        {
            if (obj is not DUT_TestClass2)
            {
                return false;
            }

            var other = (DUT_TestClass2)obj;
            Assert.AreEqual(ByteValue, other.ByteValue);
            Assert.AreEqual(WordValue, other.WordValue);
            Assert.AreEqual(DWordValue, other.DWordValue);
            Assert.AreEqual(LWordValue, other.LWordValue);

            return true;
        }

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
}