#region License

// Namespace : Stability.Data.Compression.Utility
// FileName  : OrderedRangeFactory.cs
// Created   : 2015-5-21
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Stability.Data.Compression.Utility
{
    /// <summary>
    /// This factory produces a requested number of sub-ranges, all distributed as evenly as possible.
    /// It returns a list of strongly typed range structs (Range32, Range64, IndexedRange32, or IndexedRange64).
    /// RangeXX Fields: [InclusiveStart, ExclusiveStop).
    /// IndexedRangeXX Fields: Index, [InclusiveStart, ExclusiveStop).
    /// </summary>
    /// <remarks>
    /// The stuct layout is explicit for all range types so that marshaling to native code is safe.
    /// The structs are immutable (read-only properties), so they should be threadsafe even when boxed.
    /// Obviously is you use pointers or manipute these in native code you're on your own!
    /// Primary Use Case: TPL DataFlow Static Ordered Partitioning.
    /// </remarks>
    public static class OrderedRangeFactory
    {
        public static IList<Range32> Create(int inclusiveStart, int exclusiveStop, int numRanges)
        {
            if (inclusiveStart < 0)
                throw new ArgumentException("The lower bound must be >= 0", "inclusiveStart");
            if (exclusiveStop < 0)
                throw new ArgumentException("The upper bound must be > 0", "exclusiveStop");
            if (inclusiveStart > exclusiveStop)
                throw new ArgumentException("The lower bound cannot be greater than the upper bound!", "inclusiveStart");
            if (numRanges < 0)
                throw new ArgumentException("The number of ranges requested must be non-negative.", "numRanges");

            var ranges = new List<Range32>(numRanges);
            var count = exclusiveStop - inclusiveStart;
            var baseSize = count / numRanges;
            var remainder = (count - numRanges * baseSize) / numRanges;
            var size = baseSize + remainder;
            for (var i = 0; i < numRanges; i++)
            {
                var offset = i * size;
                var ubound = i == numRanges - 1 ? count : offset + size;
                ranges.Add(new Range32(i, offset, ubound));
            }
            return ranges;
        }
        public static IList<Range64> Create(long inclusiveStart, long exclusiveStop, int numRanges)
        {
            if (inclusiveStart < 0)
                throw new ArgumentException("The lower bound must be >= 0", "inclusiveStart");
            if (exclusiveStop < 0)
                throw new ArgumentException("The upper bound must be > 0", "exclusiveStop");
            if (inclusiveStart > exclusiveStop)
                throw new ArgumentException("The lower bound cannot be greater than the upper bound!", "inclusiveStart");
            if (numRanges < 0)
                throw new ArgumentException("The number of ranges requested must be non-negative.", "numRanges");

            var ranges = new List<Range64>(numRanges);
            var count = exclusiveStop - inclusiveStart;
            var baseSize = count / numRanges;
            var remainder = (count - numRanges * baseSize) / numRanges;
            var size = baseSize + remainder;
            for (var i = 0; i < numRanges; i++)
            {
                var offset = i * size;
                var ubound = i == numRanges - 1 ? count : offset + size;
                ranges.Add(new Range64(i, offset, ubound));
            }
            return ranges;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Range32
    {
        public Range32(int index, int inclusiveStart, int exclusiveStop)
        {
            _index = index;
            _inclusiveStart = inclusiveStart;
            _exclusiveStop = exclusiveStop;
        }

        [FieldOffset(0)]
        private readonly int _index;
        [FieldOffset(4)]
        private readonly int _inclusiveStart;
        [FieldOffset(8)]
        private readonly int _exclusiveStop;

        public int Index { get { return _index; } }
        public int InclusiveStart { get { return _inclusiveStart; } }
        public int ExclusiveStop { get { return _exclusiveStop; } }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Range64
    {
        public Range64(int index, long inclusiveStart, long exclusiveStop)
        {
            _index = index;
            _inclusiveStart = inclusiveStart;
            _exclusiveStop = exclusiveStop;
        }

        [FieldOffset(0)]
        private readonly int _index;
        [FieldOffset(4)]
        private readonly long _inclusiveStart;
        [FieldOffset(12)]
        private readonly long _exclusiveStop;

        public int Index { get { return _index; } }
        public long InclusiveStart { get { return _inclusiveStart; } }
        public long ExclusiveStop { get { return _exclusiveStop; } }
    }
}
