using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PlcInterface.OpcUa.Tests.DataTypes
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

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal class DUT_TestStructClass
    {
        public static DUT_TestStructClass Default => new DUT_TestStructClass()
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
            TimeValue = 1000u,
            TimeOfDay = 3600000u,
            LTimeValue = 1000ul,
            DateValue = new DateTime(2106, 02, 06),
            DateAndTimeValue = new DateTime(2106, 02, 06, 06, 28, 15),
            StringValue = "Test String",
            WStringValue = "Test WString",
            Nested = DUT_TestStruct2Class.Default,
            IntArray = new short[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010 },
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
            ComplexArray = new DUT_TestStruct2Class[] { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
            MultiDimensionComplexArray = new DUT_TestStruct2Class[,,]
            {
                {
                    { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
                    { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
                    { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
                },
                {
                    { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
                    { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
                    { DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default, DUT_TestStruct2Class.Default },
                }
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

        public uint TimeValue
        {
            get; set;
        }

        public uint TimeOfDay
        {
            get; set;
        }

        public ulong LTimeValue
        {
            get; set;
        }

        public DateTime DateValue
        {
            get; set;
        }

        public DateTime DateAndTimeValue
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

        public DUT_TestStruct2Class Nested
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

        public DUT_TestStruct2Class[] ComplexArray
        {
            get;
            set;
        }

        public DUT_TestStruct2Class[,,] MultiDimensionComplexArray
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DUT_TestStructClass))
            {
                return false;
            }

            var other = (DUT_TestStructClass)obj;
            Assert.AreEqual(BoolValue, other.BoolValue);
            Assert.AreEqual(ByteValue, other.ByteValue);
            Assert.AreEqual(WordValue, other.WordValue);
            Assert.AreEqual(DWordValue, other.DWordValue);
            Assert.AreEqual(LWordValue, other.LWordValue);

            Assert.AreEqual(ShortValue, other.ShortValue);
            Assert.AreEqual(IntValue, other.IntValue);
            Assert.AreEqual(DIntValue, other.DIntValue);
            Assert.AreEqual(LongValue, other.LongValue);

            Assert.AreEqual(UShortValue, other.UShortValue);
            Assert.AreEqual(UIntValue, other.UIntValue);
            Assert.AreEqual(UDIntValue, other.UDIntValue);
            Assert.AreEqual(ULongValue, other.ULongValue);

            Assert.AreEqual(FloatValue, other.FloatValue);
            Assert.AreEqual(DoubleValue, other.DoubleValue);

            Assert.AreEqual(TimeValue, other.TimeValue);
            Assert.AreEqual(TimeOfDay, other.TimeOfDay);
            Assert.AreEqual(LTimeValue, other.LTimeValue);
            Assert.AreEqual(DateValue, other.DateValue);
            Assert.AreEqual(DateAndTimeValue, other.DateAndTimeValue);

            Assert.AreEqual(StringValue, other.StringValue);
            Assert.AreEqual(WStringValue, other.WStringValue);

            Assert.AreEqual(Nested, other.Nested);

            CollectionAssert.AreEqual(IntArray, other.IntArray);
            CollectionAssert.AreEqual(MultiDimensionArray, other.MultiDimensionArray);
            CollectionAssert.AreEqual(ComplexArray, other.ComplexArray);
            CollectionAssert.AreEqual(MultiDimensionComplexArray, other.MultiDimensionComplexArray);

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = -282460972;
            hashCode = hashCode * -1521134295 + BoolValue.GetHashCode();
            hashCode = hashCode * -1521134295 + ByteValue.GetHashCode();
            hashCode = hashCode * -1521134295 + WordValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DWordValue.GetHashCode();
            hashCode = hashCode * -1521134295 + LWordValue.GetHashCode();
            hashCode = hashCode * -1521134295 + ShortValue.GetHashCode();
            hashCode = hashCode * -1521134295 + IntValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DIntValue.GetHashCode();
            hashCode = hashCode * -1521134295 + LongValue.GetHashCode();
            hashCode = hashCode * -1521134295 + UShortValue.GetHashCode();
            hashCode = hashCode * -1521134295 + UIntValue.GetHashCode();
            hashCode = hashCode * -1521134295 + UDIntValue.GetHashCode();
            hashCode = hashCode * -1521134295 + ULongValue.GetHashCode();
            hashCode = hashCode * -1521134295 + FloatValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DoubleValue.GetHashCode();
            hashCode = hashCode * -1521134295 + TimeValue.GetHashCode();
            hashCode = hashCode * -1521134295 + TimeOfDay.GetHashCode();
            hashCode = hashCode * -1521134295 + LTimeValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(DateValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(DateAndTimeValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StringValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WStringValue);
            //hashCode = hashCode * -1521134295 + EqualityComparer<short[]>.Default.GetHashCode(IntArray);
            //hashCode = hashCode * -1521134295 + EqualityComparer<short[,,]>.Default.GetHashCode(MultiDimensionArray);
            //hashCode = hashCode * -1521134295 + EqualityComparer<DUT_TestStruct2[]>.Default.GetHashCode(ComplexArray);
            //hashCode = hashCode * -1521134295 + EqualityComparer<DUT_TestStruct2[,,]>.Default.GetHashCode(MultiDimensionComplexArray);
            return hashCode;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("{ ")
                .Append(BoolValue)
                .Append(", ")
                .Append(ByteValue)
                .Append(", ")
                .Append(WordValue)
                .Append(", ")
                .Append(DWordValue)
                .Append(", ")
                .Append(LWordValue)
                .Append(", ")
                .Append(ShortValue)
                .Append(", ")
                .Append(IntValue)
                .Append(", ")
                .Append(DIntValue)
                .Append(", ")
                .Append(UShortValue)
                .Append(", ")
                .Append(UIntValue)
                .Append(", ")
                .Append(UDIntValue)
                .Append(", ")
                .Append(ULongValue)
                .Append(", ")
                .Append(FloatValue)
                .Append(", ")
                .Append(DoubleValue)
                .Append(", ")
                .Append(TimeValue)
                .Append(", ")
                .Append(TimeOfDay)
                .Append(", ")
                .Append(LTimeValue)
                .Append(", ")
                .Append(DateValue)
                .Append(", ")
                .Append(DateAndTimeValue)
                .Append(", ")
                .Append(StringValue)
                .Append(", ")
                .Append(WStringValue)
                .Append(", ")
                .Append(Nested)
                .Append(" }")
                .ToString();
        }
    }
}