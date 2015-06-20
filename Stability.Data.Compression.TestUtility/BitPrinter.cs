#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : BitPrinter.cs
// Created   : 2015-6-8
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    /// <summary>
    /// This class is meant to be used for displaying bits for various value types.
    /// The bits for a given value are printed out on a single line.
    /// The "byteSeparator" argument can be null, in which case
    /// the static DefaultByteSeparator property will be used.
    /// The static DefaultSampleCount property serves a similar purpose,
    /// i.e. to simplify repeated calls that need the same defaults.
    /// 
    /// Note that if you set DefaultSampleCount to a value less than zero,
    /// then the backing field will actually be set to zero.
    /// Thus no samples will be printed. This can be useful
    /// to inhibit printing with minimal changes to client code.
    /// </summary>
    public static class BitPrinter
    {
        #region Static Defaults

        private static string _defaultByteSeparator = " ";

        public static string DefaultByteSeparator
        {
            get { return _defaultByteSeparator; }
            set { _defaultByteSeparator = value; }
        }

        private static int _defaultSampleCount = 10;
        public static int DefaultSampleCount
        {
            get { return _defaultSampleCount; }
            set { _defaultSampleCount = value > 0 ? value : 0; }
        }
        #endregion // Static Defaults

        #region Private Static Func Maps

        private static readonly IDictionary<Type, Func<object, string, string>> PrimitiveFuncMap = new Dictionary<Type, Func<object, string, string>>
        {
            {typeof(bool), (v,s) => Print((bool) v, s)},
            {typeof(byte), (v,s) => Print((byte) v, s)},
            {typeof(sbyte), (v,s) => Print((sbyte)v, s)},
            {typeof(ushort), (v,s) => Print((ushort)v, s)},
            {typeof(short), (v,s)=> Print((short)v, s)},
            {typeof(uint), (v,s) => Print((uint)v, s)},
            {typeof(int), (v,s) => Print((int)v, s)},
            {typeof(ulong), (v,s) => Print((ulong)v, s)},
            {typeof(long), (v,s) => Print((long)v, s)},
            {typeof(float), (v,s) =>Print((float)v, s)},
            {typeof(double), (v,s) =>Print((double)v, s)},
            {typeof(decimal), (v,s) =>Print((decimal)v, s)},
            {typeof(DateTimeOffset), (v,s) =>Print((DateTimeOffset)v, s)},
            {typeof(DateTime), (v,s) =>Print((DateTime)v, s)},
            {typeof(TimeSpan), (v,s) =>Print((TimeSpan)v, s)},
            {typeof(BigInteger), (v,s) =>Print((BigInteger)v, s)},
        };
        private static readonly IDictionary<Type, Func<object, string, string>> ListFuncMap = new Dictionary<Type, Func<object, string, string>>
        {
            {typeof(bool), (v,s) => Print((IList<bool>) v, s)},
            {typeof(byte), (v,s) => Print((IList<byte>) v, s)},
            {typeof(sbyte), (v,s) => Print((IList<sbyte>)v, s)},
            {typeof(ushort), (v,s) => Print((IList<ushort>)v, s)},
            {typeof(short), (v,s) => Print((IList<short>)v, s)},
            {typeof(uint), (v,s) => Print((IList<uint>)v, s)},
            {typeof(int), (v,s) => Print((IList<int>)v, s)},
            {typeof(ulong), (v,s) => Print((IList<ulong>)v, s)},
            {typeof(long), (v,s) => Print((IList<long>)v, s)},
            {typeof(float), (v,s) => Print((IList<float>)v, s)},
            {typeof(double), (v,s) => Print((IList<double>)v, s)},
            {typeof(DateTimeOffset), (v,s) => Print((IList<DateTimeOffset>)v, s)},
            {typeof(DateTime), (v,s) => Print((IList<DateTime>)v, s)},
            {typeof(TimeSpan), (v,s) => Print((IList<TimeSpan>)v, s)},
            {typeof(decimal), (v,s) => Print((IList<decimal>)v, s)},
        };

        private static readonly IDictionary<Type, Func<object, string, string, int, int, string>> SamplesFuncMap = new Dictionary
            <Type, Func<object, string, string, int, int, string>>
        {
            {typeof (bool), (v, s1, s2, i1, i2) => PrintSamples((IList<bool>) v, s1, s2, i1, i2)},
            {typeof (byte), (v, s1, s2, i1, i2) => PrintSamples((IList<byte>) v, s1, s2, i1, i2)},
            {typeof (sbyte), (v, s1, s2, i1, i2) => PrintSamples((IList<sbyte>) v, s1, s2, i1, i2)},
            {typeof (ushort), (v, s1, s2, i1, i2) => PrintSamples((IList<ushort>) v, s1, s2, i1, i2)},
            {typeof (short), (v, s1, s2, i1, i2) => PrintSamples((IList<short>) v, s1, s2, i1, i2)},
            {typeof (uint), (v, s1, s2, i1, i2) => PrintSamples((IList<uint>) v, s1, s2, i1, i2)},
            {typeof (int), (v, s1, s2, i1, i2) => PrintSamples((IList<int>) v, s1, s2, i1, i2)},
            {typeof (ulong), (v, s1, s2, i1, i2) => PrintSamples((IList<ulong>) v, s1, s2, i1, i2)},
            {typeof (long), (v, s1, s2, i1, i2) => PrintSamples((IList<long>) v, s1, s2, i1, i2)},
            {typeof (float), (v, s1, s2, i1, i2) => PrintSamples((IList<float>) v, s1, s2, i1, i2)},
            {typeof (double), (v, s1, s2, i1, i2) => PrintSamples((IList<double>) v, s1, s2, i1, i2)},
            {typeof (decimal), (v, s1, s2, i1, i2) => PrintSamples((IList<decimal>) v, s1, s2, i1, i2)},
            {typeof (DateTimeOffset), (v, s1, s2, i1, i2) => PrintSamples((IList<DateTimeOffset>) v, s1, s2, i1, i2)},
            {typeof (DateTime), (v, s1, s2, i1, i2) => PrintSamples((IList<DateTime>) v, s1, s2, i1, i2)},
            {typeof (TimeSpan), (v, s1, s2, i1, i2) => PrintSamples((IList<TimeSpan>) v, s1, s2, i1, i2)},
        };

        #endregion // Private Static Func Maps

        #region Print Generic

        /// <summary>
        /// This is actually a "Fake" generic version of the Print method.
        /// This handles both primitive types and primitive array types.
        /// </summary>
        public static string Print<T>(T o, string byteSeparator = null)
        {
            var t = typeof(T);
            if (t.IsArray)
            {
                var e = t.GetElementType();
                if (e.IsPrimitive)
                {
                    try
                    {
                        return ListFuncMap[e].Invoke(o, byteSeparator);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            else if (t.IsPrimitive)
            {
                try
                {
                    return PrimitiveFuncMap[t].Invoke(o, byteSeparator);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return null;
        }

        #endregion // Print Generic

        #region Print Primitive Types

        public static string Print(ulong v, string byteSeparator = null)
        {
            byteSeparator = byteSeparator ?? DefaultByteSeparator;
            var sb = new StringBuilder();
            for (var i = 63; i >= 0; i--)
            {
                var bit = (v >> i) & 1;
                sb = sb.Append(bit);
                if (byteSeparator != null && (i % 8) == 0 && i > 0)
                    sb = sb.Append(byteSeparator);
            }
            return sb.ToString();
        }

        public static string Print(long v, string byteSeparator = null)
        {
            return Print((ulong)v, byteSeparator);
        }

        public static string Print(uint v, string byteSeparator = null)
        {
            byteSeparator = byteSeparator ?? DefaultByteSeparator;
            var sb = new StringBuilder();
            for (var i = 31; i >= 0; i--)
            {
                var bit = (v >> i) & 1;
                sb = sb.Append(bit);
                if (byteSeparator != null && (i % 8) == 0 && i > 0)
                    sb = sb.Append(byteSeparator);
            }
            return sb.ToString().Trim();
        }

        public static string Print(int v, string byteSeparator = null)
        {
            return Print((uint)v, byteSeparator);
        }

        public static string Print(ushort v, string byteSeparator = null)
        {
            byteSeparator = byteSeparator ?? DefaultByteSeparator;
            var sb = new StringBuilder();
            for (var i = 15; i >= 0; i--)
            {
                var bit = (v >> i) & 1;
                sb = sb.Append(bit);
                if (byteSeparator != null && (i % 8) == 0 && i > 0)
                    sb = sb.Append(byteSeparator);
            }
            return sb.ToString().Trim();
        }

        public static string Print(short v, string byteSeparator = null)
        {
            return Print((ushort)v, byteSeparator);
        }

        public static string Print(byte v, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            for (var i = 7; i >= 0; i--)
            {
                var bit = (v >> i) & 1;
                sb = sb.Append(bit);
            }
            return sb.ToString();
        }

        public static string Print(bool v, string byteSeparator = null)
        {
            return v ? "00000001" : "00000000";
        }

        public static string Print(sbyte v, string byteSeparator = null)
        {
            return Print((byte)v);
        }

        public static string Print(float v, string byteSeparator = null)
        {
            return Print(new BitsF { Value = v }.Bits, byteSeparator);
        }

        public static string Print(double v, string byteSeparator = null)
        {
            return Print(new BitsD { Value = v }.Bits, byteSeparator);
        }

        public static string Print(decimal v, string byteSeparator = null)
        {
            byteSeparator = byteSeparator ?? DefaultByteSeparator;
            var arr = Decimal.GetBits(v);
            var low = Print(arr[0], byteSeparator);
            var mid = Print(arr[1], byteSeparator);
            var high = Print(arr[2], byteSeparator);
            var exp = Print(arr[3], byteSeparator);
            return exp + (byteSeparator ?? "") + high + (byteSeparator ?? "") + mid + (byteSeparator ?? "") + low;
        }

        public static string Print(DateTimeOffset v, string byteSeparator = null)
        {
            var dt = Print(v.DateTime, byteSeparator);
            var os = Print(v.Offset, byteSeparator);
            return dt + " | " + os;
        }

        public static string Print(DateTime v, string byteSeparator = null)
        {
            return Print(v.Ticks, byteSeparator);
        }

        public static string Print(TimeSpan v, string byteSeparator = null)
        {
            return Print(v.Ticks, byteSeparator);
        }

        public static string Print(BigInteger v, string byteSeparator = null)
        {
            byteSeparator = byteSeparator ?? DefaultByteSeparator;
            var bytes = v.ToByteArray();
            var sb = new StringBuilder();
            for (var i = bytes.Length - 1; i >= 0; i--)
            {
                sb = sb.Append(Print(bytes[i]));
                if (i != 0)
                    sb = sb.Append(byteSeparator);
            }

            return sb.ToString();
        }

        #endregion // Print Primitive Types

        #region Print IList Types

        public static string Print(IList<long> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<ulong> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<int> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<uint> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<short> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<ushort> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<sbyte> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<byte> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<bool> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<float> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<double> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, v) => current.AppendLine(Print(v, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<decimal> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, t) => current.AppendLine(Print(t, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<DateTimeOffset> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, t) => current.AppendLine(Print(t, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<DateTime> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, t) => current.AppendLine(Print(t.Ticks, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<TimeSpan> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, t) => current.AppendLine(Print(t.Ticks, byteSeparator)));
            return sb.ToString();
        }

        public static string Print(IList<BigInteger> data, string byteSeparator = null)
        {
            var sb = new StringBuilder();
            sb = data.Aggregate(sb, (current, t) => current.AppendLine(Print(t, byteSeparator)));
            return sb.ToString();
        }

        #endregion // Print IList Types

        #region Print Samples

        public static string PrintSamples<T>(
            IList<T> list,
            string valueFormat = null,
            string byteSeparator = null,
            int start = 0,
            int count = 0)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            try
            {
                return SamplesFuncMap[typeof(T)].Invoke(list, valueFormat, byteSeparator, start, count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static string PrintSamples(
            IList<DateTimeOffset> list,
            string valueFormat = null,
            string byteSeparator = null,
            int start = 0,
            int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            var tmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
                var tlen = list[i].Ticks.ToString("F0").Length;
                if (tlen > tmax)
                    tmax = tlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1, {4}{5} | {6}2{7}\n",
                "{", vmax, "}", "{", tmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    list[i].Ticks.ToString("F0"),
                    Print(list[i].Ticks, byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
            IList<DateTime> list,
            string valueFormat = null,
            string byteSeparator = null,
            int start = 0,
            int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            var tmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
                var tlen = list[i].Ticks.ToString("F0").Length;
                if (tlen > tmax)
                    tmax = tlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1, {4}{5} | {6}2{7}\n",
                "{", vmax, "}", "{", tmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    list[i].Ticks.ToString("F0"),
                    Print(list[i].Ticks, byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<TimeSpan> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            var tmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
                var tlen = list[i].Ticks.ToString("F0").Length;
                if (tlen > tmax)
                    tmax = tlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1, {4}{5} | {6}2{7}\n",
                "{", vmax, "}", "{", tmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    list[i].Ticks.ToString("F0"),
                    Print(list[i].Ticks, byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<bool> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
             )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString().Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i],
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<byte> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
             )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<sbyte> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<ushort> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<short> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<uint> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<int> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<ulong> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<long> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<float> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
             IList<double> list,
             string valueFormat = null,
             string byteSeparator = null,
             int start = 0,
             int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        public static string PrintSamples(
            IList<decimal> list,
            string valueFormat = null,
            string byteSeparator = null,
            int start = 0,
            int count = 0
            )
        {
            if (list.Count == 0)
                return "\t<empty>";

            count = count > 0 ? count : DefaultSampleCount;

            if (count == 0) return string.Empty;

            count = Math.Min(start + count, list.Count);

            var vmax = 0;
            for (var i = start; i < count; i++)
            {
                var vlen = list[i].ToString(valueFormat).Length;
                if (vlen > vmax)
                    vmax = vlen;
            }

            var fmt = string.Format("{0}0, {1}{2} | {3}1{4}\n",
                "{", vmax, "}", "{", "}");

            var sb = new StringBuilder();
            for (var i = start; i < count; i++)
            {
                var s = string.Format(fmt,
                    list[i].ToString(valueFormat),
                    Print(list[i], byteSeparator));
                sb = sb.AppendFormat(s);
            }
            return sb.ToString();
        }

        #endregion // Print Samples

    }
}
