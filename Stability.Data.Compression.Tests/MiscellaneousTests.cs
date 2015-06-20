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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stability.Data.Compression.Tests
{
    [TestClass]
    public class MiscellaneousTests
    {
        [TestMethod]
        [Description("Demonstrates how casting ArraySegment to IList makes them genuinely useful.")]
        public void ArraySegmentMagic()
        {
            var list = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var arr = list.ToArray();

            var arrSegs = new ArraySegment<int>[3];
            arrSegs[0] = new ArraySegment<int>(arr, 0, 3);
            arrSegs[1] = new ArraySegment<int>(arr, 3, 3);
            arrSegs[2] = new ArraySegment<int>(arr, 6, 3);
            for (var i = 0; i < 3; i++)
            {
                var seg = arrSegs[i] as IList<int>;
                Console.Write(seg.GetType().Name.Substring(0, 12) + i);
                Console.Write(" {");
                for (var j = 0; j < seg.Count; j++)
                {
                    Console.Write("{0},", seg[j]);
                }
                Console.WriteLine("}");
            }
        }
    }
}
