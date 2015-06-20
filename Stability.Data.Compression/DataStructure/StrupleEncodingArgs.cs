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
    /// <summary>
    /// The generic Struple (structure tuple) types require an ability
    /// to specify arguments for multiple fields. This family of generic
    /// argument classes make it easier to pass the required information.
    /// </summary>
    public abstract class StrupleEncodingArgs
    {
        public const CompressionLevel DefaultLevel = CompressionLevel.Optimal;
        public const Monotonicity DefaultMonotonicity = Monotonicity.None;

        /// <summary>
        /// Resets compression level for all fields.
        /// </summary>
        public abstract void ResetLevel(CompressionLevel level = DefaultLevel);

        /// <summary>
        /// Resets monotonicity for all fields. This should be used with extreme
        /// caution because some transforms may have trouble if the field (vector)
        /// isn't actually monotonic. Generally, you should only use this to reset
        /// to <see cref="Monotonicity.None"/>. You can do this by not providing an
        /// argument. Optimizations related to this setting will usually be negligible.
        /// </summary>
        public abstract void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity);

        /// <summary>
        /// Allows clients to get the count of elements without casting to the generic list type.
        /// </summary>
        public abstract int ListCount { get; }

        /// <summary>
        /// The number of blocks that will encoded/decoded in parallel.
        /// </summary>
        public int NumBlocks { get; set; }
    }

    public class StrupleEncodingArgs<T> : StrupleEncodingArgs
        where T : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(IList<T> list, int numBlocks = 1, CompressionLevel level = DefaultLevel,
            T? granularity = default(T?), Monotonicity monotonicity = DefaultMonotonicity)
        {
            List = list;
            NumBlocks = numBlocks;
            Level = level;
            Granularity = granularity;
            Monotonicity = monotonicity;
        }
        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<T> List { get; set; }
        public CompressionLevel Level { get; set; }
        public T? Granularity { get; set; }
        public Monotonicity Monotonicity { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2> : StrupleEncodingArgs
    where T1 : struct
    where T2 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1,T2>> list, 
            int numBlocks = 1, 
            CompressionLevel level = DefaultLevel, 
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);
            Granularity10 = default(T10?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
            Level10 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
            Monotonicity10 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }
        public T10? Granularity10 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }
        public CompressionLevel Level10 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
        public Monotonicity Monotonicity10 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);
            Granularity10 = default(T10?);
            Granularity11 = default(T11?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
            Level10 = level;
            Level11 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
            Monotonicity10 = monotonicity;
            Monotonicity11 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }
        public T10? Granularity10 { get; set; }
        public T11? Granularity11 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }
        public CompressionLevel Level10 { get; set; }
        public CompressionLevel Level11 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
        public Monotonicity Monotonicity10 { get; set; }
        public Monotonicity Monotonicity11 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);
            Granularity10 = default(T10?);
            Granularity11 = default(T11?);
            Granularity12 = default(T12?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
            Level10 = level;
            Level11 = level;
            Level12 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
            Monotonicity10 = monotonicity;
            Monotonicity11 = monotonicity;
            Monotonicity12 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }
        public T10? Granularity10 { get; set; }
        public T11? Granularity11 { get; set; }
        public T12? Granularity12 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }
        public CompressionLevel Level10 { get; set; }
        public CompressionLevel Level11 { get; set; }
        public CompressionLevel Level12 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
        public Monotonicity Monotonicity10 { get; set; }
        public Monotonicity Monotonicity11 { get; set; }
        public Monotonicity Monotonicity12 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
        where T13 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);
            Granularity10 = default(T10?);
            Granularity11 = default(T11?);
            Granularity12 = default(T12?);
            Granularity13 = default(T13?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
            Level10 = level;
            Level11 = level;
            Level12 = level;
            Level13 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
            Monotonicity10 = monotonicity;
            Monotonicity11 = monotonicity;
            Monotonicity12 = monotonicity;
            Monotonicity13 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }
        public T10? Granularity10 { get; set; }
        public T11? Granularity11 { get; set; }
        public T12? Granularity12 { get; set; }
        public T13? Granularity13 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }
        public CompressionLevel Level10 { get; set; }
        public CompressionLevel Level11 { get; set; }
        public CompressionLevel Level12 { get; set; }
        public CompressionLevel Level13 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
        public Monotonicity Monotonicity10 { get; set; }
        public Monotonicity Monotonicity11 { get; set; }
        public Monotonicity Monotonicity12 { get; set; }
        public Monotonicity Monotonicity13 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
        where T13 : struct
        where T14 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);
            Granularity10 = default(T10?);
            Granularity11 = default(T11?);
            Granularity12 = default(T12?);
            Granularity13 = default(T13?);
            Granularity14 = default(T14?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
            Level10 = level;
            Level11 = level;
            Level12 = level;
            Level13 = level;
            Level14 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
            Monotonicity10 = monotonicity;
            Monotonicity11 = monotonicity;
            Monotonicity12 = monotonicity;
            Monotonicity13 = monotonicity;
            Monotonicity14 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }
        public T10? Granularity10 { get; set; }
        public T11? Granularity11 { get; set; }
        public T12? Granularity12 { get; set; }
        public T13? Granularity13 { get; set; }
        public T14? Granularity14 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }
        public CompressionLevel Level10 { get; set; }
        public CompressionLevel Level11 { get; set; }
        public CompressionLevel Level12 { get; set; }
        public CompressionLevel Level13 { get; set; }
        public CompressionLevel Level14 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
        public Monotonicity Monotonicity10 { get; set; }
        public Monotonicity Monotonicity11 { get; set; }
        public Monotonicity Monotonicity12 { get; set; }
        public Monotonicity Monotonicity13 { get; set; }
        public Monotonicity Monotonicity14 { get; set; }
    }

    public class StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : StrupleEncodingArgs
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
        where T13 : struct
        where T14 : struct
        where T15 : struct
    {
        public StrupleEncodingArgs()
            : this(null)
        {
        }

        public StrupleEncodingArgs(
            IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> list,
            int numBlocks = 1,
            CompressionLevel level = DefaultLevel,
            Monotonicity monotonicity = DefaultMonotonicity)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            NumBlocks = numBlocks;

            Granularity1 = default(T1?);
            Granularity2 = default(T2?);
            Granularity3 = default(T3?);
            Granularity4 = default(T4?);
            Granularity5 = default(T5?);
            Granularity6 = default(T6?);
            Granularity7 = default(T7?);
            Granularity8 = default(T8?);
            Granularity9 = default(T9?);
            Granularity10 = default(T10?);
            Granularity11 = default(T11?);
            Granularity12 = default(T12?);
            Granularity13 = default(T13?);
            Granularity14 = default(T14?);
            Granularity15 = default(T15?);

            ResetLevel(level);
            ResetMonotonicity(monotonicity);
        }

        public override void ResetLevel(CompressionLevel level = DefaultLevel)
        {
            Level1 = level;
            Level2 = level;
            Level3 = level;
            Level4 = level;
            Level5 = level;
            Level6 = level;
            Level7 = level;
            Level8 = level;
            Level9 = level;
            Level10 = level;
            Level11 = level;
            Level12 = level;
            Level13 = level;
            Level14 = level;
            Level15 = level;
        }

        public override void ResetMonotonicity(Monotonicity monotonicity = DefaultMonotonicity)
        {
            Monotonicity1 = monotonicity;
            Monotonicity2 = monotonicity;
            Monotonicity3 = monotonicity;
            Monotonicity4 = monotonicity;
            Monotonicity5 = monotonicity;
            Monotonicity6 = monotonicity;
            Monotonicity7 = monotonicity;
            Monotonicity8 = monotonicity;
            Monotonicity9 = monotonicity;
            Monotonicity10 = monotonicity;
            Monotonicity11 = monotonicity;
            Monotonicity12 = monotonicity;
            Monotonicity13 = monotonicity;
            Monotonicity14 = monotonicity;
            Monotonicity15 = monotonicity;
        }

        public override int ListCount { get { return List == null ? 0 : List.Count; } }
        public IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> List { get; set; }

        public T1? Granularity1 { get; set; }
        public T2? Granularity2 { get; set; }
        public T3? Granularity3 { get; set; }
        public T4? Granularity4 { get; set; }
        public T5? Granularity5 { get; set; }
        public T6? Granularity6 { get; set; }
        public T7? Granularity7 { get; set; }
        public T8? Granularity8 { get; set; }
        public T9? Granularity9 { get; set; }
        public T10? Granularity10 { get; set; }
        public T11? Granularity11 { get; set; }
        public T12? Granularity12 { get; set; }
        public T13? Granularity13 { get; set; }
        public T14? Granularity14 { get; set; }
        public T15? Granularity15 { get; set; }

        public CompressionLevel Level1 { get; set; }
        public CompressionLevel Level2 { get; set; }
        public CompressionLevel Level3 { get; set; }
        public CompressionLevel Level4 { get; set; }
        public CompressionLevel Level5 { get; set; }
        public CompressionLevel Level6 { get; set; }
        public CompressionLevel Level7 { get; set; }
        public CompressionLevel Level8 { get; set; }
        public CompressionLevel Level9 { get; set; }
        public CompressionLevel Level10 { get; set; }
        public CompressionLevel Level11 { get; set; }
        public CompressionLevel Level12 { get; set; }
        public CompressionLevel Level13 { get; set; }
        public CompressionLevel Level14 { get; set; }
        public CompressionLevel Level15 { get; set; }

        public Monotonicity Monotonicity1 { get; set; }
        public Monotonicity Monotonicity2 { get; set; }
        public Monotonicity Monotonicity3 { get; set; }
        public Monotonicity Monotonicity4 { get; set; }
        public Monotonicity Monotonicity5 { get; set; }
        public Monotonicity Monotonicity6 { get; set; }
        public Monotonicity Monotonicity7 { get; set; }
        public Monotonicity Monotonicity8 { get; set; }
        public Monotonicity Monotonicity9 { get; set; }
        public Monotonicity Monotonicity10 { get; set; }
        public Monotonicity Monotonicity11 { get; set; }
        public Monotonicity Monotonicity12 { get; set; }
        public Monotonicity Monotonicity13 { get; set; }
        public Monotonicity Monotonicity14 { get; set; }
        public Monotonicity Monotonicity15 { get; set; }
    }
}
