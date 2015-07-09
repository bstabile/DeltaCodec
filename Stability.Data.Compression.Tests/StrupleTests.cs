#region License

// Namespace : Stability.Data.Compression.Tests
// FileName  : StrupleTests.cs
// Created   : 2015-6-14
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
    public class StrupleTests
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
        public void Struple02()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan>(t0[i], t1[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan>
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
        public void Struple03()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan, long>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long>(t0[i], t1[i], t2[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long>
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
        public void Struple04()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong>(t0[i], t1[i], t2[i], t3[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong>
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
        public void Struple05()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int>(t0[i], t1[i], t2[i], t3[i],
                    t4[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int>
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
        public void Struple06()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int, uint>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint>
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
        public void Struple07()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short>
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
        public void Struple08()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;

            var count = t0.Count;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple09()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;

            var count = t0.Count;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i], t8[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple10()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;

            var count = t0.Count;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte>>
                    (count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i], t8[i], t9[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple11()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;

            var count = t0.Count;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal>>(
                    t0.Count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i], t8[i], t9[i], t10[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs
                <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple12()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;
            var t11 = Lists.Doubles;

            var count = t0.Count;

            var tuples =
                new List
                    <Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double>>(
                    t0.Count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i], t8[i], t9[i], t10[i], t11[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs
                <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            args.Granularities[11] = t11.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;
            args.Monotonicities[11] = t11.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple13()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;
            var t11 = Lists.Doubles;
            var t12 = Lists.Floats;

            var count = t0.Count;

            var tuples =
                new List
                    <
                        Struple
                            <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double,
                                float>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i],
                    t8[i], t9[i], t10[i], t11[i], t12[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs
                <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            args.Granularities[11] = t11.Granularity;
            args.Granularities[12] = t12.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;
            args.Monotonicities[11] = t11.Monotonicity;
            args.Monotonicities[12] = t12.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple14()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;
            var t11 = Lists.Doubles;
            var t12 = Lists.Floats;
            var t13 = Lists.Bools;

            var count = t0.Count;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float, bool>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float, bool>
                    (t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i],
                    t8[i], t9[i], t10[i], t11[i], t12[i], t13[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                sbyte, byte, decimal, double, float, bool>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            args.Granularities[11] = t11.Granularity;
            args.Granularities[12] = t12.Granularity;
             args.Granularities[13] = t13.Granularity;
           // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;
            args.Monotonicities[11] = t11.Monotonicity;
            args.Monotonicities[12] = t12.Monotonicity;
            args.Monotonicities[13] = t13.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple15()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;
            var t11 = Lists.Doubles;
            var t12 = Lists.Floats;
            var t13 = Lists.Bools;
            var t14 = Lists.DateTimeOffsets;

            var count = t0.Count;

            var tuples =
                new List
                    <
                        Struple
                            <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                            sbyte, byte, decimal, double, float, bool, DateTimeOffset>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                    sbyte, byte, decimal, double, float, bool, DateTimeOffset>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i],
                    t8[i], t9[i], t10[i], t11[i], t12[i], t13[i], t14[i]);
                tuples.Add(t);
            }

            var args =
                new StrupleEncodingArgs
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                        sbyte, byte, decimal, double, float, bool, DateTimeOffset>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            args.Granularities[11] = t11.Granularity;
            args.Granularities[12] = t12.Granularity;
            args.Granularities[13] = t13.Granularity;
            args.Granularities[14] = t14.Granularity;
           // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;
            args.Monotonicities[11] = t11.Monotonicity;
            args.Monotonicities[12] = t12.Monotonicity;
            args.Monotonicities[13] = t13.Monotonicity;
            args.Monotonicities[14] = t14.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple16()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;
            var t11 = Lists.Doubles;
            var t12 = Lists.Floats;
            var t13 = Lists.Bools;
            var t14 = Lists.DateTimeOffsets;
            var t15 = Lists.Chars;

            var count = t0.Count;

            var tuples =
                new List
                    <
                        Struple
                            <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                            sbyte, byte, decimal, double, float, bool, DateTimeOffset, char>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                    sbyte, byte, decimal, double, float, bool, DateTimeOffset, char>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i],
                    t8[i], t9[i], t10[i], t11[i], t12[i], t13[i], t14[i], t15[i]);
                tuples.Add(t);
            }

            var args =
                new StrupleEncodingArgs
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                        sbyte, byte, decimal, double, float, bool, DateTimeOffset, char>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            args.Granularities[11] = t11.Granularity;
            args.Granularities[12] = t12.Granularity;
            args.Granularities[13] = t13.Granularity;
            args.Granularities[14] = t14.Granularity;
            args.Granularities[15] = t15.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;
            args.Monotonicities[11] = t11.Monotonicity;
            args.Monotonicities[12] = t12.Monotonicity;
            args.Monotonicities[13] = t13.Monotonicity;
            args.Monotonicities[14] = t14.Monotonicity;
            args.Monotonicities[15] = t15.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple17()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;
            var t7 = Lists.UShorts;
            var t8 = Lists.SBytes;
            var t9 = Lists.Bytes;
            var t10 = Lists.Decimals;
            var t11 = Lists.Doubles;
            var t12 = Lists.Floats;
            var t13 = Lists.Bools;
            var t14 = Lists.DateTimeOffsets;
            var t15 = Lists.Chars;
            var t16 = Lists.Strings;

            var count = t0.Count;

            var tuples =
                new List
                    <
                        Struple
                            <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                            sbyte, byte, decimal, double, float, bool, DateTimeOffset, char, string>>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                    sbyte, byte, decimal, double, float, bool, DateTimeOffset, char, string>(
                    t0[i], t1[i], t2[i], t3[i], t4[i], t5[i], t6[i], t7[i], t8[i], 
                    t9[i], t10[i], t11[i], t12[i], t13[i], t14[i], t15[i], t16[i]);
                tuples.Add(t);
            }

            var args =
                new StrupleEncodingArgs
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                        sbyte, byte, decimal, double, float, bool, DateTimeOffset, char, string>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            args.Granularities[3] = t3.Granularity;
            args.Granularities[4] = t4.Granularity;
            args.Granularities[5] = t5.Granularity;
            args.Granularities[6] = t6.Granularity;
            args.Granularities[7] = t7.Granularity;
            args.Granularities[8] = t8.Granularity;
            args.Granularities[9] = t9.Granularity;
            args.Granularities[10] = t10.Granularity;
            args.Granularities[11] = t11.Granularity;
            args.Granularities[12] = t12.Granularity;
            args.Granularities[13] = t13.Granularity;
            args.Granularities[14] = t14.Granularity;
            args.Granularities[15] = t15.Granularity;
            args.Granularities[16] = t16.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;
            args.Monotonicities[3] = t3.Monotonicity;
            args.Monotonicities[4] = t4.Monotonicity;
            args.Monotonicities[5] = t5.Monotonicity;
            args.Monotonicities[6] = t6.Monotonicity;
            args.Monotonicities[7] = t7.Monotonicity;
            args.Monotonicities[8] = t8.Monotonicity;
            args.Monotonicities[9] = t9.Monotonicity;
            args.Monotonicities[10] = t10.Monotonicity;
            args.Monotonicities[11] = t11.Monotonicity;
            args.Monotonicities[12] = t12.Monotonicity;
            args.Monotonicities[13] = t13.Monotonicity;
            args.Monotonicities[14] = t14.Monotonicity;
            args.Monotonicities[15] = t15.Monotonicity;
            args.Monotonicities[16] = t16.Monotonicity;

            MultiFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Example()
        {
            var list = new List<Struple<DateTime, Double>>
            {new Struple<DateTime, Double> {Item1 = DateTime.Now, Item2 = 2300.00}};
            var args = new StrupleEncodingArgs<DateTime, Double>
                (list, numBlocks: 1, level: CompressionLevel.Fastest);
                // Granularity
            args.Granularities[0] = new DateTime(1);
            args.Granularities[1] = 1.0;

            var codec = DeflateDeltaCodec.Instance;
            var bytes = codec.Encode(args);
        }
    }
}
