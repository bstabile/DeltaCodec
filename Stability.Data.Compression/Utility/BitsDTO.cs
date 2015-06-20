#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : BitsDT.cs
// Created   : 2015-4-24
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Runtime.InteropServices;

namespace Stability.Data.Compression.Utility
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BitsDTO
    {
        [FieldOffset(0)]
        public ulong Bits;

        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public DateTime Value;

        [FieldOffset(64)] 
        public short Offset;

        [FieldOffset(64)] 
        public ushort OffsetBits;

        #region Constructors

        public BitsDTO(DateTimeOffset value)
        {
            Bits = 0;
            Long = 0;
            Value = value.DateTime;
            OffsetBits = 0;
            Offset = (short)(value.Offset.Ticks/TimeSpan.TicksPerMinute);
        }

        public BitsDTO(long value, short offset)
        {
            Bits = 0;
            Value = DateTime.MinValue;
            Long = value;
            OffsetBits = 0;
            Offset = offset;
        }

        public BitsDTO(ulong value, ushort offsetBits)
        {
            Long = 0;
            Value = DateTime.MinValue;
            Bits = value;
            Offset = 0;
            OffsetBits = offsetBits;
        }

        #endregion // Constructors

    }

}
