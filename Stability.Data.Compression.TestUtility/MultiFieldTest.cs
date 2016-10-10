#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : MultiFieldTest.cs
// Created   : 2015-6-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    /// <summary>
    /// This static class helps automate tests for both "Struple" and "Tuple" types.
    /// Both of these types allow multiple fields to be encoded together.
    ///
    /// There are some significant differences between the two:
    /// 
    /// Struple is a value type, Tuple is a reference type.
    /// Struple field values can be changed, Tuple field values are read only.
    /// Struple has up to 17 type arguments, Tuple has 7 plus 1 that is
    /// another Tuple (TRest). Thus daisy-chaining can be used with Tuples,
    /// but it was thought to be inconvenient and confusing for typical use
    /// cases.
    /// </summary>
    public static class MultiFieldTest
    {
        public static void Run(IDeltaCodec[] codecs, MultiFieldEncodingArgs args, string configDisplayName = "")
        {
            Warmup(codecs, args); // Let everything get jitted by running through once

            var rawBytes = DeltaUtility.GetTotalBytes(args);
            var dataTypeName = GetMultiFieldTypeName(args);

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
                        // Block Header Info
                        Config = configDisplayName,
                        DataType = dataTypeName,
                        DataCount = args.DataCount,
                        RawBytes = rawBytes,
                        // Line Info
                        CodecName = codec.DisplayName,
                        EncBytes = encBytes,
                        Ratio = ratio,
                        Multiple = multiple,
                        EncMS = elapsedEncode.TotalMilliseconds,
                        DecMS = elapsedDecode.TotalMilliseconds,
                    });

                if (args is MapEncodingArgs)
                {
                    ValidateMap(args, listOut);
                }
                else
                {
                    Validate(args, listOut);
                }
            }
            PrintResults(resultsList);
        }

        public static byte[] Encode(IDeltaCodec codec, MultiFieldEncodingArgs args)
        {
            var method = GetEncodingMethod(codec, args);
            var bytes = method.Invoke(codec, new object[] { args });
            return (byte[])bytes;
        }

        public static dynamic Decode(IDeltaCodec codec, MultiFieldEncodingArgs args, byte[] bytes)
        {
            var method = GetDecodingMethod(codec, args);
            dynamic list = method.Invoke(codec, new object[] { bytes });
            return list;
        }

        #region Private

        private static void Warmup(IDeltaCodec[] codecs, MultiFieldEncodingArgs args)
        {
            if (codecs == null) throw new ArgumentNullException("codecs");
            foreach (var codec in codecs)
            {
                var bytes = Encode(codec, args);
                var listOut = Decode(codec, args, bytes);
            }
        }

        private static void Validate(dynamic args, dynamic listOut)
        {
            Assert.AreEqual(args.DataCount, listOut.Length);
            for (var i = 0; i < listOut.Length; i++)
            {
                var v = listOut[i];
                Assert.AreEqual(args.Data[i].Item1, v.Item1);
                Assert.AreEqual(args.Data[i].Item2, v.Item2);
            }
        }

        private static void ValidateMap(dynamic args, dynamic mapOut)
        {
            var list = args.Data.ToList();
            var listOut = mapOut.ToList();

            Assert.AreEqual(list.Count, listOut.Count);
            for (var i = 0; i < mapOut.Length; i++)
            {
                Assert.AreEqual(list[i].Key, listOut[i].Key);
                Assert.AreEqual(list[i].Item2, listOut[i].Value);
            }
        }

        private static void PrintResults(IList<Result> results)
        {
            if (results == null) throw new ArgumentNullException("results");
            if (results.Count == 0)
                return;

            Console.WriteLine(results[0].BlockHeader());
            Console.WriteLine(Result.LineHeaders);
            Console.WriteLine(Result.HorizontalRule);

            foreach (var r in results)
            {
                Console.WriteLine(r.ToString());
            }
            Console.WriteLine(Result.HorizontalRule);
        }

        private static string GetMultiFieldTypeName(MultiFieldEncodingArgs args)
        {
            var fieldTypes = args.GetType().GetGenericArguments();

            var listTypeName = args.GetType().GetProperty("Data").PropertyType.GetGenericArguments().First().Name;
            var rootTypeName = listTypeName.Substring(0, listTypeName.IndexOf('`'));

            var sb = new StringBuilder();
            sb = sb.Append(rootTypeName + "<");
            for (var i = 0; i < fieldTypes.Length; i++)
            {
                sb = sb.Append(fieldTypes[i].Name);
                if (i < fieldTypes.Length - 1)
                    sb = sb.Append(",");
            }
            sb = sb.Append(">");
            return sb.ToString();
        }

        private static MethodInfo GetEncodingMethod(IDeltaCodec codec, MultiFieldEncodingArgs args)
        {
            // I'm sure there must be a better way to reflect this but...
            var t = args.GetType();
            var types = t.GetGenericArguments();

            var encodingMethods = codec.GetType()
                .GetMethods().Where(
                    m =>
                        m.Name == "Encode"
                        && m.IsGenericMethod
                        && m.GetGenericArguments().Length == types.Length
                        && m.GetParameters().Count(p => p.ParameterType.BaseType == args.GetType().BaseType) > 0
                ).ToList();

            if (encodingMethods.Count < 1)
                throw new MethodAccessException(
                    "An encoding method matching the targeted signature could not be found.");
            if (encodingMethods.Count > 1)
                throw new MethodAccessException("Multiple encoding methods match the targeted signature.");

            var method = encodingMethods[0].MakeGenericMethod(types);
            return method;
        }

        private static MethodInfo GetDecodingMethod(IDeltaCodec codec, MultiFieldEncodingArgs args)
        {
            // I'm sure there must be a better way to reflect this but...
            var t = args.GetType();
            var types = t.GetGenericArguments();

            var listTypeDef = t.GetProperty("Data").PropertyType
                .GetGenericArguments().First().GetGenericTypeDefinition();

            var filter = args is MapEncodingArgs
                ? "DecodeMap"
                : args is StrupleEncodingArgs 
                ? "DecodeStruple" 
                : args is TupleEncodingArgs 
                ? "DecodeTuple" : null;

            if (filter == null)
                throw new InvalidOperationException("Unknown MultiFieldEncodingArgs type.");

            var mi = codec.GetType().GetMethods().First(m => m.Name.Contains(filter)
                                                             && m.IsGenericMethod
                                                             && m.GetGenericArguments().Length == types.Length);
            if (mi == null)
                throw new InvalidOperationException("Unable to find a matching generic decoding method.");

            //var decodingMethods = codec.GetType()
            //    .GetMethods().Where(
            //        m =>
            //            m.Name.Contains("Decode")
            //            && m.IsGenericMethod
            //            && m.GetGenericArguments().Length == types.Length
            //            && m.ReturnType.GetGenericArguments().First().GetGenericTypeDefinition() == listTypeDef
            //    ).ToList();

            //if (decodingMethods.Count < 1)
            //    throw new MethodAccessException("A decoding method matching the targeted signature could not be found.");
            //if (decodingMethods.Count > 1)
            //    throw new MethodAccessException("Multiple decoding methods match the targeted signature.");

            //var method = decodingMethods[0].MakeGenericMethod(types);
            var method = mi.MakeGenericMethod(types);
            return method;
        }

        private class Result
        {
            private const string LineFormat = "{0, -25}{1, 15}{2, 10}{3, 10}{4, 12}{5, 12}";
            public static readonly string LineHeaders = string.Format(LineFormat, "Codec", "EncBytes", "Ratio", "Multiple", "EncodeMS", "DecodeMS");
            public static readonly string HorizontalRule = ("").PadRight(LineHeaders.Length, '=');

            // Block Info
            public string Config;
            public string DataType;
            public int DataCount;
            public int RawBytes;
            // Line Info
            public string CodecName;
            public int EncBytes;
            public double Ratio;
            public double Multiple;
            public double EncMS;
            public double DecMS;

            public string BlockHeader()
            {
                var sb = new StringBuilder();
                sb = sb.AppendFormat("{0, -10} = {1}\n", "Config", Config);
                sb = sb.AppendFormat("{0, -10} = {1}\n", "DataType", DataType);
                sb = sb.AppendFormat("{0, -10} = {1, 12}\n", "DataCount", DataCount.ToString("N0"));
                sb = sb.AppendFormat("{0, -10} = {1, 12}\n", "RawBytes", RawBytes.ToString("N0"));
                return sb.ToString();
            }

            public override string ToString()
            {
                var s = string.Format(LineFormat,
                    CodecName,
                    EncBytes.ToString("N0"),
                    Ratio.ToString("F2"),
                    Multiple.ToString("F2"),
                    EncMS.ToString("N2"),
                    DecMS.ToString("F2")
                    );
                return s;
            }
        }

        #endregion // Private
    }
}
