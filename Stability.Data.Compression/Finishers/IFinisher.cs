#region License

// Namespace : Stability.Data.Compression.Finishers
// FileName  : IFinisher.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Stability.Data.Compression.Finishers
{
    public interface IFinisher
    {
        // Unsigned
        byte[] Encode(IList<ushort> list, CompressionLevel level);
        IList<ushort> DecodeUInt16(byte[] bytes);
        byte[] Encode(IList<byte> list, CompressionLevel level);
        IList<byte> DecodeByte(byte[] bytes);
        byte[] Encode(IList<uint> list, CompressionLevel level);
        IList<uint> DecodeUInt32(byte[] bytes);
        byte[] Encode(IList<ulong> list, CompressionLevel level);
        IList<ulong> DecodeUInt64(byte[] bytes);

        // Signed
        byte[] Encode(IList<sbyte> list, CompressionLevel level);
        IList<sbyte> DecodeSByte(byte[] bytes);
        byte[] Encode(IList<short> list, CompressionLevel level);
        IList<short> DecodeInt16(byte[] bytes);
        byte[] Encode(IList<int> list, CompressionLevel level);
        IList<int> DecodeInt32(byte[] bytes);
        byte[] Encode(IList<long> list, CompressionLevel level);
        IList<long> DecodeInt64(byte[] bytes);

        // Bool
        byte[] Encode(IList<bool> list, CompressionLevel level);
        IList<bool> DecodeBoolean(byte[] bytes);

        // Real
        byte[] Encode(IList<float> list, CompressionLevel level);
        IList<float> DecodeSingle(byte[] bytes);
        byte[] Encode(IList<double> list, CompressionLevel level);
        IList<double> DecodeDouble(byte[] bytes);
        byte[] Encode(IList<decimal> list, CompressionLevel level);
        IList<decimal> DecodeDecimal(byte[] bytes); 

        // DateTimeOffset, DateTime, and TimeSpan
        byte[] Encode(IList<DateTimeOffset> list, CompressionLevel level);
        IList<DateTimeOffset> DecodeDateTimeOffset(byte[] bytes);
        byte[] Encode(IList<DateTime> list, CompressionLevel level);
        IList<DateTime> DecodeDateTime(byte[] bytes);
        byte[] Encode(IList<TimeSpan> list, CompressionLevel level);
        IList<TimeSpan> DecodeTimeSpan(byte[] bytes);

        // Streams
        byte[] Encode(MemoryStream input, CompressionLevel level = CompressionLevel.Optimal);
        MemoryStream EncodeToStream(MemoryStream input, CompressionLevel level = CompressionLevel.Optimal);
        MemoryStream DecodeToStream(byte[] data);
    }
}
