#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : BitsF.cs
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
    public struct BitsF
    {
        [FieldOffset(0)]
        public uint Bits;

        [FieldOffset(0)]
        public int Int;

        [FieldOffset(0)]
        public float Value;

        #region Constructors

        public BitsF(float value)
        {
            Bits = 0;
            Int = 0;
            Value = value;
        }

        public BitsF(int value)
        {
            Bits = 0;
            Value = 0.0f;
            Int = value;
        }

        public BitsF(uint value)
        {
            Int = 0;
            Value = 0.0f;
            Bits = value;
        }

        #endregion // Constructors

    }

}
