using System.Collections.Generic;
using System.IO.Compression;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Transforms;

namespace Stability.Data.Compression.Utility
{
    public interface IBlockState
    {
        IFinisher Finisher { get; }
        int BlockIndex { get; }
        int ListCount { get; }
        byte[] Bytes { get; }

        DeltaFlags Flags { get; }
    }

    /// <summary>
    /// This type is a generalization of the <see cref="DeltaBlockState{T}"/>
    /// (which is constrained to handling intrinsic value types).
    /// 
    /// Reference types are usually, but not always, complex data 
    /// classes that have intrinsic value type fields. Either way,
    /// transformation strategies are usually not as uniform as
    /// they are for data types that can be handled numerically.
    /// 
    /// For example, a <see cref="Dictionary{DateTime, Double}"/> can 
    /// be encoded as two numeric vectors (Keys, Values). The vectors
    /// can be transformed using math and/or bit manipulation. 
    /// But a <see cref="Dictionary{String, String}"/> would have 
    /// to be handled differently. For one thing, each vector here 
    /// is really like a two-dimensional jagged array (char[][]). 
    /// 
    /// In other words, the transforms for reference types will
    /// usually be specialized to decompose data in some way 
    /// before it is altered, serialized, and/or passed along
    /// for finishing compression (<see  cref="Finisher"/>). 
    /// </summary>
    public class BlockState<T> : IBlockState
    {
        /// <summary>
        /// Resharper warnings can safely be ignored here.
        /// We'll get a different instance of the Finisher for each type.
        /// 
        /// Resharper disable StaticFieldInGenericType
        /// </summary>
        private static readonly IFinisher DefaultFinisher = DeflateFinisher.Instance;
        // Resharper restore StaticFieldInGenericType

        public BlockState(IList<T> list,
            CompressionLevel level = CompressionLevel.Fastest,
            IFinisher finisher = null,
            int blockIndex = 0)
        {
            List = list;
            ListCount = list.Count;
            Anchor = list[0];
            Flags = new DeltaFlags(typeof(T), level);
            Finisher = finisher ?? DefaultFinisher;
            BlockIndex = blockIndex;
        }

        public BlockState(byte[] bytes, IFinisher finisher = null, int blockIndex = 0)
        {
            Bytes = bytes;
            Flags = new DeltaFlags(typeof(T));
            Finisher = finisher ?? DefaultFinisher;
            BlockIndex = blockIndex;
        }

        /// <summary>
        /// This is used to maintain order within a Frame.
        /// </summary>
        public int BlockIndex { get; set; }

        public IFinisher Finisher { get; set; }

        /// <summary>
        /// Encoding: This is the series that will be encoded.
        /// Decoding: This is the series that has been decoded.
        /// </summary>
        public IList<T> List { get; set; }

        /// <summary>
        /// Encoding: This value isn't really needed.
        /// Decoding: This value is needed to properly reconstitute the series.
        /// This value must always be serialized with other header information.
        /// </summary>
        public int ListCount { get; set; }

        /// <summary>
        /// Encoding: Initially null, encoded bytes after processing.
        /// Decoding: The encoded bytes that will be processed.
        /// This value must always be serialized along with header information.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// This is used when writing and reading header information with Streams.
        /// The reason we need it is to know how many bytes were serialized, and thus
        /// how many need to be read (before we actually have the Bytes property available).
        /// It is particularly important when working with blocks, so we know the position
        /// of each successive block.
        /// </summary>
        public int ByteCount { get; set; }

        /// <summary>
        /// Encoding: This will be set by the encoding method. Initial value doesn't matter.
        /// Decoding: Must be set before calling the decoding method.
        /// This value must always be serialized with other header information.
        /// </summary>
        public T Anchor { get; set; }

        public DeltaFlags Flags { get; private set; }

        /// <summary>
        /// This property is used to pass any additional information needed
        /// by <see cref="DeltaTransform"/> methods.
        /// </summary>
        public object CustomArgs { get; set; }
    }
}
