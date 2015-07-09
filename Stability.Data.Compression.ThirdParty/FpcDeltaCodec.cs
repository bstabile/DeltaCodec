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
// FileName  : FpcDeltaCodec.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Transforms;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.ThirdParty
{
    public sealed class FpcDeltaCodec : DeltaCodec<NullTransform>
    {
        #region Private Function Map

        /// <summary>
        /// The function maps provide the ability to use the codec in a generic manner
        /// even though it is not truly generic. FPC only encodes lists of doubles, so
        /// we need to override the "fake" generic mapping in the base class.
        /// </summary>
        private readonly IDictionary<Type, Func<IEncodingArgs, byte[]>> _encodeFuncMap;

        /// <summary>
        /// The function maps provide the ability to use the codec in a generic manner
        /// even though it is not truly generic. FPC only encodes lists of doubles, so
        /// we need to override the "fake" generic mapping in the base class.
        /// </summary>
        private readonly IDictionary<Type, Func<byte[], object>> _decodeFuncMap;

        #endregion // Private Function Map

        #region Constructors

        /// <summary>
        /// FPC is a self-contained codec that doesn't rely on any other IDeltaTransform or IFinisher.
        /// </summary>
        private FpcDeltaCodec()
            : this(DeflateFinisher.Instance)
        {
        }

        /// <summary>
        /// FPC is unusual because it directly processes lists rather than byte arrays or streams.
        /// It is specialized to only handle lists of doubles. The Transform and Finisher are
        /// irrelevant, so it doesn't matter what is specified for those.
        /// </summary>
        private FpcDeltaCodec(IFinisher defaultFinisher)
            : base(defaultFinisher)
        {
            _encodeFuncMap = new Dictionary<Type, Func<IEncodingArgs, byte[]>>
            {
                {typeof (double), (a) => Encode((NumericEncodingArgs<double>) a)},
            };
            _decodeFuncMap = new Dictionary<Type, Func<byte[], object>>
            {
                {typeof (double), DecodeDouble},
            };
        }


        #endregion // Constructors

        #region Static

        /// <summary>
        /// The static "Instance" property gets initialized on first use. This makes it easier
        /// to reuse a single codec when there is any kind of more complicated setup logic that
        /// must apply across all instances.
        /// </summary>
        /// <remarks>
        /// The implementor of advanced codecs must ensure that the instance reflects any
        /// setup logic required.
        /// </remarks>
        static FpcDeltaCodec()
        {
            Instance = new FpcDeltaCodec();
        }

        public static FpcDeltaCodec Instance { get; protected set; }

        #endregion // Static

        #region Generic Encode<T> and Decode<T> Overrides

        public override byte[] Encode<T>(NumericEncodingArgs<T> args) 
        {
            try
            {
                return _encodeFuncMap[typeof(T)].Invoke(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public override IList<T> Decode<T>(byte[] bytes)
        {
            try
            {
                return (IList<T>)_decodeFuncMap[typeof(T)].Invoke(bytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion // Generic Encode<T> and Decode<T>

        // This is the only type (IList<double>) that FPC can handle. 
        // Everything else throws an exception when the type doesn't map to any function.    
        #region Double

        /// <summary>
        /// Deflate and Inflate occur in RWT for Doubles.
        /// That's because they should best be run in parallel
        /// when aliasing to Decimal is possible.
        /// </summary>
        /// <remarks>
        /// It may seem counterintuitive to cast up to Decimal (when possible).
        /// But testing has shown this to be beneficial (in both time and space).
        /// Obviously, this will not be possible for extreme scientific data.
        /// But for some series, such as those typical in financial or business
        /// applications, this is well worth the check of the series range.
        /// </remarks>
        private byte[] Encode(IList<double> list,
            CompressionLevel level = CompressionLevel.Fastest,
            double granularity = 0,
            Monotonicity monotonicity = Monotonicity.None,
            int numBlocks = 1)
        {
            var fullCodecName = GetType().FullName;
            var magicNumber = fullCodecName.GetHashCode();
            var arr = list.ToArray();

            // Sanity Check!
            if (numBlocks < 1)
                numBlocks = 1;
            if (numBlocks > MaxNumParallelBlocks)
                numBlocks = MaxNumParallelBlocks;

            // If we can find a granularity here, then we won't do it for each block!
            if (granularity.Equals(0))
            {
                granularity = DeltaUtility.Factor(list);
            }

            var encodedBlocks = new byte[numBlocks][];
            var ranges = OrderedRangeFactory.Create(0, list.Count, numBlocks);
            try
            {
                Parallel.For(0, ranges.Count, r =>
                {
                    var range = ranges[r];
                    var start = range.InclusiveStart;
                    var stop = range.ExclusiveStop;
                    var block = new ArraySegment<double>(arr, start, stop - start);

                    // FPC TRANSFORM BEGIN
                    // We're doing our own compression here with no finisher, so we specify "NoCompression".
                    // That way, when we deserialize nothing is done (other than parsing the headers).
                    var state = new DeltaBlockState<double>(block, CompressionLevel.NoCompression, 
                        granularity, monotonicity, DefaultFinisher, blockIndex: range.Index);
                    var tail = new double[list.Count - 1];
                    for (var i = 1; i < list.Count; i++)
                    {
                        tail[i - 1] = block.Array[i];
                    }
                    //Array.Copy(list.ToArray(), 1, tail, 0, list.Count - 1);
                    var fpc = new Internal.Fpc.FcmCompressor();
                    using (var ms = new MemoryStream())
                    {
                        fpc.Compress(ms, tail);
                        var bytes = ms.ToArray();
                        state.Bytes = bytes;
                        state.ByteCount = bytes.Length;
                    }
                    // FPC TRANSFORM END

                    var final = DeltaBlockSerializer.Serialize(state);
                    encodedBlocks[r] = final;
                });
            }
            catch (Exception ex)
            {
                // This can happen when the client is passing in an invalid list type.
                Debug.WriteLine(ex.Message);
                throw;
            }

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(magicNumber);
                writer.Write(numBlocks);
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = encodedBlocks[i];
                    writer.Write(block.Length);
                    writer.Write(block);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deflate and Inflate occur in RWT for Doubles.
        /// That's because they should best be run in parallel
        /// when aliasing to Decimal is possible.
        /// </summary>
        /// <remarks>
        /// It may seem counterintuitive to cast up to Decimal (when possible).
        /// But testing has shown this to be beneficial (in both time and space).
        /// For many series, such as those typical in financial or business
        /// applications, this is well worth the check of the series range.
        /// </remarks>
        private IList<double> DecodeDouble(byte[] deflated)
        {
            using (var ms = new MemoryStream(deflated))
            using (var reader = new BinaryReader(ms))
            {
                var magicNumber = reader.ReadInt32();
                if (magicNumber != GetType().FullName.GetHashCode())
                {
                    throw new InvalidOperationException("Codec hashcode does not match the encoded magic number!");
                }

                var numBlocks = reader.ReadInt32();

                var encodedBlocks = new byte[numBlocks][];
                for (var i = 0; i < numBlocks; i++)
                {
                    var blockLength = reader.ReadInt32();
                    var block = reader.ReadBytes(blockLength);
                    encodedBlocks[i] = block;
                }

                var decodedBlocks = new IList<double>[numBlocks];
                try
                {
                    Parallel.For(0, numBlocks, i =>
                    {
                        var state = new DeltaBlockState<double>(encodedBlocks[i], DefaultFinisher);
                        DeltaBlockSerializer.Deserialize(state);
                        var listCount = state.ListCount;

                        // FPC TRANSFORM BEGIN
                        var arrSeg = new double[listCount - 1];
                        var fpc = new Internal.Fpc.FcmCompressor();
                        using (var stream = new MemoryStream(state.Bytes))
                        {
                            fpc.Decompress(stream, arrSeg);
                        }
                        var block = new List<double>(listCount) { state.Anchor };
                        block.AddRange(arrSeg);
                        // FPC TRANSFORM END

                        decodedBlocks[i] = block;
                    });
                }
                catch (Exception ex)
                {
                    // This can happen when the client is passing in an invalid list type.
                    Debug.WriteLine(ex.Message);
                    throw;
                }
                var totalCount = decodedBlocks.Select(b => b.Count).Sum();
                var list = new List<double>(totalCount);

                // Combine Blocks
                for (var i = 0; i < decodedBlocks.Length; i++)
                {
                    var block = decodedBlocks[i];

                    list.AddRange(block);
                }

                return list;
            }
        }

        #endregion // Double

        // EXAMPLE USAGE IN CODEC TESTS:
        // TODO: Set this up so it can be part of the standard CodecTests, but only active for IList<double>.
        //private void RunFpc()
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    var fpc = new ThirdParty.Fpc.FcmCompressor();
        //    var ms = new MemoryStream();
        //    var arr = ListIn.Cast<double>().ToArray();
        //    fpc.Compress(ms, arr);
        //    var bytes = ms.ToArray();
        //    ElapsedEncode = stopwatch.Elapsed;
        //    stopwatch.Stop();

        //    stopwatch.Restart();
        //    var listOut = new Double[ListIn.Count];
        //    // We cannot reuse an existing compressor since the internal predictors need to be fresh!
        //    fpc = new ThirdParty.Fpc.FcmCompressor();
        //    fpc.Decompress(new MemoryStream(bytes), listOut);
        //    ElapsedDecode = stopwatch.Elapsed;
        //    stopwatch.Stop();

        //    ValidateResults((IList<double>)ListIn, listOut);
        //    CalcCompressionStatistics(bytes);
        //}
    }
}
