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
using System.Runtime.Serialization;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Finishers;

namespace Stability.Data.Compression
{
    public interface IDeltaCodec
    {
        #region Public Properties

        string DisplayName { get; }

        IFinisher DefaultFinisher { get; }

        #endregion // Public Properties

        /// <summary>
        /// This is actually a "Fake" generic version of the Encode method.
        /// </summary>
        byte[] Encode<T>(EncodingArgs<T> args);

        /// <summary>
        /// This is actually a "Fake" generic version of the Encode method.
        /// </summary>
        byte[] Encode<T>(NumericEncodingArgs<T> args);

        /// <summary>
        /// This is actually a "Fake" generic version of the Decode method.
        /// </summary>
        IList<T> Decode<T>(byte[] bytes);


        #region Struples

        IList<Struple<T>> DecodeStruple<T>(byte[] bytes);

        byte[] Encode<T1, T2>(StrupleEncodingArgs<T1, T2> args);

        IList<Struple<T1, T2>> DecodeStruple<T1, T2>(byte[] bytes);

        byte[] Encode<T1, T2, T3>(StrupleEncodingArgs<T1, T2, T3> args);

        IList<Struple<T1, T2, T3>> DecodeStruple<T1, T2, T3>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4>(StrupleEncodingArgs<T1, T2, T3, T4> args);

        IList<Struple<T1, T2, T3, T4>> DecodeStruple<T1, T2, T3, T4>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5>(StrupleEncodingArgs<T1, T2, T3, T4, T5> args);

        IList<Struple<T1, T2, T3, T4, T5>> DecodeStruple<T1, T2, T3, T4, T5>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> args);

        IList<Struple<T1, T2, T3, T4, T5, T6>> DecodeStruple<T1, T2, T3, T4, T5, T6>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(byte[] bytes);

        byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> args);

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(byte[] bytes);

        #endregion // Struples

    }
}
