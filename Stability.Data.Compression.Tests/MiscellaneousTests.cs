#region License

// Namespace : Stability.Data.Compression.Tests
// FileName  : MiscellaneousTests.cs
// Created   : 2015-5-19
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stability.Data.Compression.DataStructure;
using Stability.Data.Compression.TestUtility;

namespace Stability.Data.Compression.Tests
{
    [TestClass]
    public class MiscellaneousTests
    {
        [TestMethod]
        public void StringAnalysis()
        {
            // We can't make the list too long because our max number of total characters is Int32.MaxValue.
            // Eventually we might want to use LongLengths, but available memory will have to accommodate.
            var listCount = 100000;
            var list = TimeSeriesProvider.RandomAlphanumericString(listCount, seed: 0, minLength: 10, maxLength: 100);

            //var q = list.GroupBy(c => c)
            //    .Select(g => new { g.Key, Count = g.Count() })
            //    .OrderByDescending(g => g.Count)
            //    .Select(g => g.Key).ToList();

            var stopwatch = Stopwatch.StartNew();

            using (var ms = new MemoryStream(listCount*10))
            {
                var writer = new BinaryWriter(ms);
                for (var i = 0; i < list.Count; i++)
                {
                    writer.Write(list[i]);
                }
                var bytes = ms.ToArray();
                var codes = bytes.Distinct().ToArray();
                var map = new Dictionary<byte, int>(codes.Length);
                for (var i = 0; i < codes.Length; i++)
                {
                    map.Add(codes[i], 0);
                }
                for (var i = 0; i < bytes.Length; i++)
                {
                    map[bytes[i]]++;
                }
            }

            var elapsed = stopwatch.Elapsed.TotalMilliseconds;

            Console.WriteLine("Elapsed Freqency Count: {0}", elapsed.ToString("F2"));
        }

        [TestMethod]
        [Description("For automated testing it is useful to set defaults.")]
        public void ReflectGenericArgumentsToGetDefaultValue()
        {
            var struple = new Struple<int, double, string>();

            var types = struple.GetType().GetGenericArguments();

            for (var i = 0; i < types.Length; i++)
            {
                var t = types[i];
                
                Console.WriteLine("Type: {0, 12} :  {1}", t.Name, GetDefault(t));
            }
        }

        #region Private Static Methods

        private static object GetDefault(Type t)
        {
            Func<object> f = GetDefault<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, null);
        }

        private static T GetDefault<T>()
        {
            return default(T);
        }

        #endregion // Private Static Methods

    }
}
