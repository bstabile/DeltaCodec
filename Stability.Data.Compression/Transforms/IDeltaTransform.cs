#region License

// Namespace : Stability.Data.Compression.Transforms
// FileName  : IDeltaTransform.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

using System;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.Transforms
{
    public interface IDeltaTransform
    {
        void Encode<T>(BlockState<T> state);
        void Decode<T>(BlockState<T> state);

        void Encode(DeltaBlockState<DateTimeOffset> state);
        void Decode(DeltaBlockState<DateTimeOffset> state);
        void Encode(DeltaBlockState<DateTime> state);
        void Decode(DeltaBlockState<DateTime> state);
        void Encode(DeltaBlockState<TimeSpan> state);
        void Decode(DeltaBlockState<TimeSpan> state);
        void Encode(DeltaBlockState<long> state);
        void Decode(DeltaBlockState<long> state);
        void Encode(DeltaBlockState<ulong> state);
        void Decode(DeltaBlockState<ulong> state);
        void Encode(DeltaBlockState<int> state);
        void Decode(DeltaBlockState<int> state);
        void Encode(DeltaBlockState<uint> state);
        void Decode(DeltaBlockState<uint> state);
        void Encode(DeltaBlockState<short> state);
        void Decode(DeltaBlockState<short> state);
        void Encode(DeltaBlockState<ushort> state);
        void Decode(DeltaBlockState<ushort> state);
        void Encode(DeltaBlockState<sbyte> state);
        void Decode(DeltaBlockState<sbyte> state);
        void Encode(DeltaBlockState<byte> state);
        void Decode(DeltaBlockState<byte> state);

        void Encode(DeltaBlockState<bool> state);
        void Decode(DeltaBlockState<bool> state);

        void Encode(DeltaBlockState<float> state);
        void Decode(DeltaBlockState<float> state);

        void Encode(DeltaBlockState<double> state);
        void Decode(DeltaBlockState<double> state);

        void Encode(DeltaBlockState<decimal> state);
        void Decode(DeltaBlockState<decimal> state);

        void Encode(DeltaBlockState<char> state);
        void Decode(DeltaBlockState<char> state);

    }
}
