#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : MapEncodingArgs.cs
// Created   : 2015-7-5
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Stability.Data.Compression.DataStructure
{
    public interface IMapEncodingArgs : IMultiFieldEncodingArgs
    {
    }

    public abstract class MapEncodingArgs : MultiFieldEncodingArgs, IMapEncodingArgs
    {
        protected MapEncodingArgs(int numBlocks, dynamic custom)
        {
            NumBlocks = numBlocks;
            Custom = custom;
        }
    }

    public class MapEncodingArgs<T1, T2> : MapEncodingArgs
    {
        public MapEncodingArgs()
            : this(new Dictionary<T1, T2>())
        {
        }

        public MapEncodingArgs(
            IDictionary<T1, T2> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities[0] = default(T1);
            Granularities[1] = default(T2);

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IDictionary<T1, T2> Data { get; set; }
    }
}
