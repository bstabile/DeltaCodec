#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : CodecTest.cs
// Created   : 2015-4-22
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stability.Data.Compression.TestUtility
{
    public interface ICodecTest
    {
        IDeltaCodec Codec { get; }
        string DisplayName { get; set; }
        bool Underline { get; set; }
        CodecTestGroup Group { get; set; }
        int RunnerId { get; set; }
        int Precision { get; set; }
        CodecTestStatistics Stats { get; }
        CompressionLevel Level { get; set; }
        Monotonicity Monotonicity { get; set; }
        FactorMode FactorMode { get; set; }
        int NumBlocks { get; set; }
        bool IsParallel { get; set; }
        bool Validate { get; set; }
        void Run();
    }

    public class CodecTest<T> : ICodecTest
        where T : struct, IComparable<T>, IEquatable<T>
    {
        public CodecTest(
            string displayName,
            IList<T> list,
            IDeltaCodec codec, 
            CompressionLevel level = CompressionLevel.Fastest,
            T granularity = default(T), 
            Monotonicity monotonicity = Monotonicity.None, 
            bool validate = true,
            bool isParallel = false)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (list.Count == 0)
                throw new InvalidOperationException("The input list is empty!");

            DisplayName = displayName;
            Codec = codec;
            ListIn = list;
            IsParallel = isParallel;

            Granularity = granularity;
            Factor = Granularity;
     
            Precision = GenericPrecision.Calc(Granularity);
            Level = level;
            Monotonicity = monotonicity;
            Validate = validate;

            Stats = new CodecTestStatistics(typeof (T), ListIn.Count);
        }

        #region Public Properties

        public bool Underline { get; set; }

        public IList<T> ListIn { get; protected set; }

        public T Granularity { get; protected set; }

        public T Factor { get; protected set; }

        public FactorMode FactorMode { get; set; }

        public IDeltaCodec Codec { get; protected set; }

        public string DisplayName { get; set; }

        public CodecTestGroup Group { get; set; }

        public int RunnerId { get; set; }

        public CompressionLevel Level { get; set; }

        public Monotonicity Monotonicity { get; set; }

        public int NumBlocks { get; set; }

        public bool IsParallel { get; set; }

        public bool Validate { get; set; }

        public int Precision { get; set; }

        public CodecTestStatistics Stats { get; protected set; }

        #endregion // Public Properties

        public void Run()
        {
            Stats.Reset();
            if (ListIn == null)
                throw new InvalidOperationException("ListIn is null!");

            var arr = ListIn.ToArray();

            // Auto = 0, Granular = Known Granularity, None = empty (will be replaced with 1 by codec)
            // We have to do this here because there is no easy way to specify 1 for a generic type.
            var factor = FactorMode == FactorMode.Auto ? default(T)
                : FactorMode == FactorMode.Granular ? Granularity : default(T?);

            NumBlocks = IsParallel ? Environment.ProcessorCount * 1 : 1;

            GC.Collect(generation: 2, mode: GCCollectionMode.Forced, blocking: true);

            // WARMUP BEGIN (JIT compile the codec and spin up the thread pool on sample of data)
            if (arr.Length > 4)
            {
                var seg = new ArraySegment<T>(arr, 0, arr.Length / 4);
                var tmp = Codec.Encode(seg);
                Codec.Decode<T>(tmp);
            }
            // WARMUP END

            var stopwatch = Stopwatch.StartNew();
            var bytes = Codec.Encode(arr, NumBlocks, Level, factor, Monotonicity);
            Stats.ElapsedEncode = stopwatch.Elapsed;

            GC.Collect(generation: 2, mode: GCCollectionMode.Forced, blocking: true);

            stopwatch.Restart();
            // Number of blocks is stored with the data, so parallelism is implicit
            var listOut = Codec.Decode<T>(bytes);
            Stats.ElapsedDecode = stopwatch.Elapsed;

            stopwatch.Stop();

            ValidateResults(arr, listOut);
            Stats.Calc(bytes);

            //ThreadPool.GetMinThreads(out minWorker, out minIOC);

        }

        #region Validation

        public void ValidateResults(IList<T> listIn, IList<T> listOut)
        {
            if (!Validate) return;

            Assert.IsNotNull(listIn);
            Assert.IsNotNull(listOut);
            Assert.AreEqual(listIn.Count, listOut.Count);

            // Doubles are inexact, so they are handled differently when granularity is requested.
            //if (typeof(T) == typeof(double) && !Granularity.Equals(1))
            //{
            //    ValidateDoubles((IList<double>)listIn, (IList<double>)listOut);
            //    return;
            //}

            // Floats are inexact, so they are handled differently when granularity is requested.
            //if (typeof(T) == typeof(float) && !Granularity.Equals(1))
            //{
            //    ValidateFloats((IList<float>)listIn, (IList<float>)listOut);
            //    return;
            //}

            for (var i = 0; i < listIn.Count; i++)
            {
                if (!listIn[i].Equals(listOut[i]))
                {
                    var s = string.Format("{0} : ListIn = {1} ListOut = {2}", i, listIn[i], listOut[i]);
                    Trace.WriteLine(s);
                }
                Assert.AreEqual(listIn[i], listOut[i]);
            }
        }

        private void ValidateDoubles(IList<double> listIn, IList<double> listOut)
        {
            var granularity = Convert.ToDouble(Granularity);

            if (Granularity.Equals(0) || Granularity.Equals(1))
            {
                for (var i = 0; i < listIn.Count; i++)
                {
                    Assert.AreEqual(listIn[i], listOut[i]);
                }
                return;
            }
            try
            {
                var precision = GenericPrecision.Calc(Granularity);
                for (var i = 0; i < listIn.Count; i++)
                {
                    var vin = listIn[i];
                    var vout = listOut[i];
                    //if (!vin.Equals(vout))
                    //{
                    //    Debug.WriteLine("{0} : Expect {1}  Actual {2}",
                    //        i, vin.ToString("R"), vout.ToString("R"));
                    //}
                    var sin = vin.ToString("F" + precision);
                    var sout = vout.ToString("F" + precision);
                    if (sin != sout)
                    {

                        Console.WriteLine("{0} Error: ValueIn = {1} ValueOut = {2} Grain = {3} Precision = {4}",
                            DisplayName, sin, sout, granularity, Precision);
                        Assert.Fail("{0} : Difference between listIn[{1}] and listOut[{1}] is greater than expected.",
                            DisplayName, i);
                    }
                }
            }
            catch (Exception ex)
            {
                ////Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void ValidateFloats(IList<float> listIn, IList<float> listOut)
        {
            var granularity = Convert.ToSingle(Granularity);

            if (granularity.Equals(0.0f) || granularity.Equals(1))
            {
                for (var i = 0; i < listIn.Count; i++)
                {
                    Assert.AreEqual(listIn[i], listOut[i]);
                }
                return;
            }
            try
            {
                var precision = GenericPrecision.Calc(Granularity);
                for (var i = 0; i < listIn.Count; i++)
                {
                    var vin = listIn[i];
                    var vout = listOut[i];
                    //if (!vin.Equals(vout))
                    //{
                    //    Debug.WriteLine("{0} : Expect {1}  Actual {2}",
                    //        i, vin.ToString("R"), vout.ToString("R"));
                    //}
                    var sin = listIn[i].ToString("F" + precision);
                    var sout = listOut[i].ToString("F" + precision);
                    if (sin != sout)
                    {

                        Console.WriteLine("{0} Error: ValueIn = {1} ValueOut = {2} Grain = {3} Precision = {4}",
                            DisplayName, sin, sout, granularity, precision);
                        Assert.Fail("Difference between listIn[{0}] and listOut[{0}] is greater than expected.", i);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion // Validation

        public override string ToString()
        {
            return TestDisplay.ToString(this);
        }
        
    }
}
