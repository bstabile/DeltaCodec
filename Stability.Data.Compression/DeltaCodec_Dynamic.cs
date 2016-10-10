using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        #region Dynamic<T>

        public virtual byte[] Encode<TObject, T>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 2;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T)properties[0].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();
                    properties[0].SetValue(instance, decodedBlocks1[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T>
        
        #region Dynamic<T1, T2>

        public virtual byte[] Encode<TObject, T1, T2>(DynamicEncodingArgs<TObject> args) 
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 2;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2>

        #region Dynamic<T1, T2, T3>

        public virtual byte[] Encode<TObject, T1, T2, T3>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 3;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3>

        #region Dynamic<T1, T2, T3, T4>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 4;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4>

        #region Dynamic<T1, T2, T3, T4, T5>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 5;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];
            var arr5 = new T5[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);

                    list.Add(instance);

                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5>

        #region Dynamic<T1, T2, T3, T4, T5, T6>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 6;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];
            var arr5 = new T5[list.Count];
            var arr6 = new T6[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);

                    list.Add(instance);

                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 7;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

            var arr1 = new T1[list.Count];
            var arr2 = new T2[list.Count];
            var arr3 = new T3[list.Count];
            var arr4 = new T4[list.Count];
            var arr5 = new T5[list.Count];
            var arr6 = new T6[list.Count];
            var arr7 = new T7[list.Count];

            Parallel.For(0, list.Count, i =>
            {
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 8;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
            var numBlocks = args.NumBlocks;
            FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 9;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 10;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 11;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 12;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
                arr12[i] = (T12)properties[11].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);
                    properties[11].SetValue(instance, decodedBlocks12[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 13;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
                arr12[i] = (T12)properties[11].GetValue(list[i]);
                arr13[i] = (T13)properties[12].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);
                    properties[11].SetValue(instance, decodedBlocks12[i][j]);
                    properties[12].SetValue(instance, decodedBlocks13[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 14;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
                arr12[i] = (T12)properties[11].GetValue(list[i]);
                arr13[i] = (T13)properties[12].GetValue(list[i]);
                arr14[i] = (T14)properties[13].GetValue(list[i]);
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
                        granularity: (T14)args.Granularities[13],
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);
                    properties[11].SetValue(instance, decodedBlocks12[i][j]);
                    properties[12].SetValue(instance, decodedBlocks13[i][j]);
                    properties[13].SetValue(instance, decodedBlocks14[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 15;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
                arr12[i] = (T12)properties[11].GetValue(list[i]);
                arr13[i] = (T13)properties[12].GetValue(list[i]);
                arr14[i] = (T14)properties[13].GetValue(list[i]);
                arr15[i] = (T15)properties[14].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);
                    properties[11].SetValue(instance, decodedBlocks12[i][j]);
                    properties[12].SetValue(instance, decodedBlocks13[i][j]);
                    properties[13].SetValue(instance, decodedBlocks14[i][j]);
                    properties[14].SetValue(instance, decodedBlocks15[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 16;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
                arr12[i] = (T12)properties[11].GetValue(list[i]);
                arr13[i] = (T13)properties[12].GetValue(list[i]);
                arr14[i] = (T14)properties[13].GetValue(list[i]);
                arr15[i] = (T15)properties[14].GetValue(list[i]);
                arr16[i] = (T16)properties[15].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);
                    properties[11].SetValue(instance, decodedBlocks12[i][j]);
                    properties[12].SetValue(instance, decodedBlocks13[i][j]);
                    properties[13].SetValue(instance, decodedBlocks14[i][j]);
                    properties[14].SetValue(instance, decodedBlocks15[i][j]);
                    properties[15].SetValue(instance, decodedBlocks16[i][j]);

                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15, T16>

        #region Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>

        public virtual byte[] Encode<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(DynamicEncodingArgs<TObject> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Data == null)
                throw new ArgumentException("The args.List property is null.", "args");

            const byte numVectors = 17;
            const ushort shortFlags = 0; // Reserved for future use

            var list = args.Data;
           var numBlocks = args.NumBlocks; FieldInfo[] properties = typeof(TObject).GetFields();

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
                arr1[i] = (T1)properties[0].GetValue(list[i]);
                arr2[i] = (T2)properties[1].GetValue(list[i]);
                arr3[i] = (T3)properties[2].GetValue(list[i]);
                arr4[i] = (T4)properties[3].GetValue(list[i]);
                arr5[i] = (T5)properties[4].GetValue(list[i]);
                arr6[i] = (T6)properties[5].GetValue(list[i]);
                arr7[i] = (T7)properties[6].GetValue(list[i]);
                arr8[i] = (T8)properties[7].GetValue(list[i]);
                arr9[i] = (T9)properties[8].GetValue(list[i]);
                arr10[i] = (T10)properties[9].GetValue(list[i]);
                arr11[i] = (T11)properties[10].GetValue(list[i]);
                arr12[i] = (T12)properties[11].GetValue(list[i]);
                arr13[i] = (T13)properties[12].GetValue(list[i]);
                arr14[i] = (T14)properties[13].GetValue(list[i]);
                arr15[i] = (T15)properties[14].GetValue(list[i]);
                arr16[i] = (T16)properties[15].GetValue(list[i]);
                arr17[i] = (T17)properties[16].GetValue(list[i]);
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

        public virtual IList<TObject> DecodeDynamic<TObject, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(byte[] bytes)
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
            IList<TObject> list = new List<TObject>(listCount);

            Type type = typeof(TObject);
            FieldInfo[] properties = typeof(TObject).GetFields();

            for (var i = 0; i < decodedBlocks1.Length; i++)
            {
                for (var j = 0; j < decodedBlocks1[i].Count; j++)
                {
                    TObject instance = Activator.CreateInstance<TObject>();

                    properties[0].SetValue(instance, decodedBlocks1[i][j]);
                    properties[1].SetValue(instance, decodedBlocks2[i][j]);
                    properties[2].SetValue(instance, decodedBlocks3[i][j]);
                    properties[3].SetValue(instance, decodedBlocks4[i][j]);
                    properties[4].SetValue(instance, decodedBlocks5[i][j]);
                    properties[5].SetValue(instance, decodedBlocks6[i][j]);
                    properties[6].SetValue(instance, decodedBlocks7[i][j]);
                    properties[7].SetValue(instance, decodedBlocks8[i][j]);
                    properties[8].SetValue(instance, decodedBlocks9[i][j]);
                    properties[9].SetValue(instance, decodedBlocks10[i][j]);
                    properties[10].SetValue(instance, decodedBlocks11[i][j]);
                    properties[11].SetValue(instance, decodedBlocks12[i][j]);
                    properties[12].SetValue(instance, decodedBlocks13[i][j]);
                    properties[13].SetValue(instance, decodedBlocks14[i][j]);
                    properties[14].SetValue(instance, decodedBlocks15[i][j]);
                    properties[15].SetValue(instance, decodedBlocks16[i][j]);
                    properties[16].SetValue(instance, decodedBlocks17[i][j]);


                    list.Add(instance);
                }
            }
            return list;
        }

        #endregion // Dynamic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13. T14, T15, T16, T17>

    }

}