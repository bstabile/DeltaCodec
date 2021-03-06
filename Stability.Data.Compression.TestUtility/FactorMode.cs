﻿#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : FactorMode.cs
// Created   : 2015-5-28
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

namespace Stability.Data.Compression.TestUtility
{
    public enum FactorMode
    {
        None = 0,
        Granular = 1,
        Auto = 2,
        All = 3, // CodecConfigs cloned: "FactoringComparison"
    }
}
