#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : EncodingArgs.cs
// Created   : 2015-6-15
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
    public interface IStrupleEncodingArgs { }

    public abstract class StrupleEncodingArgs : MultiFieldEncodingArgs, IStrupleEncodingArgs
    {
        protected StrupleEncodingArgs(int numBlocks = 1, object custom = null)
            : base(numBlocks, custom)
        {
        }
    }

    public class StrupleEncodingArgs<T> : StrupleEncodingArgs
     {
        public StrupleEncodingArgs()
            : this(new List<Struple<T>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T>> data, 
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

            Levels = new CompressionLevel[1];
            Monotonicities = new Monotonicity[1];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1,T2>> data, 
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

        public IList<Struple<T1, T2>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3>> data, 
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

        public IList<Struple<T1, T2, T3>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4>> data,
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

        public IList<Struple<T1, T2, T3, T4>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5>> data, 
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

        public IList<Struple<T1, T2, T3, T4, T5>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6>> data,
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

        public IList<Struple<T1, T2, T3, T4, T5, T6>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7)
            };

            Levels = new CompressionLevel[7];
            Monotonicities = new Monotonicity[7];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> data, 
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
            };

            Levels = new CompressionLevel[8];
            Monotonicities = new Monotonicity[8];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
            };

            Levels = new CompressionLevel[9];
            Monotonicities = new Monotonicity[9];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
            };

            Levels = new CompressionLevel[10];
            Monotonicities = new Monotonicity[10];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
            };

            Levels = new CompressionLevel[11];
            Monotonicities = new Monotonicity[11];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
                default(T12),
            };

            Levels = new CompressionLevel[12];
            Monotonicities = new Monotonicity[12];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>())
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
                default(T12),
                default(T13),
            };

            Levels = new CompressionLevel[13];
            Monotonicities = new Monotonicity[13];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Data { get; set; }

    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
                default(T12),
                default(T13),
                default(T14),
            };

            Levels = new CompressionLevel[14];
            Monotonicities = new Monotonicity[14];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
                default(T12),
                default(T13),
                default(T14),
                default(T15),
            };

            Levels = new CompressionLevel[15];
            Monotonicities = new Monotonicity[15];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
                default(T12),
                default(T13),
                default(T14),
                default(T15),
                default(T16),
            };

            Levels = new CompressionLevel[16];
            Monotonicities = new Monotonicity[16];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> Data { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : StrupleEncodingArgs
    {
        public StrupleEncodingArgs()
            : this(new List<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>>())
        {
        }

        public StrupleEncodingArgs(IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>> data,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Granularities = new object[]
            {
                default(T1), 
                default(T2), 
                default(T3), 
                default(T4), 
                default(T5), 
                default(T6), 
                default(T7),
                default(T8),
                default(T9),
                default(T10),
                default(T11),
                default(T12),
                default(T13),
                default(T14),
                default(T15),
                default(T16),
                default(T17),
            };

            Levels = new CompressionLevel[17];
            Monotonicities = new Monotonicity[17];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>> Data { get; set; }
    }
}
