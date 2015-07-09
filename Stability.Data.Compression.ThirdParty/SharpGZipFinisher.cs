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
// FileName  : SharpGZipFinisher.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)
using System.IO;
using System.IO.Compression;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.ThirdParty.Internal.SharpZipLib.Zip;
using SharpZipLibGZipInputStream = Stability.Data.Compression.ThirdParty.Internal.SharpZipLib.GZip.GZipInputStream;
using SharpZipLibGZipOutputStream = Stability.Data.Compression.ThirdParty.Internal.SharpZipLib.GZip.GZipOutputStream;

namespace Stability.Data.Compression.ThirdParty
{
    /// <summary>
    /// This concrete implementation only needs to handle compression of a stream
    /// that has already had data written to it by the abstract base class. This
    /// arrangement makes it very easy hook up different finishing compression 
    /// without worrying about intrinsic data types.
    /// </summary>
    public sealed class SharpGZipFinisher : Finisher
    {
        public static readonly SharpGZipFinisher Instance = new SharpGZipFinisher();

        /// <summary>
        /// This encodes an input stream to an output stream.
        /// </summary>
        /// <param name="input">A stream of data to encode.</param>
        /// <param name="level">The level of compression desired (not applicable to GZip).</param>
        /// <returns>A stream that has been encoded.</returns>
        /// <remarks>
        /// This does NOT close or alter the "Position" property of the returned stream.
        /// </remarks>
        public override MemoryStream EncodeToStream(MemoryStream input, CompressionLevel level = CompressionLevel.Optimal)
        {
            var lev = Deflater.DEFAULT_COMPRESSION;
            if (level == CompressionLevel.Optimal)
            {
                lev = Deflater.BEST_COMPRESSION;
            }
            else if (level == CompressionLevel.Fastest)
            {
                lev = Deflater.BEST_SPEED;
            }
            var deflater = new Deflater(lev);

            input.Position = 0;
            var output = new MemoryStream();
            using (var compressor = new SharpZipLibGZipOutputStream(output, deflater))
            {
                input.CopyTo(compressor);
            }
            return output;
        }

        /// <summary>
        /// After decoding, this method sets the position of the returned stream back to zero.
        /// Clients should not forget to close the stream, when finished using it.
        /// </summary>
        /// <param name="data">The encoded data that will be decoded.</param>
        /// <returns>An open <see cref="MemoryStream"/> wrapping the decoded data, with position set to zero.</returns>
        public override MemoryStream DecodeToStream(byte[] data)
        {
            var output = new MemoryStream();
            using (var input = new MemoryStream(data))
            using (var decompressor = new SharpZipLibGZipInputStream(input))
            {
                decompressor.CopyTo(output);
            }
            output.Position = 0;
            return output;
        }

    }
}
