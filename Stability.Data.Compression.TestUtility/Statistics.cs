#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : Statistics.cs
// Created   : 2015-4-22
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

// Some of this code was borrowed from AForge.NET library 
// in order to make certain modifcations and extensions.

// ATTRIBUTION...
// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    using System;

    /// <summary>
    /// Set of statistics functions.
    /// </summary>
    /// 
    /// <remarks>The class represents collection of simple functions used
    /// in statistics.</remarks>
    /// 
    public static class Statistics
    {
        /// <summary>
        /// Calculate mean value.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns mean value.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate mean value
        /// double mean = Statistics.Mean( histogram );
        /// // output it (5.759)
        /// Console.WriteLine( "mean = " + mean.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public static double Mean(int[] values)
        {
            int hits;
            long total = 0;
            double mean = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                hits = values[i];
                // accumulate mean
                mean += i * hits;
                // accumalate total
                total += hits;
            }
            return (total == 0) ? 0 : mean / total;
        }

        /// <summary>
        /// Calculate standard deviation.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns value of standard deviation.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate standard deviation value
        /// double stdDev = Statistics.StdDev( histogram );
        /// // output it (1.999)
        /// Console.WriteLine( "std.dev. = " + stdDev.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public static double StdDev(int[] values)
        {
            return StdDev(values, Mean(values));
        }

        /// <summary>
        /// Calculate standard deviation.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// <param name="mean">Mean value of the histogram.</param>
        /// 
        /// <returns>Returns value of standard deviation.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>The method is an equevalent to the <see cref="StdDev(int[])"/> method,
        /// but it relieas on the passed mean value, which is previously calculated
        /// using <see cref="Mean"/> method.</para>
        /// </remarks>
        /// 
        public static double StdDev(int[] values, double mean)
        {
            double stddev = 0;
            double diff;
            int hits;
            int total = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                hits = values[i];
                diff = (double)i - mean;
                // accumulate std.dev.
                stddev += diff * diff * hits;
                // accumalate total
                total += hits;
            }

            return (total == 0) ? 0 : Math.Sqrt(stddev / total);
        }

        /// <summary>
        /// Calculate median value.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns value of median.</returns>
        /// 
        /// <remarks>
        /// <para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para><note>The median value is calculated accumulating histogram's
        /// values starting from the <b>left</b> point until the sum reaches 50% of
        /// histogram's sum.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate median value
        /// int median = Statistics.Median( histogram );
        /// // output it (6)
        /// Console.WriteLine( "median = " + median );
        /// </code>
        /// </remarks>
        /// 
        public static int Median(int[] values)
        {
            int total = 0, n = values.Length;

            // for all values
            for (int i = 0; i < n; i++)
            {
                // accumalate total
                total += values[i];
            }

            int halfTotal = total / 2;
            int median = 0, v = 0;

            // find median value
            for (; median < n; median++)
            {
                v += values[median];
                if (v >= halfTotal)
                    break;
            }

            return median;
        }

        /// <summary>
        /// Get range around median containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage
        /// of values.</returns>
        /// 
        /// <remarks>
        /// <para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>The method calculates range of stochastic variable, which summary probability
        /// comprises the specified percentage of histogram's hits.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // get 75% range around median
        /// IntRange range = Statistics.GetRange( histogram, 0.75 );
        /// // output it ([4, 8])
        /// Console.WriteLine( "range = [" + range.Min + ", " + range.Max + "]" );
        /// </code>
        /// </remarks>
        /// 
        public static IntRange GetRange(int[] values, double percent)
        {
            int total = 0, n = values.Length;

            // for all values
            for (int i = 0; i < n; i++)
            {
                // accumalate total
                total += values[i];
            }

            int min, max, hits;
            int h = (int)(total * (percent + (1 - percent) / 2));

            // get range min value
            for (min = 0, hits = total; min < n; min++)
            {
                hits -= values[min];
                if (hits < h)
                    break;
            }
            // get range max value
            for (max = n - 1, hits = total; max >= 0; max--)
            {
                hits -= values[max];
                if (hits < h)
                    break;
            }
            return new IntRange(min, max);
        }

        /// <summary>
        /// Calculate mode value.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns mode value of the histogram array.</returns>
        /// 
        /// <remarks>
        /// <para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para><note>Returns the minimum mode value if the specified histogram is multimodal.</note></para>
        ///
        /// <para>Sample usage:</para>
        /// <code>
        /// // create array
        /// int[] values = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate mode value
        /// int mode = Statistics.Mode( values );
        /// // output it (7)
        /// Console.WriteLine( "mode = " + mode );
        /// </code>
        /// </remarks>
        /// 
        public static int Mode(int[] values)
        {
            int mode = 0, curMax = 0;

            for (int i = 0, length = values.Length; i < length; i++)
            {
                if (values[i] > curMax)
                {
                    curMax = values[i];
                    mode = i;
                }
            }

            return mode;
        }

        /// <summary>
        /// Calculate the entropy value for a set of frequencies.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns entropy value of the specified histagram array.</returns>
        /// 
        /// <remarks>
        /// DO NOT USE THIS METHOD WITH A RAW DATA SERIES!
        /// <para>
        /// The input array is treated as a  histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).
        /// </para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array with 2 values of equal probabilities
        /// int[] histogram1 = new int[2] { 3, 3 };
        /// // calculate entropy
        /// double entropy1 = Statistics.Entropy( histogram1 );
        /// // output it (1.000)
        /// Console.WriteLine( "entropy1 = " + entropy1.ToString( "F3" ) );
        /// 
        /// // create histogram array with 4 values of equal probabilities
        /// int[] histogram2 = new int[4] { 1, 1, 1, 1 };
        /// // calculate entropy
        /// double entropy2 = Statistics.Entropy( histogram2 );
        /// // output it (2.000)
        /// Console.WriteLine( "entropy2 = " + entropy2.ToString( "F3" ) );
        /// 
        /// // create histogram array with 4 values of different probabilities
        /// int[] histogram3 = new int[4] { 1, 2, 3, 4 };
        /// // calculate entropy
        /// double entropy3 = Statistics.Entropy( histogram3 );
        /// // output it (1.846)
        /// Console.WriteLine( "entropy3 = " + entropy3.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public static double Entropy(int[] values)
        {
            int n = values.Length;
            int total = 0;
            double entropy = 0;
            double p;

            // calculate total amount of hits
            for (int i = 0; i < n; i++)
            {
                total += values[i];
            }

            if (total != 0)
            {
                // for all values
                for (var i = 0; i < n; i++)
                {
                    // get item's probability
                    p = (double)values[i] / total;
                    // calculate entropy
                    if (!p.Equals(0))
                        entropy += (-p * Math.Log(p, 2));
                }
            }
            return entropy;
        }

        /// <summary>
        /// This method calculates the standard "Entropy" for a series of bytes.
        /// In this library it is primarily used to determine the entropy AFTER compression.
        /// Is you are trying to test the entropy of a series BEFORE some form of "Delta Encoding",
        /// then you should be using the "DeltaEntropy" methods instead.
        /// </summary>
        /// <param name="buffer">
        /// A series of bytes (usually compressed) that will be used to construct a frequency distribution.
        /// The resulting distribution will then be passed to the standard "Entropy" processing method.
        /// </param>
        public static double BufferEntropy(IList<byte> buffer)
        {
            var dict = new Dictionary<byte, int>();
            foreach (var b in buffer)
            {
                if (dict.ContainsKey(b))
                    dict[b]++;
                else
                {
                    dict.Add(b, 1);
                }
            }
            return Entropy(dict.Values.ToArray());
        }

        #region DeltaEntropy (added 2014-04-12 by BRS)

        /// <summary>
        /// A "Fake Generic" mapping from data type to function call.
        /// Other types with a defined subtraction operator can be added. 
        /// </summary>
        private static readonly IDictionary<Type, Func<object, double>> DeltaEntropyFuncMap = new Dictionary
            <Type, Func<object, double>>
        {
            {typeof(decimal), list => DeltaEntropy((IList<decimal>) list, (a, b) => a - b)},
            {typeof(DateTimeOffset), list => DeltaEntropy((IList<DateTimeOffset>) list, (a, b) => 
                new DateTimeOffset(new DateTime((a.Ticks + a.Offset.Ticks) - (b.Ticks + b.Offset.Ticks))))},
            {typeof(DateTime), list => DeltaEntropy((IList<DateTime>) list, (a, b) => new DateTime(a.Ticks - b.Ticks))},
            {typeof(TimeSpan), list => DeltaEntropy((IList<TimeSpan>) list, (a, b) => a - b)},
            {typeof(double), list => DeltaEntropy((IList<double>) list, (a, b) => a - b)},
            {typeof(float), list => DeltaEntropy((IList<float>) list, (a, b) => a - b)},
            {typeof(long), list => DeltaEntropy((IList<long>) list, (a, b) => a - b)},
            {typeof(ulong), list => DeltaEntropy((IList<ulong>) list, (a, b) => a - b)},
            {typeof(int), list => DeltaEntropy((IList<int>) list, (a, b) => a - b)},
            {typeof(uint), list => DeltaEntropy((IList<uint>) list, (a, b) => a - b)},
            {typeof(short), list => DeltaEntropy((IList<short>) list, (a, b) => (short)(a - b))},
            {typeof(ushort), list => DeltaEntropy((IList<ushort>) list, (a, b) => (ushort)(a - b))},
            {typeof(sbyte), list => DeltaEntropy((IList<sbyte>) list, (a, b) => (sbyte)(a - b))},
            {typeof(byte), list => DeltaEntropy((IList<byte>) list, (a, b) => (byte)(a - b))},
       };

        /// <summary>
        /// Differences a data series and returns the Entropy of those differences.
        /// This is actually a "Fake Generic" method that looks up the Func to call
        /// in a static map (Dictionary) to avoid relatively messy condition testing.
        /// The "true generic" method accepts a Func to perform the differencing op.
        /// </summary>
        public static double DeltaEntropy<T>(IList<T> list)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            // Special case for Boolean list
            if (typeof (T) == typeof (bool))
            {
                return DeltaEntropy((IList<bool>) list);
            }
            return DeltaEntropyFuncMap[typeof(T)].Invoke(list);
        }

        /// <summary>
        /// Boolean is a special case. We need to convert these to single bits,
        /// write them to a stream and find DeltaEntropy for the bytes.
        /// </summary>
        private static double DeltaEntropy(IList<bool> list)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new BitWriter(ms);
                for (var i = 0; i < list.Count; i++)
                {
                    writer.WriteBit(list[i] ? 1U : 0U);
                }
                var arr = ms.ToArray();
                return DeltaEntropy(arr, (a, b) => (byte)(a - b));

            }
        }

        /// <summary>
        /// Differences a data series and returns the Entropy of those differences.
        /// </summary>
        private static double DeltaEntropy<T>(IList<T> list, Func<T, T, T> diffFunc)
        {
            var dict = new Dictionary<T, int>();
            for (var i = 1; i < list.Count; i++)
            {
                var diff = diffFunc(list[i], list[i - 1]);
                if (dict.ContainsKey(diff))
                {
                    dict[diff]++;
                }
                else
                {
                    dict.Add(diff, 1);
                }
            }
            return Entropy(dict.Values.ToArray());
        }

        #endregion // DeltaEntropy (added 2014-04-12 by BRS)

    }

    /// <summary>
    /// Represents a range with minimum and maximum values, which are single precision numbers (floats).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class represents a single precision range with inclusive limits -
    /// both minimum and maximum values of the range are included into it.
    /// Mathematical notation of such range is <b>[min, max]</b>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create [0.25, 1.5] range
    /// Range range1 = new Range( 0.25f, 1.5f );
    /// // create [1.00, 2.25] range
    /// Range range2 = new Range( 1.00f, 2.25f );
    /// // check if values is inside of the first range
    /// if ( range1.IsInside( 0.75f ) )
    /// {
    ///     // ...
    /// }
    /// // check if the second range is inside of the first range
    /// if ( range1.IsInside( range2 ) )
    /// {
    ///     // ...
    /// }
    /// // check if two ranges overlap
    /// if ( range1.IsOverlapping( range2 ) )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    ///
    [Serializable]
    public struct Range
    {
        private float min, max;

        /// <summary>
        /// Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents minimum value (left side limit) or the range -
        /// [<b>min</b>, max].</para></remarks>
        /// 
        public float Min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents maximum value (right side limit) or the range -
        /// [min, <b>max</b>].</para></remarks>
        /// 
        public float Max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Length of the range (deffirence between maximum and minimum values).
        /// </summary>
        public float Length
        {
            get { return max - min; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> structure.
        /// </summary>
        /// 
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// 
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Check if the specified value is inside of the range.
        /// </summary>
        /// 
        /// <param name="x">Value to check.</param>
        /// 
        /// <returns><b>True</b> if the specified value is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(float x)
        {
            return ((x >= min) && (x <= max));
        }

        /// <summary>
        /// Check if the specified range is inside of the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check.</param>
        /// 
        /// <returns><b>True</b> if the specified range is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(Range range)
        {
            return ((IsInside(range.min)) && (IsInside(range.max)));
        }

        /// <summary>
        /// Check if the specified range overlaps with the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check for overlapping.</param>
        /// 
        /// <returns><b>True</b> if the specified range overlaps with the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsOverlapping(Range range)
        {
            return ((IsInside(range.min)) || (IsInside(range.max)) ||
                     (range.IsInside(min)) || (range.IsInside(max)));
        }

        /// <summary>
        /// Convert the signle precision range to integer range.
        /// </summary>
        /// 
        /// <param name="provideInnerRange">Specifies if inner integer range must be returned or outer range.</param>
        /// 
        /// <returns>Returns integer version of the range.</returns>
        /// 
        /// <remarks>If <paramref name="provideInnerRange"/> is set to <see langword="true"/>, then the
        /// returned integer range will always fit inside of the current single precision range.
        /// If it is set to <see langword="false"/>, then current single precision range will always
        /// fit into the returned integer range.</remarks>
        ///
        public IntRange ToIntRange(bool provideInnerRange)
        {
            int iMin, iMax;

            if (provideInnerRange)
            {
                iMin = (int)Math.Ceiling(min);
                iMax = (int)Math.Floor(max);
            }
            else
            {
                iMin = (int)Math.Floor(min);
                iMax = (int)Math.Ceiling(max);
            }

            return new IntRange(iMin, iMax);
        }

        /// <summary>
        /// Equality operator - checks if two ranges have equal min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are equal.</returns>
        ///
        public static bool operator ==(Range range1, Range range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }

        /// <summary>
        /// Inequality operator - checks if two ranges have different min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are not equal.</returns>
        ///
        public static bool operator !=(Range range1, Range range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));

        }

        /// <summary>
        /// Check if this instance of <see cref="Range"/> equal to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another range to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals(object obj)
        {
            return (obj is Range) ? (this == (Range)obj) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode()
        {
            return min.GetHashCode() + max.GetHashCode();
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
        ///
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", min, max);
        }
    }

    /// <summary>
    /// Represents an integer range with minimum and maximum values.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class represents an integer range with inclusive limits -
    /// both minimum and maximum values of the range are included into it.
    /// Mathematical notation of such range is <b>[min, max]</b>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create [1, 10] range
    /// IntRange range1 = new IntRange( 1, 10 );
    /// // create [5, 15] range
    /// IntRange range2 = new IntRange( 5, 15 );
    /// // check if values is inside of the first range
    /// if ( range1.IsInside( 7 ) )
    /// {
    ///     // ...
    /// }
    /// // check if the second range is inside of the first range
    /// if ( range1.IsInside( range2 ) )
    /// {
    ///     // ...
    /// }
    /// // check if two ranges overlap
    /// if ( range1.IsOverlapping( range2 ) )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    ///
    [Serializable]
    public struct IntRange
    {
        private int min, max;

        /// <summary>
        /// Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents minimum value (left side limit) or the range -
        /// [<b>min</b>, max].</para></remarks>
        /// 
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents maximum value (right side limit) or the range -
        /// [min, <b>max</b>].</para></remarks>
        /// 
        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Length of the range (deffirence between maximum and minimum values).
        /// </summary>
        public int Length
        {
            get { return max - min; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRange"/> structure.
        /// </summary>
        /// 
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// 
        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Check if the specified value is inside of the range.
        /// </summary>
        /// 
        /// <param name="x">Value to check.</param>
        /// 
        /// <returns><b>True</b> if the specified value is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(int x)
        {
            return ((x >= min) && (x <= max));
        }

        /// <summary>
        /// Check if the specified range is inside of the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check.</param>
        /// 
        /// <returns><b>True</b> if the specified range is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(IntRange range)
        {
            return ((IsInside(range.min)) && (IsInside(range.max)));
        }

        /// <summary>
        /// Check if the specified range overlaps with the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check for overlapping.</param>
        /// 
        /// <returns><b>True</b> if the specified range overlaps with the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsOverlapping(IntRange range)
        {
            return ((IsInside(range.min)) || (IsInside(range.max)) ||
                     (range.IsInside(min)) || (range.IsInside(max)));
        }

        /// <summary>
        /// Implicit conversion to <see cref="Range"/>.
        /// </summary>
        /// 
        /// <param name="range">Integer range to convert to single precision range.</param>
        /// 
        /// <returns>Returns new single precision range which min/max values are implicitly converted
        /// to floats from min/max values of the specified integer range.</returns>
        /// 
        public static implicit operator Range(IntRange range)
        {
            return new Range(range.Min, range.Max);
        }

        /// <summary>
        /// Equality operator - checks if two ranges have equal min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are equal.</returns>
        ///
        public static bool operator ==(IntRange range1, IntRange range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }

        /// <summary>
        /// Inequality operator - checks if two ranges have different min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are not equal.</returns>
        ///
        public static bool operator !=(IntRange range1, IntRange range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));

        }

        /// <summary>
        /// Check if this instance of <see cref="Range"/> equal to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another range to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals(object obj)
        {
            return (obj is IntRange) ? (this == (IntRange)obj) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode()
        {
            return min.GetHashCode() + max.GetHashCode();
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
        ///
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", min, max);
        }
    }
}
