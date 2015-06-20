#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : DeltaType.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;

namespace Stability.Data.Compression.Utility
{
    public static class DeltaType
    {
        public const int CodeMask = 0xF;

        #region Maps

        private static readonly IDictionary<Type, byte> TypeToCodeMap = new Dictionary<Type, byte>
        {
            {typeof(object), 0},
            {typeof(bool), 1},
            {typeof(byte), 2},
            {typeof(sbyte), 3},
            {typeof(UInt16), 4},
            {typeof(Int16), 5},
            {typeof(UInt32), 6},
            {typeof(Int32), 7},
            {typeof(UInt64), 8},
            {typeof(Int64), 9},
            {typeof(float), 10},
            {typeof(double), 11},
            {typeof(decimal), 12},
            {typeof(DateTime), 13},
            {typeof(TimeSpan), 14},
            {typeof(DateTimeOffset), 15},
        };

        private static readonly IDictionary<byte, Type> CodeToTypeMap = new Dictionary<byte, Type>
        {
            {0, typeof(object)},
            {1, typeof(bool)},
            {2, typeof(byte)},
            {3, typeof(sbyte)},
            {4, typeof(UInt16)},
            {5, typeof(Int16)},
            {6, typeof(UInt32)},
            {7, typeof(Int32)},
            {8, typeof(UInt64)},
            {9, typeof(Int64)},
            {10, typeof(float)},
            {11, typeof(double)},
            {12, typeof(decimal)},
            {13, typeof(DateTime)},
            {14, typeof(TimeSpan)},  
            {15, typeof(DateTimeOffset)},
        };

        private static readonly IDictionary<Type, int> SizeMap = new Dictionary<Type, int>
        {
            {typeof (object), 0},
            {typeof (bool), 1},
            {typeof (byte), 1},
            {typeof (sbyte), 1},
            {typeof (UInt16), 2},
            {typeof (Int16), 2},
            {typeof (UInt32), 4},
            {typeof (Int32), 4},
            {typeof (UInt64), 8},
            {typeof (Int64), 8},
            {typeof (float), 4},
            {typeof (double), 8},
            {typeof (decimal), 16},
            {typeof (DateTime), 8},
            {typeof (TimeSpan), 8},
            {typeof (DateTimeOffset), 10},
        };

        private static readonly IDictionary<Type, int> BitsMap = new Dictionary<Type, int>
        {
            {typeof (object), 0},
            {typeof (bool), 8},
            {typeof (byte), 8},
            {typeof (sbyte), 8},
            {typeof (UInt16), 16},
            {typeof (Int16), 16},
            {typeof (UInt32), 32},
            {typeof (Int32), 32},
            {typeof (UInt64), 64},
            {typeof (Int64), 64},
            {typeof (float), 32},
            {typeof (double), 64},
            {typeof (decimal), 128},
            {typeof (DateTime), 64},
            {typeof (TimeSpan), 64},
            {typeof (DateTimeOffset), 80},
        };

        #endregion // Maps

        public static byte TypeToCode(Type type)
        {
            return type == null ? (byte) 0 : TypeToCodeMap[type];
        }

        public static Type CodeToType(byte code)
        {
            return CodeToTypeMap[code];
        }



        public static int Size(Type type)
        {
            return SizeMap[type];
        }

        public static int Bits(Type type)
        {
            return BitsMap[type];
        }

    }
}
