#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : TestHelper.cs
// Created   : 2015-5-16
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : All Rights Reserved
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License

using System;
using System.Collections.Generic;
using System.Threading;
using Stability.Data.Compression.Utility;

namespace Stability.Data.Compression.TestUtility
{
    public static class TestHelper
    {
        public static CodecTestGroup RunEncodingTests<T>(CodecTestGroup runners, IList<T> list, int runCount = 2, 
            T granularity = default(T), Monotonicity monotonicity = Monotonicity.None, bool validate = true)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            // We don't want to disrupt the tests by having to spin up a standard number of threads
            // for parallel block encoding. So we set those now and hope that we don't increase contention!
            int minWorker;
            int minIoc;
            var defaultMinWorker = Environment.ProcessorCount*2;
            ThreadPool.GetMinThreads(out minWorker, out minIoc);
            if (minWorker < defaultMinWorker)
                ThreadPool.SetMinThreads(defaultMinWorker, minIoc);

            // Entropy of Input
            var bufferEntropy = Statistics.BufferEntropy(DeltaUtility.ConvertToBuffer(list)) / 8; // Divisor is always 8 bits for BufferEntropy

            var deltaEntropy = Statistics.DeltaEntropy(list) / DeltaUtility.BitCount(typeof(T));

            foreach (var r in runners)
            {
                r.BufferEntropy = bufferEntropy;
                r.DeltaEntropy = deltaEntropy;
                r.RunStructTest(runCount, list, granularity, monotonicity, validate);

                // Run() creates the tests in the runner!
                foreach (var test in r.Tests)
                {
                    var t = test as CodecStructTest<T>;
                    if (t != null)
                        t.Stats.EntropyIn = bufferEntropy;
                }
            }

            return runners;
        }
        public static CodecTestGroup RunClassEncodingTests<T>(CodecTestGroup runners, IList<T> list, int runCount = 2,
            bool validate = true)
            where T : class, IComparable<T>, IEquatable<T>
        {
            // We don't want to disrupt the tests by having to spin up a standard number of threads
            // for parallel block encoding. So we set those now and hope that we don't increase contention!
            int minWorker;
            int minIoc;
            var defaultMinWorker = Environment.ProcessorCount * 2;
            ThreadPool.GetMinThreads(out minWorker, out minIoc);
            if (minWorker < defaultMinWorker)
                ThreadPool.SetMinThreads(defaultMinWorker, minIoc);

            // Entropy of Input
            // Divisor is always 8 bits for BufferEntropy
            var bufferEntropy = Statistics.BufferEntropy(DeltaUtility.ConvertToBuffer(list)) / 8; 

            var deltaEntropy = bufferEntropy;

            foreach (CodecTestRunner r in runners)
            {
                r.BufferEntropy = bufferEntropy;
                r.DeltaEntropy = deltaEntropy;
                r.RunClassTest(runCount, list, validate);

                // Run() creates the tests in the runner!
                foreach (var test in r.Tests)
                {
                    test.Stats.EntropyIn = bufferEntropy;
                }
            }

            return runners;
        }
    }
}
