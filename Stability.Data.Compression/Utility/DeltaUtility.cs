#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : DeltaUtility.cs
// Created   : 2015-4-22
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Stability.Data.Compression.DataStructure;

namespace Stability.Data.Compression.Utility
{
    public static class DeltaUtility
    {
        #region Factor

        private static readonly IDictionary<Type, Func<object, object>> FactorFuncMap = new Dictionary
            <Type, Func<object, object>>
        {
            {typeof (byte), o => Factor((IList<byte>) o)},
            {typeof (sbyte), o => Factor((IList<sbyte>) o)},
            {typeof (ushort), o => Factor((IList<ushort>) o)},
            {typeof (short), o => Factor((IList<short>) o)},
            {typeof (uint), o => Factor((IList<uint>) o)},
            {typeof (int), o => Factor((IList<int>) o)},
            {typeof (ulong), o => Factor((IList<ulong>) o)},
            {typeof (long), o => Factor((IList<long>) o)},
            // DateTime and TimeSpan
            {typeof (DateTimeOffset), o => Factor((IList<DateTimeOffset>) o)},
            {typeof (DateTime), o => Factor((IList<DateTime>) o)},
            {typeof (TimeSpan), o => Factor((IList<TimeSpan>) o)},
            // Reals
            {typeof (float), o => Factor((IList<float>) o)},
            {typeof (double), o => Factor((IList<double>) o)},
            {typeof (decimal), o => Factor((IList<decimal>) o)},
            // Char
            {typeof (char), o => Factor((IList<char>) o)},
        };

        public static T Factor<T>(IList<T> list)
        {
            try
            {
                return (T) FactorFuncMap[typeof (T)].Invoke(list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static DateTimeOffset Factor(IList<DateTimeOffset> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0].Ticks;
            var lastTicks = 0L;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f].Ticks;
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1].Ticks;
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return new DateTime(factor);
        }

        public static DateTime Factor(IList<DateTime> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0].Ticks;
            var lastTicks = 0L;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f].Ticks;
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1].Ticks;
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return new DateTime(factor);
        }

        public static TimeSpan Factor(IList<TimeSpan> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0].Ticks;
            var lastTicks = 0L;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f].Ticks;
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1].Ticks;
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return new TimeSpan(factor);
        }

        public static long Factor(IList<long> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = 0L;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static ulong Factor(IList<ulong> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = 0UL;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static int Factor(IList<int> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = 0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static uint Factor(IList<uint> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = 0U;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static short Factor(IList<short> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = (short)0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = (short)(a % b);
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static ushort Factor(IList<ushort> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = (ushort)0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = (ushort)(a % b);
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static sbyte Factor(IList<sbyte> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = (sbyte)0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = (sbyte)(a % b);
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static byte Factor(IList<byte> arr)
        {
            var n = arr.Count - 1;
            var factor = arr[0];
            var lastTicks = (byte)0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = (byte)(a % b);
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return factor;
        }

        public static float Factor(IList<float> arr)
        {
            var n = arr.Count - 1;
            var bits = new BitsF(arr[0]);
            var factor = bits.Int;
            var lastTicks = 0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            
            while (factor == 0 && f < arr.Count)
            {
                bits.Value = arr[f];
                factor = bits.Int;
                f++;
            }
            for (var i = f; i < n; i++)
            {
                bits.Value = arr[i + 1];
                var a = bits.Int;
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || (a % factor).Equals(0))
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            bits.Int = factor;
            return bits.Value;
        }

        public static double Factor(IList<double> arr)
        {
            var n = arr.Count - 1;
            var bits = new BitsD(arr[0]);
            var factor = bits.Long;
            var lastTicks = 0L;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                bits.Value = arr[f];
                factor = bits.Long;
                f++;
            }
            for (var i = f; i < n; i++)
            {
                bits.Value = arr[i + 1];
                var a = bits.Long;
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            bits.Long = factor;
            return bits.Value;
        }

        public static decimal Factor(IList<decimal> arr)
        {
            // The Euler method seems to work but it likely won't break out 
            // of the loop properly because we don't know what the epsilon
            // should be. 
            // Possibly it could be Math.Pow(10, -28).
            // But then that would almost always lead to a full traversal.
            // Instead we can do a parallel loop and just check for the
            // highest precision. However, this doesn't always get us the ideal
            // value (e.g. granularity = 0.25m).

            //// PRECISION METHOD:
            //var precision = 0;
            //Parallel.For(0, arr.Count, i =>
            //{
            //    //var v = arr[i];
            //    var p = Precision(arr[i]);
            //    if (p > precision)
            //    {
            //        Interlocked.Add(ref precision, p - precision);
            //    }
            //});
            //return (decimal)Math.Pow(10, precision * -1);
            
            // EULER METHOD:
            var n = arr.Count - 1;
            var factor = arr[0];
            var last = 0m;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == last || a % factor == 0)
                    continue; // Already been here!
                last = a;

                while (b != 0)
                {
                    var c = a % b;
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }

            return factor;
        }

        public static char Factor(IList<char> arr)
        {
            var n = arr.Count - 1;
            var factor = (short) arr[0];
            var lastTicks = (short)0;
            // We need to try and start with a non-zero factor.
            // TODO: Consider the case where all values are equal (e.g. all zeros)
            var f = 1;
            while (factor == 0 && f < arr.Count)
            {
                factor = (short) arr[f];
                f++;
            }
            for (var i = f; i < n; i++)
            {
                var a = (short) arr[i + 1];
                var b = factor;

                // Short-circuiting here can shave 2% off time on a full factor run
                if (a == lastTicks || a % factor == 0)
                    continue; // Already been here!
                lastTicks = a;

                while (b != 0)
                {
                    var c = (short)(a % b);
                    a = b;
                    b = c;
                }
                factor = a;
                if (factor == 1)
                    break;
            }
            return (char) factor;
        }

        #endregion // Factor

        #region Precision

        public static int Precision(double value)
        {
            return (Decimal.GetBits((decimal)value)[3] >> 16) & 0x000000FF;
        }

        public static int Precision(float value)
        {
            var d = Math.Round(value, 7);
            return (Decimal.GetBits((decimal)d)[3] >> 16) & 0x000000FF;
        }

        public static int Precision(decimal value)
        {
            var d = Math.Round(value, 28);
            return (Decimal.GetBits(d)[3] >> 16) & 0x000000FF;
        }


        #endregion // Precision

        #region AliasChecking

        public static bool CanAliasToLong(DeltaBlockState<float> state)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var factor = state.Factor;
            var hasFactor = !factor.Equals(0) && !factor.Equals(1);
            if (!hasFactor)
                return false;

            //var precision = Precision(factor);

            var max = state.List.Max();
            var min = state.List.Min();

            //var smax = max.ToString("F" + precision);
            //var smin = min.ToString("F" + precision);

            //var vmax = float.Parse(smax);
            //var vmin = float.Parse(smin);

            //if (!max.Equals(vmax) || !min.Equals(vmin))
            //{
            //    return false;
            //}

            try
            {
                checked
                {
                    var lmax = (long)(hasFactor ? (max / factor) : max);
                    var lmin = (long)(hasFactor ? (min / factor) : min);
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                stopwatch.Stop();
                Debug.WriteLine("CanAliasToDecimal Check took {0} ms", stopwatch.ElapsedMilliseconds);
            }
            return true;
        }

        public static bool CanAliasToLong(DeltaBlockState<double> state)
        {
            //var stopwatch = Stopwatch.StartNew();

            var factor = state.Factor;
            var hasFactor = !factor.Equals(0);

            var max = state.List.Max();
            var min = state.List.Min();
            try
            {
                checked
                {
                    var lmax = (long)(hasFactor ? (max / factor) : max);
                    var lmin = (long)(hasFactor ? (min / factor) : min);
                }
            }
            catch
            {
                return false;
            }
            //finally
            //{
            //    stopwatch.Stop();
            //    Debug.WriteLine("CanAliasToDecimal Check took {0} ms", stopwatch.ElapsedMilliseconds);
            //}
            return true;
        }

        public static bool CanAliasToDecimal(DeltaBlockState<float> state)
        {
            //var stopwatch = Stopwatch.StartNew();

            var factor = state.Factor;
            var hasFactor = !factor.Equals(0);

            var max = state.List.AsParallel().Max();
            var min = state.List.AsParallel().Min();

            try
            {
                checked
                {
                    var mmax = (decimal)(hasFactor ? (max / factor) : max);
                    var mmin = (decimal)(hasFactor ? (min / factor) : min);
                }
            }
            catch
            {
                return false;
            }
            //finally
            //{
            //    stopwatch.Stop();
            //    Debug.WriteLine("CanAliasToDecimal Check took {0} ms", stopwatch.ElapsedMilliseconds);
            //}
            return true;
        }

        public static bool CanAliasToDecimal(DeltaBlockState<double> state)
        {
            //var stopwatch = Stopwatch.StartNew();

            var factor = state.Factor;
            var hasFactor = !factor.Equals(0);

            var max = state.List.AsParallel().Max();
            var min = state.List.AsParallel().Min();
            try
            {
                checked
                {
                    var mmax = (decimal)(hasFactor ? (max / factor) : max);
                    var mmin = (decimal)(hasFactor ? (min / factor) : min);
                }
            }
            catch
            {
                return false;
            }
            //finally
            //{
            //    stopwatch.Stop();
            //    Debug.WriteLine("CanAliasToDecimal Check took {0} ms", stopwatch.ElapsedMilliseconds);
            //}
            return true;
        }

        #endregion // AliasChecking

        #region ConvertToBuffer

        /// <summary>
        /// A "Fake Generic" mapping from data type to function call.
        /// Other types with a defined subtraction operator can be added. 
        /// </summary>
        private static readonly IDictionary<Type, Func<object, byte[]>> ConvertToBufferFuncMap = new Dictionary<Type, Func<object, byte[]>>
        {
            {typeof(decimal), list => ConvertToBuffer((IList<decimal>) list)},
            {typeof(DateTimeOffset), list => ConvertToBuffer((IList<DateTimeOffset>) list)},
            {typeof(DateTime), list => ConvertToBuffer((IList<DateTime>) list)},
            {typeof(TimeSpan), list => ConvertToBuffer((IList<TimeSpan>) list)},
            {typeof(double), list => ConvertToBuffer((IList<double>) list)},
            {typeof(float), list => ConvertToBuffer((IList<float>) list)},
            {typeof(long), list => ConvertToBuffer((IList<long>) list)},
            {typeof(ulong), list => ConvertToBuffer((IList<ulong>) list)},
            {typeof(int), list => ConvertToBuffer((IList<int>) list)},
            {typeof(uint), list => ConvertToBuffer((IList<uint>) list)},
            {typeof(short), list => ConvertToBuffer((IList<short>) list)},
            {typeof(ushort), list => ConvertToBuffer((IList<ushort>) list)},
            {typeof(sbyte), list => ConvertToBuffer((IList<sbyte>) list)},
            {typeof(byte), list => ConvertToBuffer((IList<byte>) list)},
            {typeof(bool), list => ConvertToBuffer((IList<bool>) list)},
            {typeof(char), list => ConvertToBuffer((IList<char>) list)},
            {typeof(string), list => ConvertToBuffer((IList<string>) list)},
       };

        public static byte[] ConvertToBuffer<T>(IList<T> list)
            where T : IComparable<T>, IEquatable<T>
        {
            return ConvertToBufferFuncMap[typeof(T)].Invoke(list);
        }

        public static byte[] ConvertToBuffer(IList<decimal> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<double> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<float> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<UInt64> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<Int64> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<UInt32> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<Int32> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<UInt16> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<Int16> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<SByte> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<byte> list)
        {
            return list.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<bool> list)
        {
            var arr = new byte[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                arr[i] = (byte)(list[i] ? 1 : 0);
            }
            return arr;
        }


        public static byte[] ConvertToBuffer(IList<DateTimeOffset> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v.Ticks);
                writer.Write(v.Offset.Ticks / TimeSpan.TicksPerMinute);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<DateTime> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v.Ticks);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<TimeSpan> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v.Ticks);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<char> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        public static byte[] ConvertToBuffer(IList<string> list)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            foreach (var v in list)
            {
                writer.Write(v);
            }
            return ms.ToArray();
        }

        #endregion // ConvertToBuffer

        public static int BitCount(Type type)
        {
            var typeName = type.Name;
            switch (typeName)
            {
                case "Decimal":
                    return 128;
                case "Int64":
                case "UInt64":
                case "Double":
                case "DateTimeOffset":
                    return 80; // 64 + 16
                case "DateTime":
                case "TimeSpan":
                    return 64;
                case "Int32":
                case "UInt32":
                case "Single":
                    return 32;
                case "Int16":
                case "UInt16":
                case "Char":
                    return 16;
                case "Byte":
                case "SByte":
                case "Boolean":
                    return 8;
                default:
                    throw new StrongTypingException("Must be a numeric value type (or Boolean, Char, DateTimeOffset, DateTime, TimeSpan");
            }
        }

        public static int GetSizeOfType(Type t, object list = null)
        {
            if (t == typeof (string))
            {
                var strings = list as IList<string>;
                if (strings != null)
                {
                    var charCount = strings.Sum(s => s.Length);
                    return (charCount * 2) / strings.Count;
                }
                return 0;
            }
            if (t == typeof (Boolean))
            {
                return 1;
            }
            if (t == typeof (char))
            {
                return 2;
            }
            if (t == typeof (DateTimeOffset))
            {
                return 10;
            }
            if (t == typeof (DateTime) || t == typeof (TimeSpan))
            {
                return 8;
            }
            return Marshal.SizeOf(t);
        }

        public static int GetTotalBytes(MultiFieldEncodingArgs args)
        {
            if (args == null) throw new ArgumentNullException("args");

            var argsType = args.GetType();
            var types = argsType.GetGenericArguments();

            var simpleBytes = types.Where(t => t != typeof(string))
                .Sum(t => GetSizeOfType(t, args.DynamicData)) * args.DataCount;
            var stringBytes = GetTotalBytesForStringFields(args);
            var total = simpleBytes + stringBytes;
            return total;
        }

        public static int GetTotalBytesForStringFields(MultiFieldEncodingArgs args)
        {
            var argsType = args.GetType();
            var listType = argsType.GetProperty("Data").PropertyType.GetGenericArguments().First();
            var props = listType.GetFields().Where(p => p.FieldType == typeof(string)).ToList();

            var sumChars = 0;
            var stringBytes = 0;
            if (props.Count != 0)
            {
                var dynList = args.DynamicData;
                for (var i = 0; i < props.Count; i++)
                {
                    var p = props[i];
                    for (var j = 0; j < dynList.Count; j++)
                    {
                        sumChars += p.GetValue(dynList[j]).Length;
                    }
                }
                stringBytes = 2 * sumChars;
            }
            return stringBytes;
        }
    }
}
