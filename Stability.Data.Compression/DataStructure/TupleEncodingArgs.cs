#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : TupleEncodingArgs.cs
// Created   : 2015-7-2
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
    public interface ITupleEncodingArgs : IMultiFieldEncodingArgs { }

    public abstract class TupleEncodingArgs : MultiFieldEncodingArgs, ITupleEncodingArgs
    {
        protected TupleEncodingArgs(int numBlocks, object custom = null)
            : base(numBlocks, custom)
        {
        }
    }

    public class TupleEncodingArgs<T> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T>>())
        {
        }

        public TupleEncodingArgs(IList<Tuple<T>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T) };

            Levels = new CompressionLevel[2];
            Monotonicities = new Monotonicity[2];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T>> Data { get; set; }

    }

    public class TupleEncodingArgs<T1, T2> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T1, T2>>())
        {
        }

        public TupleEncodingArgs(IList<Tuple<T1, T2>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T1), default(T2) };

            Levels = new CompressionLevel[2];
            Monotonicities = new Monotonicity[2];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T1, T2>> Data { get; set; }

    }

    public class TupleEncodingArgs<T1, T2, T3> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T1, T2, T3>>())
        {
        }

        public TupleEncodingArgs(IList<Tuple<T1, T2, T3>> data, 
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T1), default(T2), default(T3) };

            Levels = new CompressionLevel[3];
            Monotonicities = new Monotonicity[3];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T1, T2, T3>> Data { get; set; }
    }

    public class TupleEncodingArgs<T1, T2, T3, T4> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T1, T2, T3, T4>>())
        {
        }

        public TupleEncodingArgs(IList<Tuple<T1, T2, T3, T4>> data, 
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T1), default(T2), default(T3), default(T4) };

            Levels = new CompressionLevel[4];
            Monotonicities = new Monotonicity[4];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T1, T2, T3, T4>> Data { get; set; }
    }

    public class TupleEncodingArgs<T1, T2, T3, T4, T5> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T1, T2, T3, T4, T5>>())
        {
        }

        public TupleEncodingArgs(
            IList<Tuple<T1, T2, T3, T4, T5>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T1), default(T2), default(T3), default(T4), default(T5) };

            Levels = new CompressionLevel[5];
            Monotonicities = new Monotonicity[5];
            
            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T1, T2, T3, T4, T5>> Data { get; set; }

    }

    public class TupleEncodingArgs<T1, T2, T3, T4, T5, T6> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T1, T2, T3, T4, T5, T6>>())
        {
        }

        public TupleEncodingArgs(
            IList<Tuple<T1, T2, T3, T4, T5, T6>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T1), default(T2), default(T3), default(T4), default(T5), default(T6) };

            Levels = new CompressionLevel[6];
            Monotonicities = new Monotonicity[6];
            
            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T1, T2, T3, T4, T5, T6>> Data { get; set; }
    }

    public class TupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> : TupleEncodingArgs
    {
        public TupleEncodingArgs()
            : this(new List<Tuple<T1, T2, T3, T4, T5, T6, T7>>())
        {
        }

        public TupleEncodingArgs(
            IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[] { default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), default(T7) };

            Levels = new CompressionLevel[7];
            Monotonicities = new Monotonicity[7];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> Data { get; set; }
    }

}
