#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : CodecTestConfig.cs
// Created   : 2015-5-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System.IO.Compression;

namespace Stability.Data.Compression.TestUtility
{
    public class CodecTestConfig
    {
        public CodecTestConfig(IDeltaCodec codec,
            string displayName,
            CompressionLevel level = CompressionLevel.Optimal,
            Monotonicity monotonicity = Monotonicity.None,
            int numBlocks = 1,
            bool isGranular = false,
            bool validate = true)
        {
            Codec = codec;
            DisplayName = displayName;
            Level = level;
            Monotonicity = monotonicity;
            NumBlocks = numBlocks;
            IsGranular = isGranular;
            Validate = validate;
        }
        
        public IDeltaCodec Codec { get; set; }

        public string DisplayName { get; set; }
        public CompressionLevel Level { get; set; }
        public bool IsGranular { get; set; }
        public Monotonicity Monotonicity { get; set; }
        public int NumBlocks { get; set; }
        public bool Validate { get; set; }
    }
}
