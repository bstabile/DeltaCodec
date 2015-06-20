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
using System.Text;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public class CodecTestRunner
   {
        private static readonly object RunnerIdLock = new object();
        private static int _runnerId;

        public CodecTestRunner(CodecTestGroup group = null)
        {
            Tests = new List<ICodecTest>();
            lock (RunnerIdLock)
            {
                RunnerId = _runnerId;
                _runnerId++;
            }
            Group = group;
        }

        #region Public Properties

        public string DisplayName { get; set; }
        public CodecTestGroup Group { get; set; }
        public int RunnerId { get; private set; }
        public int RunCount { get; set; }
        public bool IsParallel { get; set; }
        public bool Underline { get; set; }
        public IDeltaCodec Codec { get; set; }
        public CompressionLevel Level { get; set; }
        public object ListIn { get; private set; }
        public int ListCount { get; private set; }
        public Type DataType { get; private set; }
        public object Granularity { get; private set; }
        public FactorMode FactorMode { get; set; }
        public int Precision { get; private set; }
        public Monotonicity Monotonicity { get; private set; }
        public double BufferEntropy { get; set; }
        public double DeltaEntropy { get; set; }
        public double EntropyOut { get; internal set; }
        public int BytesForType { get; private set; }
        public int RawBytes { get; private set; }
        public IList<ICodecTest> Tests { get; private set; }

        #endregion // Public Properties
        
        #region Public Methods

        public void Run<T>(int runCount, IList<T> list, T granularity, Monotonicity monotonicity, bool validate)
         where T : struct, IComparable<T>, IEquatable<T>
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

            BytesForType = DeltaUtility.GetSizeOfIntrinsicType(typeof(T));
            RawBytes = list.Count * BytesForType;

            Tests.Clear();
            for (var i = 0; i < runCount; i++)
            {
                var t = new CodecTest<T>(DisplayName, list, Codec, Level, granularity, monotonicity,
                    validate, IsParallel) { Group = Group, RunnerId = RunnerId, Underline = Underline, FactorMode = FactorMode };
                Tests.Add(t);
                t.Run();
            }
        }

        #endregion // Public Methods

    }
}
