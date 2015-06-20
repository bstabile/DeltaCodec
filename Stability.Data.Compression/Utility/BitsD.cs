#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : BitsD.cs
// Created   : 2015-4-24
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System.Runtime.InteropServices;

namespace Stability.Data.Compression.Utility
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BitsD
    {
        [FieldOffset(0)]
        public ulong Bits;

        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public double Value;

        #region Constructors

        public BitsD(double value)
        {
            Bits = 0;
            Long = 0;
            Value = value;
        }

        public BitsD(long value)
        {
            Bits = 0;
            Value = 0.0;
            Long = value;
        }

        public BitsD(ulong value)
        {
            Long = 0;
            Value = 0.0;
            Bits = value;
        }

        #endregion // Constructors

    }

}
