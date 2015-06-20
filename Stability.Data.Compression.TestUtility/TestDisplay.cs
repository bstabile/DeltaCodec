#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : TestDisplay.cs
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
    public static class TestDisplay
    {
        #region Constants and Static Fields

        private const string SummaryLineFormatString = "{0, 13} {1, 13} {2, 13}{3, 13}";

        private const string ResultsLineFormatString =
            "{0, -21}{1, 6}{2, 6}{3, 6}{4, 13}{5, 7}{6, 8}{7, 8}{8, 8}{9, 9}{10, 9}{11, 11}{12, 9}{13, 9}{14, 9}{15, 9}";

        private const string FullLineFormatString =
            "{0, 13} {1, 9} {2, 21} {3, 13} {4, 13} {5, 13} {6, 9} {7, 9} {8, 8} {9, 9} {10, 8}{11, 9} {12, 11} {13, 11} {14, 11} {15, 11}{16, 9}";

        private static string[] SummaryLineHeaders =
        {
            "DataType",
            "BufferEntropy",
            "ListCount",
            "RawBytes",
        };

        private static string[] SummaryBlockHeaders =
        {
            "TestGroup     = {0, 13}\n",
            "DataType      = {0, 13}\n",
            "Granularity   = {0, 13}\n",
            "Precision     = {0, 13}\n",
            "Monotonicity  = {0, 13}\n",
            "BufferEntropy = {0, 13}\n",
            "DeltaEntropy  = {0, 13}\n",
            "ListCount     = {0, 13}\n",
            "RawBytes      = {0, 13}\n",
        };

        private static string[] FullLineHeaders =
        {
            "DataType",
            "BufferEntropy",
            "EncType",
            "ListCount",
            "RawBytes",
            "EncBytes",
            "Ratio",
            "Multiple",
            "Enc(ms)",
            "Dec(ms)",
            "CmbsEnc",
            "CmbsDec",
            "SpaceRank",
            "TRankEnc",
            "TRankDec",
            "Balance",
            "Entropy",
        };

        private static string[] FullBlockHeaders =
        {
            "DataType       = {0, 21}\n",
            "BufferEntropy  = {0, 21}\n",
            "EncType        = {0, 21}\n",
            "ListCount      = {0, 18}\n",
            "RawBytes       = {0, 18}\n",
            "EncBytes       = {0, 18}\n",
            "Ratio          = {0, 18}\n",
            "Multiple       = {0, 18}\n",
            "Enc(ms)        = {0, 18}\n",
            "Dec(ms)        = {0, 18}\n",
            "CmbsEnc        = {0, 18}\n",
            "CmbsDec        = {0, 18}\n",
            "SpaceRank      = {0, 18}\n",
            "TRankEnc       = {0, 18}\n",
            "TRankDec       = {0, 18}\n",
            "Balance        = {0, 18}\n",
            "Entropy        = {0, 18}\n",
        };

        private static string[] ResultLineHeaders =
        {
            "EncType",
            "Factor",
            "Level",
            "Parts",
            "EncBytes",
            "Ratio",
            "Mult",
            "Enc(ms)",
            "Dec(ms)",
            "CmbsEnc",
            "CmbsDec",
            "SpaceRank",
            "TRankEnc",
            "TRankDec",
            "Balance",
            "Entropy",
        };

        private static string[] ResultBlockHeaders =
        {
            "EncType   = {0, 18}\n",
            "EncBytes  = {0, 18}\n",
            "Ratio     = {0, 18}\n",
            "Multiple  = {0, 18}\n",
            "Enc(ms)   = {0, 18}\n",
            "Dec(ms)   = {0, 18}\n",
            "CmbsEnc   = {0, 18}\n",
            "CmbsDec   = {0, 18}\n",
            "SpaceRank = {0, 18}\n",
            "TRankEnc  = {0, 18}\n",
            "TRankDec  = {0, 18}\n",
            "Balance   = {0, 18}\n",
            "Entropy   = {0, 18}\n",
        };

        #endregion // Constants and Static Fields

        #region ToStringXXX

        public static string ToString(ICodecTest t)
        {
            var sb = new StringBuilder();
            sb = sb.AppendFormat(FullBlockHeaders[0], t.Stats.DataType.Name);
            sb = sb.AppendFormat(FullBlockHeaders[1], t.Stats.EntropyIn.ToString("P2"));
            sb = sb.AppendFormat(FullBlockHeaders[2], t.DisplayName);
            sb = sb.AppendFormat(FullBlockHeaders[3], t.Stats.ListCount.ToString("N0"));
            sb = sb.AppendFormat(FullBlockHeaders[4], t.Stats.RawBytes.ToString("N0"));
            sb = sb.AppendFormat(FullBlockHeaders[5], t.Stats.EncodedBytes.ToString("N0"));
            sb = sb.AppendFormat(FullBlockHeaders[6], t.Stats.Ratio.ToString("F2"));
            sb = sb.AppendFormat(FullBlockHeaders[7], t.Stats.Multiple.ToString("F2"));
            sb = sb.AppendFormat(FullBlockHeaders[8], t.Stats.ElapsedEncode.TotalMilliseconds.ToString("N0"));
            sb = sb.AppendFormat(FullBlockHeaders[9], t.Stats.ElapsedDecode.TotalMilliseconds.ToString("N0"));
            sb = sb.AppendFormat(FullBlockHeaders[10], t.Stats.CmbsEncode.ToString("N2"));
            sb = sb.AppendFormat(FullBlockHeaders[11], t.Stats.CmbsDecode.ToString("N2"));
            sb = sb.AppendFormat(FullBlockHeaders[12], t.Stats.SpaceRank.ToString("P1"));
            sb = sb.AppendFormat(FullBlockHeaders[13], t.Stats.TimeRankEncode.ToString("P1"));
            sb = sb.AppendFormat(FullBlockHeaders[14], t.Stats.TimeRankDecode.ToString("P1"));
            sb = sb.AppendFormat(FullBlockHeaders[15], t.Stats.Balance.ToString("P1"));
            sb = sb.AppendFormat(FullBlockHeaders[16], t.Stats.EntropyOut.ToString("P2"));
            return sb.ToString();
        }

        public static string ToSummaryLineHeadersString()
        {
            return string.Format(SummaryLineFormatString,
                SummaryLineHeaders[0],
                SummaryLineHeaders[1],
                SummaryLineHeaders[2],
                SummaryLineHeaders[3]
                );
        }

        public static string ToSummaryLineString(ICodecTest t)
        {
            return string.Format(SummaryLineFormatString,
                t.Stats.DataType.Name,
                t.Stats.EntropyIn.ToString("P2"),
                t.Stats.ListCount.ToString("N0"),
                t.Stats.RawBytes.ToString("N0"));
        }

        public static string ToSummaryBlockString(ICodecTest t)
       {
            var sb = new StringBuilder();
            sb = sb.AppendFormat(SummaryBlockHeaders[0], t.Stats.DataType.Name);
            sb = sb.AppendFormat(SummaryBlockHeaders[1], t.Stats.EntropyIn.ToString("P2"));
            sb = sb.AppendFormat(SummaryBlockHeaders[2], t.Stats.ListCount.ToString("N0"));
            sb = sb.AppendFormat(SummaryBlockHeaders[3], t.Stats.RawBytes.ToString("N0"));
            return sb.ToString();
        }

        public static string ToSummaryBlockString(CodecTestRunner r)
        {
            var groupType = (r.Group != null ? r.Group.GroupType : TestGroupType.None);
            var sb = new StringBuilder();
            sb = sb.AppendFormat(SummaryBlockHeaders[0], groupType);
            sb = sb.AppendFormat(SummaryBlockHeaders[1], r.DataType.Name);
            sb = sb.AppendFormat(SummaryBlockHeaders[2], r.Granularity);
            sb = sb.AppendFormat(SummaryBlockHeaders[3], r.Precision);
            sb = sb.AppendFormat(SummaryBlockHeaders[4], r.Monotonicity);
            sb = sb.AppendFormat(SummaryBlockHeaders[5], r.BufferEntropy.ToString("P2"));
            sb = sb.AppendFormat(SummaryBlockHeaders[6], r.DeltaEntropy.ToString("P2"));
            sb = sb.AppendFormat(SummaryBlockHeaders[7], r.ListCount.ToString("N0"));
            sb = sb.AppendFormat(SummaryBlockHeaders[8], r.RawBytes.ToString("N0"));
            return sb.ToString();
        }

        public static string ToFullHeadersString()
        {
            return string.Format(FullLineFormatString,
                FullLineHeaders[0],
                FullLineHeaders[1],
                FullLineHeaders[2],
                FullLineHeaders[3],
                FullLineHeaders[4],
                FullLineHeaders[5],
                FullLineHeaders[6],
                FullLineHeaders[7],
                FullLineHeaders[8],
                FullLineHeaders[9],
                FullLineHeaders[10],
                FullLineHeaders[11],
                FullLineHeaders[12],
                FullLineHeaders[13],
                FullLineHeaders[14],
                FullLineHeaders[15],
                FullLineHeaders[16]
                );
        }

        public static string ToFullLineString(ICodecTest t)
        {
            return string.Format(FullLineFormatString,
                t.Stats.DataType.Name,
                t.Stats.EntropyIn.ToString("P2"),
                t.DisplayName,
                t.Stats.ListCount.ToString("N0"),
                t.Stats.RawBytes.ToString("N0"),
                t.Stats.EncodedBytes.ToString("N0"),
                t.Stats.Ratio.ToString("F2"),
                t.Stats.Multiple.ToString("F2"),
                t.Stats.ElapsedEncode.TotalMilliseconds.ToString("N0"),
                t.Stats.ElapsedDecode.TotalMilliseconds.ToString("N0"),
                t.Stats.CmbsEncode.ToString("N2"),
                t.Stats.CmbsDecode.ToString("N2"),
                t.Stats.SpaceRank.ToString("P1"),
                t.Stats.TimeRankEncode.ToString("P1"),
                t.Stats.TimeRankDecode.ToString("P1"),
                t.Stats.Balance.ToString("P1"),
                t.Stats.EntropyOut.ToString("P2")
                );
        }

        public static string ToResultHeadersString()
        {
            return string.Format(ResultsLineFormatString,
                ResultLineHeaders[0],
                ResultLineHeaders[1],
                ResultLineHeaders[2],
                ResultLineHeaders[3],
                ResultLineHeaders[4],
                ResultLineHeaders[5],
                ResultLineHeaders[6],
                ResultLineHeaders[7],
                ResultLineHeaders[8],
                ResultLineHeaders[9],
                ResultLineHeaders[10],
                ResultLineHeaders[11],
                ResultLineHeaders[12],
                ResultLineHeaders[13],
                ResultLineHeaders[14],
                ResultLineHeaders[15]
                );
        }

        public static string ToResultsLineString(ICodecTest t)
        {
            var fm = t.FactorMode == FactorMode.Granular ? "Gran" : t.FactorMode.ToString();
            var level = t.Level == CompressionLevel.NoCompression
                ? "None"
                : t.Level == CompressionLevel.Fastest
                    ? "Fast"
                    : "Opt";

            return string.Format(ResultsLineFormatString,
                t.DisplayName,
                fm, // FactorMode
                level, // CompressionLevel
                t.NumBlocks,
                t.Stats.EncodedBytes.ToString("N0"),
                t.Stats.Ratio.ToString("F2"),
                t.Stats.Multiple.ToString("F2"),
                t.Stats.ElapsedEncode.TotalMilliseconds.ToString("N0"),
                t.Stats.ElapsedDecode.TotalMilliseconds.ToString("N0"),
                t.Stats.CmbsEncode.ToString("N2"),
                t.Stats.CmbsDecode.ToString("N2"),
                t.Stats.SpaceRank.ToString("P1"),
                t.Stats.TimeRankEncode.ToString("P1"),
                t.Stats.TimeRankDecode.ToString("P1"),
                t.Stats.Balance.ToString("P1"),
                t.Stats.EntropyOut.ToString("P2")
                );
        }

        public static string ToResultsBlockString(ICodecTest t)
        {
            var sb = new StringBuilder();
            sb = sb.AppendFormat(ResultBlockHeaders[0], t.DisplayName);
            sb = sb.AppendFormat(ResultBlockHeaders[1], t.Stats.EncodedBytes.ToString("N0"));
            sb = sb.AppendFormat(ResultBlockHeaders[2], t.Stats.Ratio.ToString("F2"));
            sb = sb.AppendFormat(ResultBlockHeaders[3], t.Stats.Multiple.ToString("F2"));
            sb = sb.AppendFormat(ResultBlockHeaders[4], t.Stats.ElapsedEncode.TotalMilliseconds.ToString("N0"));
            sb = sb.AppendFormat(ResultBlockHeaders[5], t.Stats.ElapsedDecode.TotalMilliseconds.ToString("N0"));
            sb = sb.AppendFormat(ResultBlockHeaders[6], t.Stats.CmbsEncode.ToString("N2"));
            sb = sb.AppendFormat(ResultBlockHeaders[7], t.Stats.CmbsDecode.ToString("N2"));
            sb = sb.AppendFormat(ResultBlockHeaders[8], t.Stats.SpaceRank.ToString("P1"));
            sb = sb.AppendFormat(ResultBlockHeaders[9], t.Stats.TimeRankEncode.ToString("P1"));
            sb = sb.AppendFormat(ResultBlockHeaders[10], t.Stats.TimeRankDecode.ToString("P1"));
            sb = sb.AppendFormat(ResultBlockHeaders[11], t.Stats.Balance.ToString("P1"));
            sb = sb.AppendFormat(ResultBlockHeaders[12], t.Stats.Balance.ToString("F4"));
            return sb.ToString();
        }

        #endregion // ToStringXXX

        #region PrintResults

        public static void PrintResults(CodecTestGroup runners, string valueFormat = null)
        {
            var s = PrintBestOfRunResults(runners);
            Console.WriteLine(s);
        }

        public static void PrintSamples<T>(IList<T> list, string valueFormat = null)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            Console.WriteLine("\nData Samples:");
            var samples = BitPrinter.PrintSamples(list, valueFormat);
            Console.WriteLine(samples);
        }

        public static string PrintBestOfRunResults(CodecTestGroup runners)
        {
            var bestOfRuns = new List<ICodecTest>();
            for (var i = 0; i < runners.Count; i++)
            {
                var r = runners[i];
                CalcRankings(r.Tests);
                var best = r.Tests.OrderByDescending(t => t.Stats.Balance).First();
                bestOfRuns.Add(best);
            }
            CalcRankings(bestOfRuns);

            var summary = ToSummaryBlockString(runners[0]);
            var headers = ToResultHeadersString();
            var headerUnderLine = "".PadRight(headers.Length, '=');
            var testGroupUnderline = "".PadRight(headers.Length, '-');

            var sb = new StringBuilder();
            sb = sb.AppendLine(summary);
            sb = sb.AppendLine();
            sb = sb.AppendLine(headers);
            sb = sb.AppendLine(headerUnderLine);

            var sortedBestOfRuns = bestOfRuns.OrderBy(r => r.RunnerId).ToList();
            for (var i = 0; i < sortedBestOfRuns.Count; i++)
            {
                var test = sortedBestOfRuns[i];
                //var line = test.ToResultsLineString();
                var line = ToResultsLineString(test);
                sb = sb.AppendLine(line);
                if (i != sortedBestOfRuns.Count - 1)
                {
                    if (test.Underline)
                        sb = sb.AppendLine(testGroupUnderline);
                }
            }
            sb = sb.AppendLine(headerUnderLine);

            return sb.ToString();
        }

        public static void PrintTestRunnerResults(CodecTestRunner testRunner)
        {
            foreach (var test in testRunner.Tests)
            {
                Console.WriteLine(ToResultsLineString(test));
            }
        }

        private static void CalcRankings(IList<ICodecTest> testList)
        {
            if (testList == null)
                throw new ArgumentNullException("testList");
            if (testList.Count == 0)
                return;

            var n = testList.Count;
            var space = new double[n];
            var timeEnc = new double[n];
            var timeDec = new double[n];
            var balance = new double[n];

            for (var i = 0; i < n; i++)
            {
                space[i] = testList[i].Stats.Multiple;
                timeEnc[i] = testList[i].Stats.ElapsedEncode.TotalSeconds;
                timeDec[i] = testList[i].Stats.ElapsedDecode.TotalSeconds;
            }

            var minSpace = double.MaxValue;
            var maxSpace = double.MinValue;
            var minTimeEnc = double.MaxValue;
            var maxTimeEnc = double.MinValue;
            var minTimeDec = double.MaxValue;
            var maxTimeDec = double.MinValue;
            var minBalance = double.MaxValue;
            var maxBalance = double.MinValue;

            for (var i = 0; i < n; i++)
            {
                if (space[i] < minSpace) minSpace = space[i];
                if (space[i] > maxSpace) maxSpace = space[i];
                if (timeEnc[i] < minTimeEnc) minTimeEnc = timeEnc[i];
                if (timeEnc[i] > maxTimeEnc) maxTimeEnc = timeEnc[i];
                if (timeDec[i] < minTimeDec) minTimeDec = timeDec[i];
                if (timeDec[i] > maxTimeDec) maxTimeDec = timeDec[i];
            }

            var spaceRange = maxSpace - minSpace;
            var timeRangeEnc = maxTimeEnc - minTimeEnc;
            var timeRangeDec = maxTimeDec - minTimeDec;

            for (var i = 0; i < n; i++)
            {
                var t = testList[i];
                t.Stats.SpaceRank = (space[i] - minSpace) / spaceRange;
                t.Stats.TimeRankEncode = 1 - ((timeEnc[i] - minTimeEnc) / timeRangeEnc);
                t.Stats.TimeRankDecode = 1 - ((timeDec[i] - minTimeDec) / timeRangeDec);

                var bal = (t.Stats.SpaceRank + t.Stats.TimeRankEncode + t.Stats.TimeRankDecode) / 3;
                balance[i] = bal;
                if (bal < minBalance) minBalance = bal;
                if (bal > maxBalance) maxBalance = bal;
            }

            var balanceRange = maxBalance - minBalance;

            for (var i = 0; i < n; i++)
            {
                var t = testList[i];
                t.Stats.Balance = (balance[i] - minBalance) / balanceRange;
            }
        }

        #endregion // PrintResults
    }
}
