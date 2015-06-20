#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : TestGroupType.cs
// Created   : 2015-5-29
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
namespace Stability.Data.Compression.TestUtility
{
    public enum TestGroupType
    {
        None,
        Custom,
        // Serial
        SerialFast,
        SerialOptimal,
        SerialDeltaNoFactorFast,
        SerialDeltaNoFactorOptimal,
        SerialDeltaAutoFactorFast,
        SerialDeltaAutoFactorOptimal,
        SerialDeltaGranularFast,
        SerialDeltaGranularOptimal,
        // Parallel
        ParallelFast,
        ParallelOptimal,
        ParallelDeltaNoFactorFast,
        ParallelDeltaNoFactorOptimal,
        ParallelDeltaAutoFactorFast,
        ParallelDeltaAutoFactorOptimal,
        ParallelDeltaGranularFast,
        ParallelDeltaGranularOptimal,
        // Serial Versus Parallel
        SerialVersusParallelFast,
        SerialVersusParallelOptimal,
        SerialVersusParallelDeltaFast,
        SerialVersusParallelDeltaOptimal,
        // NullTransformVersusDelta
        SerialNullTransformVersusDeltaFast,
        SerialNullTransformVersusDeltaOptimal,
        ParallelNullTransformVersusDeltaFast,
        ParallelNullTransformVersusDeltaOptimal,
        // OptimalVersusFast
        SerialOptimalVersusFast,
        SerialDeltaOptimalVersusFast,
        ParallelOptimalVersusFast,
        ParallelDeltaOptimalVersusFast,
        // FactoringComparison
        SerialFactoringComparisonFast,
        SerialFactoringComparisonOptimal,
        ParallelFactoringComparisonFast,
        ParallelFactoringComparisonOptimal,
    }
}
