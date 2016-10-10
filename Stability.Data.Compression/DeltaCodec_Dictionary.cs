#region License

// Namespace : Stability.Data.Compression
// FileName  : DeltaCodec_Dictionary.cs
// Created   : 2015-7-5
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression
{
    public abstract partial class DeltaCodec<TTransform>
    {
        public virtual byte[] Encode<T1, T2>(MapEncodingArgs<T1, T2> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.Data property is null.", "args");

            const byte numVectors = 2;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data.ToList();
            var numBlocks = args.NumBlocks;

            var n = list.Count;
            var arr1 = new T1[n];
            var arr2 = new T2[n];
            Parallel.For(0, n, i =>
            {
                arr1[i] = list[i].Key;
                arr2[i] = list[i].Value;
            });

            // Sanity Check!
            if (numBlocks < 1)
                numBlocks = 1;
            if (numBlocks > MaxNumParallelBlocks)
                numBlocks = MaxNumParallelBlocks;

            var encodedBlocks = new List<byte[][]>(numVectors);
            for (var i = 0; i < numVectors; i++)
            {
                encodedBlocks.Add(new byte[numBlocks][]);
            }
            var ranges = OrderedRangeFactory.Create(0, list.Count, numBlocks);

            try
            {
                Parallel.For(0, ranges.Count, r =>
                {
                    encodedBlocks[0][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Levels[0],
                        granularity: (T1)args.Granularities[0],
                        monotonicity: args.Monotonicities[0]);

                    encodedBlocks[1][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Levels[1],
                        granularity: (T2)args.Granularities[1],
                        monotonicity: args.Monotonicities[1]);
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

        public virtual IDictionary<T1, T2> DecodeMap<T1, T2>(byte[] bytes)
        {
            const byte numVectorsExpected = 2;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T1>[encodedBlocks[0].Count];
            var decodedBlocks2 = new IList<T2>[encodedBlocks[1].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeNumericBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                });
            }
            catch (Exception ex)
            {
                // This can happen when the client is passing in an invalid list type.
                Debug.WriteLine(ex.Message);
                throw;
            }

            // Combine Blocks

            var listCount = decodedBlocks1.Select(b => b.Count).Sum();
            var list = new Dictionary<T1, T2>(listCount);

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list.Add(decodedBlocks1[i][j], decodedBlocks2[i][j]);
                }
            }
            return list;
        }

    }
}
