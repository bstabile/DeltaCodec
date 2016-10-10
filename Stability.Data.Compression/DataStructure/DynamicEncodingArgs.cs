#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : StrupleEncodingArgs.cs
// Created   : 2015-6-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;

namespace Stability.Data.Compression.DataStructure
{
    public interface IDynamicEncodingArgs { }

    public abstract class DynamicEncodingArgs : MultiFieldEncodingArgs, IDynamicEncodingArgs
    {
        protected DynamicEncodingArgs(int numBlocks = 1, object custom = null)
            : base(numBlocks, custom)
        {
        }
    }

    public class DynamicEncodingArgs<T> : DynamicEncodingArgs
    {
        public DynamicEncodingArgs()
            : this(new List<T>())
        {
        }

        public DynamicEncodingArgs(IList<T> data, 
            int numBlocks = 1, 
            CompressionLevel level = DefaultLevel, 
            Monotonicity monotonicity = DefaultMonotonicity,
            object custom = null)
            : base(numBlocks, custom)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;

            Type type = typeof(T);
            Granularities = new object[type.GetFields().Length];

            int i = 0;
            foreach (FieldInfo property in type.GetFields())
            {
                Granularities[i] = property.FieldType.IsValueType ? Activator.CreateInstance(property.FieldType) : null;
                i++;
            }

            //Granularities = new object[] { default(T) };

            Levels = new CompressionLevel[type.GetFields().Length];
            Monotonicities = new Monotonicity[type.GetFields().Length];

            ResetLevels(level);
            ResetMonotonicities(monotonicity);
        }

        public override dynamic DynamicData { get { return Data; } }

        public IList<T> Data { get; set; }
    }

}
