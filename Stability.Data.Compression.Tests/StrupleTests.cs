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
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;

            var tuples = new List<Struple<DateTime, TimeSpan>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan>(t1.Data[i], t2.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple03()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;

            var tuples = new List<Struple<DateTime, TimeSpan, long>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long>(t1.Data[i], t2.Data[i], t3.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple04()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong>(t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple05()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int>(t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i],
                    t5.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                Granularity5 = t5.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
                Monotonicity5 = t5.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple06()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int, uint>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                Granularity5 = t5.Granularity,
                Granularity6 = t6.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
                Monotonicity5 = t5.Monotonicity,
                Monotonicity6 = t6.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple07()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                Granularity5 = t5.Granularity,
                Granularity6 = t6.Granularity,
                Granularity7 = t7.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
                Monotonicity5 = t5.Monotonicity,
                Monotonicity6 = t6.Monotonicity,
                Monotonicity7 = t7.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple08()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;

            var tuples = new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                Granularity5 = t5.Granularity,
                Granularity6 = t6.Granularity,
                Granularity7 = t7.Granularity,
                Granularity8 = t8.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
                Monotonicity5 = t5.Monotonicity,
                Monotonicity6 = t6.Monotonicity,
                Monotonicity7 = t7.Monotonicity,
                Monotonicity8 = t8.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple09()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                Granularity5 = t5.Granularity,
                Granularity6 = t6.Granularity,
                Granularity7 = t7.Granularity,
                Granularity8 = t8.Granularity,
                Granularity9 = t9.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
                Monotonicity5 = t5.Monotonicity,
                Monotonicity6 = t6.Monotonicity,
                Monotonicity7 = t7.Monotonicity,
                Monotonicity8 = t8.Monotonicity,
                Monotonicity9 = t9.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple10()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;
            var t10 = Lists.Bytes;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte>>
                    (t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i], t10.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel)
            {
                // Granularity
                Granularity1 = t1.Granularity,
                Granularity2 = t2.Granularity,
                Granularity3 = t3.Granularity,
                Granularity4 = t4.Granularity,
                Granularity5 = t5.Granularity,
                Granularity6 = t6.Granularity,
                Granularity7 = t7.Granularity,
                Granularity8 = t8.Granularity,
                Granularity9 = t9.Granularity,
                Granularity10 = t10.Granularity,
                // Monotonicity
                Monotonicity1 = t1.Monotonicity,
                Monotonicity2 = t2.Monotonicity,
                Monotonicity3 = t3.Monotonicity,
                Monotonicity4 = t4.Monotonicity,
                Monotonicity5 = t5.Monotonicity,
                Monotonicity6 = t6.Monotonicity,
                Monotonicity7 = t7.Monotonicity,
                Monotonicity8 = t8.Monotonicity,
                Monotonicity9 = t9.Monotonicity,
                Monotonicity10 = t10.Monotonicity,
            };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple11()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;
            var t10 = Lists.Bytes;
            var t11 = Lists.Decimals;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal>>(
                    t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i], t10.Data[i], t11.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel)
                {
                    // Granularity
                    Granularity1 = t1.Granularity,
                    Granularity2 = t2.Granularity,
                    Granularity3 = t3.Granularity,
                    Granularity4 = t4.Granularity,
                    Granularity5 = t5.Granularity,
                    Granularity6 = t6.Granularity,
                    Granularity7 = t7.Granularity,
                    Granularity8 = t8.Granularity,
                    Granularity9 = t9.Granularity,
                    Granularity10 = t10.Granularity,
                    Granularity11 = t11.Granularity,
                    // Monotonicity
                    Monotonicity1 = t1.Monotonicity,
                    Monotonicity2 = t2.Monotonicity,
                    Monotonicity3 = t3.Monotonicity,
                    Monotonicity4 = t4.Monotonicity,
                    Monotonicity5 = t5.Monotonicity,
                    Monotonicity6 = t6.Monotonicity,
                    Monotonicity7 = t7.Monotonicity,
                    Monotonicity8 = t8.Monotonicity,
                    Monotonicity9 = t9.Monotonicity,
                    Monotonicity10 = t10.Monotonicity,
                    Monotonicity11 = t11.Monotonicity,
                };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple12()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;
            var t10 = Lists.Bytes;
            var t11 = Lists.Decimals;
            var t12 = Lists.Doubles;

            var tuples =
                new List
                    <Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double>>(
                    t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i], t10.Data[i], t11.Data[i], t12.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel)
                {
                    // Granularity
                    Granularity1 = t1.Granularity,
                    Granularity2 = t2.Granularity,
                    Granularity3 = t3.Granularity,
                    Granularity4 = t4.Granularity,
                    Granularity5 = t5.Granularity,
                    Granularity6 = t6.Granularity,
                    Granularity7 = t7.Granularity,
                    Granularity8 = t8.Granularity,
                    Granularity9 = t9.Granularity,
                    Granularity10 = t10.Granularity,
                    Granularity11 = t11.Granularity,
                    Granularity12 = t12.Granularity,
                    // Monotonicity
                    Monotonicity1 = t1.Monotonicity,
                    Monotonicity2 = t2.Monotonicity,
                    Monotonicity3 = t3.Monotonicity,
                    Monotonicity4 = t4.Monotonicity,
                    Monotonicity5 = t5.Monotonicity,
                    Monotonicity6 = t6.Monotonicity,
                    Monotonicity7 = t7.Monotonicity,
                    Monotonicity8 = t8.Monotonicity,
                    Monotonicity9 = t9.Monotonicity,
                    Monotonicity10 = t10.Monotonicity,
                    Monotonicity11 = t11.Monotonicity,
                    Monotonicity12 = t12.Monotonicity,
                };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple13()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;
            var t10 = Lists.Bytes;
            var t11 = Lists.Decimals;
            var t12 = Lists.Doubles;
            var t13 = Lists.Floats;

            var tuples =
                new List
                    <
                        Struple
                            <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double,
                                float>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i], t10.Data[i], t11.Data[i], t12.Data[i], t13.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel)
                {
                    // Granularity
                    Granularity1 = t1.Granularity,
                    Granularity2 = t2.Granularity,
                    Granularity3 = t3.Granularity,
                    Granularity4 = t4.Granularity,
                    Granularity5 = t5.Granularity,
                    Granularity6 = t6.Granularity,
                    Granularity7 = t7.Granularity,
                    Granularity8 = t8.Granularity,
                    Granularity9 = t9.Granularity,
                    Granularity10 = t10.Granularity,
                    Granularity11 = t11.Granularity,
                    Granularity12 = t12.Granularity,
                    Granularity13 = t13.Granularity,
                    // Monotonicity
                    Monotonicity1 = t1.Monotonicity,
                    Monotonicity2 = t2.Monotonicity,
                    Monotonicity3 = t3.Monotonicity,
                    Monotonicity4 = t4.Monotonicity,
                    Monotonicity5 = t5.Monotonicity,
                    Monotonicity6 = t6.Monotonicity,
                    Monotonicity7 = t7.Monotonicity,
                    Monotonicity8 = t8.Monotonicity,
                    Monotonicity9 = t9.Monotonicity,
                    Monotonicity10 = t10.Monotonicity,
                    Monotonicity11 = t11.Monotonicity,
                    Monotonicity12 = t12.Monotonicity,
                    Monotonicity13 = t13.Monotonicity,
                };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple14()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;
            var t10 = Lists.Bytes;
            var t11 = Lists.Decimals;
            var t12 = Lists.Doubles;
            var t13 = Lists.Floats;
            var t14 = Lists.Bools;

            var tuples =
                new List<Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float, bool>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple<DateTime, TimeSpan, long, ulong, int, uint, short, ushort, sbyte, byte, decimal, double, float, bool>
                    (t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i], t10.Data[i], t11.Data[i], t12.Data[i], t13.Data[i], t14.Data[i]);
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new StrupleEncodingArgs<DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                    sbyte, byte, decimal, double, float, bool>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel)
                {
                    // Granularity
                    Granularity1 = t1.Granularity,
                    Granularity2 = t2.Granularity,
                    Granularity3 = t3.Granularity,
                    Granularity4 = t4.Granularity,
                    Granularity5 = t5.Granularity,
                    Granularity6 = t6.Granularity,
                    Granularity7 = t7.Granularity,
                    Granularity8 = t8.Granularity,
                    Granularity9 = t9.Granularity,
                    Granularity10 = t10.Granularity,
                    Granularity11 = t11.Granularity,
                    Granularity12 = t12.Granularity,
                    Granularity13 = t13.Granularity,
                    Granularity14 = t14.Granularity,
                    // Monotonicity
                    Monotonicity1 = t1.Monotonicity,
                    Monotonicity2 = t2.Monotonicity,
                    Monotonicity3 = t3.Monotonicity,
                    Monotonicity4 = t4.Monotonicity,
                    Monotonicity5 = t5.Monotonicity,
                    Monotonicity6 = t6.Monotonicity,
                    Monotonicity7 = t7.Monotonicity,
                    Monotonicity8 = t8.Monotonicity,
                    Monotonicity9 = t9.Monotonicity,
                    Monotonicity10 = t10.Monotonicity,
                    Monotonicity11 = t11.Monotonicity,
                    Monotonicity12 = t12.Monotonicity,
                    Monotonicity13 = t13.Monotonicity,
                    Monotonicity14 = t14.Monotonicity,
                };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Struple15()
        {
            var t1 = Lists.DateTimes;
            var t2 = Lists.TimeSpans;
            var t3 = Lists.Longs;
            var t4 = Lists.ULongs;
            var t5 = Lists.Ints;
            var t6 = Lists.UInts;
            var t7 = Lists.Shorts;
            var t8 = Lists.UShorts;
            var t9 = Lists.SBytes;
            var t10 = Lists.Bytes;
            var t11 = Lists.Decimals;
            var t12 = Lists.Doubles;
            var t13 = Lists.Floats;
            var t14 = Lists.Bools;
            var t15 = Lists.DateTimeOffsets;

            var tuples =
                new List
                    <
                        Struple
                            <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                            sbyte, byte, decimal, double, float, bool, DateTimeOffset>>(t1.Data.Count);
            for (var i = 0; i < t1.Data.Count; i++)
            {
                var t = new Struple
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                    sbyte, byte, decimal, double, float, bool, DateTimeOffset>(
                    t1.Data[i], t2.Data[i], t3.Data[i], t4.Data[i], t5.Data[i], t6.Data[i], t7.Data[i], t8.Data[i],
                    t9.Data[i], t10.Data[i], t11.Data[i], t12.Data[i], t13.Data[i], t14.Data[i], t15.Data[i]);
                tuples.Add(t);
            }

            var args =
                new StrupleEncodingArgs
                    <DateTime, TimeSpan, long, ulong, int, uint, short, ushort,
                    sbyte, byte, decimal, double, float, bool, DateTimeOffset>
                    (tuples, DefaultNumBlocks, DefaultCompressionLevel)
                {
                    // Granularity
                    Granularity1 = t1.Granularity,
                    Granularity2 = t2.Granularity,
                    Granularity3 = t3.Granularity,
                    Granularity4 = t4.Granularity,
                    Granularity5 = t5.Granularity,
                    Granularity6 = t6.Granularity,
                    Granularity7 = t7.Granularity,
                    Granularity8 = t8.Granularity,
                    Granularity9 = t9.Granularity,
                    Granularity10 = t10.Granularity,
                    Granularity11 = t11.Granularity,
                    Granularity12 = t12.Granularity,
                    Granularity13 = t13.Granularity,
                    Granularity14 = t14.Granularity,
                    Granularity15 = t15.Granularity,
                    // Monotonicity
                    Monotonicity1 = t1.Monotonicity,
                    Monotonicity2 = t2.Monotonicity,
                    Monotonicity3 = t3.Monotonicity,
                    Monotonicity4 = t4.Monotonicity,
                    Monotonicity5 = t5.Monotonicity,
                    Monotonicity6 = t6.Monotonicity,
                    Monotonicity7 = t7.Monotonicity,
                    Monotonicity8 = t8.Monotonicity,
                    Monotonicity9 = t9.Monotonicity,
                    Monotonicity10 = t10.Monotonicity,
                    Monotonicity11 = t11.Monotonicity,
                    Monotonicity12 = t12.Monotonicity,
                    Monotonicity13 = t13.Monotonicity,
                    Monotonicity14 = t14.Monotonicity,
                    Monotonicity15 = t15.Monotonicity,
                };

            StrupleTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [TestMethod]
        public void Example()
        {
            var list = new List<Struple<DateTime, Double>>
            {new Struple<DateTime, Double> {Item1 = DateTime.Now, Item2 = 2300.00}};
            var args = new StrupleEncodingArgs<DateTime, Double>
                (list, numBlocks: 1, level: CompressionLevel.Fastest)
            {
                // Granularity
                Granularity1 = new DateTime(1),
                Granularity2 = 1.0,
            };
            var codec = DeflateDeltaCodec.Instance;
            var bytes = codec.Encode(args);
        }
    }
}
