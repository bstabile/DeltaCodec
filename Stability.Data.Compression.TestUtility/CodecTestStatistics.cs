#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : CodecTestStatistics.cs
// Created   : 2015-5-16
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public class CodecTestStatistics
    {
        public CodecTestStatistics(Type dataType, int listCount)
        {
            DataType = dataType;
            BytesForType = DeltaUtility.GetSizeOfIntrinsicType(dataType);
            ListCount = listCount;
            RawBytes = BytesForType*ListCount;
        }

        // The following fields don't change.
        public Type DataType { get; set; }
        public int BytesForType { get; set; }
        public int ListCount { get; set; }
        public long RawBytes { get; set; }
        public double EntropyIn { get; set; }

        // The following fields change when Calc() or Reset() is called.
        public long EncodedBytes { get; set; }
        public double Ratio { get; set; }
        public double Multiple { get; set; }
        public double CmbsEncode { get; set; }
        public double CmbsDecode { get; set; }
        public double EntropyOut { get; set; }

        // The following fields change when Reset() is called
        public TimeSpan ElapsedEncode { get; set; }
        public TimeSpan ElapsedDecode { get; set; }
        public double SpaceRank { get; set; }
        public double TimeRankEncode { get; set; }
        public double TimeRankDecode { get; set; }
        public double Balance { get; set; }

        public void Calc(byte[] compressedBytes)
        {
            EncodedBytes = compressedBytes.Length;
            Ratio = ((double)EncodedBytes) / RawBytes;
            Multiple = EncodedBytes == 0 ? 0 : RawBytes / ((double)EncodedBytes);
            // Compressed MB/sec
            var compressed = RawBytes - EncodedBytes;
            CmbsEncode = (compressed / ElapsedEncode.TotalSeconds) / 1000000;
            CmbsDecode = (compressed / ElapsedDecode.TotalSeconds) / 1000000;

            EntropyOut = Statistics.BufferEntropy(compressedBytes) / 8; // Divisor is always 8 bits
        }

        public void Reset()
        {
            EncodedBytes = 0;
            Ratio = 0;
            Multiple = 0;
            
            ElapsedEncode = default(TimeSpan);
            ElapsedDecode = default(TimeSpan);
            
            CmbsEncode = 0;
            CmbsDecode = 0;
            EntropyOut = 0;

            // These rankings are calculated externally because they are relative to other tests.
            SpaceRank = 0;
            TimeRankEncode = 0;
            TimeRankDecode = 0;
            Balance = 0;
        }

    }
}
