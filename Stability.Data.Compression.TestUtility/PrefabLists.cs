#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : PrefabLists.cs
// Created   : 2015-6-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

using System;
using System.Collections.Generic;

namespace Stability.Data.Compression.TestUtility
{
    public class PrefabLists
    {
        public PrefabLists(int dataLength = 100000)
        {
            Initialize(dataLength);
        }

        public void Initialize(int dataLength = 100000)
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            // For DateTimeOffsets the list will start with 0 offset and change half way through
            var offsetChange = new TimeSpan(0, -5, 0); // EST

            DateTimeOffsets = new PrefabList<DateTimeOffset>
            (
                TimeSeriesProvider.RandomDateTimeOffsetsBySeconds(dataLength, startDate, min:0, max:10, seed:0, offsetChange: offsetChange),
                startValue: startDate,
                granularity: DateTime.MinValue.AddSeconds(1),
                monotonicity: Monotonicity.None // Can't be sure when offset changes arbitrarily
            );
            DateTimes = new PrefabList<DateTime>
            (
                TimeSeriesProvider.RandomDateTimesByDays(dataLength, startDate, min: 0, max: 3, seed: 0),
                startValue: startDate,
                granularity: DateTime.MinValue.AddDays(1),
                monotonicity: Monotonicity.NonDecreasing
            );

            var startTime = new TimeSpan(9999, 0, 0, 0);
            TimeSpans = new PrefabList<TimeSpan>
            (
                TimeSeriesProvider.RandomTimeSpansBySeconds(dataLength, startTime),
                startValue: startTime,
                granularity: TimeSpan.FromSeconds(1),
                monotonicity: Monotonicity.NonDecreasing
            );

            Ints = new PrefabList<Int32>
            (
                TimeSeriesProvider.Int32RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                startValue: 230000,
                granularity: 25
            );
            UInts = new PrefabList<UInt32>
            (
                TimeSeriesProvider.UInt32RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                startValue: 230000,
                granularity: 25
            );

            Longs = new PrefabList<Int64>
            (
                TimeSeriesProvider.Int64RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                startValue: 230000,
                granularity: 25
            );
            ULongs = new PrefabList<UInt64>
            (
                TimeSeriesProvider.UInt64RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                startValue: 230000,
                granularity: 25
            );

            Shorts = new PrefabList<Int16>
            (
                TimeSeriesProvider.Int16RandomWalk(dataLength, startValue: 1000, granularity: 5, min: -4, max: 4, seed: 0),
                startValue: 1000,
                granularity: 5
            );
            UShorts = new PrefabList<UInt16>
            (
                TimeSeriesProvider.UInt16RandomWalk(dataLength, startValue: 1000, granularity: 5, min: -4, max: 4, seed: 0),
                startValue: 1000,
                granularity: 5
            );

            SBytes = new PrefabList<SByte>
            (
                TimeSeriesProvider.SByteRandomWalk(dataLength, startValue: 0, granularity: 2, min: -4, max: 4, seed: 0),
                startValue: 0,
                granularity: 2
            );
            Bytes = new PrefabList<Byte>
            (
                TimeSeriesProvider.ByteRandomWalk(dataLength, startValue: 120, granularity: 2, min: -4, max: 4, seed: 0),
                startValue: 120,
                granularity: 2
            );

            Bools = new PrefabList<bool>
            (
                TimeSeriesProvider.CoinToss(dataLength, seed:0),
                startValue: false,
                granularity: true
            );

            Decimals = new PrefabList<Decimal>
            (
                TimeSeriesProvider.DecimalRandomWalk(dataLength, startValue: 2300m, granularity: 0.25m, min: -4, max: 4, seed: 0),
                startValue: 2300m,
                granularity: 0.25m
            );

            Doubles = new PrefabList<Double>
            (
                TimeSeriesProvider.DoubleRandomWalk(dataLength, startValue: 2300.0, granularity: 0.25, min: -4, max: 4, seed: 0),
                startValue: 2300.0,
                granularity: 0.25
            );

            Floats = new PrefabList<Single>
            (
                TimeSeriesProvider.FloatRandomWalk(dataLength, startValue: 2300.0f, granularity: 0.25f, min: -4, max: 4, seed: 0),
                startValue: 2300.0f,
                granularity: 0.25f
            );

            Chars = new PrefabList<char>
            (
                TimeSeriesProvider.RandomCharAlphaNumeric(dataLength, seed: 0)
            );

            Strings = new PrefabList<string>
            (
                //TimeSeriesProvider.RandomAlphanumericString(dataLength, seed: 0),
                TimeSeriesProvider.RandomStringQuasiCusip(dataLength, seed: 0)
            );
        }

        public PrefabList<DateTimeOffset> DateTimeOffsets { get; protected set; } 
        public PrefabList<DateTime> DateTimes { get; protected set; }
        public PrefabList<TimeSpan> TimeSpans { get; protected set; }
        public PrefabList<Int64> Longs { get; protected set; }
        public PrefabList<UInt64> ULongs { get; protected set; }
        public PrefabList<Int32> Ints { get; protected set; }
        public PrefabList<UInt32> UInts { get; protected set; }
        public PrefabList<Int16> Shorts { get; protected set; }
        public PrefabList<UInt16> UShorts { get; protected set; }
        public PrefabList<SByte> SBytes { get; protected set; }
        public PrefabList<Byte> Bytes { get; protected set; }
        public PrefabList<bool> Bools { get; protected set; }
        public PrefabList<Decimal> Decimals { get; protected set; }
        public PrefabList<Double> Doubles { get; protected set; }
        public PrefabList<Single> Floats { get; protected set; }

        public PrefabList<char> Chars { get; protected set; }
        public PrefabList<string> Strings { get; protected set; } 
    }

    public class PrefabList<T> : List<T>
    {
        public PrefabList()
        {
            Monotonicity = Monotonicity.None;
        }

        public PrefabList(IList<T> data, T startValue = default(T), T granularity = default(T), Monotonicity monotonicity = Monotonicity.None)
            : base(data)
        {
            StartValue = startValue;
            Granularity = granularity;
            Monotonicity = monotonicity;
        }

        public T StartValue { get; set; }
        public T Granularity { get; set; }
        public Monotonicity Monotonicity { get; set; }
    }
}
