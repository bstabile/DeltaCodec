#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : DeltaFlags.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.IO.Compression;

namespace Stability.Data.Compression.Utility
{
    /// <summary>
    /// DataTypeCode: Bits 0-3 (bottom nibble)
    /// CompressionLevel: Bits 4-5 (NoCompression, Fastest, Optimal)
    /// Monotonicity: Bits 6-7 (None, NonDecreasing, NonIncreasing)
    /// Reserved: Bits 8-12
    /// AliasDataTypeCode Bits 13-16 (top nibble)
    /// </summary>
    public class DeltaFlags
    {
        public DeltaFlags(Type dataType,
            CompressionLevel level = CompressionLevel.Fastest, 
            Monotonicity monotonicity = Monotonicity.None, 
            bool isAliased = false,
            Type aliasDataType = null)
        {
            DataTypeCode = DeltaType.TypeToCode(dataType);
            Level = level;
            Monotonicity = monotonicity;
            IsAliased = isAliased;
            AliasDataTypeCode = DeltaType.TypeToCode(aliasDataType);
        }

        public byte DataTypeCode { get; set; }

        /// <summary>
        /// By default this is zero, which is the code for "object",
        /// but it really means that it is unspecified.
        /// </summary>
        public byte AliasDataTypeCode { get; set; }

        public CompressionLevel Level { get; set; }

        public Monotonicity Monotonicity { get; set; }

        /// <summary>
        /// This is used if a type can be aliased to an alternate type.
        /// This is currently only used for Real types such as <see cref="System.Double"/>.
        /// DateTime and TimeSpan are encoded as Ticks (which we don't consider aliasing).
        /// </summary>
        public bool IsAliased { get; set; }

        public ushort Value
        {
            get
            {
                var lev = Level == CompressionLevel.NoCompression
                    ? 0
                    : Level == CompressionLevel.Fastest
                        ? 1
                        : 2; // CompressionLevel.Optimial

                var isAliased = IsAliased ? (1 << 8) : 0;
                var aliasType = (ushort)(AliasDataTypeCode << 12); // Top nibble

                return (ushort) (DataTypeCode | (lev << 4) | ((int) Monotonicity << 6) | isAliased | aliasType);
            }
            set
            {
                DataTypeCode = (byte)(value & 0xF);

                var lev = (value >> 4) & 3;

                Level = lev == 0
                    ? CompressionLevel.NoCompression
                    : lev == 1 ? CompressionLevel.Fastest : CompressionLevel.Optimal;

                Monotonicity = (Monotonicity)((value >> 6) & 3);
                IsAliased = ((value >> 8) & 1) != 0;
                AliasDataTypeCode = (byte)((value >> 12) & 0xF); // Top nibble
            }
        }
    }
}
