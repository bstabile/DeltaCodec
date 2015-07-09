#region License

// Namespace : Stability.Data.Compression
// FileName  : DeltaCodec_Tuples.cs
// Created   : 2015-6-14
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
        // We only include methods for the primary 7 fields of a Tuple.
        // The last field (TRest) is to daisy-chain another Tuple.
        // Rather than confusing users with that kind of thing, we
        // decided instead to offer "Struple" types with up to 17 fields.
        // For anything beyond that it is recommended that users create
        // custom types and methods that can handle them efficiently.


        #region Tuple<T1, T2>

        public virtual byte[] Encode<T1, T2>(TupleEncodingArgs<T1, T2> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 2;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = list[i].Item1;
                arr2[i] = list[i].Item2;
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
                        granularity: (T1) args.Granularities[0],
                        monotonicity: args.Monotonicities[0]);

                    encodedBlocks[1][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Levels[1],
                        granularity: (T2) args.Granularities[1],
                        monotonicity: args.Monotonicities[1 ]);
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

        public virtual IList<Tuple<T1, T2>> DecodeTuple<T1, T2>(byte[] bytes)
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
            var list = new Tuple<T1, T2>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Tuple<T1, T2>(decodedBlocks1[i][j], decodedBlocks2[i][j]);
                }
            }
            return list;
        }

        #endregion // Tuple<T1, T2>

        #region Tuple<T1, T2, T3>

        public virtual byte[] Encode<T1, T2, T3>(TupleEncodingArgs<T1, T2, T3> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 3;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            Parallel.For(0, list.Count, i =>
            {
                var tuple = list[i];
                arr1[i] = tuple.Item1;
                arr2[i] = tuple.Item2;
                arr3[i] = tuple.Item3;
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
                        granularity: (T1) args.Granularities[0],
                        monotonicity: args.Monotonicities[0]);

                    encodedBlocks[1][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Levels[1],
                        granularity: (T2) args.Granularities[1],
                        monotonicity: args.Monotonicities[1]);

                    encodedBlocks[2][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Levels[2],
                        granularity: (T3) args.Granularities[2],
                        monotonicity: args.Monotonicities[2]);
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

        public virtual IList<Tuple<T1, T2, T3>> DecodeTuple<T1, T2, T3>(byte[] bytes)
        {
            const byte numVectorsExpected = 3;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T1>[encodedBlocks[0].Count];
            var decodedBlocks2 = new IList<T2>[encodedBlocks[1].Count];
            var decodedBlocks3 = new IList<T3>[encodedBlocks[2].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeNumericBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeNumericBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
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
            var list = new Tuple<T1, T2, T3>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Tuple<T1, T2, T3>(decodedBlocks1[i][j], decodedBlocks2[i][j], decodedBlocks3[i][j]);
                }
            }
            return list;
        }

        #endregion // Tuple<T1, T2, T3>

        #region Tuple<T1, T2, T3, T4>

        public virtual byte[] Encode<T1, T2, T3, T4>(TupleEncodingArgs<T1, T2, T3, T4> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 4;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                var tuple = list[i];
                arr1[i] = tuple.Item1;
                arr2[i] = tuple.Item2;
                arr3[i] = tuple.Item3;
                arr4[i] = tuple.Item4;
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

                    encodedBlocks[2][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Levels[2],
                        granularity: (T3)args.Granularities[2],
                        monotonicity: args.Monotonicities[2]);

                    encodedBlocks[3][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Levels[3],
                        granularity: (T4) args.Granularities[3],
                        monotonicity: args.Monotonicities[3]);
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

        public virtual IList<Tuple<T1, T2, T3, T4>> DecodeTuple<T1, T2, T3, T4>(byte[] bytes)
        {
            const byte numVectorsExpected = 4;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T1>[encodedBlocks[0].Count];
            var decodedBlocks2 = new IList<T2>[encodedBlocks[1].Count];
            var decodedBlocks3 = new IList<T3>[encodedBlocks[2].Count];
            var decodedBlocks4 = new IList<T4>[encodedBlocks[3].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeNumericBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeNumericBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeNumericBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
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
            var list = new Tuple<T1, T2, T3, T4>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Tuple<T1, T2, T3, T4>
                        (
                        decodedBlocks1[i][j],
                        decodedBlocks2[i][j],
                        decodedBlocks3[i][j],
                        decodedBlocks4[i][j]
                        );
                }
            }
            return list;
        }

        #endregion // Tuple<T1, T2, T3, T4>

        #region Tuple<T1, T2, T3, T4, T5>

        public virtual byte[] Encode<T1, T2, T3, T4, T5>(TupleEncodingArgs<T1, T2, T3, T4, T5> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 5;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];
            var arr5 = new T5[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                var tuple = list[i];
                arr1[i] = tuple.Item1;
                arr2[i] = tuple.Item2;
                arr3[i] = tuple.Item3;
                arr4[i] = tuple.Item4;
                arr5[i] = tuple.Item5;
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

                    encodedBlocks[2][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Levels[2],
                        granularity: (T3)args.Granularities[2],
                        monotonicity: args.Monotonicities[2]);

                    encodedBlocks[3][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Levels[3],
                        granularity: (T4)args.Granularities[3],
                        monotonicity: args.Monotonicities[3]);

                    encodedBlocks[4][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Levels[4],
                        granularity: (T5) args.Granularities[4],
                        monotonicity: args.Monotonicities[4]);
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

        public virtual IList<Tuple<T1, T2, T3, T4, T5>> DecodeTuple<T1, T2, T3, T4, T5>(byte[] bytes)
        {
            const byte numVectorsExpected = 5;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T1>[encodedBlocks[0].Count];
            var decodedBlocks2 = new IList<T2>[encodedBlocks[1].Count];
            var decodedBlocks3 = new IList<T3>[encodedBlocks[2].Count];
            var decodedBlocks4 = new IList<T4>[encodedBlocks[3].Count];
            var decodedBlocks5 = new IList<T5>[encodedBlocks[4].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeNumericBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeNumericBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeNumericBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeNumericBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
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
            var list = new Tuple<T1, T2, T3, T4, T5>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Tuple<T1, T2, T3, T4, T5>
                        (
                        decodedBlocks1[i][j],
                        decodedBlocks2[i][j],
                        decodedBlocks3[i][j],
                        decodedBlocks4[i][j],
                        decodedBlocks5[i][j]
                        );
                }
            }
            return list;
        }

        #endregion // Tuple<T1, T2, T3, T4, T5>

        #region Tuple<T1, T2, T3, T4, T5, T6>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6>(TupleEncodingArgs<T1, T2, T3, T4, T5, T6> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 6;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];
            var arr5 = new T5[list.Count];
            var arr6 = new T6[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                var tuple = list[i];
                arr1[i] = tuple.Item1;
                arr2[i] = tuple.Item2;
                arr3[i] = tuple.Item3;
                arr4[i] = tuple.Item4;
                arr5[i] = tuple.Item5;
                arr6[i] = tuple.Item6;
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

                    encodedBlocks[2][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Levels[2],
                        granularity: (T3)args.Granularities[2],
                        monotonicity: args.Monotonicities[2]);

                    encodedBlocks[3][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Levels[3],
                        granularity: (T4)args.Granularities[3],
                        monotonicity: args.Monotonicities[3]);

                    encodedBlocks[4][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Levels[4],
                        granularity: (T5)args.Granularities[4],
                        monotonicity: args.Monotonicities[4]);

                    encodedBlocks[5][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Levels[5],
                        granularity: (T6) args.Granularities[5],
                        monotonicity: args.Monotonicities[5]);
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

        public virtual IList<Tuple<T1, T2, T3, T4, T5, T6>> DecodeTuple<T1, T2, T3, T4, T5, T6>(byte[] bytes)
        {
            const byte numVectorsExpected = 6;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T1>[encodedBlocks[0].Count];
            var decodedBlocks2 = new IList<T2>[encodedBlocks[1].Count];
            var decodedBlocks3 = new IList<T3>[encodedBlocks[2].Count];
            var decodedBlocks4 = new IList<T4>[encodedBlocks[3].Count];
            var decodedBlocks5 = new IList<T5>[encodedBlocks[4].Count];
            var decodedBlocks6 = new IList<T6>[encodedBlocks[5].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeNumericBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeNumericBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeNumericBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeNumericBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeNumericBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
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
            var list = new Tuple<T1, T2, T3, T4, T5, T6>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Tuple<T1, T2, T3, T4, T5, T6>
                        (
                        decodedBlocks1[i][j],
                        decodedBlocks2[i][j],
                        decodedBlocks3[i][j],
                        decodedBlocks4[i][j],
                        decodedBlocks5[i][j],
                        decodedBlocks6[i][j]
                        );
                }
            }
            return list;
        }

        #endregion // Tuple<T1, T2, T3, T4, T5, T6>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7>(TupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 7;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];
            var arr5 = new T5[list.Count];
            var arr6 = new T6[list.Count];
            var arr7 = new T7[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                var tuple = list[i];
                arr1[i] = tuple.Item1;
                arr2[i] = tuple.Item2;
                arr3[i] = tuple.Item3;
                arr4[i] = tuple.Item4;
                arr5[i] = tuple.Item5;
                arr6[i] = tuple.Item6;
                arr7[i] = tuple.Item7;
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

                    encodedBlocks[2][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Levels[2],
                        granularity: (T3)args.Granularities[2],
                        monotonicity: args.Monotonicities[2]);

                    encodedBlocks[3][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Levels[3],
                        granularity: (T4)args.Granularities[3],
                        monotonicity: args.Monotonicities[3]);

                    encodedBlocks[4][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Levels[4],
                        granularity: (T5)args.Granularities[4],
                        monotonicity: args.Monotonicities[4]);

                    encodedBlocks[5][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Levels[5],
                        granularity: (T6)args.Granularities[5],
                        monotonicity: args.Monotonicities[5]);

                    encodedBlocks[6][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Levels[6],
                        granularity: (T7) args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);
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

        public virtual IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> DecodeTuple<T1, T2, T3, T4, T5, T6, T7>(byte[] bytes)
        {
            const byte numVectorsExpected = 7;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T1>[encodedBlocks[0].Count];
            var decodedBlocks2 = new IList<T2>[encodedBlocks[1].Count];
            var decodedBlocks3 = new IList<T3>[encodedBlocks[2].Count];
            var decodedBlocks4 = new IList<T4>[encodedBlocks[3].Count];
            var decodedBlocks5 = new IList<T5>[encodedBlocks[4].Count];
            var decodedBlocks6 = new IList<T6>[encodedBlocks[5].Count];
            var decodedBlocks7 = new IList<T7>[encodedBlocks[6].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeNumericBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeNumericBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeNumericBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeNumericBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeNumericBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeNumericBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
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
            var list = new Tuple<T1, T2, T3, T4, T5, T6, T7>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Tuple<T1, T2, T3, T4, T5, T6, T7>
                        (
                        decodedBlocks1[i][j],
                        decodedBlocks2[i][j],
                        decodedBlocks3[i][j],
                        decodedBlocks4[i][j],
                        decodedBlocks5[i][j],
                        decodedBlocks6[i][j],
                        decodedBlocks7[i][j]
                        );
                }
            }
            return list;
        }

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7>

    }
}
