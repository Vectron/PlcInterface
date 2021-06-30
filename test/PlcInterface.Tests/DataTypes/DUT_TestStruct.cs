using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PlcInterface.Tests.DataTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct DUT_TestStruct : IEquatable<DUT_TestStruct>
    {
        public static DUT_TestStruct Default => new DUT_TestStruct()
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
            ComplexArray = new DUT_TestStruct2[] { DUT_TestStruct2.Default, DUT_TestStruct2.Default, DUT_TestStruct2.Default },
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

        public override bool Equals(object obj) => obj is DUT_TestStruct @struct && Equals(@struct);

        public bool Equals(DUT_TestStruct other)
        {
            AssertExtension.DUT_TestStructEquals(this, other);
            return BoolValue == other.BoolValue
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
                && StringValue == other.StringValue && WStringValue == other.WStringValue
                && EqualityComparer<DUT_TestStruct2>.Default.Equals(Nested, other.Nested)
                && IntArray.SequenceEqual(other.IntArray)
                && MultiDimensionArray.SequenceEqual<short>(other.MultiDimensionArray)
                && ComplexArray.SequenceEqual(other.ComplexArray)
                && MultiDimensionComplexArray.SequenceEqual<DUT_TestStruct2>(other.MultiDimensionComplexArray);
        }

        public override string ToString()
            => new StringBuilder()
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

        public override int GetHashCode()
        {
            var hashCode = 1307849462;
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
            hashCode = hashCode * -1521134295 + DateValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DateAndTimeValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StringValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WStringValue);
            hashCode = hashCode * -1521134295 + Nested.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<short[]>.Default.GetHashCode(IntArray);
            hashCode = hashCode * -1521134295 + EqualityComparer<short[,,]>.Default.GetHashCode(MultiDimensionArray);
            hashCode = hashCode * -1521134295 + EqualityComparer<DUT_TestStruct2[]>.Default.GetHashCode(ComplexArray);
            hashCode = hashCode * -1521134295 + EqualityComparer<DUT_TestStruct2[,,]>.Default.GetHashCode(MultiDimensionComplexArray);
            return hashCode;
        }

        public static bool operator ==(DUT_TestStruct left, DUT_TestStruct right) => left.Equals(right);

        public static bool operator !=(DUT_TestStruct left, DUT_TestStruct right) => !(left == right);
    }
}