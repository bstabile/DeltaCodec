#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : EncodingArgs.cs
// Created   : 2015-6-27
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System.Collections.Generic;
using System.IO.Compression;

namespace Stability.Data.Compression.DataStructure
{
    #region EncodingArgs

    public interface IEncodingArgs
    {
        dynamic DynamicList { get; }
        int ListCount { get; }
        int NumBlocks { get; set; }
        CompressionLevel Level { get; set; }
        object Custom { get; set; }
    }

    public class EncodingArgs<T> : IEncodingArgs
    {
        public const CompressionLevel DefaultLevel = CompressionLevel.Fastest;
        public const Monotonicity DefaultMonotonicity = Monotonicity.None;

        public EncodingArgs(IList<T> list, int numBlocks = 1, CompressionLevel level = CompressionLevel.Optimal, object custom = null)
        {
            List = list;
            NumBlocks = numBlocks;
            Level = level;
            Custom = custom;
        }

        public dynamic DynamicList { get { return List; } }
        public IList<T> List { get; protected set; }

        /// <summary>
        /// Allows clients to get the count of elements without casting to the generic list type.
        /// </summary>
        public int ListCount
        {
            get { return DynamicList == null ? 0 : DynamicList.Count; }
        }

        public int NumBlocks { get; set; }
        public CompressionLevel Level { get; set; }
        public object Custom { get; set; }
    }

    #endregion // EncodingArgs

    #region NumericEncodingArgs

    public interface INumericEncodingArgs<T> : IEncodingArgs
    {
        T Granularity { get; set; }
        Monotonicity Monotonicity { get; set; }
    }

    public class NumericEncodingArgs<T> : EncodingArgs<T>, INumericEncodingArgs<T>
    {
        public NumericEncodingArgs(IList<T> list, int numBlocks = 1, CompressionLevel level = CompressionLevel.Optimal, 
            T granularity = default(T), Monotonicity monotonicity = Monotonicity.None, object custom = null)
            : base(list, numBlocks, level, custom)
        {
            NumBlocks = numBlocks;
            Level = level;
            Granularity = granularity;
            Monotonicity = monotonicity;
            Custom = custom;
        }

        public T Granularity { get; set; }
        public Monotonicity Monotonicity { get; set; }
    }

    #endregion // NumericEncodingArgs
}
