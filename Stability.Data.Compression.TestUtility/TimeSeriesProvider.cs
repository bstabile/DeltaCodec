#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : TimeSeriesProvider.cs
// Created   : 2015-4-24
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public static class TimeSeriesProvider
    {
        public const int DefaultDateTimeCount = 1000000;
        public const long DefaultStartDateTicks = 0;

        #region Dates

        /// <summary>
        /// This method creates a list of dates from DateTime.MinValue.Date to DateTime.MaxValue.Date inclusive.
        /// Optionally, the caller can exclude weekdays.
        /// </summary>
        /// <param name="weekdaysOnly">If true, only weekdays will be included.</param>
        /// <returns>The complete list of dates (without time) that can be represented by the DateTime datatype.</returns>
        public static IList<DateTime> GenerateDatesOnlyFromMinToMax(bool weekdaysOnly = false)
        {
            var dt = DateTime.MinValue.Date;
            var list = new List<DateTime>();
            list.Add(dt);
            while (dt.Date < DateTime.MaxValue.Date)
            {
                dt = dt.AddDays(1).Date;
                if (weekdaysOnly && dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                list.Add(dt);
            }
            return list;
        }

        public static IList<DateTime> GenerateDatesOnlyBetween(DateTime firstDate, DateTime lastDate,
            bool weekdaysOnly = false)
        {
            var dt = firstDate.Date;
            var list = new List<DateTime>();
            list.Add(dt);
            while (dt.Date < lastDate.Date)
            {
                dt = dt.AddDays(1).Date;
                if (weekdaysOnly && dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday) 
                    continue;
                list.Add(dt);
            }
            return list;
        }

        #endregion // Dates

        #region DateTimeOffset

        public static IList<DateTimeOffset> RandomDateTimeOffsetsByDays(int n, DateTimeOffset startDate,
            int min = 0, int max = 3, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n/2;

            var dateTimes = RandomDateTimesByDays(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = i <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        public static IList<DateTimeOffset> RandomDateTimeOffsetsByHours(int n, DateTimeOffset startDate,
            int min = 0, int max = 4, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n / 2;

            var dateTimes = RandomDateTimesByHours(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = n <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        public static IList<DateTimeOffset> RandomDateTimeOffsetsByMinutes(int n, DateTimeOffset startDate,
            int min = 0, int max = 5, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n / 2;

            var dateTimes = RandomDateTimesByMinutes(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = n <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        public static IList<DateTimeOffset> RandomDateTimeOffsetsBySeconds(int n, DateTimeOffset startDate,
            int min = 0, int max = 10, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n / 2;

            var dateTimes = RandomDateTimesBySeconds(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = n <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        public static IList<DateTimeOffset> RandomDateTimeOffsetsByMilliseconds(int n, DateTimeOffset startDate,
            int min = 0, int max = 100, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n / 2;

            var dateTimes = RandomDateTimesByMilliseconds(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = n <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        /// <summary>
        /// This generates random DateTime values from intervals of Microseconds (10 Ticks).
        /// </summary>
        public static IList<DateTimeOffset> RandomDateTimeOffsetsByMicroseconds(int n, DateTimeOffset startDate,
            int min = 0, int max = 1000, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n / 2;

            var dateTimes = RandomDateTimesByMicroseconds(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = n <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        /// <summary>
        /// This generates random DateTime values from intervals of Ticks.
        /// </summary>
        public static IList<DateTimeOffset> RandomDateTimeOffsetsByTicks(int n, DateTimeOffset startDate,
            int min = 0, int max = 10000, int seed = 0, TimeSpan offsetChange = default(TimeSpan))
        {
            // We'll do half the list with the offset of the startDate, and half with offsetChange
            // This allows users to test that elements with different offsets are encoded properly.
            var offset1 = startDate.Offset;
            var offset2 = offsetChange;
            var half = n / 2;

            var dateTimes = RandomDateTimesByTicks(n, startDate.DateTime, min, max, seed);
            var list = new List<DateTimeOffset>(n);
            for (var i = 0; i < n; i++)
            {
                var offset = n <= half ? offset1 : offset2;
                list.Add(new DateTimeOffset(dateTimes[i], offset));
            }
            return list;
        }

        #endregion // DateTimeOffset

        #region DateTime

        public static IList<DateTime> RandomDateTimesByDays(int n, DateTime startDate,
            int min = 0, int max = 3, int seed = 0)
        {
            var rng = new Random(seed);

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDate;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                dt = dt.AddDays(diff);
                dates.Add(dt);
            }
            return dates;
        }

        public static IList<DateTime> RandomDateTimesByHours(int n, DateTime startDate,
            int min = 0, int max = 4, int seed = 0)
        {
            var rng = new Random(seed);

            //var rand = new MersenneTwisterFast(0); // Seed for predictable results!
            var startDateTime = startDate;

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDateTime;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                dt = dt.AddHours(diff);
                dates.Add(dt);
            }
            return dates;
        }

        public static IList<DateTime> RandomDateTimesByMinutes(int n, DateTime startDate,
            int min = 0, int max = 5, int seed = 0)
        {
            var rng = new Random(seed);

            //var rand = new MersenneTwisterFast(0); // Seed for predictable results!
            var startDateTime = startDate;

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDateTime;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                dt = dt.AddMinutes(diff);
                dates.Add(dt);
            }
            return dates;
        }

        public static IList<DateTime> RandomDateTimesBySeconds(int n, DateTime startDate,
            int min = 0, int max = 10, int seed = 0)
        {
            var rng = new Random(seed);

            //var rand = new MersenneTwisterFast(0); // Seed for predictable results!
            var startDateTime = startDate;

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDateTime;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                dt = dt.AddSeconds(diff);
                dates.Add(dt);
            }
            return dates;
        }

        public static IList<DateTime> RandomDateTimesByMilliseconds(int n, DateTime startDate,
            int min = 0, int max = 100, int seed = 0)
        {
            var rng = new Random(seed);

            //var rand = new MersenneTwisterFast(0); // Seed for predictable results!
            var startDateTime = startDate;

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDateTime;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 500 inclusive (min and max)
                dt = dt.AddMilliseconds(diff);
                dates.Add(dt);
            }
            return dates;
        }

        /// <summary>
        /// This generates random DateTime values from intervals of Microseconds (10 Ticks).
        /// </summary>
        public static IList<DateTime> RandomDateTimesByMicroseconds(int n, DateTime startDate,
            int min = 0, int max = 1000, int seed = 0)
        {
            var rng = new Random(seed);

            //var rand = new MersenneTwisterFast(0); // Seed for predictable results!
            var startDateTime = startDate;

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDateTime;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max
                dt = dt.AddTicks(diff * 10);
                dates.Add(dt);
            }
            return dates;
        }

        /// <summary>
        /// This generates random DateTime values from intervals of Ticks.
        /// </summary>
        public static IList<DateTime> RandomDateTimesByTicks(int n, DateTime startDate,
            int min = 0, int max = 10000, int seed = 0)
        {
            var rng = new Random(seed);

            //var rand = new MersenneTwisterFast(0); // Seed for predictable results!
            var startDateTime = startDate;

            var dates = new List<DateTime>(n);

            // Add the staring value
            var dt = startDateTime;
            dates.Add(dt);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max
                dt = dt.AddTicks(diff);
                dates.Add(dt);
            }
            return dates;
        }

        #endregion // DateTime

        #region DateTime32

        //public static IList<DateTime32> RandomDateTime32BySeconds(int n, DateTime startDate, 
        //    int seed = 0, int min = 0, int max = 2)
        //{
        //    var rng = new Random(seed);

        //    var dates = new List<DateTime32>(n);
        //    var uints = new List<uint>(n);
        //    // Add the staring value
        //    var dt = startDate;
        //    dates.Add(new DateTime32(dt));
        //    for (var i = 0; i < n - 1; i++)
        //    {
        //        var sec = rng.Next(min, max); // Get a random integer between min and max inclusive
        //        dt = dt.AddSeconds(sec);
        //        var dt32 = new DateTime32(dt);
        //        dates.Add(dt32);
        //        uints.Add(dt32.Data);
        //    }
        //    return dates;
        //}

        #endregion DateTime32

        #region TimeSpan

        public static IList<TimeSpan> RandomTimeSpansByDays(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 3, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive (min and max)
                t = t.Add(TimeSpan.FromDays(diff));
                times.Add(t);
            }
            return times;
        }

        public static IList<TimeSpan> RandomTimeSpansByHours(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 4, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                t = t.Add(TimeSpan.FromHours(diff));
                times.Add(t);
            }
            return times;
        }

        public static IList<TimeSpan> RandomTimeSpansByMinutes(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 5, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                t = t.Add(TimeSpan.FromMinutes(diff));
                times.Add(t);
            }
            return times;
        }

        public static IList<TimeSpan> RandomTimeSpansBySeconds(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 10, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                t = t.Add(TimeSpan.FromSeconds(diff));
                times.Add(t);
            }
            return times;
        }

        public static IList<TimeSpan> RandomTimeSpansByMilliseconds(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 100, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                t = t.Add(TimeSpan.FromMilliseconds(diff));
                times.Add(t);
            }
            return times;
        }

        /// <summary>
        /// This generates random DateTime values from intervals of Microseconds (10 Ticks).
        /// </summary>
        public static IList<TimeSpan> RandomTimeSpansByMicroseconds(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 1000, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                t = t.Add(TimeSpan.FromTicks(diff * 10));
                times.Add(t);
            }
            return times;
        }

        /// <summary>
        /// This generates random DateTime values from intervals of Microseconds (10 Ticks).
        /// </summary>
        public static IList<TimeSpan> RandomTimeSpansByTicks(int n, TimeSpan startTime = default(TimeSpan),
            int min = 0, int max = 10000, int seed = 0)
        {
            var rng = new Random(seed);

            var times = new List<TimeSpan>(n);

            // Add the staring value
            var t = startTime;
            times.Add(t);

            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between 0 and 2 inclusive (min and max)
                t = t.Add(TimeSpan.FromTicks(diff));
                times.Add(t);
            }
            return times;
        }

        #endregion // TimeSpan

        #region Integer

        public static IList<long> Int64RandomWalk(int n, long startValue, long granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<long>(n);
            // Add the staring value
            var v = startValue;
            list.Add(v);
            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += diff * granularity;
                list.Add(v);
            }
            return list;
        }

        public static IList<ulong> UInt64RandomWalk(int n, ulong startValue, ulong granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<ulong>(n);
            // Add the staring value
            var v = (long)startValue;
            list.Add((ulong)v);
            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += diff * (long)granularity;
                if (v < 0)
                    v = 0;
                list.Add((ulong)v);
            }
            return list;
        }

        public static IList<int> Int32RandomWalk(int n, int startValue, int granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<int>(n);
            // Add the staring value
            var v = startValue;
            list.Add(v);
            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += diff * granularity;
                list.Add(v);
            }
            return list;
        }

        //public static IList<uint> UInt32RandomWalk(int n, uint startValue,
        //    int seed = 0, int min = -100, int max = 100)
        //{
        //    var rng = new Random(seed);

        //    var list = new List<uint>(n);
        //    // Add the staring value
        //    var v = startValue;
        //    list.Add(v);
        //    for (var i = 0; i < n - 1; i++)
        //    {
        //        var diff = rng.Next(min, max); // Get a random integer between min and max inclusive
        //        if (((int)v) + diff >= 0)
        //            v = (uint)(((int)v) + diff);
        //        else
        //        {
        //            v = (uint)(((int)v) - diff);
        //        }
        //        list.Add(v);
        //    }
        //    return list;
        //}

        public static IList<uint> UInt32RandomWalk(int n, uint startValue, uint granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<uint>(n);
            // Add the staring value
            var v = (int)startValue;
            list.Add((uint)v);
            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += diff * (int)granularity;
                if (v < 0)
                    v = 0;
                list.Add((uint)v);
            }
            return list;
        }

        public static IList<short> Int16RandomWalk(int n, short startValue, short granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<short>(n);
            // Add the staring value
            var v = startValue;
            list.Add(v);
            for (var i = 1; i < n; i++)
            {
                var diff = (short) rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += (short)(diff * granularity);
                list.Add(v);
            }
            return list;
        }

        public static IList<ushort> UInt16RandomWalk(int n, ushort startValue, ushort granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<ushort>(n);
            // Add the staring value
            var v = (short)startValue;
            list.Add((ushort)v);
            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += (short)(diff * (short)granularity);
                if (v < 0)
                    v = 0;
                list.Add((ushort)v);
            }
            return list;
        }

        public static IList<sbyte> SByteRandomWalk(int n, sbyte startValue, sbyte granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<sbyte>(n);
            // Add the staring value
            var v = startValue;
            list.Add(v);
            for (var i = 1; i < n; i++)
            {
                var diff = (sbyte)rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                v += (sbyte)(diff * granularity);
                list.Add(v);
            }
            return list;
        }

        public static IList<byte> ByteRandomWalk(int n, byte startValue, byte granularity = 1, 
            int min = -4, int max = 4, int seed = 0)
        {
            if (granularity == 0)
                granularity = 1;
            var rng = new Random(seed);

            var list = new List<byte>(n);
            // Add the staring value
            var v = startValue;
            list.Add(startValue);
            for (var i = 1; i < n; i++)
            {
                var diff = rng.Next(min, max + 1); // Get a random integer between min and max inclusive
                diff = diff*granularity;
                if (diff < 0)
                {
                    if (Math.Abs(diff) > v)
                        v += (byte)Math.Abs(diff);
                    else
                        v -= (byte) Math.Abs(diff);
                }
                else
                {
                    var maxDiff = 255 - v;
                    if (maxDiff >= diff)
                        v += (byte) diff;
                    else
                    {
                        v -= (byte) diff;
                    }
                }
                list.Add(v);
            }
            return list;
        }

        #endregion // Integer

        #region Boolean (CoinToss)

        public static IList<bool> CoinToss(int n, int seed = 0)
        {
            var rng = new Random(seed);
            var list = new List<bool>(n);
            for (var i = 0; i < n; i++)
            {
                list.Add(rng.Next(0, 2) != 0);
            }
            return list;
        }

        #endregion // Boolean (CoinToss)

        #region Char

        /// <summary>
        /// The character list created here will be random within the combined
        /// range of digits and letters (upper and lower case). This is not going
        /// to be as easy to compress as real-world character series that are
        /// typically going to be drawn from quite limited sets, and that are
        /// probably going to have runs that can be additionally compressed with
        /// RLE. Consider, for example, database fields that may only represent
        /// a relatively small number of option codes.
        /// </summary>
        public static IList<char> RandomCharAlphaNumeric(int n, int seed = 0)
        {
            var rng = new Random(seed);

            // Digits, Uppercase, Lowercase
            var chars = new char[10 + 26 + 26];

            for (var i = 0; i < 10; i++)
            {
                chars[i] = (char) (i + 0x30);
            }
            for (var i = 0; i < 26; i++)
            {
                chars[i + 10] = (char) (i + 0x41);
                chars[i + 36] = (char) (i + 0x61);
            }
            
            var list = new List<char>(n);
            for (var i = 0; i < n; i++)
            {
                list.Add(chars[rng.Next(0, 62)]);
            }
            return list;
        }

        #endregion // Char

        #region String

        /// <summary>
        /// This method returns random alphanumeric strings which range in
        /// length from minLength to maxLength.
        /// </summary>
        public static IList<string> RandomAlphanumericString(int n, int seed = 0, int minLength = 10, int maxLength = 100)
        {
            var rng = new Random(seed);

            // Dictionary: Digits, Uppercase letters, Lowercase Letters
            var chars = new char[10 + 26 + 26];

            for (var i = 0; i < 10; i++)
            {
                chars[i] = (char)(i + 0x30);
            }
            for (var i = 0; i < 26; i++)
            {
                chars[i + 10] = (char)(i + 0x41);
                chars[i + 36] = (char)(i + 0x61);
            }

            // Start with a string that will show in the display of test samples
            var list = new List<string>(n) { "RandomStrings" };
            for (var i = 1; i < n; i++)
            {
                var len = rng.Next(minLength, maxLength + 1);
                var s = new char[len];
                for (var j = 0; j < len; j++)
                {
                    var idx = rng.Next(0, 62);
                    s[j] = chars[idx];
                }
                list.Add(new string(s));
            }
            return list;
        }

        /// <summary>
        /// This method returns quasi-cusips (fake security identifiers) that are 
        /// either 8 or 9 characters in length. The ninth character in a cusip  
        /// is a checksum, which are sometimes ignored in real applications. 
        /// To make our list of strings slightly more interesting, we mix cusips 
        /// that sometimes include the checksum, and other times not. Although this 
        /// is meant to simulate cusips, it doesn't obey any of the semantics of 
        /// real-world identifiers.
        /// </summary>
        /// This is a relatively trivial list of strings, but the random
        /// construction makes the data relatively hard to compress anyway.
        /// What we mainly are interested in testing is string termination
        /// handling in encoding methods. To do that, the encodings will 
        /// usually embed private-use unicode values such as 0x92.
        /// <remarks>
        /// </remarks>
        public static IList<string> RandomStringQuasiCusip(int n, int seed = 0)
        {
            var rng = new Random(seed);

            // Dictionary: digits, Uppercase letters
            var chars = new char[10 + 26];

            for (var i = 0; i < 10; i++)
            {
                chars[i] = (char)(i + 0x30);
            }
            for (var i = 0; i < 26; i++)
            {
                chars[i + 10] = (char)(i + 0x41);
            }
            // Start with a string that will show in the display of test samples
            var list = new List<string>(n) { "CUSIP0000" };
            for (var i = 1; i < n; i++)
            {
                var len = 8 + rng.Next(0, 2);
                var s = new char[len];
                for (var j = 0; j < len; j++)
                {
                    var idx = rng.Next(0, 36);
                    s[j] = chars[idx];
                }
                list.Add(new string(s));
            }
            return list;
        }

        #endregion // String

        #region Real

        public static IList<float> FloatRandomWalk(int n, float startValue, float granularity = 1f, 
            int min = -4, int max = 4, int seed = 0)
        {
            var precision = DeltaUtility.Precision((decimal)granularity);
            var rng = new Random(seed);

            var list = new List<float>(n);
            // Add the staring value
            var v = startValue;
            list.Add(v);
            for (var i = 1; i < n; i++)
            {
                var range = max - min;
                var next = (float) (rng.NextDouble() * range);
                var diff = next + min;
                v += diff;
                list.Add(Granularize(v, granularity, precision));
            }
            return list;
        }

        public static IList<double> DoubleRandomWalk(int n, double startValue, double granularity = 1d, 
            int min = -4, int max = 4, int seed = 0)
        {
            var precision = DeltaUtility.Precision((decimal) granularity);
            var rng = new Random(seed);

            var list = new List<double>(n);
            // Add the staring value
            var v = startValue;
            list.Add(v);
            for (var i = 1; i < n; i++)
            {
                var range = max - min;
                var next = rng.NextDouble() * range;
                var diff = next + min;
                v += diff;
                list.Add(Granularize(v, granularity, precision));
            }
            return list;
        }

        public static IList<decimal> DecimalRandomWalk(int n, decimal startValue, decimal granularity = 1m,
            int min = -4, int max = 4, int seed = 0)
        {
            var listD = DoubleRandomWalk(n, (double) startValue, (double) granularity,
                min, max, seed);
            var list = new List<decimal>(n);
            for (var i = 0; i < listD.Count; i++)
            {
                list.Add(Granularize(new decimal(listD[i]), granularity));
            }
            return list;
        }

        #endregion // Real

        #region Granularize

        public static long Granularize(long value, long granularity)
        {
            // For signed types, the min value is truncated rather than rounded
            return granularity == 0 || Math.Abs(granularity) == 1 ? value 
                : value == long.MinValue ? (value/granularity) * granularity 
                : (long)Math.Round(((decimal)value)/granularity, 0) * granularity;
        }

        public static ulong Granularize(ulong value, ulong granularity)
        {
            return granularity == 0 || granularity == 1UL ? value
                : (ulong)Math.Round(((decimal)value) / granularity, 0) * granularity;
        }

        public static int Granularize(int value, int granularity)
        {
            return granularity == 0 || Math.Abs(granularity) == 1 ? value
                 : value == int.MinValue ? (value / granularity) * granularity
               : (int)Math.Round(((double)value) / granularity, 0) * granularity;
        }

        public static uint Granularize(uint value, uint granularity)
        {
            return granularity == 0 || granularity == 1 ? value
                : (uint)Math.Round(((double)value) / granularity, 0) * granularity;
        }

        public static short Granularize(short value, short granularity)
        {
            return (short) ((granularity == 0 || Math.Abs(granularity) == 1) ? value
                  : value == short.MinValue ? (value / granularity) * granularity
               : Math.Round(((double)value) / granularity, 0) * granularity);
        }

        public static ushort Granularize(ushort value, ushort granularity)
        {
            return (ushort) (granularity == 0 || granularity == 1 ? value 
                : Math.Round(((double)value)/granularity, 0)*granularity);
        }

        public static sbyte Granularize(sbyte value, sbyte granularity)
        {
            return (sbyte)(Math.Abs(granularity) == 1 ? value
                 : value == sbyte.MinValue ? (value / granularity) * granularity
                : (sbyte)Math.Round(((double)value) / granularity, 0) * granularity);
        }

        public static byte Granularize(byte value, byte granularity)
        {
            return (byte)(granularity == 0 || granularity == 1 ? value 
                : (byte)Math.Round(((double)value)/granularity, 0)*granularity);
        }

        /// <summary>
        /// Uses this method to granularize when you just need to granularize a single value.
        /// If you need to granularize an entire series of values, you should first get the
        /// precision of the granularity, and then use the overload with a precision argument. 
        /// </summary>
        /// <param name="value">The value to granularize.</param>
        /// <param name="granularity">The granularity desired.</param>
        /// <returns>A granularized version of the input value.</returns>
        public static double Granularize(double value, double granularity)
        {
            var p = DeltaUtility.Precision(granularity);
            var g = Decimal.Parse(granularity.ToString("F" + p));
            var m = Decimal.Parse(value.ToString("F" + p));
            m = Math.Round(m / g, 0) * g;
            return double.Parse(m.ToString("F" + p));
        }

        /// <summary>
        /// Uses this method to granularize when you already know the precision of the Granularity.
        /// This is considerably more efficient when generating lengthly granularized series 
        /// </summary>
        /// <param name="value">The value to granularize.</param>
        /// <param name="precision">The precision of the granularity.</param>
        /// <returns>A granularized version of the input value.</returns>
        public static double Granularize(double value, int precision)
        {
            var m = Decimal.Parse(value.ToString("F" + precision));
            var d = double.Parse(m.ToString(CultureInfo.CurrentCulture));
            return d;
        }

        /// <summary>
        /// Uses this method to granularize when you need to granularize multiple values.
        /// First get the precision of the granularity to avoid finding that for every value. 
        /// </summary>
        /// <param name="value">The value to granularize.</param>
        /// <param name="granularity">The granularity desired.</param>
        /// <param name="precision">The predetermined precision of the granularity.</param>
        /// <returns>A granularized version of the input value.</returns>
        public static double Granularize(double value, double granularity, int precision)
        {
            var p = precision;
            var g = Decimal.Parse(granularity.ToString("F" + p));
            var m = Decimal.Parse(value.ToString("F" + p));
            m = Math.Round(m / g, 0) * g;
            return double.Parse(m.ToString("F" + p));
        }

        /// <summary>
        /// Uses this method to granularize when you just need to granularize a single value.
        /// If you need to granularize an entire series of values, you should first get the
        /// precision of the granularity, and then use the overload with a precision argument. 
        /// </summary>
        /// <param name="value">The value to granularize.</param>
        /// <param name="granularity">The granularity desired.</param>
        /// <returns>A granularized version of the input value.</returns>
        private static float Granularize(float value, float granularity)
        {
            var p = DeltaUtility.Precision(granularity);
            var g = Decimal.Parse(granularity.ToString("F" + p));
            var m = Decimal.Parse(value.ToString("F" + p));
            m = Math.Round(m / g, 0) * g;
            return float.Parse(m.ToString("F" + p));
        }

        /// <summary>
        /// Uses this method to granularize when you already know the precision of the Granularity.
        /// This is considerably more efficient when generating lengthly granularized series 
        /// </summary>
        /// <param name="value">The value to granularize.</param>
        /// <param name="precision">The precision of the granularity.</param>
        /// <returns>A granularized version of the input value.</returns>
        public static float Granularize(float value, int precision)
        {
            var d = Decimal.Parse(value.ToString("F" + precision));
            var f = float.Parse(d.ToString(CultureInfo.CurrentCulture));
            return f;
        }

        /// <summary>
        /// Uses this method to granularize when you need to granularize multiple values.
        /// First get the precision of the granularity to avoid finding that for every value. 
        /// </summary>
        /// <param name="value">The value to granularize.</param>
        /// <param name="granularity">The granularity desired.</param>
        /// <param name="precision">The predetermined precision of the granularity.</param>
        /// <returns>A granularized version of the input value.</returns>
        public static float Granularize(float value, float granularity, int precision)
        {
            var p = precision;
            var g = Decimal.Parse(granularity.ToString("F" + p));
            var m = Decimal.Parse(value.ToString("F" + p));
            m = Math.Round(m / g, 0) * g;
            return float.Parse(m.ToString("F" + p));
        }

        public static decimal Granularize(decimal value, decimal granularity)
        {
            return granularity == 0 || Math.Abs(granularity) == 1 || granularity == 0 ? value
                 : (value / granularity) * granularity;
        }

        #endregion // Granularize
    }
}

