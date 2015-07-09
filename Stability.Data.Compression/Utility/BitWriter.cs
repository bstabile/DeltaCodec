#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : BitWriter.cs
// Created   : 2015-6-18
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.IO;

namespace Stability.Data.Compression.Utility
{
	public class BitWriter
	{
		private readonly MemoryStream _stream;
		private int _bitBuffer;
		private int _bitsToGo;
		private int _bitsToFollow;

		public BitWriter(MemoryStream stream)
		{
			_stream = stream;
			_bitsToGo = 8;
		}

		public MemoryStream BaseStream
		{
			get { return _stream; }
		}

		public int BitsToFollow
		{
			get { return _bitsToFollow; }
			set { _bitsToFollow = value; }
		}

		public void WriteBitPlusFollow(UInt32 bit)
		{
			WriteBit(bit);
			while (_bitsToFollow > 0)
			{
				if (bit == 0)
					WriteBit(1);
				else
					WriteBit(0);
				_bitsToFollow -= 1;
			}
		}
		
		public void WriteBit(UInt32 bit)
		{
			_bitBuffer >>= 1;
			if (bit != 0)
			{
				_bitBuffer |= 0x80;
			}
			if (--_bitsToGo == 0)
			{
				_stream.WriteByte((byte)_bitBuffer);
				_bitsToGo = 8;
			}
		}

		public void WriteBits(Int32 bits, int numBits)
		{
			for (var i = 0; i < numBits && i < 32; i++)
			{
				WriteBit((uint)bits & 1);
				bits >>= 1;
			}
		}

		public void WriteByte(byte b)
		{
			for (var i = 0; i < 8; i++)
			{
				WriteBit((uint)b & 1);
				b >>= 1;
			}
		}

		public void Flush()
		{
			if (_bitsToGo < 8)
			{
				_stream.WriteByte((byte)(_bitBuffer >> _bitsToGo));
				_bitsToGo  = 8;
				_bitBuffer = 0;
			}
		}

		public void Flush(UInt32 lastBit)
		{
			_bitsToFollow += 1;
			WriteBitPlusFollow(lastBit);
			Flush();
		}
	}
}
