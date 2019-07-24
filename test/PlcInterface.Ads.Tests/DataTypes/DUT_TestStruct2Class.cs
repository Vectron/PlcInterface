using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System.Text;

namespace PlcInterface.Ads.Tests.DataTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal class DUT_TestStruct2Class
    {
        public static DUT_TestStruct2Class Default => new DUT_TestStruct2Class()
        {
            ByteValue = byte.MaxValue,
            WordValue = ushort.MaxValue,
            DWordValue = uint.MaxValue,
            LWordValue = ulong.MaxValue,
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
            if (!(obj is DUT_TestStruct2Class))
            {
                return false;
            }

            var other = (DUT_TestStruct2Class)obj;
            Assert.AreEqual(ByteValue, other.ByteValue);
            Assert.AreEqual(WordValue, other.WordValue);
            Assert.AreEqual(DWordValue, other.DWordValue);
            Assert.AreEqual(LWordValue, other.LWordValue);

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = -1110352730;
            hashCode = hashCode * -1521134295 + ByteValue.GetHashCode();
            hashCode = hashCode * -1521134295 + WordValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DWordValue.GetHashCode();
            hashCode = hashCode * -1521134295 + LWordValue.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("{ ")
                .Append(ByteValue)
                .Append(", ")
                .Append(WordValue)
                .Append(", ")
                .Append(DWordValue)
                .Append(", ")
                .Append(LWordValue)
                .Append(" }")
                .ToString();
        }
    }
}