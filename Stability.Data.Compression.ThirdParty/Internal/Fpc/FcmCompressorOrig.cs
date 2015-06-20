#region Derivative Work License (Stability.Data.Compression.ThirdParty)

// Disclaimer: All of the compression algorithms in this assembly are the work of others.
//             They are aggregated here to provide an easy way to learn about and test
//             alternative techniques. In the subfolders "Internal\<libname>" you will
//             find the minimal subset of files needed to expose each algorithm.
//             In the "Licenses" folder you will find the licensing information for each
//             of the third-party libraries. Those licenses (if more restrictive than
//             GPL v3) are meant to override.
//
// Namespace : Stability.Data.Compression.ThirdParty.Internal.Fpc
// FileName  : FcmCompressorOrig.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)
using System;

namespace Stability.Data.Compression.ThirdParty.Internal.Fpc
{
    internal class FcmCompressorOrig
    {
        private static int logOfTableSize = 16;
        private FcmPredictor predictor1 = new FcmPredictor(logOfTableSize);
        private DfcmPredictor predictor2 = new DfcmPredictor(logOfTableSize);

        public void Compress(ByteBuffer buff, double[] doubles)
        {
            for (var i = 0; i < doubles.Length; i += 2)
            {
                if (i == doubles.Length - 1)
                {
                    EncodeAndPad(buff, doubles[i]);
                }
                else
                {
                    Encode(buff, doubles[i], doubles[i + 1]);
                }
            }
        }

        public void Decompress(ByteBuffer buff, double[] dest)
        {
            for (var i = 0; i < dest.Length; i += 2)
            {
                Decode(buff, dest, i);
            }
        }

        private void Decode(ByteBuffer buff, double[] dest, int i)
        {
            byte header = buff.get();

            long prediction;

            if ((header & 0x80) != 0)
            {
                prediction = predictor2.GetPrediction();
            }
            else
            {
                prediction = predictor1.GetPrediction();
            }

            int numZeroBytes = (header & 0x70) >> 4;
            if (numZeroBytes > 3)
            {
                numZeroBytes++;
            }
            var dst = new byte[8 - numZeroBytes];
            buff.get(dst);
            long diff = ToLong(dst);
            long actual = prediction ^ diff;

            predictor1.Update(actual);
            predictor2.Update(actual);

            dest[i] = BitConverter.Int64BitsToDouble(actual);

            if ((header & 0x08) != 0)
            {
                prediction = predictor2.GetPrediction();
            }
            else
            {
                prediction = predictor1.GetPrediction();
            }

            numZeroBytes = (header & 0x07);
            if (numZeroBytes > 3)
            {
                numZeroBytes++;
            }
            dst = new byte[8 - numZeroBytes];
            buff.get(dst);
            diff = ToLong(dst);

            if (numZeroBytes == 7 && diff == 0)
            {
                return;
            }
            actual = prediction ^ diff;

            predictor1.Update(actual);
            predictor2.Update(actual);

            dest[i + 1] = BitConverter.Int64BitsToDouble(actual);
        }

        public long ToLong(byte[] dst)
        {
            long result = 0L;
            for (var i = dst.Length; i > 0; i--)
            {
                result = result << 8;
                result |= dst[i - 1] & 0xff;
            }
            return result;
        }

        private void EncodeAndPad(ByteBuffer buf, double d)
        {

            long dBits = BitConverter.DoubleToInt64Bits(d);
            long diff1d = predictor1.GetPrediction() ^ dBits;
            long diff2d = predictor2.GetPrediction() ^ dBits;

            bool predictor1BetterForD = LeadingZeroCount(diff1d) >= LeadingZeroCount(diff2d);

            predictor1.Update(dBits);
            predictor2.Update(dBits);

            byte code = 0;
            if (predictor1BetterForD)
            {
                int zeroBytes = EncodeZeroBytes(diff1d);
                code |= (byte)(zeroBytes << 4);
            }
            else
            {
                code |= 0x80;
                int zeroBytes = EncodeZeroBytes(diff2d);
                code |= (byte)(zeroBytes << 4);
            }

            code |= 0x06;

            buf.put(code);
            if (predictor1BetterForD)
            {
                buf.put(ToByteArray(diff1d));
            }
            else
            {
                buf.put(ToByteArray(diff2d));
            }

            buf.put((byte) 0);

        }

        private int EncodeZeroBytes(long diff1d)
        {
            int leadingZeroBytes = LeadingZeroCount(diff1d)/8;
            if (leadingZeroBytes >= 4)
            {
                leadingZeroBytes--;
            }
            return leadingZeroBytes;
        }

        private void Encode(ByteBuffer buf, double d, double e)
        {

            long dBits = BitConverter.DoubleToInt64Bits(d);
            long diff1d = predictor1.GetPrediction() ^ dBits;
            long diff2d = predictor2.GetPrediction() ^ dBits;

            bool predictor1BetterForD = LeadingZeroCount(diff1d) >= LeadingZeroCount(diff2d);

            predictor1.Update(dBits);
            predictor2.Update(dBits);

            long eBits = BitConverter.DoubleToInt64Bits(e);
            long diff1e = predictor1.GetPrediction() ^ eBits;
            long diff2e = predictor2.GetPrediction() ^ eBits;

            bool predictor1BetterForE = LeadingZeroCount(diff1e) >= LeadingZeroCount(diff2e);

            predictor1.Update(eBits);
            predictor2.Update(eBits);

            byte code = 0;
            if (predictor1BetterForD)
            {
                int zeroBytes = EncodeZeroBytes(diff1d);
                code |= (byte)(zeroBytes << 4);
            }
            else
            {
                code |= 0x80;
                int zeroBytes = EncodeZeroBytes(diff2d);
                code |= (byte)(zeroBytes << 4);
            }

            if (predictor1BetterForE)
            {
                int zeroBytes = EncodeZeroBytes(diff1e);
                code |= (byte)zeroBytes;
            }
            else
            {
                code |= 0x08;
                int zeroBytes = EncodeZeroBytes(diff2e);
                code |= (byte)zeroBytes;
            }

            buf.put(code);
            if (predictor1BetterForD)
            {
                buf.put(ToByteArray(diff1d));
            }
            else
            {
                buf.put(ToByteArray(diff2d));
            }

            if (predictor1BetterForE)
            {
                buf.put(ToByteArray(diff1e));
            }
            else
            {
                buf.put(ToByteArray(diff2e));
            }
        }

        public byte[] ToByteArray(long diff)
        {
            int encodedZeroBytes = EncodeZeroBytes(diff);
            if (encodedZeroBytes > 3)
            {
                encodedZeroBytes++;
            }
            var array = new byte[8 - encodedZeroBytes];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (byte) (diff & 0xff);
                diff = diff >> 8;
            }
            return array;
        }

        public static int LeadingZeroCount(long v)
        {
            if (v == 0)
                return 64;
            if (v < 0)
                return 0;

            var x = (ulong) v;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            x |= (x >> 32);
            return (64 - SetBitCount64(x));
        }

        private static int SetBitCount64(ulong x)
        {
            if (x == 0)
                return 0;

            x = x - ((x >> 1) & 0x5555555555555555UL);
            x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
            return (int)(unchecked(((x + (x >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

    }

}
