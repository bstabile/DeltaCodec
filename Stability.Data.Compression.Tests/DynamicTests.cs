#region License

// Namespace : Stability.Data.Compression.Tests
// FileName  : DynamicTests.cs
// Created   : 2015-6-14
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.TestUtility;
using Stability.Data.Compression.ThirdParty;

namespace Stability.Data.Compression.Tests
{
    [TestClass]
    public class DynamicTests
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

        [StructLayout(LayoutKind.Sequential)]
        public class clas02 { public DateTime Item1; public TimeSpan Item2; }


        [TestMethod]
        public void Dynamic02()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;

            var count = t0.Count;

            var tuples = new List<clas02>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas02() { Item1 = t0[i], Item2 = t1[i],};
                tuples.Add(t);
            }

            // The compression level can be set individually for each field, 
            // but we are setting them all to be the same so it's easier to switch.
            var args = new DynamicEncodingArgs<clas02>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas03
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3;
        }

        [TestMethod]
        public void Dynamic03()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;

            var count = t0.Count;

            var tuples = new List<clas03>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas03()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas03>
                (tuples, DefaultNumBlocks, DefaultCompressionLevel);
            // Granularity
            args.Granularities[0] = t0.Granularity;
            args.Granularities[1] = t1.Granularity;
            args.Granularities[2] = t2.Granularity;
            // Monotonicity
            args.Monotonicities[0] = t0.Monotonicity;
            args.Monotonicities[1] = t1.Monotonicity;
            args.Monotonicities[2] = t2.Monotonicity;

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas04
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4;
        }
        [TestMethod]
        public void Dynamic04()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;

            var count = t0.Count;

            var tuples = new List<clas04>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas04()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas04>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas05
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; 
        }
        [TestMethod]
        public void Dynamic05()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;

            var count = t0.Count;

            var tuples = new List<clas05>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas05()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas05>
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


            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas06
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; 
        }
        [TestMethod]
        public void Dynamic06()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;

            var count = t0.Count;

            var tuples = new List<clas06>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas06()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas06>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas07
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; 
        }
        [TestMethod]
        public void Dynamic07()
        {
            var t0 = Lists.DateTimes;
            var t1 = Lists.TimeSpans;
            var t2 = Lists.Longs;
            var t3 = Lists.ULongs;
            var t4 = Lists.Ints;
            var t5 = Lists.UInts;
            var t6 = Lists.Shorts;

            var count = t0.Count;

            var tuples = new List<clas07>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas07()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas07>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas08
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
        }

        [TestMethod]
        public void Dynamic08()
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

            var tuples = new List<clas08>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas08()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas08>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas09
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; 
        }
        [TestMethod]
        public void Dynamic09()
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

            var tuples = new List<clas09>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas09()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas09>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas10
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10;
        }

        [TestMethod]
        public void Dynamic10()
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

            var tuples = new List<clas10>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas10()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas10>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas11
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; 
        }
        [TestMethod]
        public void Dynamic11()
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

            var tuples = new List<clas11>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas11()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas11>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas12
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; public double Item12;
        }
        [TestMethod]
        public void Dynamic12()
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

            var tuples = new List<clas12>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas12()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                    Item12 = t11[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas12>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas13
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; public double Item12; public float Item13; 
        }
        [TestMethod]
        public void Dynamic13()
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

            var tuples = new List<clas13>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas13()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                    Item12 = t11[i],
                    Item13 = t12[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas13>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas14
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; public double Item12; public float Item13; public bool Item14;
        }
        [TestMethod]
        public void Dynamic14()
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

            var tuples = new List<clas14>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas14()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                    Item12 = t11[i],
                    Item13 = t12[i],
                    Item14 = t13[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas14>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas15
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; public double Item12; public float Item13; public bool Item14; public DateTimeOffset Item15;
        }

        [TestMethod]
        public void Dynamic15()
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

            var tuples = new List<clas15>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas15()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                    Item12 = t11[i],
                    Item13 = t12[i],
                    Item14 = t13[i],
                    Item15 = t14[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas15>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas16
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; public double Item12; public float Item13; public bool Item14; public DateTimeOffset Item15; public char Item16;
        }

        [TestMethod]
        public void Dynamic16()
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

            var tuples = new List<clas16>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas16()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                    Item12 = t11[i],
                    Item13 = t12[i],
                    Item14 = t13[i],
                    Item15 = t14[i],
                    Item16 = t15[i]
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas16>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }

        [StructLayout(LayoutKind.Sequential)]
        public class clas17
        {
            public DateTime Item1; public TimeSpan Item2; public long Item3; public ulong Item4; public int Item5; public uint Item6; public short Item7; public ushort Item8;
            public sbyte Item9; public byte Item10; public decimal Item11; public double Item12; public float Item13; public bool Item14; public DateTimeOffset Item15; public char Item16;
            public string Item17;
        }

        [TestMethod]
        public void Dynamic17()
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

            var tuples = new List<clas17>(count);
            for (var i = 0; i < count; i++)
            {
                var t = new clas17()
                {
                    Item1  = t0[i],
                    Item2  = t1[i],
                    Item3  = t2[i],
                    Item4  = t3[i],
                    Item5  = t4[i],
                    Item6  = t5[i],
                    Item7  = t6[i],
                    Item8  = t7[i],
                    Item9  = t8[i],
                    Item10 = t9[i],
                    Item11 = t10[i],
                    Item12 = t11[i],
                    Item13 = t12[i],
                    Item14 = t13[i],
                    Item15 = t14[i],
                    Item16 = t15[i],
                    Item17 = t16[i],
                };
                tuples.Add(t);
            }

            var args =
                new DynamicEncodingArgs<clas17>
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

            DynamicFieldTest.Run(Codecs, args, GetTestConfigurationName());
        }



        public class clas01 { public DateTime Item1; public double Item2; }
        [TestMethod]
        public void Example()
        {
            var list = new List<clas01>
            {new clas01() {Item1 = DateTime.Now, Item2 = 2300.00}};
            var args = new DynamicEncodingArgs<clas01>
                (list, numBlocks: 1, level: CompressionLevel.Fastest);
            // Granularity
            args.Granularities[0] = new DateTime(1);
            args.Granularities[1] = 1.0;

            var codec = DeflateDeltaCodec.Instance;
            var bytes = codec.Encode<clas01, DateTime, double>(args);
        }
    }
}
