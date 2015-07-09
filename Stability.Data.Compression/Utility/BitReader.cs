using System;
using System.IO;

namespace Stability.Data.Compression.Utility
{
	public class BitReader
	{
		public const int CodeValueBits = 16;

		private readonly MemoryStream _stream;

		private int _bitBuffer;
		private int _bitsToGo;
		private int _garbageBits;

		public BitReader(MemoryStream stream)
		{
			// Do NOT change the position of the stream.
			// The coder reserves space for global versioning info.
			// The position will be set to the first byte AFTER that.
			_stream = stream;

			_bitBuffer = 0;
			_bitsToGo = 0;
			_garbageBits = 0;
		}

		public MemoryStream BaseStream
		{
			get { return _stream; }
		}

		public int ReadBit()
		{
			if (_bitsToGo == 0)
			{
				_bitBuffer = _stream.ReadByte();
				if (_bitBuffer == -1)
				{
					_garbageBits += 1;
					if (_garbageBits > CodeValueBits - 2)
						throw new ApplicationException("BitReader.Read(): Invalid input file");
					return _bitBuffer;
				}
				_bitsToGo = 8;
			}
			int bit = _bitBuffer & 1;
			_bitBuffer >>= 1;
			_bitsToGo--;
			return bit;
		}

		public uint ReadBits(int numBits)
		{
			if ( numBits < 1 || numBits > 32 )
				throw new ArgumentException("ByteReader.ReadBits: " + 
					"Invalid number of bits specified (min=1, max=32)","numBits");

			uint bits = 0;

			for ( var i = 0; i < numBits; i++ )
			{
				int bit = ReadBit();
				// if no more bits then shift valid ones and return
				if (bit == -1)
				{
					return bits;
				}
			    bits |= (uint)(bit << i);
			}
			return bits; // OK
		}
	}
}