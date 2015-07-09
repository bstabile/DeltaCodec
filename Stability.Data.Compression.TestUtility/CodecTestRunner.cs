#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : CodecTestRunner.cs
// Created   : 2015-4-22
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public interface ICodecTestRunner
    {
        string DisplayName { get; set; }
        CodecTestGroup Group { get; set; }
        int RunnerId { get; }
        int RunCount { get; set; }
        bool IsParallel { get; set; }
        bool Underline { get; set; }
        IDeltaCodec Codec { get; set; }
        CompressionLevel Level { get; set; }
        object ListIn { get; }
        int ListCount { get; }
        Type DataType { get; }

        double BufferEntropy { get; set; }
        double DeltaEntropy { get; set; }
        double EntropyOut { get; set; }

        int BytesForType { get; }
        int RawBytes { get; }
        IList<ICodecTest> Tests { get; }

        object Granularity { get; }
        FactorMode FactorMode { get; set; }
        int Precision { get; }
        Monotonicity Monotonicity { get; }
    }

    public abstract class TestRunner : ICodecTestRunner
    {
        protected static readonly object RunnerIdLock = new object();
        protected static int NextRunnerId;

        public string DisplayName { get; set; }
        public CodecTestGroup Group { get; set; }
        public int RunnerId { get; protected set; }
        public int RunCount { get; set; }
        public bool IsParallel { get; set; }
        public bool Underline { get; set; }
        public IDeltaCodec Codec { get; set; }
        public CompressionLevel Level { get; set; }
        public object ListIn { get; protected set; }
        public int ListCount { get; protected set; }
        public Type DataType { get; protected set; }

        public double BufferEntropy { get; set; }
        public double DeltaEntropy { get; set; }
        public double EntropyOut { get; set; }

        public int BytesForType { get; protected set; }
        public int RawBytes { get; protected set; }
        public IList<ICodecTest> Tests { get; protected set; }

        public object Granularity { get; protected set; }
        public FactorMode FactorMode { get; set; }
        public int Precision { get; protected set; }
        public Monotonicity Monotonicity { get; protected set; }

        public abstract void RunStructTest<T>(int runCount, IList<T> list, T granularity, Monotonicity monotonicity,
            bool validate)
            where T : struct, IComparable<T>, IEquatable<T>;

        public abstract void RunClassTest<T>(int runCount, IList<T> list, bool validate)
            where T : class, IComparable<T>, IEquatable<T>;
    }

    public class CodecTestRunner : TestRunner
   {
        public CodecTestRunner(CodecTestGroup group = null)
        {
            Tests = new List<ICodecTest>();
            lock (RunnerIdLock)
            {
                RunnerId = NextRunnerId;
                NextRunnerId++;
            }
            Group = group;
        }

        public override void RunStructTest<T>(int runCount, IList<T> list, T granularity, Monotonicity monotonicity, bool validate)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (list.Count == 0)
                throw new InvalidOperationException("The input list is empty!");

            RunCount = runCount;
            DataType = typeof(T);

            ListIn = list;
            ListCount = list.Count;
            Granularity = granularity;

            Precision = GenericPrecision.Calc(granularity);
            Monotonicity = monotonicity;

            BytesForType = DeltaUtility.GetSizeOfType(typeof(T), list);
            RawBytes = list.Count * BytesForType;

            RunNumericWarmup(list, validate);

            Tests.Clear();
            for (var i = 0; i < runCount; i++)
            {
                var t = new CodecStructTest<T>(DisplayName, list, Codec, Level, granularity, monotonicity,
                    validate, IsParallel) { Group = Group, RunnerId = RunnerId, Underline = Underline, FactorMode = FactorMode };
                Tests.Add(t);
                t.Run();
            }
        }

        public override void RunClassTest<T>(int runCount, IList<T> list, bool validate)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (list.Count == 0)
                throw new InvalidOperationException("The input list is empty!");

            RunCount = runCount;
            DataType = typeof(T);

            ListIn = list;
            ListCount = list.Count;


            BytesForType = DeltaUtility.GetSizeOfType(typeof(T), list);
            RawBytes = list.Count * BytesForType;

            RunWarmup(list, validate);

            Tests.Clear();
            for (var i = 0; i < runCount; i++)
            {
                var t = new CodecClassTest<T>(DisplayName, list, Codec, Level,validate, IsParallel) 
                { Group = Group, RunnerId = RunnerId, Underline = Underline };
                Tests.Add(t);
                t.Run();
            }
        }

        private void RunNumericWarmup<T>(IList<T> list, bool validate)
            where T : IComparable<T>, IEquatable<T>
        {
            var t = new CodecClassTest<T>(DisplayName, list, Codec, Level, validate, IsParallel)
            {
                Group = Group,
                RunnerId = RunnerId,
                Underline = Underline
            };

            var arr = list.ToArray();
            if (arr.Length > 4)
            {
                var seg = new ArraySegment<T>(arr, 0, arr.Length / 4);
                var args = new NumericEncodingArgs<T>(seg, t.NumBlocks, t.Level, (T)Granularity, Monotonicity);
                var tmp = Codec.Encode(args);
                Codec.Decode<T>(tmp);
            }
        }

        private void RunWarmup<T>(IList<T> list, bool validate)
            where T : IComparable<T>, IEquatable<T>
        {
            var t = new CodecClassTest<T>(DisplayName, list, Codec, Level, validate, IsParallel)
            {
                Group = Group,
                RunnerId = RunnerId,
                Underline = Underline
            };

            var arr = list.ToArray();
            if (arr.Length > 4)
            {
                var seg = new ArraySegment<T>(arr, 0, arr.Length / 4);
                var args = new EncodingArgs<T>(seg, t.NumBlocks, t.Level);
                var tmp = Codec.Encode(args);
                Codec.Decode<T>(tmp);
            }
        }

   }

}
