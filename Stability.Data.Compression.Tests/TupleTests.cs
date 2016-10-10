#region License

// Namespace : Stability.Data.Compression.Tests
// FileName  : TupleTests.cs
// Created   : 2015-7-2
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.TestUtility;
using Stability.Data.Compression.ThirdParty;

namespace Stability.Data.Compression.Tests
{
    [TestClass]
    public class TupleTests
    {
        #region Configuration

        private const bool IsDelta = true;
        private const int DefaultDataCount = 10000;
        private const CompressionLevel DefaultCompressionLevel = CompressionLevel.Optimal;
        private static readonly int DefaultNumBlocks = Environment.ProcessorCount;
        //private static readonly int DefaultNumBlocks = 1;
        private static readonly PrefabLists Lists = new PrefabLists(DefaultDataCount);

        #region Codecs

        private static readonly IDeltaCodec[] DeltaCodecs =
        {
            //RandomWalkCodec.Instance,
            DeflateDeltaCodec.Instance,
            IonicDeflateDeltaCodec.Instance,
            IonicZlibDeltaCodec.Instance,
            SharpDeflateDeltaCodec.Instance,
            QuickLZDeltaCodec.Instance,
            LZ4DeltaCodec.Instance,
            IonicBZip2DeltaCodec.Instance,
            SharpBZip2DeltaCodec.Instance,
        };

        private static readonly IDeltaCodec[] NullTransformCodecs =
        {
            //RandomWalkCodec.Instance,
            DeflateCodec.Instance,
            IonicDeflateCodec.Instance,
            IonicZlibCodec.Instance,
            SharpDeflateCodec.Instance,
            QuickLZCodec.Instance,
            LZ4Codec.Instance,
            IonicBZip2Codec.Instance,
            SharpBZip2Codec.Instance,
        };

        private static readonly IDeltaCodec[] Codecs = IsDelta ? DeltaCodecs : NullTransformCodecs;

        #endregion // Codecs

        private static string GetTestConfigurationName()
        {
            var sb = new StringBuilder();
            sb.Append(DefaultNumBlocks > 1 ? "Parallel" : "Serial");
            sb.Append(IsDelta ? "Delta" : "");
            sb.Append("Granular");
            var level = "None";
            switch (DefaultCompressionLevel)
            {
                case CompressionLevel.Fastest:
                    level = "Fast";
                    break;
                case CompressionLevel.Optimal:
                    level = "Optimal";
                    break;
                default:
                    level = "None";
                    break;
            }
            sb.Append(level);
            return sb.ToString();
        }

        #endregion // Configuration

        [TestMethod]
        public void Tuple02()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;

            var count = t0.Count;

            var tuples = new List<Tuple<DateTime, TimeSpan>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Tuple<DateTime, TimeSpan>(t0[i], t1[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new TupleEncodingArgs<DateTime, TimeSpan>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Tuple03()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;

            var count = t0.Count;

            var tuples = new List<Tuple<DateTime, TimeSpan, long>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Tuple<DateTime, TimeSpan, long>(t0[i], t1[i], t2[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new TupleEncodingArgs<DateTime, TimeSpan, long>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Tuple04()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;

            var count = t0.Count;

            var tuples = new List<Tuple<DateTime, TimeSpan, long, ulong>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Tuple<DateTime, TimeSpan, long, ulong>(t0[i], t1[i], t2[i], t3[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new TupleEncodingArgs<DateTime, TimeSpan, long, ulong>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Tuple05()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;

            var count = t0.Count;

            var tuples = new List<Tuple<DateTime, TimeSpan, long, ulong, int>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Tuple<DateTime, TimeSpan, long, ulong, int>(t0[i], t1[i], t2[i], t3[i],
                    t4[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new TupleEncodingArgs<DateTime, TimeSpan, long, ulong, int>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;


            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Tuple06()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;

            var count = t0.Count;

            var tuples = new List<Tuple<DateTime, TimeSpan, long, ulong, int, uint>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Tuple<DateTime, TimeSpan, long, ulong, int, uint>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new TupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Tuple07()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;

            var count = t0.Count;

            var tuples = new List<Tuple<DateTime, TimeSpan, long, ulong, int, uint, short>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Tuple<DateTime, TimeSpan, long, ulong, int, uint, short>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new TupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }


        [TestMethod]
        public void Example()
        {
            var list = new List<Tuple<DateTime, Double>> { new Tuple<DateTime, Double> (DateTime.Now, 2300.00) };
            var args = new TupleEncodingArgs<DateTime, Double>
                (list, numBlocks: 1, level: CompressionLevel.Fastest);
            // Granularity
            args.Granularities[0] = new DateTime(1);
            args.Granularities[1] = 1.0;

            var codec = DeflateDeltaCodec.Instance;
            var bytes = codec.Encode(args);
        }
    }
}
