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
            {
                Data = TimeSeriesProvider.RandomDateTimeOffsetsBySeconds(dataLength, startDate, min:0, max:10, seed:0, offsetChange: offsetChange),
                StartValue = startDate,
                Granularity = DateTime.MinValue.AddSeconds(1),
                Monotonicity = Monotonicity.None, // Can't be sure when offset changes arbitrarily
            };
            DateTimes = new PrefabList<DateTime>
            {
                Data = TimeSeriesProvider.RandomDateTimesByDays(dataLength, startDate, min: 0, max: 3, seed: 0),
                StartValue = startDate,
                Granularity = DateTime.MinValue.AddDays(1),
                Monotonicity = Monotonicity.NonDecreasing
            };

            var startTime = new TimeSpan(9999, 0, 0, 0);
            TimeSpans = new PrefabList<TimeSpan>
            {
                Data = TimeSeriesProvider.RandomTimeSpansBySeconds(dataLength, startTime),
                StartValue = startTime,
                Granularity = TimeSpan.FromSeconds(1),
                Monotonicity = Monotonicity.NonDecreasing
            };

            Ints = new PrefabList<Int32>
            {
                Data = TimeSeriesProvider.Int32RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                StartValue = 230000,
                Granularity = 25,
            };
            UInts = new PrefabList<UInt32>
            {
                Data = TimeSeriesProvider.UInt32RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                StartValue = 230000,
                Granularity = 25,
            };

            Longs = new PrefabList<Int64>
            {
                Data = TimeSeriesProvider.Int64RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                StartValue = 230000,
                Granularity = 25,
            };
            ULongs = new PrefabList<UInt64>
            {
                Data = TimeSeriesProvider.UInt64RandomWalk(dataLength, startValue: 230000, granularity: 25, min: -4, max: 4, seed: 0),
                StartValue = 230000,
                Granularity = 25,
            };

            Shorts = new PrefabList<Int16>
            {
                Data = TimeSeriesProvider.Int16RandomWalk(dataLength, startValue: 1000, granularity: 5, min: -4, max: 4, seed: 0),
                StartValue = 1000,
                Granularity = 5,
            };
            UShorts = new PrefabList<UInt16>
            {
                Data = TimeSeriesProvider.UInt16RandomWalk(dataLength, startValue: 1000, granularity: 5, min: -4, max: 4, seed: 0),
                StartValue = 1000,
                Granularity = 5,
            };

            SBytes = new PrefabList<SByte>
            {
                Data = TimeSeriesProvider.SByteRandomWalk(dataLength, startValue: 0, granularity: 2, min: -4, max: 4, seed: 0),
                StartValue = 0,
                Granularity = 2,
            };
            Bytes = new PrefabList<Byte>
            {
                Data = TimeSeriesProvider.ByteRandomWalk(dataLength, startValue: 120, granularity: 2, min: -4, max: 4, seed: 0),
                StartValue = 120,
                Granularity = 2,
            };

            Bools = new PrefabList<bool>
            {
                Data = TimeSeriesProvider.CoinToss(dataLength, seed:0),
                StartValue = false,
                Granularity = true,
            };

            Decimals = new PrefabList<Decimal>
            {
                Data = TimeSeriesProvider.DecimalRandomWalk(dataLength, startValue: 2300m, granularity: 0.25m, min: -4, max: 4, seed: 0),
                StartValue = 2300m,
                Granularity = 0.25m,
            };

            Doubles = new PrefabList<Double>
            {
                Data = TimeSeriesProvider.DoubleRandomWalk(dataLength, startValue: 2300.0, granularity: 0.25, min: -4, max: 4, seed: 0),
                StartValue = 2300.0,
                Granularity = 0.25,
            };

            Floats = new PrefabList<Single>
            {
                Data = TimeSeriesProvider.FloatRandomWalk(dataLength, startValue: 2300.0f, granularity: 0.25f, min: -4, max: 4, seed: 0),
                StartValue = 2300.0f,
                Granularity = 0.25f,
            };
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
    }

    public class PrefabList<T>
    {
        public PrefabList()
        {
            Monotonicity = Monotonicity.None;
        }

        public PrefabList(IList<T> data, T startValue, T granularity, Monotonicity monotonicity = Monotonicity.None)
        {
            Data = data;
            StartValue = startValue;
            Granularity = granularity;
            Monotonicity = monotonicity;
        }
        public IList<T> Data { get; set; }
        public T StartValue { get; set; }
        public T Granularity { get; set; }
        public Monotonicity Monotonicity { get; set; }
    }
}
