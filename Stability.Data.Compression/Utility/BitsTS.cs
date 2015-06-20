#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : BitsTS.cs
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
    public struct BitsTS
    {
        [FieldOffset(0)]
        public ulong Bits;

        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public TimeSpan Value;

        #region Constructors

        public BitsTS(TimeSpan value)
        {
            Bits = 0;
            Long = 0;
            Value = value;
        }

        public BitsTS(long value)
        {
            Bits = 0;
            Value = TimeSpan.MinValue;
            Long = value;
        }

        public BitsTS(ulong value)
        {
            Long = 0;
            Value = TimeSpan.MinValue;
            Bits = value;
        }

        #endregion // Constructors

    }

}
