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
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression
{
    public abstract partial class DeltaCodec<TTransform>
    {
        #region Tuple<T1, T2>

        byte[] IDeltaCodec.Encode<T1, T2>(StrupleEncodingArgs<T1, T2> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2>(StrupleEncodingArgs<T1, T2> args)
            where T1 : struct
            where T2 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 2;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);
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

        IList<Struple<T1, T2>> IDeltaCodec.Decode<T1, T2>(byte[] bytes)
        {
            return Decode<T1, T2>(bytes);
        }

        public virtual IList<Struple<T1, T2>> Decode<T1, T2>(byte[] bytes)
            where T1 : struct
            where T2 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2>

        #region Tuple<T1, T2, T3>

        byte[] IDeltaCodec.Encode<T1, T2, T3>(StrupleEncodingArgs<T1, T2, T3> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3>(StrupleEncodingArgs<T1, T2, T3> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 3;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);
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

        IList<Struple<T1, T2, T3>> IDeltaCodec.Decode<T1, T2, T3>(byte[] bytes)
        {
            return Decode<T1, T2, T3>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3>> Decode<T1, T2, T3>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3>

        #region Tuple<T1, T2, T3, T4>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4>(StrupleEncodingArgs<T1, T2, T3, T4> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4>(StrupleEncodingArgs<T1, T2, T3, T4> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 4;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);
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

        IList<Struple<T1, T2, T3, T4>> IDeltaCodec.Decode<T1, T2, T3, T4>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4>> Decode<T1, T2, T3, T4>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4>

        #region Tuple<T1, T2, T3, T4, T5>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5>(StrupleEncodingArgs<T1, T2, T3, T4, T5> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5>(StrupleEncodingArgs<T1, T2, T3, T4, T5> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 5;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);
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

        IList<Struple<T1, T2, T3, T4, T5>> IDeltaCodec.Decode<T1, T2, T3, T4, T5>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5>> Decode<T1, T2, T3, T4, T5>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5>

        #region Tuple<T1, T2, T3, T4, T5, T6>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 6;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);
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

        IList<Struple<T1, T2, T3, T4, T5, T6>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6>> Decode<T1, T2, T3, T4, T5, T6>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 7;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);
                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7>> Decode<T1, T2, T3, T4, T5, T6, T7>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 8;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8>> Decode<T1, T2, T3, T4, T5, T6, T7, T8>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 9;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 10;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);

                    encodedBlocks[9][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Level10,
                        granularity: args.Granularity10,
                        monotonicity: args.Monotonicity10);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 11;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);

                    encodedBlocks[9][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Level10,
                        granularity: args.Granularity10,
                        monotonicity: args.Monotonicity10);

                    encodedBlocks[10][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Level11,
                        granularity: args.Granularity11,
                        monotonicity: args.Monotonicity11);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 12;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);

                    encodedBlocks[9][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Level10,
                        granularity: args.Granularity10,
                        monotonicity: args.Monotonicity10);

                    encodedBlocks[10][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Level11,
                        granularity: args.Granularity11,
                        monotonicity: args.Monotonicity11);

                    encodedBlocks[11][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Level12,
                        granularity: args.Granularity12,
                        monotonicity: args.Monotonicity12);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 13;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);

                    encodedBlocks[9][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Level10,
                        granularity: args.Granularity10,
                        monotonicity: args.Monotonicity10);

                    encodedBlocks[10][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Level11,
                        granularity: args.Granularity11,
                        monotonicity: args.Monotonicity11);

                    encodedBlocks[11][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Level12,
                        granularity: args.Granularity12,
                        monotonicity: args.Monotonicity12);

                    encodedBlocks[12][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Level13,
                        granularity: args.Granularity13,
                        monotonicity: args.Monotonicity13);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 14;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);

                    encodedBlocks[9][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Level10,
                        granularity: args.Granularity10,
                        monotonicity: args.Monotonicity10);

                    encodedBlocks[10][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Level11,
                        granularity: args.Granularity11,
                        monotonicity: args.Monotonicity11);

                    encodedBlocks[11][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Level12,
                        granularity: args.Granularity12,
                        monotonicity: args.Monotonicity12);

                    encodedBlocks[12][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Level13,
                        granularity: args.Granularity13,
                        monotonicity: args.Monotonicity13);

                    encodedBlocks[13][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr14,
                        level: args.Level14,
                        granularity: args.Granularity14,
                        monotonicity: args.Monotonicity14);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks14[i] = DecodeBlock<T14>(encodedBlocks[13][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14>

        #region Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>

        byte[] IDeltaCodec.Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> args)
        {
            return Encode(args);
        }

        public virtual byte[] Encode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(StrupleEncodingArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> args)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct
            where T15 : struct
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.List == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 15;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.List;
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
                    encodedBlocks[0][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr1,
                        level: args.Level1,
                        granularity: args.Granularity1,
                        monotonicity: args.Monotonicity1);

                    encodedBlocks[1][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr2,
                        level: args.Level2,
                        granularity: args.Granularity2,
                        monotonicity: args.Monotonicity2);

                    encodedBlocks[2][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr3,
                        level: args.Level3,
                        granularity: args.Granularity3,
                        monotonicity: args.Monotonicity3);

                    encodedBlocks[3][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr4,
                        level: args.Level4,
                        granularity: args.Granularity4,
                        monotonicity: args.Monotonicity4);

                    encodedBlocks[4][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr5,
                        level: args.Level5,
                        granularity: args.Granularity5,
                        monotonicity: args.Monotonicity5);

                    encodedBlocks[5][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr6,
                        level: args.Level6,
                        granularity: args.Granularity6,
                        monotonicity: args.Monotonicity6);

                    encodedBlocks[6][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr7,
                        level: args.Level7,
                        granularity: args.Granularity7,
                        monotonicity: args.Monotonicity7);

                    encodedBlocks[7][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr8,
                        level: args.Level8,
                        granularity: args.Granularity8,
                        monotonicity: args.Monotonicity8);

                    encodedBlocks[8][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr9,
                        level: args.Level9,
                        granularity: args.Granularity9,
                        monotonicity: args.Monotonicity9);

                    encodedBlocks[9][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr10,
                        level: args.Level10,
                        granularity: args.Granularity10,
                        monotonicity: args.Monotonicity10);

                    encodedBlocks[10][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr11,
                        level: args.Level11,
                        granularity: args.Granularity11,
                        monotonicity: args.Monotonicity11);

                    encodedBlocks[11][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr12,
                        level: args.Level12,
                        granularity: args.Granularity12,
                        monotonicity: args.Monotonicity12);

                    encodedBlocks[12][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr13,
                        level: args.Level13,
                        granularity: args.Granularity13,
                        monotonicity: args.Monotonicity13);

                    encodedBlocks[13][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr14,
                        level: args.Level14,
                        granularity: args.Granularity14,
                        monotonicity: args.Monotonicity14);

                    encodedBlocks[14][r] = EncodeBlock(
                        blockIndex: r,
                        range: ranges[r],
                        list: arr15,
                        level: args.Level15,
                        granularity: args.Granularity15,
                        monotonicity: args.Monotonicity15);
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

        IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> IDeltaCodec.Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(byte[] bytes)
        {
            return Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(bytes);
        }

        public virtual IList<Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(byte[] bytes)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct
            where T9 : struct
            where T10 : struct
            where T11 : struct
            where T12 : struct
            where T13 : struct
            where T14 : struct
            where T15 : struct
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
                    decodedBlocks1[i] = DecodeBlock<T1>(encodedBlocks[0][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks2[i] = DecodeBlock<T2>(encodedBlocks[1][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks3[i] = DecodeBlock<T3>(encodedBlocks[2][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks4[i] = DecodeBlock<T4>(encodedBlocks[3][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks5[i] = DecodeBlock<T5>(encodedBlocks[4][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks6[i] = DecodeBlock<T6>(encodedBlocks[5][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks7[i] = DecodeBlock<T7>(encodedBlocks[6][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks8[i] = DecodeBlock<T8>(encodedBlocks[7][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks9[i] = DecodeBlock<T9>(encodedBlocks[8][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks10[i] = DecodeBlock<T10>(encodedBlocks[9][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks11[i] = DecodeBlock<T11>(encodedBlocks[10][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks12[i] = DecodeBlock<T12>(encodedBlocks[11][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks13[i] = DecodeBlock<T13>(encodedBlocks[12][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks14[i] = DecodeBlock<T14>(encodedBlocks[13][i], DefaultFinisher, blockIndex: i);
                    decodedBlocks15[i] = DecodeBlock<T15>(encodedBlocks[14][i], DefaultFinisher, blockIndex: i);
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

        #endregion // Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15>

    }
}
