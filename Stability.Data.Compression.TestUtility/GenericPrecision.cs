#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : GenericPrecision.cs
// Created   : 2015-4-22
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public static class GenericPrecision
    {
        private static readonly IDictionary<Type, Func<object, int>> CalcFuncMap = new Dictionary
            <Type, Func<object, int>>
        {
            {typeof (bool), v => Calc((bool) v)},
            {typeof (byte), v => Calc((byte) v)},
            {typeof (sbyte), v => Calc((sbyte) v)},
            {typeof (ushort), v => Calc((ushort) v)},
            {typeof (short), v => Calc((short) v)},
            {typeof (uint), v => Calc((uint) v)},
            {typeof (int), v => Calc((int) v)},
            {typeof (ulong), v => Calc((ulong) v)},
            {typeof (long), v => Calc((long) v)},
            // DateTime and TimeSpan
            {typeof (DateTimeOffset), v => Calc((DateTimeOffset) v)},
            {typeof (DateTime), v => Calc((DateTime) v)},
            {typeof (TimeSpan), v => Calc((TimeSpan) v)},
            // Reals
            {typeof (float), v => Calc((float) v)},
            {typeof (double), v => Calc((double) v)},
            {typeof (decimal), v => Calc((decimal) v)},
        };

        public static int Calc<T>(T value) where T : struct
        {
            try
            {
                return CalcFuncMap[typeof(T)].Invoke(value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static int Calc(bool value)
        {
            return 0;
        }

        public static int Calc(byte value)
        {
            return 0;
        }

        public static int Calc(sbyte value)
        {
            return 0;
        }

        public static int Calc(ushort value)
        {
            return 0;
        }

        public static int Calc(short value)
        {
            return 0;
        }

        public static int Calc(uint value)
        {
            return 0;
        }

        public static int Calc(int value)
        {
            return 0;
        }

        public static int Calc(ulong value)
        {
            return 0;
        }

        public static int Calc(long value)
        {
            return 0;
        }

        public static int Calc(double value)
        {
            var p = DeltaUtility.Precision(value);
            return p;
        }

        public static int Calc(float value)
        {
            if (value >= 0) return 0;
            var p = DeltaUtility.Precision(value);
            return p;
        }

        public static int Calc(decimal value)
        {
            if (value >= 0) return 0;
            var p = DeltaUtility.Precision(value);
            return p;
        }

        public static int Calc(DateTimeOffset value)
        {
            return 0;
        }

        public static int Calc(DateTime value)
        {
            return 0;
        }

        public static int Calc(TimeSpan value)
        {
            return 0;
        }

        public static int Calc(object value)
        {
            return 0;
        }
    }

}
