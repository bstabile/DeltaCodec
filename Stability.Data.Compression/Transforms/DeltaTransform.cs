#region License

// Namespace : Stability.Data.Compression.Transforms
// FileName  : DeltaTransform.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.Transforms
{
    /// <summary>
    /// This is a basic transform that simply differences the series and then 
    /// feeds the differenced series to whatever <see cref="IFinisher"/> has been
    /// provided. The abstract base finisher will write the data to a stream,
    /// and the concrete implementation of the finisher will then compress the 
    /// stream using a specific algorithm.
    /// </summary>
    public class DeltaTransform : IDeltaTransform
    {
        #region Private Function Map

        private readonly IDictionary<Type, Action<object>> _encodeFuncMap;

        private readonly IDictionary<Type, Action<object>> _decodeFuncMap;

        #endregion // Private Function Map

        public DeltaTransform()
        {
            _encodeFuncMap = new Dictionary
                <Type, Action<object>>
            {
                {typeof (bool), o => Encode((DeltaBlockState<bool>) o)},
                {typeof (byte), o => Encode((DeltaBlockState<byte>) o)},
                {typeof (sbyte), o => Encode((DeltaBlockState<sbyte>) o)},
                {typeof (ushort), o => Encode((DeltaBlockState<ushort>) o)},
                {typeof (short), o => Encode((DeltaBlockState<short>) o)},
                {typeof (uint), o => Encode((DeltaBlockState<uint>) o)},
                {typeof (int), o => Encode((DeltaBlockState<int>) o)},
                {typeof (ulong), o => Encode((DeltaBlockState<ulong>) o)},
                {typeof (long), o => Encode((DeltaBlockState<long>) o)},
                // DateTime and TimeSpan
                {typeof (DateTimeOffset), o => Encode((DeltaBlockState<DateTimeOffset>) o)},
                {typeof (DateTime), o => Encode((DeltaBlockState<DateTime>) o)},
                {typeof (TimeSpan), o => Encode((DeltaBlockState<TimeSpan>) o)},
                // Reals
                {typeof (float), o => Encode((DeltaBlockState<float>) o)},
                {typeof (double), o => Encode((DeltaBlockState<double>) o)},
                {typeof (decimal), o => Encode((DeltaBlockState<decimal>) o)},
            };
            _decodeFuncMap = new Dictionary
                <Type, Action<object>>
            {
                {typeof (bool), o => Decode((DeltaBlockState<bool>) o)},
                {typeof (byte), o => Decode((DeltaBlockState<byte>) o)},
                {typeof (sbyte), o => Decode((DeltaBlockState<sbyte>) o)},
                {typeof (ushort), o => Decode((DeltaBlockState<ushort>) o)},
                {typeof (short), o => Decode((DeltaBlockState<short>) o)},
                {typeof (uint), o => Decode((DeltaBlockState<uint>) o)},
                {typeof (int), o => Decode((DeltaBlockState<int>) o)},
                {typeof (ulong), o => Decode((DeltaBlockState<ulong>) o)},
                {typeof (long), o => Decode((DeltaBlockState<long>) o)},
                // DateTime and TimeSpan
                {typeof (DateTimeOffset), o => Decode((DeltaBlockState<DateTimeOffset>) o)},
                {typeof (DateTime), o => Decode((DeltaBlockState<DateTime>) o)},
                {typeof (TimeSpan), o => Decode((DeltaBlockState<TimeSpan>) o)},
                // Reals
                {typeof (float), o => Decode((DeltaBlockState<float>) o)},
                {typeof (double), o => Decode((DeltaBlockState<double>) o)},
                {typeof (decimal), o => Decode((DeltaBlockState<decimal>) o)},
            };
        }

        public static readonly DeltaTransform Instance = new DeltaTransform();

        #region Generic Encode<T> and Decode<T>

        /// <summary>
        /// This is actually a "Fake" generic version of the Transpose method.
        /// </summary>
        public virtual void Encode<T>(DeltaBlockState<T> state)
            where T : struct
        {
            try
            {
                _encodeFuncMap[typeof(T)].Invoke(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// This is actually a "Fake" generic version of the Transpose method.
        /// </summary>
        public virtual void Decode<T>(DeltaBlockState<T> state)
            where T : struct
        {
            try
            {
                _decodeFuncMap[typeof(T)].Invoke(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion // Generic Encode<T> and Decode<T>

        #region DateTimeOffset

        public virtual void Encode(DeltaBlockState<DateTimeOffset> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = state.Factor.HasValue ? state.Factor.Value.DateTime : new DateTime(1);

            var dateTimes = new DateTime[list.Count];
            var offsets = new short[list.Count];

            var baseOffset = (short)(list[0].Offset.Ticks / TimeSpan.TicksPerMinute);
            var allSameOffsets = true;
            Parallel.For(0, list.Count, i =>
            {
                dateTimes[i] = list[i].DateTime;
                var offset = (short)(list[i].Offset.Ticks/TimeSpan.TicksPerMinute);
                offsets[i] = offset;
                if (allSameOffsets && offset != baseOffset)
                    allSameOffsets = false;
            });

            using (var ms = new MemoryStream())
            {
                var writer = new BinaryWriter(ms);

                var stateD = new DeltaBlockState<DateTime>(dateTimes, CompressionLevel.NoCompression, 
                    state.Factor.Value.DateTime, Monotonicity.None, state.Finisher, state.BlockIndex);
                Encode(stateD);

                writer.Write(stateD.Bytes.Length);
                writer.Write(stateD.Bytes);

                writer.Write(allSameOffsets);
                if (!allSameOffsets)
                {
                    var stateO = new DeltaBlockState<short>(offsets, CompressionLevel.NoCompression, 1,
                        Monotonicity.None, state.Finisher, state.BlockIndex);
                    Encode(stateO);
                    writer.Write(stateO.Bytes.Length);
                    writer.Write(stateO.Bytes);
                }
                ms.Position = 0;
                state.Bytes = state.Finisher.EncodeToStream(ms, state.Flags.Level).ToArray();
                state.ByteCount = state.Bytes.Length;
            }
        }

        public virtual void Decode(DeltaBlockState<DateTimeOffset> state)
        {
            state.Factor = state.Factor ?? new DateTime(1);
            var baseOffset = (short)(state.Anchor.Offset.Ticks/TimeSpan.TicksPerMinute);

            var bytes = (byte[]) state.Finisher.DecodeByte(state.Bytes);
            using (var ms = new MemoryStream(bytes))
            {
                var reader = new BinaryReader(ms);

                var bytesD = reader.ReadBytes(reader.ReadInt32());
                var stateD = new DeltaBlockState<DateTime>(bytesD)
                {
                    ListCount = state.ListCount,
                    Anchor = state.Anchor.DateTime,
                    Factor = state.Factor.Value.DateTime,
                    Finisher = state.Finisher,
                    BlockIndex = state.BlockIndex,
                    Flags = { Level = CompressionLevel.NoCompression, Monotonicity = Monotonicity.None }
                };
                Decode(stateD);
                var dateTimes = stateD.List;

                var allSameOffsets = reader.ReadBoolean();

                IList<short> offsets = null;
                if (!allSameOffsets)
                {
                    var bytesO = reader.ReadBytes(reader.ReadInt32());
                    var stateO = new DeltaBlockState<short>(bytesO)
                    {
                        ListCount = state.ListCount,
                        Anchor = (short)(state.Anchor.Offset.Ticks / TimeSpan.TicksPerMinute),
                        Factor = 1,
                        Finisher = state.Finisher,
                        BlockIndex = state.BlockIndex,
                        Flags = { Level = CompressionLevel.NoCompression, Monotonicity = Monotonicity.None }
                    };
                    Decode(stateO);
                    offsets = stateO.List;
                }

                var dtos = new DateTimeOffset[state.ListCount];
                for (var i = 0; i < state.ListCount; i++)
                {
                    var offset = allSameOffsets ? baseOffset : offsets[i];
                    dtos[i] = new DateTimeOffset(dateTimes[i], new TimeSpan(offset * TimeSpan.TicksPerMinute));
                }
                state.List = dtos;
            }
        }

        #endregion // DateTimeOffset

        #region DateTime

        public virtual void Encode(DeltaBlockState<DateTime> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var rwcBits = new BitsDT(0);

            rwcBits.Long = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            if (rwcBits.Long == 0)
            {
                rwcBits.Value = DeltaUtility.Factor(list);
            }
            state.Factor = rwcBits.Value;
            var factor = rwcBits.Long;
            var hasFactor = factor != 0 && factor != 1;

             var diffs = new long[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1].Ticks;
                var curr = list[i].Ticks;
                var diff = curr - last;
                diffs[i - 1] = hasFactor ? diff/factor : diff;
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<DateTime> state)
        {
            var diffs = state.Finisher.DecodeInt64(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            state.Factor = new DateTime(factor);
            var hasFactor = factor != 0 && factor != 1;

            var arr = new DateTime[listCount];
            arr[0] = state.Anchor;

            var last = arr[0].Ticks;
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = last + curr;
                arr[i] = new DateTime(last);
            }
            state.List = arr;
        }

        #endregion // DateTime
        
        #region TimeSpan

        public virtual void Encode(DeltaBlockState<TimeSpan> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var rwcBits = new BitsTS(0);
            rwcBits.Long = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            if (rwcBits.Long == 0)
            {
                rwcBits.Value = DeltaUtility.Factor(list);
            }
            state.Factor = rwcBits.Value;
            var factor = rwcBits.Long;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new long[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1].Ticks;
                var curr = list[i].Ticks;
                var diff = curr - last;
                diffs[i - 1] = hasFactor ? diff / factor : diff;
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<TimeSpan> state)
        {
            var diffs = state.Finisher.DecodeInt64(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            state.Factor = new TimeSpan(factor);
            var hasFactor = factor != 0 && factor != 1;

            var arr = new TimeSpan[listCount];
            arr[0] = state.Anchor;

            var last = arr[0].Ticks;
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = last + curr;
                arr[i] = new TimeSpan(last);
            }
            state.List = arr;
        }

        #endregion // TimeSpan
        
        #region Int64

        public virtual void Encode(DeltaBlockState<long> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new long[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = curr - last;
                diffs[i - 1] = hasFactor ? diff / factor : diff;
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<long> state)
        {
            var diffs = state.Finisher.DecodeInt64(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new long[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = last + curr;
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // Int64
        
        #region UInt64

        public virtual void Encode(DeltaBlockState<ulong> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new ulong[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = (long)(curr - last);
                diffs[i - 1] = (ulong)(hasFactor ? diff / (long)factor : diff);
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<ulong> state)
        {
            var diffs = state.Finisher.DecodeUInt64(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new ulong[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = last + curr;
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // UInt64
        
        #region Int32

        public virtual void Encode(DeltaBlockState<int> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new int[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = curr - last;
                diffs[i - 1] = hasFactor ? diff / factor : diff;
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<int> state)
        {
            var diffs = state.Finisher.DecodeInt32(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new int[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = last + curr;
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // Int32
        
        #region UInt32

        public virtual void Encode(DeltaBlockState<uint> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new uint[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = (int) list[i - 1];
                var curr = (int) list[i];
                var diff = curr - last;
                diffs[i - 1] = (uint)(hasFactor ? diff / factor : diff);
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<uint> state)
        {
            var diffs = state.Finisher.DecodeUInt32(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new uint[listCount];
            arr[0] = state.Anchor;

            var last = (int) arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = (int) (hasFactor ? diffs[i - 1] * factor : diffs[i - 1]);
                last = last + curr;
                arr[i] = (uint) last;
            }
            state.List = arr;
        }

        #endregion // UInt32

        #region Int16

        public virtual void Encode(DeltaBlockState<short> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new short[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = (short)(curr - last);
                diffs[i - 1] = (hasFactor ? (short)(diff / factor) : diff);
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<short> state)
        {
            var diffs = state.Finisher.DecodeInt16(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new short[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = (short)(last + curr);
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // Int16
        
        #region UInt16

        public virtual void Encode(DeltaBlockState<ushort> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new ushort[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = (short) list[i - 1];
                var curr = (short) list[i];
                var diff = (short)(curr - last);
                diffs[i - 1] = (ushort)(hasFactor ? (diff / factor) : diff);
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<ushort> state)
        {
            var diffs = state.Finisher.DecodeUInt16(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1; // Greatest Common Divisor
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new ushort[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = (ushort)(last + curr);
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // UInt16

        #region SByte

        public virtual void Encode(DeltaBlockState<sbyte> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new sbyte[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = (sbyte)(curr - last);
                diffs[i - 1] = (hasFactor ? (sbyte)(diff / factor) : diff);
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<sbyte> state)
        {
            var diffs = state.Finisher.DecodeSByte(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1; // Greatest Common Divisor
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new sbyte[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = (sbyte)(last + curr);
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // SByte

        #region Byte

        public virtual void Encode(DeltaBlockState<byte> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new byte[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = (byte)(curr - last);
                diffs[i - 1] = (hasFactor ? (byte)(diff / factor) : diff);
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<byte> state)
        {
            var diffs = state.Finisher.DecodeByte(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var factor = state.Factor ?? 1; // Greatest Common Divisor
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new byte[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = (byte)(last + curr);
                arr[i] = last;
            }
            state.List = arr;
        }

        #endregion // Byte

        #region Boolean

        public virtual void Encode(DeltaBlockState<bool> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var capacity = (int) Math.Ceiling(list.Count/8.0);
            using (var ms = new MemoryStream(capacity))
            {
                var bw = new BitWriter(ms);
                for (var i = 0; i < list.Count; i++)
                {
                    bw.WriteBit(list[i] ? 1U : 0U);
                }
                state.Bytes = state.Finisher.EncodeToStream(ms, state.Flags.Level).ToArray();
                state.ByteCount = state.Bytes.Length;
            }
        }

        public virtual void Decode(DeltaBlockState<bool> state)
        {
            var bytes = (byte[]) state.Finisher.DecodeByte(state.Bytes);
            var listCount = state.ListCount; // Number of values

            var arr = new bool[listCount];
            using (var ms = new MemoryStream(bytes))
            {
                var reader = new BitReader(ms);
                for (var i = 0; i < listCount; i++)
                {
                    arr[i] = reader.ReadBit() != 0;
                }
            }
            state.List = arr;
        }

        #endregion // Boolean

        #region Float

        public virtual void Encode(DeltaBlockState<float> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                var rwcBits = new BitsF(list[0]);
                var last = rwcBits.Int;
                for (var i = 1; i < list.Count; i++)
                {
                    rwcBits.Value = list[i];
                    var curr = rwcBits.Int;
                    var diff = curr - last;
                    writer.Write(diff);
                    last = curr;
                }
                state.Bytes = state.Finisher.Encode(stream, state.Flags.Level);
                state.ByteCount = state.Bytes.Length;
            }
        }

        public virtual void Decode(DeltaBlockState<float> state)
        {
            var stream = state.Finisher.DecodeToStream(state.Bytes);
            var listCount = state.ListCount; // Number of values

            using (stream)
            {
                var reader = new BinaryReader(stream);

                var arr = new float[listCount];
                arr[0] = state.Anchor;

                var rwcBits = new BitsF(arr[0]);
                var last = rwcBits.Int; // First value
                for (var i = 1; i < listCount; i++)
                {
                    last = reader.ReadInt32() + last;
                    rwcBits.Int = last;
                    arr[i] = rwcBits.Value;
                }
                state.List = arr;
            }
        }

        #endregion // Float
        
        #region Double

        public virtual void Encode(DeltaBlockState<double> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                var rwcBits = new BitsD(list[0]);
                var last = rwcBits.Long;
                for (var i = 1; i < list.Count; i++)
                {
                    rwcBits.Value = list[i];
                    var curr = rwcBits.Long;
                    var diff = curr - last;
                    writer.Write(diff);
                    last = curr;
                }
                state.Bytes = state.Finisher.Encode(stream, state.Flags.Level);
                state.ByteCount = state.Bytes.Length;
            }
        }

        public virtual void Decode(DeltaBlockState<double> state)
        {
            var stream = state.Finisher.DecodeToStream(state.Bytes);
            var listCount = state.ListCount; // Number of values

            using (stream)
            {
                var reader = new BinaryReader(stream);

                var arr = new double[listCount];
                arr[0] = state.Anchor;

                var rwcBits = new BitsD(arr[0]);
                var last = rwcBits.Long; // First value
                for (var i = 1; i < listCount; i++)
                {
                    last = reader.ReadInt64() + last;
                    rwcBits.Long = last;
                    arr[i] = rwcBits.Value;
                }
                state.List = arr;
            }
        }

        #endregion // Double

        #region Decimal

        public virtual void Encode(DeltaBlockState<decimal> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0)
            {
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var diffs = new decimal[state.ListCount - 1];
            Parallel.For(1, state.ListCount, i =>
            {
                var last = list[i - 1];
                var curr = list[i];
                var diff = curr - last;
                diffs[i - 1] = hasFactor ? diff / factor : diff;
            });

            state.Bytes = state.Finisher.Encode(diffs, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<decimal> state)
        {
            var diffs = state.Finisher.DecodeDecimal(state.Bytes);
            var listCount = state.ListCount; // Number of value

            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new decimal[listCount];
            arr[0] = state.Anchor;

            var last = arr[0];
            for (var i = 1; i < state.ListCount; i++)
            {
                var curr = hasFactor ? diffs[i - 1] * factor : diffs[i - 1];
                last = last + curr;
                arr[i] = last;
            }
            state.List = arr;
        }

        public virtual void Encode2(DeltaBlockState<decimal> state)
        {
            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];

            var factor = state.Factor ?? 1;
            if (factor == 0 && state.Flags.Level == CompressionLevel.Optimal)
            {
                // This is time consuming, so we only do it if the best compression is desired!
                factor = DeltaUtility.Factor(list);
            }
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                var last = hasFactor ? list[0] / factor : list[0];
                for (var i = 1; i < list.Count; i++)
                {
                    var curr = hasFactor ? list[i] / factor : list[i];
                    var diff = curr - last;
                    last = curr;
                    writer.Write(diff);
                }
                state.Bytes = state.Finisher.Encode(stream, state.Flags.Level);
                state.ByteCount = state.Bytes.Length;
            }
        }

        public virtual void Decode2(DeltaBlockState<decimal> state)
        {
            var listCount = state.ListCount;
            var factor = state.Factor ?? 1;
            state.Factor = factor;
            var hasFactor = factor != 0 && factor != 1;

            var arr = new decimal[listCount];
            arr[0] = state.Anchor;

            using (var stream = state.Finisher.DecodeToStream(state.Bytes))
            {
                var reader = new BinaryReader(stream);
                var last = arr[0];
                for (var i = 1; i < listCount; i++)
                {
                    var curr = reader.ReadDecimal();
                    arr[i] = (hasFactor ? curr * factor : curr) + last;
                    last = arr[i];
                }
            }

            state.List = arr;
            state.ListCount = arr.Length;
        }

        #endregion // Decimal
    }
}
