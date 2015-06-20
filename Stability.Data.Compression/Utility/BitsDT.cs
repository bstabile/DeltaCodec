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
    public struct BitsDT
    {
        [FieldOffset(0)]
        public ulong Bits;

        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public DateTime Value;

        #region Constructors

        public BitsDT(DateTime value)
        {
            Bits = 0;
            Long = 0;
            Value = value;
        }

        public BitsDT(long value)
        {
            Bits = 0;
            Value = DateTime.MinValue;
            Long = value;
        }

        public BitsDT(ulong value)
        {
            Long = 0;
            Value = DateTime.MinValue;
            Bits = value;
        }

        #endregion // Constructors

    }

}
