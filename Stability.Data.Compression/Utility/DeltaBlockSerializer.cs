#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : DeltaBlockSerializer.cs
// Created   : 2015-4-24
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace Stability.Data.Compression.Utility
{
    public static class DeltaBlockSerializer
    {
        #region Private Static Function Map

        private static readonly IDictionary<Type, Func<object, byte[]>> SerializeFuncMap = new Dictionary
            <Type, Func<object, byte[]>>
        {
            {typeof (bool), o => Serialize((DeltaBlockState<bool>) o)},
            {typeof (byte), o => Serialize((DeltaBlockState<byte>) o)},
            {typeof (sbyte), o => Serialize((DeltaBlockState<sbyte>) o)},
            {typeof (ushort), o => Serialize((DeltaBlockState<ushort>) o)},
            {typeof (short), o => Serialize((DeltaBlockState<short>) o)},
            {typeof (uint), o => Serialize((DeltaBlockState<uint>) o)},
            {typeof (int), o => Serialize((DeltaBlockState<int>) o)},
            {typeof (ulong), o => Serialize((DeltaBlockState<ulong>) o)},
            {typeof (long), o => Serialize((DeltaBlockState<long>) o)},
            // DateTime and TimeSpan
            {typeof (DateTimeOffset), o => Serialize((DeltaBlockState<DateTimeOffset>) o)},
            {typeof (DateTime), o => Serialize((DeltaBlockState<DateTime>) o)},
            {typeof (TimeSpan), o => Serialize((DeltaBlockState<TimeSpan>) o)},
            // Reals
            {typeof (float), o => Serialize((DeltaBlockState<float>) o)},
            {typeof (double), o => Serialize((DeltaBlockState<double>) o)},
            {typeof (decimal), o => Serialize((DeltaBlockState<decimal>) o)},
        };

        private static readonly IDictionary<Type, Action<object>> DeserializeFuncMap = new Dictionary
            <Type, Action<object>>
        {
            {typeof (bool), o => Deserialize((DeltaBlockState<bool>) o)},
            {typeof (byte), o => Deserialize((DeltaBlockState<byte>) o)},
            {typeof (sbyte), o => Deserialize((DeltaBlockState<sbyte>) o)},
            {typeof (ushort), o => Deserialize((DeltaBlockState<ushort>) o)},
            {typeof (short), o => Deserialize((DeltaBlockState<short>) o)},
            {typeof (uint), o => Deserialize((DeltaBlockState<uint>) o)},
            {typeof (int), o => Deserialize((DeltaBlockState<int>) o)},
            {typeof (ulong), o => Deserialize((DeltaBlockState<ulong>) o)},
            {typeof (long), o => Deserialize((DeltaBlockState<long>) o)},
            // DateTime and TimeSpan
            {typeof (DateTimeOffset), o => Deserialize((DeltaBlockState<DateTimeOffset>) o)},
            {typeof (DateTime), o => Deserialize((DeltaBlockState<DateTime>) o)},
            {typeof (TimeSpan), o => Deserialize((DeltaBlockState<TimeSpan>) o)},
            // Reals
            {typeof (float), o => Deserialize((DeltaBlockState<float>) o)},
            {typeof (double), o => Deserialize((DeltaBlockState<double>) o)},
            {typeof (decimal), o => Deserialize((DeltaBlockState<decimal>) o)},
        };

        #endregion // Private Static Function Map

        #region Generic Serialize<T> and Deserialize<T>

        /// <summary>
        /// This is actually a "Fake" generic version of the Transpose method.
        /// </summary>
        public static byte[] Serialize<T>(DeltaBlockState<T> state)
            where T : struct
        {
            try
            {
                return SerializeFuncMap[typeof(T)].Invoke(state);
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
        public static void Deserialize<T>(DeltaBlockState<T> state)
            where T : struct
        {
            try
            {
                DeserializeFuncMap[typeof(T)].Invoke(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion // Generic Encode<T> and Decode<T>

        #region Serialize

        public static byte[] Serialize(DeltaBlockState<DateTimeOffset> state)
        {
            var factor = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);

                writer.Write(state.Anchor.Ticks);
                writer.Write((short)(state.Anchor.Offset.Ticks / TimeSpan.TicksPerMinute));

                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<DateTime> state)
        {
            var factor = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor.Ticks);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<TimeSpan> state)
        {
            var factor = state.Factor.HasValue ? state.Factor.Value.Ticks : 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor.Ticks);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<long> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<ulong> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<int> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<uint> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<short> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<ushort> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<sbyte> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<byte> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<bool> state)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write((byte)1);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<float> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<double> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(DeltaBlockState<decimal> state)
        {
            var factor = state.Factor ?? 1;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(state.Flags.Value);
                writer.Write(state.ListCount);
                writer.Write(state.Anchor);
                writer.Write(factor);
                writer.Write(state.ByteCount);
                writer.Write(state.Bytes);
                return ms.ToArray();
            }
        }

        #endregion // Serialize

        #region Deserialize

        public static void Deserialize(DeltaBlockState<DateTimeOffset> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                //var flags = reader.ReadUInt16();
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();

                var dateTime = new DateTime(reader.ReadInt64());
                var offset = new TimeSpan(reader.ReadInt16()*TimeSpan.TicksPerMinute);
                state.Anchor = new DateTimeOffset(dateTime, offset);

                state.Factor = new DateTime(reader.ReadInt64());
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<DateTime> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                //var flags = reader.ReadUInt16();
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = new DateTime(reader.ReadInt64());
                state.Factor = new DateTime(reader.ReadInt64());
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<TimeSpan> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = new TimeSpan(reader.ReadInt64());
                state.Factor = new TimeSpan(reader.ReadInt64());
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<long> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadInt64();
                state.Factor = reader.ReadInt64();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<ulong> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadUInt64();
                state.Factor = reader.ReadUInt64();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<int> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadInt32();
                state.Factor = reader.ReadInt32();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<uint> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadUInt32();
                state.Factor = reader.ReadUInt32();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<short> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadInt16();
                state.Factor = reader.ReadInt16();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<ushort> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadUInt16();
                state.Factor = reader.ReadUInt16();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<sbyte> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadSByte();
                state.Factor = reader.ReadSByte();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<byte> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadByte();
                state.Factor = reader.ReadByte();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<bool> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadByte() != 0;
                state.Factor = reader.ReadByte() != 0;
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<float> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadSingle();
                state.Factor = reader.ReadSingle();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<double> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadDouble();
                state.Factor = reader.ReadDouble();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        public static void Deserialize(DeltaBlockState<decimal> state)
        {
            using (var ms = new MemoryStream(state.Bytes))
            using (var reader = new BinaryReader(ms))
            {
                state.Flags.Value = reader.ReadUInt16();
                state.ListCount = reader.ReadInt32();
                state.Anchor = reader.ReadDecimal();
                state.Factor = reader.ReadDecimal();
                state.ByteCount = reader.ReadInt32();
                state.Bytes = reader.ReadBytes(state.ByteCount);
            }
        }

        #endregion // Deserialize
    }
}
