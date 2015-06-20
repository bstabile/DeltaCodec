#region License

// Namespace : Stability.Data.Compression.Transforms
// FileName  : NullTransform.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.Transforms
{
    /// <summary>
    /// This is a pseudo transform that simply goes straight to the finisher.
    /// </summary>
    public class NullTransform : IDeltaTransform
    {
        #region Private Function Maps

        private readonly IDictionary<Type, Action<object>> _encodeFuncMap;

        private readonly IDictionary<Type, Action<object>> _decodeFuncMap;

        #endregion // Private Function Maps

        public NullTransform()
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

        public static readonly NullTransform Instance = new NullTransform();

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
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = default(DateTimeOffset);

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<DateTimeOffset> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeDateTimeOffset(state.Bytes);
            state.List = list;
        }

        #endregion // DateTimeOffset

        #region DateTime

        public virtual void Encode(DeltaBlockState<DateTime> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = default(DateTime);

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<DateTime> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeDateTime(state.Bytes);
            state.List = list;
        }

        #endregion // DateTime
        
        #region TimeSpan

        public virtual void Encode(DeltaBlockState<TimeSpan> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = default(TimeSpan);

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<TimeSpan> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeTimeSpan(state.Bytes);
            state.List = list;
        }

        #endregion // TimeSpan
        
        #region Int64

        public virtual void Encode(DeltaBlockState<long> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<long> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeInt64(state.Bytes);
            state.List = list;
        }

        #endregion // Int64
        
        #region UInt64

        public virtual void Encode(DeltaBlockState<ulong> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<ulong> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeUInt64(state.Bytes);
            state.List = list;
        }

        #endregion // UInt64
        
        #region Int32

        public virtual void Encode(DeltaBlockState<int> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<int> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeInt32(state.Bytes);
            state.List = list;
        }

        #endregion // Int32
        
        #region UInt32

        public virtual void Encode(DeltaBlockState<uint> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<uint> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeUInt32(state.Bytes);
            state.List = list;
        }

        #endregion // UInt32

        #region Int16

        public virtual void Encode(DeltaBlockState<short> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<short> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeInt16(state.Bytes);
            state.List = list;
        }

        #endregion // Int16
        
        #region UInt16

        public virtual void Encode(DeltaBlockState<ushort> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<ushort> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeUInt16(state.Bytes);
            state.List = list;
        }

        #endregion // UInt16

        #region SByte

        public virtual void Encode(DeltaBlockState<sbyte> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<sbyte> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeSByte(state.Bytes);
            state.List = list;
        }

        #endregion // SByte

        #region Byte

        public virtual void Encode(DeltaBlockState<byte> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<byte> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeByte(state.Bytes);
            state.List = list;
        }

        #endregion // Byte

        #region Boolean

        public virtual void Encode(DeltaBlockState<bool> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;

            // NOTE: Anchor and Factor are not relevant to Boolean encoding

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<bool> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            // NOTE: Anchor and Factor are not relevant to Boolean encoding

            var list = state.Finisher.DecodeBoolean(state.Bytes);
            state.List = list;
        }

        #endregion // Boolean

        #region Float

        public virtual void Encode(DeltaBlockState<float> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<float> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeSingle(state.Bytes);
            state.List = list;
        }

        #endregion // Float
        
        #region Double

        public virtual void Encode(DeltaBlockState<double> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<double> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeDouble(state.Bytes);
            state.List = list;
        }

        #endregion // Double

        #region Decimal

        public virtual void Encode(DeltaBlockState<decimal> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.List;
            state.ListCount = list.Count;
            state.Anchor = list[0];
            state.Factor = 0;

            state.Bytes = state.Finisher.Encode(list, state.Flags.Level);
            state.ByteCount = state.Bytes.Length;
        }

        public virtual void Decode(DeltaBlockState<decimal> state)
        {
            if (state.Finisher == null)
                throw new ArgumentException("A finisher is required.", "state");

            var list = state.Finisher.DecodeDecimal(state.Bytes);
            state.List = list;
        }

        #endregion // Decimal
    }
}
