#region License

// Namespace : Stability.Data.Compression.Finishers
// FileName  : Finisher.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Stability.Data.Compression.Finishers
{
    /// <summary>
    /// This abstract implementation of IFinisher writes intrinsic data to a stream. 
    /// In final concrete implementations we only need to handle the methods that 
    /// remain abstract. That is, the actual finishing compression is executed against 
    /// a stream that has already had the data written to it.
    /// </summary>
    public abstract class Finisher : IFinisher
    {
        #region Private Static Function Map

        private readonly IDictionary<Type, Func<object, CompressionLevel, byte[]>> _encodeFuncMap;

        private readonly IDictionary<Type, Func<byte[], object>> _decodeFuncMap;

        #endregion // Private Static Function Map

        protected Finisher()
        {
            _encodeFuncMap = new Dictionary<Type, Func<object, CompressionLevel, byte[]>>
            {
                {typeof (byte), (a, l) => Encode((IList<byte>) a, l)},
                {typeof (sbyte), (a, l) => Encode((IList<sbyte>) a, l)},
                {typeof (ushort), (a, l) => Encode((IList<ushort>) a, l)},
                {typeof (short), (a, l) => Encode((IList<short>) a, l)},
                {typeof (uint), (a, l) => Encode((IList<uint>) a, l)},
                {typeof (int), (a, l) => Encode((IList<int>) a, l)},
                {typeof (ulong), (a, l) => Encode((IList<ulong>) a, l)},
                {typeof (long), (a, l) => Encode((IList<long>) a, l)},
                // DateTime and TimeSpan
                {typeof (DateTimeOffset), (a, l) => Encode((IList<DateTimeOffset>) a, l)},
                {typeof (DateTime), (a, l) => Encode((IList<DateTime>) a, l)},
                {typeof (TimeSpan), (a, l) => Encode((IList<TimeSpan>) a, l)},
                // Reals
                {typeof (float), (a, l) => Encode((IList<float>) a, l)},
                {typeof (double), (a, l) => Encode((IList<double>) a, l)},
                {typeof (decimal), (a, l) => Encode((IList<decimal>) a, l)},
                // Char
                {typeof (char), (a, l) => Encode((IList<char>) a, l)},
                // String
                {typeof (string), (a, l) => Encode((IList<string>) a, l)},
            };

            _decodeFuncMap = new Dictionary<Type, Func<byte[], object>>
            {
                {typeof (byte), DecodeByte},
                {typeof (sbyte), DecodeSByte},
                {typeof (ushort), DecodeUInt16},
                {typeof (short), DecodeInt16},
                {typeof (uint), DecodeUInt32},
                {typeof (int), DecodeInt32},
                {typeof (ulong), DecodeUInt64},
                {typeof (long), DecodeInt64},
                // DateTime and TimeSpan
                {typeof (DateTimeOffset), DecodeDateTimeOffset},
                {typeof (DateTime), DecodeDateTime},
                {typeof (TimeSpan), DecodeTimeSpan},
                // Reals
                {typeof (float), DecodeSingle},
                {typeof (double), DecodeDouble},
                {typeof (decimal), DecodeDecimal},

            };
        }

        #region Encode

        /// <summary>
        /// This is actually a "Fake" generic version of the Transpose method.
        /// </summary>
        public byte[] Encode<T>(IList<T> arr, CompressionLevel level = CompressionLevel.Fastest)
        {
            try
            {
                return _encodeFuncMap[typeof(T)].Invoke(arr, level);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public byte[] Encode(byte[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data))
            {
                return Encode(ms, level);
            }
        }

        public byte[] Encode(IList<byte> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.ToArray()))
            {
                return Encode(ms, level);
            }
        }

        public byte[] Encode(IList<bool> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(sbyte[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<sbyte> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(ushort[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<ushort> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(short[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<short> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(uint[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 4))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<uint> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 4))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(int[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 4))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<int> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 4))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(ulong[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<ulong> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(long[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<long> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(float[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 4))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<float> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 4))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(double[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<double> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(decimal[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 16))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<decimal> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 16))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(DateTimeOffset[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            // DateTimeOffset has a DateTime value and an Int16 offsetMinutes value (total bytes = 10)
            using (var ms = new MemoryStream(data.Length * 10))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i].Ticks);
                        writer.Write((short) (data[i].Offset.Ticks / TimeSpan.TicksPerMinute));
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<DateTimeOffset> data, CompressionLevel level = CompressionLevel.Optimal)
        {
            // DateTimeOffset has a DateTime value and an Int16 offsetMinutes value (total bytes = 10)
            using (var ms = new MemoryStream(data.Count * 10))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i].Ticks);
                        writer.Write((short) (data[i].Offset.Ticks / TimeSpan.TicksPerMinute));
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(DateTime[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i].Ticks);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<DateTime> data, CompressionLevel level = CompressionLevel.Optimal)
        {
            using (var ms = new MemoryStream(data.Count * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i].Ticks);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(TimeSpan[] data, CompressionLevel level = CompressionLevel.Optimal)
        {
            using (var ms = new MemoryStream(data.Length * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i].Ticks);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<TimeSpan> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 8))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i].Ticks);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(char[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Length * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write((ushort) data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<char> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream(data.Count * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write((ushort) data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(string[] data, CompressionLevel level = CompressionLevel.Fastest)
        {
            // We don't know the appropriate capacity since strings are variable length.
            // We'll just go on the assumption that strings average at least 1 char each.
            using (var ms = new MemoryStream(data.Length * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(IList<string> data, CompressionLevel level = CompressionLevel.Fastest)
        {
            // We don't know the appropriate capacity since strings are variable length.
            // We'll just go on the assumption that strings average at least 1 char each.
            using (var ms = new MemoryStream(data.Count * 2))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        writer.Write(data[i]);
                    }
                    writer.Flush();
                    return Encode(ms, level);
                }
            }
        }

        public byte[] Encode(MemoryStream input, CompressionLevel level = CompressionLevel.Fastest)
        {
            input.Position = 0;
            using (var stream = EncodeToStream(input, level))
            {
                return stream.ToArray();
            }
        }

        /// <summary>
        /// This encodes an input stream to an output stream.
        /// </summary>
        /// <returns>A stream that has been encoded.</returns>
        /// <remarks>
        /// This does NOT close or alter the "Position" property of the returned stream.
        /// </remarks>
        public abstract MemoryStream EncodeToStream(MemoryStream input, CompressionLevel level = CompressionLevel.Fastest);

        #endregion // Encode

        #region Decode

        public IList<T> Decode<T>(byte[] data)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            try
            {
                return (IList<T>)_decodeFuncMap[typeof(T)].Invoke(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<bool> DecodeBoolean(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int)stream.Length;
                var list = new bool[n];
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list[i] = reader.ReadBoolean();
                }
                return list;
            }
        }

        public IList<byte> DecodeByte(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                return stream.ToArray();
            }
        }

        public IList<sbyte> DecodeSByte(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var list = new List<sbyte>((int) stream.Length);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < stream.Length; i++)
                {
                    list.Add(reader.ReadSByte());
                }
                return list;
            }
        }

        public IList<ushort> DecodeUInt16(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 2;
                var list = new List<ushort>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadUInt16());
                }
                return list;
            }
        }

        public IList<short> DecodeInt16(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 2;
                var list = new List<short>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadInt16());
                }
                return list;
            }
        }

        public IList<uint> DecodeUInt32(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 4;
                var list = new List<uint>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadUInt32());
                }
                return list;
            }
        }

        public IList<int> DecodeInt32(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 4;
                var list = new List<int>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadInt32());
                }
                return list;
            }
        }

        public IList<ulong> DecodeUInt64(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 8;
                var list = new List<ulong>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadUInt64());
                }
                return list;
            }
        }

        public IList<long> DecodeInt64(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 8;
                var list = new List<long>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadInt64());
                }
                return list;
            }
        }

        public IList<float> DecodeSingle(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 4;
                var list = new List<float>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadSingle());
                }
                return list;
            }
        }

        public IList<double> DecodeDouble(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 8;
                var list = new List<double>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadDouble());
                }
                return list;
            }
        }

        public IList<Decimal> DecodeDecimal(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 16;
                var list = new List<decimal>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadDecimal());
                }
                return list;
            }
        }

        public IList<DateTimeOffset> DecodeDateTimeOffset(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                // DateTimeOffset has a DateTime value and an Int16 offsetMinutes value (total bytes = 10)
                var n = (int)stream.Length / 10;
                var list = new List<DateTimeOffset>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    var dt = new DateTime(reader.ReadInt64());
                    var os = new TimeSpan(0, reader.ReadInt16(), 0);
                    list.Add(new DateTimeOffset(dt, os));
                }
                return list;
            }
        }

        public IList<DateTime> DecodeDateTime(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int)stream.Length / 8;
                var list = new List<DateTime>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(new DateTime(reader.ReadInt64()));
                }
                return list;
            }
        }

        public IList<TimeSpan> DecodeTimeSpan(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) stream.Length / 8;
                var list = new List<TimeSpan>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(new TimeSpan(reader.ReadInt64()));
                }
                return list;
            }
        }

        public IList<char> DecodeChar(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int) (stream.Length / 2);
                var list = new List<char>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add((char) reader.ReadUInt16());
                }
                return list;
            }
        }

        public IList<String> DecodeString(byte[] data)
        {
            using (var stream = DecodeToStream(data))
            {
                var n = (int)(stream.Length / 2);
                var list = new List<string>(n);
                var reader = new BinaryReader(stream);
                for (var i = 0; i < n; i++)
                {
                    list.Add(reader.ReadString());
                }
                return list;
            }
        }

        /// <summary>
        /// After decoding, this method sets the position of the returned stream back to zero.
        /// Clients should not forget to close the stream, when finished using it.
        /// </summary>
        /// <param name="data">The encoded data that will be decoded.</param>
        /// <returns>An open <see cref="MemoryStream"/> wrapping the decoded data, with position set to zero.</returns>
        public abstract MemoryStream DecodeToStream(byte[] data);

        #endregion // Decode
    }
}
