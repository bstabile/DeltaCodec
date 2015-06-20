#region Derivative Work License (Stability.Data.Compression.ThirdParty)

// Disclaimer: All of the compression algorithms in this assembly are the work of others.
//             They are aggregated here to provide an easy way to learn about and test
//             alternative techniques. In the subfolders "Internal\<libname>" you will
//             find the minimal subset of files needed to expose each algorithm.
//             In the "Licenses" folder you will find the licensing information for each
//             of the third-party libraries. Those licenses (if more restrictive than
//             GPL v3) are meant to override.
//
// Namespace : Stability.Data.Compression.ThirdParty
// FileName  : QuickLZFinisher.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.ThirdParty.Internal.QuickLZ;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Stability.Data.Compression.ThirdParty
{
    /// <summary>
    /// This concrete implementation only needs to handle compression of a stream
    /// that has already had data written to it by the abstract base class. This
    /// arrangement makes it very easy hook up different finishing compression 
    /// without worrying about intrinsic data types.
    /// </summary>
    public class QuickLZFinisher : Finisher
    {
        public static readonly QuickLZFinisher Instance = new QuickLZFinisher();

        /// <summary>
        /// This encodes an input stream to an output stream.
        /// </summary>
        /// <param name="input">A stream of data to encode.</param>
        /// <param name="level">The level of compression desired (Fastest or Optimal).</param>
        /// <returns>A stream that has been encoded.</returns>
        /// <remarks>
        /// This does NOT close or alter the "Position" property of the returned stream.
        /// </remarks>
        public override MemoryStream EncodeToStream(MemoryStream input, CompressionLevel level = CompressionLevel.Optimal)
        {
            var lev = level == CompressionLevel.Optimal ? 3 : 1;
            return new MemoryStream(QuickLZ.Compress(input.ToArray(), lev));
        }

        /// <summary>
        /// After decoding, this method sets the position of the returned stream back to zero.
        /// Clients should not forget to close the stream, when finished using it.
        /// </summary>
        /// <param name="data">The encoded data that will be decoded.</param>
        /// <returns>An open <see cref="MemoryStream"/> wrapping the decoded data, with position set to zero.</returns>
        public override MemoryStream DecodeToStream(byte[] data)
        {
            return new MemoryStream(QuickLZ.Decompress(data));
        }

    }
}
