#region License

// Namespace : Stability.Data.Compression
// FileName  : IDeltaCodec.cs
// Created   : 2015-6-13
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System.Collections.Generic;
using System.IO.Compression;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression
{
    public interface IDeltaCodec
    {
        #region Public Properties

        string DisplayName { get; }

        IFinisher DefaultFinisher { get; }

        #endregion // Public Properties

        #region Generic Methods

        /// <summary>
        /// This is actually a "Fake" generic version of the Encode method.
        /// </summary>
        byte[] Encode<T>(StrupleEncodingArgs<T> args)
            where T : struct;

        /// <summary>
        /// This is actually a "Fake" generic version of the Encode method.
        /// </summary>
        byte[] Encode<T>(IList<T> list,
            int numBlocks = 1,
            CompressionLevel level = CompressionLevel.Fastest,
            T? granularity = default(T?),
            Monotonicity monotonicity = Monotonicity.None)
            where T : struct;

        /// <summary>
        /// This is actually a "Fake" generic version of the Decode method.
        /// </summary>
        IList<T> Decode<T>(byte[] bytes)
            where T : struct;

        #region Tuples

        byte[] Encode<T1, T2>(StrupleEncodingArgs<T1, T2> args)
            where T1 : struct
            where T2 : struct;

        IList<Struple<T1, T2>> Decode<T1, T2>(byte[] bytes)
            where T1 : struct
            where T2 : struct;

        byte[] Encode<T1, T2, T3>(StrupleEncodingArgs<T1, T2, T3> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct;

        IList<Struple<T1, T2, T3>> Decode<T1, T2, T3>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct;

        byte[] Encode<T1, T2, T3, T4>(StrupleEncodingArgs<T1, T2, T3, T4> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct;

        IList<Struple<T1, T2, T3, T4>> Decode<T1, T2, T3, T4>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct;

        byte[] Encode<T1, T2, T3, T4, T5>(StrupleEncodingArgs<T1, T2, T3, T4, T5> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct;

        IList<Struple<T1, T2, T3, T4, T5>> Decode<T1, T2, T3, T4, T5>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6>> Decode<T1, T2, T3, T4, T5, T6>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7>> Decode<T1, T2, T3, T4, T5, T6, T7>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> Decode<T1, T2, T3, T4, T5, T6, T7, T8>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct;

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct
            where T15 : struct;

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct
            where T15 : struct;

        #endregion // Tuples

        #endregion // Generic Methods
    }
}
