#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : DeltaBlockState.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

using System.Collections.Generic;
using System.IO.Compression;
using Stability.Data.Compression.Finishers;

namespace Stability.Data.Compression.Utility
{
    public class DeltaBlockState<T> : BlockState<T>
    {
        public DeltaBlockState(IList<T> list, 
            CompressionLevel level = CompressionLevel.Fastest,
            T granularity = default(T), 
            Monotonicity monotonicity = Monotonicity.None, 
            IFinisher finisher = null,
            int blockIndex = 0)
            : base(list, level, finisher, blockIndex)
        {
            Factor = granularity;
            Flags.Monotonicity = monotonicity;
        }

        public DeltaBlockState(byte[] bytes, IFinisher finisher = null, int blockIndex = 0)
            : base(bytes, finisher, blockIndex)
        {
        }


        /// <summary>
        /// Usually, this value is determined during encoding and set here.
        /// But it is also possible to pass in a factor (if known in advance),
        /// and that will then result in a substantial performance gain.
        /// If the value is initially set to default(T), the encoding method will
        /// calculate the factor internally, and save the result here.
        /// 
        /// This value must always be serialized with other header information.
        /// </summary>
        public T Factor { get; set; }

    }
}
