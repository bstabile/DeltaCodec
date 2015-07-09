#region License

// Namespace : Stability.Data.Compression
// FileName  : DeltaCodec_Struples.cs
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

        #region Struple<T>

        public virtual byte[] Encode<T>(StrupleEncodingArgs<T> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 2;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;

            var arr1 = new T[list.Count];
            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = list[i].Item1;
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
                        granularity: (T)args.Granularities[0],
                        monotonicity: args.Monotonicities[0]);
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

        public virtual IList<Struple<T>> DecodeStruple<T>(byte[] bytes)
        {
            const byte numVectorsExpected = 2;

            ushort shortFlags;
            var encodedBlocks = ReadEncodedBlocks(bytes, numVectorsExpected, out shortFlags);

            // Now that we've deserialized the raw blocks, we need to use DeltaBlockSerializer to do the rest.

            var decodedBlocks1 = new IList<T>[encodedBlocks[0].Count];

            try
            {
                Parallel.For(0, encodedBlocks[0].Count, i =>
                {
                    decodedBlocks1[i] = DecodeNumericBlock<T>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T>
                    {
                        Item1 = decodedBlocks1[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2>

        #region Struple<T1, T2>

        public virtual byte[] Encode<T1, T2>(StrupleEncodingArgs<T1, T2> args)
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

        public virtual IList<Struple<T1, T2>> DecodeStruple<T1, T2>(byte[] bytes)
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
            var list = new Struple<T1, T2>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2>

        #region Struple<T1, T2, T3>

        public virtual byte[] Encode<T1, T2, T3>(StrupleEncodingArgs<T1, T2, T3> args)
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

        public virtual IList<Struple<T1, T2, T3>> DecodeStruple<T1, T2, T3>(byte[] bytes)
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
            var list = new Struple<T1, T2, T3>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3>

        #region Struple<T1, T2, T3, T4>

        public virtual byte[] Encode<T1, T2, T3, T4>(StrupleEncodingArgs<T1, T2, T3, T4> args)
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
                        granularity: (T4)args.Granularities[3],
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

        public virtual IList<Struple<T1, T2, T3, T4>> DecodeStruple<T1, T2, T3, T4>(byte[] bytes)
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
            var list = new Struple<T1, T2, T3, T4>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4>

        #region Struple<T1, T2, T3, T4, T5>

        public virtual byte[] Encode<T1, T2, T3, T4, T5>(StrupleEncodingArgs<T1, T2, T3, T4, T5> args)
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
                        granularity: (T5)args.Granularities[4],
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

        public virtual IList<Struple<T1, T2, T3, T4, T5>> DecodeStruple<T1, T2, T3, T4, T5>(byte[] bytes)
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
            var list = new Struple<T1, T2, T3, T4, T5>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5>

        #region Struple<T1, T2, T3, T4, T5, T6>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> args)
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
                        granularity: (T6)args.Granularities[5],
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6>> DecodeStruple<T1, T2, T3, T4, T5, T6>(byte[] bytes)
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
            var list = new Struple<T1, T2, T3, T4, T5, T6>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6>

        #region Struple<T1, T2, T3, T4, T5, T6, T7>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> args)
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
                        granularity: (T7)args.Granularities[6],
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7>(byte[] bytes)
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 8;
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
            var arr8 = new T8[list.Count];

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
                arr8[i] = tuple.Item8;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8) args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8>(byte[] bytes)
        {
            const byte numVectorsExpected = 8;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 9;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9) args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9>(byte[] bytes)
        {
            const byte numVectorsExpected = 9;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 10;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10) args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(byte[] bytes)
        {
            const byte numVectorsExpected = 10;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 11;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11) args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(byte[] bytes)
        {
            const byte numVectorsExpected = 11;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 12;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];
            var arr12 = new T12[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
                arr12[i] = tuple.Item12;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11)args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);

                    encodedBlocks[11][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Levels[11],
                        granularity: (T12) args.Granularities[11],
                        monotonicity: args.Monotonicities[11]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(byte[] bytes)
        {
            const byte numVectorsExpected = 12;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];
            var decodedBlocks12 = new IList<T12>[encodedBlocks[11].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeNumericBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                        Item12 = decodedBlocks12[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 13;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];
            var arr12 = new T12[list.Count];
            var arr13 = new T13[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
                arr12[i] = tuple.Item12;
                arr13[i] = tuple.Item13;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11)args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);

                    encodedBlocks[11][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Levels[11],
                        granularity: (T12)args.Granularities[11],
                        monotonicity: args.Monotonicities[11]);

                    encodedBlocks[12][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Levels[12],
                        granularity: (T13) args.Granularities[12],
                        monotonicity: args.Monotonicities[12]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(byte[] bytes)
        {
            const byte numVectorsExpected = 13;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];
            var decodedBlocks12 = new IList<T12>[encodedBlocks[11].Count];
            var decodedBlocks13 = new IList<T13>[encodedBlocks[12].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeNumericBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeNumericBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                        Item12 = decodedBlocks12[i][j],
                        Item13 = decodedBlocks13[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 14;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];
            var arr12 = new T12[list.Count];
            var arr13 = new T13[list.Count];
            var arr14 = new T14[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
                arr12[i] = tuple.Item12;
                arr13[i] = tuple.Item13;
                arr14[i] = tuple.Item14;
            });

            var fullCodecName = GetType().FullName;
            var magicNumber = fullCodecName.GetHashCode();

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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11)args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);

                    encodedBlocks[11][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Levels[11],
                        granularity: (T12)args.Granularities[11],
                        monotonicity: args.Monotonicities[11]);

                    encodedBlocks[12][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Levels[12],
                        granularity: (T13)args.Granularities[12],
                        monotonicity: args.Monotonicities[12]);

                    encodedBlocks[13][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr14,
                        level: args.Levels[13],
                        granularity: (T14) args.Granularities[13],
                        monotonicity: args.Monotonicities[13]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(byte[] bytes)
        {
            const byte numVectorsExpected = 14;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];
            var decodedBlocks12 = new IList<T12>[encodedBlocks[11].Count];
            var decodedBlocks13 = new IList<T13>[encodedBlocks[12].Count];
            var decodedBlocks14 = new IList<T14>[encodedBlocks[13].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeNumericBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeNumericBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks14[i] = DecodeNumericBlock<T14>(encodedBlocks[13][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                        Item12 = decodedBlocks12[i][j],
                        Item13 = decodedBlocks13[i][j],
                        Item14 = decodedBlocks14[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 15;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];
            var arr12 = new T12[list.Count];
            var arr13 = new T13[list.Count];
            var arr14 = new T14[list.Count];
            var arr15 = new T15[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
                arr12[i] = tuple.Item12;
                arr13[i] = tuple.Item13;
                arr14[i] = tuple.Item14;
                arr15[i] = tuple.Item15;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11)args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);

                    encodedBlocks[11][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Levels[11],
                        granularity: (T12)args.Granularities[11],
                        monotonicity: args.Monotonicities[11]);

                    encodedBlocks[12][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Levels[12],
                        granularity: (T13)args.Granularities[12],
                        monotonicity: args.Monotonicities[12]);

                    encodedBlocks[13][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr14,
                        level: args.Levels[13],
                        granularity: (T14)args.Granularities[13],
                        monotonicity: args.Monotonicities[13]);

                    encodedBlocks[14][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr15,
                        level: args.Levels[14],
                        granularity: (T15) args.Granularities[14],
                        monotonicity: args.Monotonicities[14]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(byte[] bytes)
        {
            const byte numVectorsExpected = 15;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];
            var decodedBlocks12 = new IList<T12>[encodedBlocks[11].Count];
            var decodedBlocks13 = new IList<T13>[encodedBlocks[12].Count];
            var decodedBlocks14 = new IList<T14>[encodedBlocks[13].Count];
            var decodedBlocks15 = new IList<T15>[encodedBlocks[14].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeNumericBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeNumericBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks14[i] = DecodeNumericBlock<T14>(encodedBlocks[13][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks15[i] = DecodeNumericBlock<T15>(encodedBlocks[14][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                        Item12 = decodedBlocks12[i][j],
                        Item13 = decodedBlocks13[i][j],
                        Item14 = decodedBlocks14[i][j],
                        Item15 = decodedBlocks15[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 16;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];
            var arr12 = new T12[list.Count];
            var arr13 = new T13[list.Count];
            var arr14 = new T14[list.Count];
            var arr15 = new T15[list.Count];
            var arr16 = new T16[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
                arr12[i] = tuple.Item12;
                arr13[i] = tuple.Item13;
                arr14[i] = tuple.Item14;
                arr15[i] = tuple.Item15;
                arr16[i] = tuple.Item16;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11)args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);

                    encodedBlocks[11][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Levels[11],
                        granularity: (T12)args.Granularities[11],
                        monotonicity: args.Monotonicities[11]);

                    encodedBlocks[12][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Levels[12],
                        granularity: (T13)args.Granularities[12],
                        monotonicity: args.Monotonicities[12]);

                    encodedBlocks[13][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr14,
                        level: args.Levels[13],
                        granularity: (T14)args.Granularities[13],
                        monotonicity: args.Monotonicities[13]);

                    encodedBlocks[14][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr15,
                        level: args.Levels[14],
                        granularity: (T15)args.Granularities[14],
                        monotonicity: args.Monotonicities[14]);

                    encodedBlocks[15][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr16,
                        level: args.Levels[15],
                        granularity: (T16) args.Granularities[15],
                        monotonicity: args.Monotonicities[15]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(byte[] bytes)
        {
            const byte numVectorsExpected = 16;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];
            var decodedBlocks12 = new IList<T12>[encodedBlocks[11].Count];
            var decodedBlocks13 = new IList<T13>[encodedBlocks[12].Count];
            var decodedBlocks14 = new IList<T14>[encodedBlocks[13].Count];
            var decodedBlocks15 = new IList<T15>[encodedBlocks[14].Count];
            var decodedBlocks16 = new IList<T16>[encodedBlocks[15].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeNumericBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeNumericBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks14[i] = DecodeNumericBlock<T14>(encodedBlocks[13][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks15[i] = DecodeNumericBlock<T15>(encodedBlocks[14][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks16[i] = DecodeNumericBlock<T16>(encodedBlocks[15][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                        Item12 = decodedBlocks12[i][j],
                        Item13 = decodedBlocks13[i][j],
                        Item14 = decodedBlocks14[i][j],
                        Item15 = decodedBlocks15[i][j],
                        Item16 = decodedBlocks16[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15, T16>

        #region Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 17;
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
            var arr8 = new T8[list.Count];
            var arr9 = new T9[list.Count];
            var arr10 = new T10[list.Count];
            var arr11 = new T11[list.Count];
            var arr12 = new T12[list.Count];
            var arr13 = new T13[list.Count];
            var arr14 = new T14[list.Count];
            var arr15 = new T15[list.Count];
            var arr16 = new T16[list.Count];
            var arr17 = new T17[list.Count];

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
                arr8[i] = tuple.Item8;
                arr9[i] = tuple.Item9;
                arr10[i] = tuple.Item10;
                arr11[i] = tuple.Item11;
                arr12[i] = tuple.Item12;
                arr13[i] = tuple.Item13;
                arr14[i] = tuple.Item14;
                arr15[i] = tuple.Item15;
                arr16[i] = tuple.Item16;
                arr17[i] = tuple.Item17;
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
                        granularity: (T7)args.Granularities[6],
                        monotonicity: args.Monotonicities[6]);

                    encodedBlocks[7][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Levels[7],
                        granularity: (T8)args.Granularities[7],
                        monotonicity: args.Monotonicities[7]);

                    encodedBlocks[8][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Levels[8],
                        granularity: (T9)args.Granularities[8],
                        monotonicity: args.Monotonicities[8]);

                    encodedBlocks[9][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Levels[9],
                        granularity: (T10)args.Granularities[9],
                        monotonicity: args.Monotonicities[9]);

                    encodedBlocks[10][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Levels[10],
                        granularity: (T11)args.Granularities[10],
                        monotonicity: args.Monotonicities[10]);

                    encodedBlocks[11][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Levels[11],
                        granularity: (T12)args.Granularities[11],
                        monotonicity: args.Monotonicities[11]);

                    encodedBlocks[12][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Levels[12],
                        granularity: (T13)args.Granularities[12],
                        monotonicity: args.Monotonicities[12]);

                    encodedBlocks[13][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr14,
                        level: args.Levels[13],
                        granularity: (T14)args.Granularities[13],
                        monotonicity: args.Monotonicities[13]);

                    encodedBlocks[14][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr15,
                        level: args.Levels[14],
                        granularity: (T15)args.Granularities[14],
                        monotonicity: args.Monotonicities[14]);

                    encodedBlocks[15][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr16,
                        level: args.Levels[15],
                        granularity: (T16)args.Granularities[15],
                        monotonicity: args.Monotonicities[15]);

                    encodedBlocks[16][r] = EncodeNumericBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr17,
                        level: args.Levels[16],
                        granularity: (T17)args.Granularities[16],
                        monotonicity: args.Monotonicities[16]);
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

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>> DecodeStruple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(byte[] bytes)
        {
            const byte numVectorsExpected = 17;

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
            var decodedBlocks8 = new IList<T8>[encodedBlocks[7].Count];
            var decodedBlocks9 = new IList<T9>[encodedBlocks[8].Count];
            var decodedBlocks10 = new IList<T10>[encodedBlocks[9].Count];
            var decodedBlocks11 = new IList<T11>[encodedBlocks[10].Count];
            var decodedBlocks12 = new IList<T12>[encodedBlocks[11].Count];
            var decodedBlocks13 = new IList<T13>[encodedBlocks[12].Count];
            var decodedBlocks14 = new IList<T14>[encodedBlocks[13].Count];
            var decodedBlocks15 = new IList<T15>[encodedBlocks[14].Count];
            var decodedBlocks16 = new IList<T16>[encodedBlocks[15].Count];
            var decodedBlocks17 = new IList<T17>[encodedBlocks[16].Count];

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
                    decodedBlocks8[i] = DecodeNumericBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeNumericBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeNumericBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeNumericBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeNumericBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeNumericBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks14[i] = DecodeNumericBlock<T14>(encodedBlocks[13][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks15[i] = DecodeNumericBlock<T15>(encodedBlocks[14][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks16[i] = DecodeNumericBlock<T16>(encodedBlocks[15][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks17[i] = DecodeNumericBlock<T17>(encodedBlocks[16][i], DefaultFinisher, blockIndex: i);
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
            var list = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>[listCount];

            var k = 0;
            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    list[k++] = new Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
                    {
                        Item1 = decodedBlocks1[i][j],
                        Item2 = decodedBlocks2[i][j],
                        Item3 = decodedBlocks3[i][j],
                        Item4 = decodedBlocks4[i][j],
                        Item5 = decodedBlocks5[i][j],
                        Item6 = decodedBlocks6[i][j],
                        Item7 = decodedBlocks7[i][j],
                        Item8 = decodedBlocks8[i][j],
                        Item9 = decodedBlocks9[i][j],
                        Item10 = decodedBlocks10[i][j],
                        Item11 = decodedBlocks11[i][j],
                        Item12 = decodedBlocks12[i][j],
                        Item13 = decodedBlocks13[i][j],
                        Item14 = decodedBlocks14[i][j],
                        Item15 = decodedBlocks15[i][j],
                        Item16 = decodedBlocks16[i][j],
                        Item17 = decodedBlocks17[i][j],
                    };
                }
            }
            return list;
        }

        #endregion // Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15, T16, T17>

    }
}
