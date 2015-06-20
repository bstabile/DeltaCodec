#region License

// Namespace : Stability.Data.Compression
// FileName  : Monotonicity.cs
// Created   : 2015-4-22
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability.Data.Compression
{
    public enum Monotonicity
    {
        None = 0,
        NonDecreasing = 1,
        NonIncreasing = 2,
    }
}
