#region License

// Namespace : Stability.Data.Compression.Tests
// FileName  : ComparativeTests.cs
// Created   : 2015-4-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

using System;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Tests.Utility;
using Stability.Data.Compression.TestUtility;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.Tests
{
    [TestClass]
    public class ComparativeTests
    {
        #region Configuration

        // Tests that include parallelism use Environment.ProcessorCount as the default number of blocks
        // to process concurrently. The value of the "numBlocks" parameter is set in the CodecTest.Run() 
        // method. You can change this in the source code if you want. But we have found that using the
        // number of logical processors on any given machine seems to give good results. On a system with
        // lots of ambient concurrency, such as on a busy web server, a lower number might be better.
        // Higher values will result in diminishing returns, or even degraded performance. That's why
        // the value is not exposed as a setting in these "standardized" comparison tests.
        // Naturally, when you use codecs directly you can pass in whatever value you want.

        // NOTE: The timing results for "Parallel" tests have higher variance than "Serial" tests.
        //       Increasing the run count will usually reduce this variance but slows things down.
        //       The fastest run for each codec is used for comparison relative to other codecs.
        //       The reason we don't use a typical scheme of throwing out high and low and averaging
        //       the rest is because that means you wouldn't be able to specify a lower run count.
        //       Sometimes you might only be interested in ratio/multiple statistics and a rough
        //       idea of relative speed. More runs can make the total execution time excruciating.
        //       This will depend on the length of the lists, of course. So if you want to use a
        //       high number of runs, then consider using shorter lists.

        private const int DefaultRunCount = 3;
        private const int DefaultDataCount = 1000000;

        // Codecs in the core and "ThirdParty" assemblies have two versions,
        // one that uses a NullTransform and another that uses a DeltaTransform. 
        // The NullTransform version is generally only useful for comparison.

        // Uncomment any ONE of the enumeration values below to run the desired group of comparison tests.
        private const TestGroupType GroupType =
            //TestGroupType.SerialFast;
            //TestGroupType.SerialOptimal;
            //TestGroupType.SerialDeltaNoFactorFast;
            //TestGroupType.SerialDeltaNoFactorOptimal;
            //TestGroupType.SerialDeltaAutoFactorFast;
            //TestGroupType.SerialDeltaAutoFactorOptimal;
            //TestGroupType.SerialDeltaGranularFast;
            //TestGroupType.SerialDeltaGranularOptimal;
            //TestGroupType.ParallelFast;
            //TestGroupType.ParallelOptimal;
            //TestGroupType.ParallelDeltaNoFactorFast;
            //TestGroupType.ParallelDeltaNoFactorOptimal;
            //TestGroupType.ParallelDeltaAutoFactorFast;
            //TestGroupType.ParallelDeltaAutoFactorOptimal;
            TestGroupType.ParallelDeltaGranularFast;
            //TestGroupType.ParallelDeltaGranularOptimal;
            //TestGroupType.SerialVersusParallelFast;
            //TestGroupType.SerialVersusParallelOptimal;
            //TestGroupType.SerialVersusParallelDeltaFast;
            //TestGroupType.SerialVersusParallelDeltaOptimal;
            //TestGroupType.SerialNullTransformVersusDeltaFast;
            //TestGroupType.SerialNullTransformVersusDeltaOptimal;
            //TestGroupType.ParallelNullTransformVersusDeltaFast;
            //TestGroupType.ParallelNullTransformVersusDeltaOptimal;
            //TestGroupType.SerialOptimalVersusFast;
            //TestGroupType.SerialDeltaOptimalVersusFast;
            //TestGroupType.ParallelOptimalVersusFast;
            //TestGroupType.ParallelDeltaOptimalVersusFast;
            //TestGroupType.SerialFactoringComparisonFast;
            //TestGroupType.SerialFactoringComparisonOptimal;
            //TestGroupType.ParallelFactoringComparisonFast;
            //TestGroupType.ParallelFactoringComparisonOptimal;

        private static readonly CodecTestGroup TestGroup = TestRunnerFactory.GetGroup(GroupType);

        // There is a performance document in the "Docs" subfolder that presents the results for all
        // of the test groups listed above. You can save time by browsing that instead of running
        // these yourself. The tests here are still useful, however, for learning how codecs behave
        // with ad hoc chnages to parameter values and input data.

        #endregion // Configuration

        [TestMethod]
        public void Example()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const int granularity = 25;
            var list = TimeSeriesProvider.Int64RandomWalk(
                n: 1000, startValue: 230000, granularity: granularity, min: -4, max: 4, seed: 0);

            var codec = DeflateCodec.Instance;

            // There are several different ways to initialize the arguments...

            // Use defaults (except for the list itself)
            var args0 = new NumericEncodingArgs<long>(list);

            // Constructor with some combination of arguments
            var args1 = new NumericEncodingArgs<long>(
                list: list,
                numBlocks: 1,
                level: CompressionLevel.Optimal,
                granularity: granularity,
                monotonicity: monotonicity,
                custom: null
                );

            // Property initializer
            var args2 = new NumericEncodingArgs<long>(list)
            {
                NumBlocks = 1,
                Level = CompressionLevel.Optimal,
                Granularity = granularity,
                Monotonicity = monotonicity,
                Custom = null,
            };

            // Inline
            var bytes = codec.Encode(new NumericEncodingArgs<long>(list));

            var listOut = codec.Decode<long>(bytes);

            Assert.AreEqual(list.Count, listOut.Count);
            for (var i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], listOut[i]);
            }
        }

        #region DateTimeOffset

        [TestMethod]
        public void DateTimeOffsetByDays()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTimeOffset.MinValue.AddDays(1);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsByDays(
                DefaultDataCount, startDate, min: 0, max: 3, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeOffsetByHours()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTimeOffset.MinValue.AddHours(1);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsByHours(
                DefaultDataCount, startDate, min: 0, max: 4, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeOffsetByMinutes()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTimeOffset.MinValue.AddMinutes(1);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsByMinutes(
                DefaultDataCount, startDate, min: 0, max: 5, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeOffsetBySeconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTimeOffset.MinValue.AddSeconds(1);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsBySeconds(
                DefaultDataCount, startDate, min: 0, max: 10, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeOffsetByMilliseconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTimeOffset.MinValue.AddMilliseconds(1);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsByMilliseconds(
                DefaultDataCount, startDate, min: 0, max: 100, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeOffsetByMicroseconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = new DateTime(TimeSpan.TicksPerMillisecond / 1000);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsByMicroseconds(
                DefaultDataCount, startDate, min: 0, max: 1000, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeOffsetByTicks()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = new DateTime((long)1);
            var now = DateTime.Now;

            var offset = new TimeSpan(-5, 0, 0); // PST
            var offsetChange = new TimeSpan(-5, 0, 0); // EST
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, offset);

            var list = TimeSeriesProvider.RandomDateTimeOffsetsByTicks(
                DefaultDataCount, startDate, min: 0, max: 10000, seed: 0, offsetChange: offsetChange);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        #endregion // DateTimeOffset
        #region DateTime

        // The only difference between all DateTime/TimeSpan tests is that we use a different granularity.
        // As the granularity gets smaller we are intentionally widening the min and max multiple of that.
        // But, if we used the same min and max multiple of granulary with, say, Days and Microseconds, 
        // the performance will be exactly equal. In other words, variance is the determinant when dealing 
        // with factored differences.
        // NOTE: The same is true for all integral types, but we only demonstrate with DateTime and TimeSpan.

        [TestMethod]
        public void DateTimeByDays()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTime.MinValue.AddDays(1);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesByDays(
                DefaultDataCount, startDate, min: 0, max: 3, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeByHours()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTime.MinValue.AddHours(1);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesByHours(
                DefaultDataCount, startDate, min: 0, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeByMinutes()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTime.MinValue.AddMinutes(1);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesByMinutes(
                DefaultDataCount, startDate, min: 0, max: 5, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeBySeconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTime.MinValue.AddSeconds(1);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesBySeconds(
                DefaultDataCount, startDate, min: 0, max: 10, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeByMilliseconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = DateTime.MinValue.AddMilliseconds(1);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesByMilliseconds(
                DefaultDataCount, startDate, min: 0, max: 100, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeByMicroseconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = new DateTime(TimeSpan.TicksPerMillisecond / 1000);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesByMicroseconds(
                DefaultDataCount, startDate, min: 0, max: 1000, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void DateTimeByTicks()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = new DateTime((long)1);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var list = TimeSeriesProvider.RandomDateTimesByTicks(
                DefaultDataCount, startDate, min: 0, max: 10000, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        #endregion // DateTime
        #region TimeSpan

        // The only difference between all DateTime/TimeSpan tests is that we use a different granularity.
        // As the granularity gets smaller we are intentionally widening the min and max multiple of that.
        // But, if we used the same min and max multiple of granulary with, say, Days and Microseconds, 
        // the performance will be exactly equal. In other words, variance is the determinant when dealing 
        // with factored differences.
        // NOTE: The same is true for all integral types, but we only demonstrate with DateTime and TimeSpan.

        [TestMethod]
        public void TimeSpanByDays()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = TimeSpan.FromDays(1);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansByDays(
                DefaultDataCount, start, min: 0, max: 3, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void TimeSpanByHours()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = TimeSpan.FromHours(1);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansByHours(
                DefaultDataCount, start, min: 0, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void TimeSpanByMinutes()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = TimeSpan.FromMinutes(1);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansByMinutes(
                DefaultDataCount, start, min: 0, max: 5, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void TimeSpanBySeconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = TimeSpan.FromSeconds(1);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansBySeconds(
                DefaultDataCount, start, min: 0, max: 10, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void TimeSpanByMilliseconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = TimeSpan.FromMilliseconds(1);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansByMilliseconds(
                DefaultDataCount, start, min: 0, max: 100, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void TimeSpanByMicroseconds()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = new TimeSpan(TimeSpan.TicksPerMillisecond / 1000);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansByMicroseconds(
                DefaultDataCount, start, min: 0, max: 1000, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void TimeSpanByTicks()
        {
            const Monotonicity monotonicity = Monotonicity.NonDecreasing;
            var granularity = TimeSpan.FromTicks(1);
            var start = new TimeSpan(9999, 0, 0, 0);
            var list = TimeSeriesProvider.RandomTimeSpansByTicks(
                DefaultDataCount, start, min: 0, max: 10000, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }


        #endregion // TimeSpan
        #region Integer

        [TestMethod]
        public void Int64()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const int granularity = 25;
            var list = TimeSeriesProvider.Int64RandomWalk(
                n: DefaultDataCount, startValue: 230000, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void UInt64()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const ulong granularity = 25UL;
            var list = TimeSeriesProvider.UInt64RandomWalk(
                n: DefaultDataCount, startValue: 230000, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void Int32()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const int granularity = 25;
            var list = TimeSeriesProvider.Int32RandomWalk(
                n: DefaultDataCount, startValue: 230000, granularity: granularity, min: -3, max: 3, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void UInt32()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const uint granularity = 5U;
            var list = TimeSeriesProvider.UInt32RandomWalk(
                n: DefaultDataCount, startValue: 100000, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void Int16()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const short granularity = (short)5;
            var list = TimeSeriesProvider.Int16RandomWalk(
                n: DefaultDataCount, startValue: 1000, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void UInt16()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const ushort granularity = (ushort)5;
            var list = TimeSeriesProvider.UInt16RandomWalk(
                n: DefaultDataCount, startValue: 1000, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        #endregion // Integer
        #region Byte

        [TestMethod]
        public void SByte()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const sbyte granularity = (sbyte)2;
            var list = TimeSeriesProvider.SByteRandomWalk(
                n: DefaultDataCount, startValue: 0, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        [TestMethod]
        public void Byte()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const byte granularity = (byte)2;
            var list = TimeSeriesProvider.ByteRandomWalk(
                n: DefaultDataCount, startValue: 128, granularity: granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        #endregion // Byte
        #region Boolean

        [TestMethod]
        public void Boolean()
        {
            var list = TimeSeriesProvider.CoinToss(DefaultDataCount, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        #endregion // Boolean
        #region Real

        [TestMethod]
        [Description("Floating point data is very difficult to manipulate in preprocessing Transforms. Be very careful!")]
        public void Float()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const float granularity = 0.25f;
            const float start = 2300f;
            var precision = DeltaUtility.Precision(granularity);

            var list = TimeSeriesProvider.FloatRandomWalk(
                DefaultDataCount, start, granularity, min: -3, max: 3, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list, "F" + precision);
        }

        [TestMethod]
        [Description("Floating point data is very difficult to manipulate in preprocessing Transforms. Be very careful!")]
        public void Double()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const double granularity = 0.25;
            const double start = 2300.0;
            var precision = DeltaUtility.Precision(granularity);

            var list = TimeSeriesProvider.DoubleRandomWalk(
                DefaultDataCount, start, granularity, min: -3, max: 3, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);

            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list, "F" + precision);
        }

        #region Double White Noise

        /// <summary>
        /// This test is implemented because it shows how badly RWC (and all other techniques)
        /// perform on floating point data that is decidedly not the result of a Random Walk process. 
        /// 
        /// Ideally, we would like to implement TimeSeriesProvider methods that use a Variance 
        /// argument to cover the spectrum from trivial Random Walk all the way to full White Noise. 
        /// 
        /// There should also be a way of evaluating the suitability of a series so that fall-back 
        /// compression with an alternative scheme can be made automatic.
        /// 
        /// NOTE: The RWT ratio (without any finishing compression) should equal almost exactly one
        /// when there is no exploitable degree of Random Walk. I don't believe there should ever be
        /// a reason for the ratio to be worse than 1 other than what is caused by the addition of a
        /// few bytes of header information. Oddly, for this series the ratio is actually about 1.4
        /// indicating that there is at least some advantage provided by a delta transform.
        /// </summary>
        /// <remarks>
        /// To show how reasonable compression can be possible for such a tough series,
        /// the raw values are forced into a suitable range. 
        /// So for values that are originally simliar to...
        ///    "-2.1427123219402650e-18" (17 digits)
        /// we can change them to...
        ///    "-2.14271232194026" (15 digits, exponent adjusted)
        /// and then the RWT can compress to 70% (with optimal Deflate applied as a finisher).
        /// This beats all other methods tested with the same preconditioned data. 
        /// The next best compression results are stuck at 97% 
        /// and several methods can't compress this data at all!
        /// 
        /// The important point is that one has to carefully consider the viability of lossless 
        /// compression for specific floating point data sets. If the data falls within suitable 
        /// scale and precision constraints, the results with RWT can be quite good. 
        /// Otherwise, one might need to consider lossy compression if space conservation 
        /// or transmission speed is paramount.
        /// 
        /// For this particular data series, there is no clear strategy that will achieve lossless
        /// compression. By giving up 2 digits of precision (reducing the total from 17 to 15, 
        /// which is the maximum guaranteed by System.Double), and adjusting the exponent, 
        /// we can compress the data by 30-40%. 
        /// 
        /// That's not too bad in comparison to the other techniques tested.
        /// 
        /// TODO: This test should be implemented using Decimal!!!
        /// Oddly, I needed to shift the decimal point in order to make this work. 
        /// That should not be "theoretically" necessary. It merits further investigation. 
        /// For this ad hoc test I simply wanted to figure out what would make the existing 
        /// algorithm work as expected. Obviously, the algorithm should know how to do this
        /// without user intervention.
        /// </remarks>
        //[Ignore]
        [TestMethod]
        [Description("We expect RWC to perform poorly on any series that is decidedly NOT a Random Walk. (This test is surprising!)")]
        [DeploymentItem(@"Data\H2-STRAIN_4096Hz-815045078-256.defb", @"Data")]
        public void DoubleWhiteNoise()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const double granularity = 1;
            var fi = new FileInfo(@".\Data\H2-STRAIN_4096Hz-815045078-256.defb");
            Assert.IsTrue(fi.Exists);

            byte[] bytes;
            using (var fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                bytes = reader.ReadBytes((int)fs.Length);
            }

            var list = new DeflateFinisher().DecodeDouble(bytes);

            // Here we are adjusting the data to put it in a "viable" scale/precision range.
            // This is actually a lossy technique. But it illustrates the constraints that
            // should be considered for lossless compression of scientific floating point data.
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = Math.Round(list[i] * Math.Pow(10, 18), 14);
            }
            Assert.AreEqual(list.Count, 1000000);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list);
        }

        #endregion // Double White Noise

        [TestMethod]
        [Description("Decimal is an EXACT base10 floating point type. So the problems associated with binary Single and Double types don't apply.")]
        public void Decimal()
        {
            const Monotonicity monotonicity = Monotonicity.None;
            const decimal granularity = 0.25m;
            const decimal start = 2300.0m;
            var precision = DeltaUtility.Precision(granularity);

            var list = TimeSeriesProvider.DecimalRandomWalk(
                DefaultDataCount, start, granularity, min: -4, max: 4, seed: 0);

            var runners = TestHelper.RunEncodingTests(TestGroup, list, DefaultRunCount, granularity, monotonicity);
            TestDisplay.PrintResults(runners);
            TestDisplay.PrintSamples(list, "F" + precision);
        }

        #endregion // Real

    }

}
