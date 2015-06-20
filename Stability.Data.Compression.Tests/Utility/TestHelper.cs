#region License

// Namespace : Stability.Data.Compression.Tests.Utility
// FileName  : TestHelper.cs
// Created   : 2015-4-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using Stability.Data.Compression.TestUtility;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.Tests.Utility
{
    public static class TestHelper
    {
        public static CodecTestGroup RunEncodingTests<T>(CodecTestGroup runners, IList<T> list, int runCount = 2, 
            T granularity = default(T), Monotonicity monotonicity = Monotonicity.None, bool validate = true)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            // Entropy of Input
            var bufferEntropy = Statistics.BufferEntropy(DeltaUtility.ConvertToBuffer(list)) / 8; // Divisor is always 8 bits for BufferEntropy
            var deltaEntropy = Statistics.DeltaEntropy(list) / DeltaUtility.BitCount(typeof(T));
            foreach (var r in runners)
            {
                r.BufferEntropy = bufferEntropy;
                r.DeltaEntropy = deltaEntropy;
                r.Run(runCount, list, granularity, monotonicity, validate);

                // Run() creates the tests in the runner!
                foreach (var test in r.Tests)
                {
                    var t = test as CodecTest<T>;
                    if (t != null)
                        t.Stats.EntropyIn = bufferEntropy;
                }
            }

            return runners;
        }
    }
}
