#region Derivative Work License (Stability.Data.Compression.ThirdParty)

// Disclaimer: All of the compression algorithms in this assembly are the work of others.
//             They are aggregated here to provide an easy way to learn about and test
//             alternative techniques. In the subfolders "Internal\<libname>" you will
//             find the minimal subset of files needed to expose each algorithm.
//             In the "Licenses" folder you will find the licensing information for each
//             of the third-party libraries. Those licenses (if more restrictive than
//             GPL v3) are meant to override.
//
// Namespace : Stability.Data.Compression.ThirdParty.Internal.DotNetZip.BZip2
// FileName  : BZip2.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)
namespace Stability.Data.Compression.ThirdParty.Internal.DotNetZip.BZip2
{
    internal static class BZip2
    {
        internal static T[][] InitRectangularArray<T>(int d1, int d2)
        {
            var x = new T[d1][];
            for (int i = 0; i < d1; i++)
            {
                x[i] = new T[d2];
            }
            return x;
        }

        public static readonly int BlockSizeMultiple = 100000;
        public static readonly int MinBlockSize = 1;
        public static readonly int MaxBlockSize = 9;
        public static readonly int MaxAlphaSize = 258;
        public static readonly int MaxCodeLength = 23;
        public static readonly char RUNA = (char)0;
        public static readonly char RUNB = (char)1;
        public static readonly int NGroups = 6;
        public static readonly int G_SIZE = 50;
        public static readonly int N_ITERS = 4;
        public static readonly int MaxSelectors = (2 + (900000 / G_SIZE));
        public static readonly int NUM_OVERSHOOT_BYTES = 20;
        /*
         * <p> If you are ever unlucky/improbable enough to get a stack
         * overflow whilst sorting, increase the following constant and
         * try again. In practice I have never seen the stack go above 27
         * elems, so the following limit seems very generous.  </p>
         */
        internal static readonly int QSORT_STACK_SIZE = 1000;


    }
}
