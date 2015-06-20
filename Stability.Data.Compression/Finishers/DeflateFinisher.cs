#region License

// Namespace : Stability.Data.Compression.Finishers
// FileName  : DeflateFinisher.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System.IO;
using System.IO.Compression;

namespace Stability.Data.Compression.Finishers
{
    /// <summary>
    /// This concrete implementation only needs to handle compression of a stream
    /// that has already had data written to it by the abstract base class. This
    /// arrangement makes it very easy hook up different finishing compression 
    /// without worrying about intrinsic data types.
    /// </summary>
    public sealed class DeflateFinisher : Finisher
    {
         public static readonly DeflateFinisher Instance = new DeflateFinisher();

        /// <summary>
        /// This encodes an input stream to an output stream.
        /// </summary>
        /// <returns>A stream that has been encoded.</returns>
        /// <remarks>
        /// This does NOT close or alter the "Position" property of the returned stream.
        /// </remarks>
        public override MemoryStream EncodeToStream(MemoryStream input, CompressionLevel level = CompressionLevel.Fastest)
        {
            input.Position = 0;
            var output = new MemoryStream();
            using (var compressor = new DeflateStream(output, level))
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
            using (var decompressor = new DeflateStream(input, CompressionMode.Decompress))
            {
                decompressor.CopyTo(output);
            }
            output.Position = 0;
            return output;
        }

        /// <summary>
        /// After decoding, this method sets the position of the returned stream back to zero.
        /// Clients should not forget to close the stream, when finished using it.
        /// </summary>
        /// <param name="stream">The encoded data that will be decoded.</param>
        /// <returns>An open <see cref="MemoryStream"/> wrapping the decoded data, with position set to zero.</returns>
        public byte[] DecodeFromStream(MemoryStream stream)
        {
            var output = new MemoryStream();
            using (var decompressor = new DeflateStream(stream, CompressionMode.Decompress))
            {
                decompressor.CopyTo(output);
            }
            output.Position = 0;
            return output.ToArray();
        }

    }
}
