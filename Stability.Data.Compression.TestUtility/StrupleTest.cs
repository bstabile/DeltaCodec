#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : StrupleTest.cs
// Created   : 2015-6-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public static class StrupleTest
    {
        public static void Run(IDeltaCodec[] codecs, StrupleEncodingArgs args, string configDisplayName = "")
        {
            Warmup(codecs, args); // Let everything get jitted by running through once

            var rawBytes = GetRawBytesCount(args);

            var resultsList = new List<Result>(codecs.Length);
            foreach (var codec in codecs)
            {
                GC.Collect(generation: 2, mode: GCCollectionMode.Forced, blocking: true);

                var stopwatch = Stopwatch.StartNew();
                var bytes = Encode(codec, args);
                var elapsedEncode = stopwatch.Elapsed;
                stopwatch.Restart();
                var listOut = Decode(codec, args, bytes);
                var elapsedDecode = stopwatch.Elapsed;
                stopwatch.Stop();

                var encBytes = bytes.Length;
                var ratio = rawBytes == 0 ? 0 : encBytes / (double)rawBytes;
                var multiple = encBytes == 0 ? 0 : rawBytes / (double)encBytes;

                resultsList.Add(
                    new Result
                    {
                        CodecName = codec.DisplayName,
                        RawBytes = rawBytes,
                        EncBytes = encBytes,
                        Ratio = ratio,
                        Multiple = multiple,
                        EncMS = elapsedEncode.TotalMilliseconds,
                        DecMS = elapsedDecode.TotalMilliseconds,
                    });

                Validate(args, listOut);
            }
            PrintResults(args, resultsList, configDisplayName);
        }

        private static void Warmup(IDeltaCodec[] codecs, StrupleEncodingArgs args)
        {
            foreach (var codec in codecs)
            {
                var bytes = Encode(codec, args);
                var listOut = Decode(codec, args, bytes);
            }
        }

        private static int GetRawBytesCount(StrupleEncodingArgs args)
        {
            var t = args.GetType();
            var types = t.GetGenericArguments();
            return types.Sum(type => DeltaUtility.GetSizeOfIntrinsicType(type)) * args.ListCount;
        }

        private static void Validate(dynamic args, dynamic listOut)
        {
            Assert.AreEqual(args.ListCount, listOut.Length);
            for (var i = 0; i < listOut.Length; i++)
            {
                var tuple = listOut.GetValue(i);
                Assert.AreEqual(args.List[i].Item1, tuple.Item1);
                Assert.AreEqual(args.List[i].Item2, tuple.Item2);
            }
        }
        private static void PrintResults(StrupleEncodingArgs args, IList<Result> results, string configDisplayName = "")
        {
            const string fmt = "{0, -25}{1, 15}{2, 15}{3, 10}{4, 10}{5, 12}{6, 12}";

            var strupleTypes = args.GetType().GetGenericArguments();
            var sb = new StringBuilder();
            sb = sb.Append("Struple<");
            for (var i = 0; i < strupleTypes.Length; i++)
            {
                sb = sb.Append(strupleTypes[i].Name);
                if (i < strupleTypes.Length - 1)
                    sb = sb.Append(",");
            }
            sb = sb.Append(">");

            var headers = string.Format(fmt, "Codec", "RawBytes", "EncBytes", "Ratio", "Multiple", "EncodeMS", "DecodeMS");
            if (configDisplayName.Length > 0)
                Console.WriteLine("Configuration: {0}\n", configDisplayName);
            Console.WriteLine(sb.ToString());
            Console.WriteLine("\nListCount = {0}", args.ListCount.ToString("N0"));
            Console.WriteLine();
            Console.WriteLine(headers);
            Console.WriteLine(("").PadRight(headers.Length, '='));
            foreach (var r in results)
            {
                Console.WriteLine(fmt,
                    r.CodecName,
                    r.RawBytes.ToString("N0"),
                    r.EncBytes.ToString("N0"),
                    r.Ratio.ToString("F2"),
                    r.Multiple.ToString("F2"),
                    r.EncMS.ToString("N2"),
                    r.DecMS.ToString("N2")
                    );
            }
            Console.WriteLine(("").PadRight(headers.Length, '='));
        }

        private class Result
        {
            public string StrupleType;
            public string CodecName;
            public int RawBytes;
            public int EncBytes;
            public double Ratio;
            public double Multiple;
            public double EncMS;
            public double DecMS;
        }

        public static byte[] Encode(IDeltaCodec codec, StrupleEncodingArgs args)
        {
            var t = args.GetType();
            var types = t.GetGenericArguments();
            var encodingMethods = codec.GetType()
                .GetMethods().Where(m =>
                    m.Name == "Encode"
                    && m.IsGenericMethod
                    && m.GetGenericArguments().Length == types.Length).ToList();

            if (encodingMethods.Count > 1)
                throw new MethodAccessException("Multiple encoding methods match the specified type parameters.");

            var method = encodingMethods[0].MakeGenericMethod(types);
            var bytes = method.Invoke(codec, new object[] { args });
            return (byte[])bytes;
        }

        public static dynamic Decode(IDeltaCodec codec, StrupleEncodingArgs args, byte[] bytes)
        {
            var t = args.GetType();
            var types = t.GetGenericArguments();
            var encodingMethods = codec.GetType()
                .GetMethods().Where(m =>
                    m.Name == "Decode"
                    && m.IsGenericMethod
                    && m.GetGenericArguments().Length == types.Length).ToList();

            if (encodingMethods.Count > 1)
                throw new MethodAccessException("Multiple decoding methods match the specified type parameters.");

            var method = encodingMethods[0].MakeGenericMethod(types);
            dynamic list = method.Invoke(codec, new object[] { bytes });
            return list;
        }
    }
}
