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
            // Reals
            {typeof (float), v => Calc((float) v)},
            {typeof (double), v => Calc((double) v)},
            {typeof (decimal), v => Calc((decimal) v)},
        };

        public static int Calc<T>(T value)
        {
            try
            {
                if (typeof (T) == typeof (double)
                    || typeof (T) == typeof (float)
                    || typeof (T) == typeof (decimal))
                {
                    return CalcFuncMap[typeof (T)].Invoke(value);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
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
    }

}
