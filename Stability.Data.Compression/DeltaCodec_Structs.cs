#region License

// Namespace : Stability.Data.Compression
// FileName  : DeltaCodec_Structs.cs
// Created   : 2015-6-25
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression
{
    public abstract partial class DeltaCodec<TTransform>
    {
        #region Generic Methods

        public virtual byte[] Encode<T>(EncodingArgs<T> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 1;
            const ushort shortFlags = 0; // reserved for future use

            var arr = args.List.ToArray();

            // Sanity Check!
            var numBlocks = args.NumBlocks;
            if (numBlocks < 1)
                numBlocks = 1;
            if (numBlocks > MaxNumParallelBlocks)
                numBlocks = MaxNumParallelBlocks;

            var encodedBlocks = new List<byte[][]>(numVectors);
            for (var i = 0; i < numVectors; i++)
            {
                encodedBlocks.Add(new byte[numBlocks][]);
            }
            var ranges = OrderedRangeFactory.Create(0, args.List.Count, numBlocks);

            try
            {
                Parallel.For(0, ranges.Count, r =>
                {
                    encodedBlocks[0][r] = EncodeBlock(
                        range: ranges[r],
                        list: arr,
                        blockIndex: r,
                        level: args.Level);
                });
            }
            catch (Exception ex)
            {
                // This can happen when the client is passing in an invalid list type.
                Debug.WriteLine(ex.Message);
                throw;
            }

            return WriteEncodedBlocks(numBlocks, shortFlags, encodedBlocks);
        }

        public virtual byte[] Encode<T>(NumericEncodingArgs<T> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 1;
            const ushort shortFlags = 0; // reserved for future use

            var arr = args.List.ToArray();

            // Sanity Check!
            var numBlocks = args.NumBlocks;
            if (numBlocks < 1)
                numBlocks = 1;
            if (numBlocks > MaxNumParallelBlocks)
                numBlocks = MaxNumParallelBlocks;

            var encodedBlocks = new List<byte[][]>(numVectors);
            for (var i = 0; i < numVectors; i++)
            {
                encodedBlocks.Add(new byte[numBlocks][]);
            }
            var ranges = OrderedRangeFactory.Create(0, args.List.Count, numBlocks);

            try
            {
                Parallel.For(0, ranges.Count, r =>
                {
                    encodedBlocks[0][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr,
                        level: args.Level,
                        granularity: args.Granularity,
                        monotonicity: args.Monotonicity);
                });
            }
            catch (Exception ex)
            {
                // This can happen when the client is passing in an invalid list type.
                Debug.WriteLine(ex.Message);
                throw;
            }

            return WriteEncodedBlocks(numBlocks, shortFlags, encodedBlocks);
        }

        public virtual IList<T> Decode<T>(byte[] bytes)
        {
            const byte numVectorsExpected = 1;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T>[encodedBlocks[0].Count];
            try
            {
                if (typeof (T).IsClass)
                {
                    Parallel.For(0, encodedBlocks[0].Count, i =>
                    {
                        decodedBlocks1[i] = DecodeBlock<T>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    });
                }
                else
                {
                    Parallel.For(0, encodedBlocks[0].Count, i =>
                    {
                        decodedBlocks1[i] = DecodeNumericBlock<T>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    });
                }

            }
            catch (Exception ex)
            {
                // This can happen when the client is passing in an invalid list type.
                Debug.WriteLine(ex.Message);
                throw;
            }

            // Combine Blocks

            var listCount = decodedBlocks1.Select(b => b.Count).Sum();
            var list = new T[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = decodedBlocks1[i][j];
                }
            }
            return list;
        }

        #endregion // Generic Methods

        #region Protected Methods

        protected virtual byte[] EncodeBlock<T>(Range32 range, T[] list, int blockIndex, CompressionLevel level)
        {
            var start = range.InclusiveStart;
            var stop = range.ExclusiveStop;
            var block = new ArraySegment<T>(list, start, stop - start);
            var state = new BlockState<T>(
                list: block,
                level: level,
                finisher: DefaultFinisher,
                blockIndex: blockIndex);
            Transform.Encode(state);
            return DeltaBlockSerializer.Serialize(state);
        }

        protected virtual byte[] EncodeNumericBlock<T>(Range32 range, T[] list, int blockIndex, CompressionLevel level, T granularity,
            Monotonicity monotonicity)
        {
            var start = range.InclusiveStart;
            var stop = range.ExclusiveStop;
            var block = new ArraySegment<T>(list, start, stop - start);
            var state = new DeltaBlockState<T>(
                list: block,
                level: level,
                granularity: granularity,
                monotonicity: monotonicity,
                finisher: DefaultFinisher,
                blockIndex: blockIndex);
            Transform.Encode(state);
            return DeltaBlockSerializer.Serialize(state);
        }

        protected virtual IList<T> DecodeBlock<T>(byte[] block, IFinisher finisher, int blockIndex = 0)
        {
            var state = new BlockState<T>(block, finisher, blockIndex);
            DeltaBlockSerializer.Deserialize(state);
            Transform.Decode(state);
            return state.List;
        }

        protected virtual IList<T> DecodeNumericBlock<T>(byte[] block, IFinisher finisher, int blockIndex = 0)
        {
            var state = new DeltaBlockState<T>(block, finisher, blockIndex);
            DeltaBlockSerializer.Deserialize(state);
            Transform.Decode(state);
            return state.List;
        }


        #endregion // Protected Methods
    }
}
