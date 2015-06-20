#region License

// Namespace : Stability.Data.Compression.Tests.Utility
// FileName  : TestRunnerFactory.cs
// Created   : 2015-5-16
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO.Compression;
using Stability.Data.Compression.TestUtility;
using Stability.Data.Compression.ThirdParty;
using Stability.Data.Compression.Transforms;

namespace Stability.Data.Compression.Tests.Utility
{
    public static class TestRunnerFactory
    {
        #region Runners

        #region RwcNoFactor

        //public static CodecTestRunner RwcNoFactorOptimal(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcNoFactorFast(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcNoFactorNone(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.NoCompression, FactorMode.None, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcNoFactorOptimalBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        //}

        //public static CodecTestRunner RwcNoFactorFastBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        //}

        //public static CodecTestRunner RwcNoFactorNoneBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.NoCompression, FactorMode.None, isParallel: true, underline: underline);
        //}

        #endregion // RwcNoFactor

        #region RwcAutoFactor

        //public static CodecTestRunner RwcAutoFactorOptimal(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcAutoFactorFast(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcAutoFactorNone(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.NoCompression, FactorMode.Auto, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcAutoFactorOptimalBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        //}

        //public static CodecTestRunner RwcAutoFactorFastBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        //}

        //public static CodecTestRunner RwcAutoFactorNoneBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.NoCompression, FactorMode.Auto, isParallel: true, underline: underline);
        //}

        #endregion // RwcAutoFactor

        #region RwcGranular

        //public static CodecTestRunner RwcGranularOptimal(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        //}
        //public static CodecTestRunner RwcGranularFast(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        //}
        //public static CodecTestRunner RwcGranularNone(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.NoCompression, FactorMode.Granular, isParallel: false, underline: underline);
        //}

        //public static CodecTestRunner RwcGranularOptimalBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        //}
        //public static CodecTestRunner RwcGranularFastBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        //}
        //public static CodecTestRunner RwcGranularNoneBP(bool underline = false)
        //{
        //    return CreateRunner(RandomWalkCodec.Instance, CompressionLevel.NoCompression, FactorMode.Granular, isParallel: true, underline: underline);
        //}

        #endregion // RwcGranular

        #region DeflateDeltaNoFactor

        public static CodecTestRunner DeflateDeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaNoFactorFast(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaNoFactorFastBP(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // DeflateDeltaNoFactor

        #region DeflateDeltaAutoFactor

        public static CodecTestRunner DeflateDeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaAutoFactorFast(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaAutoFactorFastBP(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // DeflateDeltaAutoFactor

        #region DeflateDeltaGranular

        public static CodecTestRunner DeflateDeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaGranularFast(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        public static CodecTestRunner DeflateDeltaGranularFastBP(bool underline = false)
        {
            return CreateRunner(DeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // DeflateDeltaGranular

        #region Deflate (does not support factoring)

        public static CodecTestRunner DeflateOptimal(bool underline = false)
        {
            return CreateRunner(DeflateCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateFast(bool underline = false)
        {
            return CreateRunner(DeflateCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner DeflateOptimalBP(bool underline = false)
        {
            return CreateRunner(DeflateCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner DeflateFastBP(bool underline = false)
        {
            return CreateRunner(DeflateCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // Deflate (does not support factoring)

        #region IonicDeflate (does not support factoring)

        public static CodecTestRunner IonicDeflateOptimal(bool underline = false)
        {
            return CreateRunner(IonicDeflateCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateFast(bool underline = false)
        {
            return CreateRunner(IonicDeflateCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicDeflateFastBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflate (does not support factoring)

        #region IonicDeflateDeltaNoFactor

        public static CodecTestRunner IonicDeflateDeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaNoFactorFast(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaNoFactorFastBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflateDelta

        #region IonicDeflateDeltaAutoFactor

        public static CodecTestRunner IonicDeflateDeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaAutoFactorFast(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaAutoFactorFastBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflateDeltaAutoFactor

        #region IonicDeflateDeltaGranular

        public static CodecTestRunner IonicDeflateDeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaGranularFast(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicDeflateDeltaGranularFastBP(bool underline = false)
        {
            return CreateRunner(IonicDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflateDeltaGranular

        #region IonicZlib (does not support factoring)

        public static CodecTestRunner IonicZlibOptimal(bool underline = false)
        {
            return CreateRunner(IonicZlibCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibFast(bool underline = false)
        {
            return CreateRunner(IonicZlibCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicZlibCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicZlibFastBP(bool underline = false)
        {
            return CreateRunner(IonicZlibCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflate (does not support factoring)

        #region IonicZlibDeltaNoFactor

        public static CodecTestRunner IonicZlibDeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaNoFactorFast(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaNoFactorFastBP(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflateDeltaNoFactor

        #region IonicZlibDeltaAutoFactor

        public static CodecTestRunner IonicZlibDeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaAutoFactorFast(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaAutoFactorFastBP(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflateDeltaAutoFactor

        #region IonicZlibDeltaGranular

        public static CodecTestRunner IonicZlibDeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaGranularFast(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        public static CodecTestRunner IonicZlibDeltaGranularFastBP(bool underline = false)
        {
            return CreateRunner(IonicZlibDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // IonicDeflateDeltaGranular

        #region IonicBZip2 (does not support factoring)

        public static CodecTestRunner IonicBZip2Optimal(bool underline = false)
        {
            return CreateRunner(IonicBZip2Codec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicBZip2OptimalBP(bool underline = false)
        {
            return CreateRunner(IonicBZip2Codec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // IonicBZip2 (does not support factoring)

        #region IonicBZip2DeltaNoFactor

        public static CodecTestRunner IonicBZip2DeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(IonicBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicBZip2DeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // IonicBZip2DeltaNoFactor

        #region IonicBZip2DeltaAutoFactor

        public static CodecTestRunner IonicBZip2DeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(IonicBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicBZip2DeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // IonicBZip2DeltaAutoFactor

        #region IonicBZip2DeltaGranular

        public static CodecTestRunner IonicBZip2DeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(IonicBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner IonicBZip2DeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(IonicBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // IonicBZip2DeltaGranular

        #region QuickLZ (does not support factoring)

        public static CodecTestRunner QuickLZOptimal(bool underline = false)
        {
            return CreateRunner(QuickLZCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZFast(bool underline = false)
        {
            return CreateRunner(QuickLZCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZOptimalBP(bool underline = false)
        {
            return CreateRunner(QuickLZCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner QuickLZFastBP(bool underline = false)
        {
            return CreateRunner(QuickLZCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // QuickLZ (does not support factoring)

        #region QuickLZDeltaNoFactor

        public static CodecTestRunner QuickLZDeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaNoFactorFast(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaNoFactorFastBP(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // QuickLZDeltaNoFactor

        #region QuickLZDeltaAutoFactor

        public static CodecTestRunner QuickLZDeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaAutoFactorFast(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaAutoFactorFastBP(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // QuickLZDeltaAutoFactor

        #region QuickLZDeltaGranular

        public static CodecTestRunner QuickLZDeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaGranularFast(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        public static CodecTestRunner QuickLZDeltaGranularFastBP(bool underline = false)
        {
            return CreateRunner(QuickLZDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // QuickLZDeltaGranular

        #region SharpDeflate (does not support factoring)

        public static CodecTestRunner SharpDeflateOptimal(bool underline = false)
        {
            return CreateRunner(SharpDeflateCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateFast(bool underline = false)
        {
            return CreateRunner(SharpDeflateCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner SharpDeflateFastBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // SharpDeflate (does not support factoring)

        #region SharpDeflateDeltaNoFactor

        public static CodecTestRunner SharpDeflateDeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaNoFactorFast(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaNoFactorFastBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // SharpDeflateDeltaNoFactor

        #region SharpDeflateDeltaAutoFactor

        public static CodecTestRunner SharpDeflateDeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaAutoFactorFast(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaAutoFactorFastBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // SharpDeflateDeltaAutoFactor

        #region SharpDeflateDeltaGranular

        public static CodecTestRunner SharpDeflateDeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaGranularFast(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        public static CodecTestRunner SharpDeflateDeltaGranularFastBP(bool underline = false)
        {
            return CreateRunner(SharpDeflateDeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // SharpDeflateDeltaGranular

        #region SharpBZip2 (does not support factoring)

        public static CodecTestRunner SharpBZip2Optimal(bool underline = false)
        {
            return CreateRunner(SharpBZip2Codec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpBZip2OptimalBP(bool underline = false)
        {
            return CreateRunner(SharpBZip2Codec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // SharpBZip2 (does not support factoring)

        #region SharpBZip2DeltaNoFactor

        public static CodecTestRunner SharpBZip2DeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(SharpBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpBZip2DeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // SharpBZip2DeltaNoFactor

        #region SharpBZip2DeltaAutoFactor

        public static CodecTestRunner SharpBZip2DeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(SharpBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpBZip2DeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // SharpBZip2DeltaAutoFactor

        #region SharpBZip2DeltaGranular

        public static CodecTestRunner SharpBZip2DeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(SharpBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner SharpBZip2DeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(SharpBZip2DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // SharpBZip2DeltaGranular

        #region LZ4 (does not support factoring)

        public static CodecTestRunner LZ4Optimal(bool underline = false)
        {
            return CreateRunner(LZ4Codec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4Fast(bool underline = false)
        {
            return CreateRunner(LZ4Codec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4OptimalBP(bool underline = false)
        {
            return CreateRunner(LZ4Codec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner LZ4FastBP(bool underline = false)
        {
            return CreateRunner(LZ4Codec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // Deflate (does not support factoring)

        #region LZ4DeltaNoFactor

        public static CodecTestRunner LZ4DeltaNoFactorOptimal(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaNoFactorFast(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaNoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.None, isParallel: true, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaNoFactorFastBP(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.None, isParallel: true, underline: underline);
        }

        #endregion // LZ4DeltaNoFactor

        #region LZ4DeltaAutoFactor

        public static CodecTestRunner LZ4DeltaAutoFactorOptimal(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaAutoFactorFast(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaAutoFactorOptimalBP(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Auto, isParallel: true, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaAutoFactorFastBP(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Auto, isParallel: true, underline: underline);
        }

        #endregion // LZ4DeltaAutoFactor

        #region LZ4DeltaGranular

        public static CodecTestRunner LZ4DeltaGranularOptimal(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaGranularFast(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: false, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaGranularOptimalBP(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Optimal, FactorMode.Granular, isParallel: true, underline: underline);
        }

        public static CodecTestRunner LZ4DeltaGranularFastBP(bool underline = false)
        {
            return CreateRunner(LZ4DeltaCodec.Instance, CompressionLevel.Fastest, FactorMode.Granular, isParallel: true, underline: underline);
        }

        #endregion // LZ4DeltaGranular

        public static CodecTestRunner CreateRunner(IDeltaCodec codec, CompressionLevel level, FactorMode factorMode, bool isParallel, bool underline = false)
        {
            var spar = isParallel ? "BP" : "";

            return new CodecTestRunner
            {
                DisplayName = codec.GetType().Name.Replace("Codec", spar),
                Codec = codec,
                Level = level,
                FactorMode = factorMode,
                IsParallel = isParallel,
                Underline = underline,
            };
        }

        #endregion // Runners

        #region Groups

        public static CodecTestGroup GetGroup(TestGroupType group)
        {
            switch (group)
            {
                // Serial Optimal
                case TestGroupType.SerialOptimal:
                    return SerialOptimal();
                case TestGroupType.SerialDeltaNoFactorOptimal:
                    return SerialDeltaNoFactorOptimal();
                case TestGroupType.SerialDeltaAutoFactorOptimal:
                    return SerialDeltaAutoFactorOptimal();
                case TestGroupType.SerialDeltaGranularOptimal:
                    return SerialDeltaGranularOptimal();

                // Serial Fast
                case TestGroupType.SerialFast:
                    return SerialFast();
                case TestGroupType.SerialDeltaNoFactorFast:
                    return SerialDeltaNoFactorFast();
                case TestGroupType.SerialDeltaAutoFactorFast:
                    return SerialDeltaAutoFactorFast();
                case TestGroupType.SerialDeltaGranularFast:
                    return SerialDeltaGranularFast();

                // Parallel Optimal
                case TestGroupType.ParallelOptimal:
                    return ParallelOptimal();
                case TestGroupType.ParallelDeltaNoFactorOptimal:
                    return ParallelDeltaNoFactorOptimal();
                case TestGroupType.ParallelDeltaAutoFactorOptimal:
                    return ParallelDeltaAutoFactorOptimal();
                case TestGroupType.ParallelDeltaGranularOptimal:
                    return ParallelDeltaGranularOptimal();

                // Parallel Fast
                case TestGroupType.ParallelFast:
                    return ParallelFast();
                case TestGroupType.ParallelDeltaNoFactorFast:
                    return ParallelDeltaNoFactorFast();
                case TestGroupType.ParallelDeltaAutoFactorFast:
                    return ParallelDeltaAutoFactorFast();
                case TestGroupType.ParallelDeltaGranularFast:
                    return ParallelDeltaGranularFast();

                // Serial Versus Parallel
                case TestGroupType.SerialVersusParallelOptimal:
                    return SerialVersusParallelOptimal();
                case TestGroupType.SerialVersusParallelDeltaOptimal:
                    return SerialVersusParallelDeltaOptimal();
                case TestGroupType.SerialVersusParallelFast:
                    return SerialVersusParallelFast();
                case TestGroupType.SerialVersusParallelDeltaFast:
                    return SerialVersusParallelDeltaFast();

                // NullTransform Versus Delta
                case TestGroupType.SerialNullTransformVersusDeltaOptimal:
                    return SerialNullTransformVersusDeltaOptimal();
                case TestGroupType.ParallelNullTransformVersusDeltaOptimal:
                    return ParallelNullTransformVersusDeltaOptimal();
                case TestGroupType.SerialNullTransformVersusDeltaFast:
                    return SerialNullTransformVersusDeltaFast();
                case TestGroupType.ParallelNullTransformVersusDeltaFast:
                    return ParallelNullTransformVersusDeltaFast();

                // FactoringComparison
                case TestGroupType.SerialFactoringComparisonOptimal:
                    return SerialFactoringComparisonOptimal();
                case TestGroupType.ParallelFactoringComparisonOptimal:
                    return ParallelFactoringComparisonOptimal();
                case TestGroupType.SerialFactoringComparisonFast:
                    return SerialFactoringComparisonFast();
                case TestGroupType.ParallelFactoringComparisonFast:
                    return ParallelFactoringComparisonFast();

                // OptimalVersusFast
                case TestGroupType.SerialOptimalVersusFast:
                    return SerialDeltaOptimalVersusFast();
                case TestGroupType.SerialDeltaOptimalVersusFast:
                    return SerialOptimalVersusFast();
                case TestGroupType.ParallelOptimalVersusFast:
                    return ParallelOptimalVersusFast();
                case TestGroupType.ParallelDeltaOptimalVersusFast:
                    return ParallelDeltaOptimalVersusFast();

                // Other
                default:
                    throw new ArgumentException("Unknown enum value: CodecTestGroup", "group");
            }
        }

        #region Serial

        public static CodecTestGroup SerialOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialOptimal)
            {
                //RwcGranularOptimal(), 
                //RwcGranularNone(underline: true), // We always include this for RWC
                DeflateOptimal(),
                IonicDeflateOptimal(),
                IonicZlibOptimal(),
                SharpDeflateOptimal(underline: true),
                QuickLZOptimal(),
                LZ4Optimal(underline: true),
                IonicBZip2Optimal(),
                SharpBZip2Optimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaNoFactorOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaNoFactorOptimal)
            {
                //RwcGranularOptimal(), 
                //RwcGranularNone(underline: true),
                DeflateDeltaNoFactorOptimal(),
                IonicDeflateDeltaNoFactorOptimal(),
                IonicZlibDeltaNoFactorOptimal(),
                SharpDeflateDeltaNoFactorOptimal(underline: true),
                QuickLZDeltaNoFactorOptimal(),
                LZ4DeltaNoFactorOptimal(underline: true),
                IonicBZip2DeltaNoFactorOptimal(),
                SharpBZip2DeltaNoFactorOptimal(),
           };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaAutoFactorOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaAutoFactorOptimal)
            {
                //RwcAutoFactorOptimal(), 
                //RwcAutoFactorNone(underline: true),
                DeflateDeltaAutoFactorOptimal(),
                IonicDeflateDeltaAutoFactorOptimal(),
                IonicZlibDeltaAutoFactorOptimal(),
                SharpDeflateDeltaAutoFactorOptimal(underline: true),
                QuickLZDeltaAutoFactorOptimal(),
                LZ4DeltaAutoFactorOptimal(underline: true),
                IonicBZip2DeltaAutoFactorOptimal(),
                SharpBZip2DeltaAutoFactorOptimal(),
           };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaGranularOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaGranularOptimal)
            {
                //RwcGranularOptimal(), 
                //RwcGranularNone(underline: true),
                DeflateDeltaGranularOptimal(),
                IonicDeflateDeltaGranularOptimal(),
                IonicZlibDeltaGranularOptimal(),
                SharpDeflateDeltaGranularOptimal(underline: true),
                QuickLZDeltaGranularOptimal(),
                LZ4DeltaGranularOptimal(underline: true),
                IonicBZip2DeltaGranularOptimal(),
                SharpBZip2DeltaGranularOptimal(),
           };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialFast)
            {
                //RwcGranularFast(), 
                //RwcGranularNone(underline: true),
                DeflateFast(),
                IonicDeflateFast(),
                IonicZlibFast(),
                SharpDeflateFast(underline: true),
                QuickLZFast(),
                LZ4Fast(underline: true),
                IonicBZip2Optimal(),
                SharpBZip2Optimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaNoFactorFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaNoFactorFast)
            {
                //RwcGranularFast(), 
                //RwcGranularNone(underline: true),
                DeflateDeltaNoFactorFast(),
                IonicDeflateDeltaNoFactorFast(),
                IonicZlibDeltaNoFactorFast(),
                SharpDeflateDeltaNoFactorFast(underline: true),
                QuickLZDeltaNoFactorFast(),
                LZ4DeltaNoFactorFast(underline: true),
                IonicBZip2DeltaNoFactorOptimal(),
                SharpBZip2DeltaNoFactorOptimal(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaAutoFactorFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaAutoFactorFast)
            {
                //RwcAutoFactorFast(), 
                //RwcAutoFactorNone(underline: true),
                DeflateDeltaAutoFactorFast(),
                IonicDeflateDeltaAutoFactorFast(),
                IonicZlibDeltaAutoFactorFast(),
                SharpDeflateDeltaAutoFactorFast(underline: true),
                QuickLZDeltaAutoFactorFast(),
                LZ4DeltaAutoFactorFast(underline: true),
                IonicBZip2DeltaAutoFactorOptimal(),
                SharpBZip2DeltaAutoFactorOptimal(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaGranularFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaGranularFast)
            {
                //RwcGranularFast(), 
                //RwcGranularNone(underline: true),
                DeflateDeltaGranularFast(),
                IonicDeflateDeltaGranularFast(),
                IonicZlibDeltaGranularFast(),
                SharpDeflateDeltaGranularFast(underline: true),
                QuickLZDeltaGranularFast(),
                LZ4DeltaGranularFast(underline: true),
                IonicBZip2DeltaGranularOptimal(),
                SharpBZip2DeltaGranularOptimal(),
            };
            group.InitializeRunners();
            return group;
        }

        #endregion // Serial

        #region Parallel

        public static CodecTestGroup ParallelOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelOptimal)
            {
                //RwcGranularOptimalBP(), 
                //RwcGranularNoneBP(underline: true), // We always include this for RWC
                DeflateOptimalBP(),
                IonicDeflateOptimalBP(),
                IonicZlibOptimalBP(),
                SharpDeflateOptimalBP(underline: true),
                QuickLZOptimalBP(),
                LZ4OptimalBP(underline: true),
                IonicBZip2OptimalBP(),
                SharpBZip2OptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaNoFactorOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaNoFactorOptimal)
            {
                //RwcGranularOptimalBP(), 
                //RwcGranularNoneBP(underline: true),
                DeflateDeltaNoFactorOptimalBP(),
                IonicDeflateDeltaNoFactorOptimalBP(),
                IonicZlibDeltaNoFactorOptimalBP(),
                SharpDeflateDeltaNoFactorOptimalBP(underline: true),
                QuickLZDeltaNoFactorOptimalBP(),
                LZ4DeltaNoFactorOptimalBP(underline: true),
                IonicBZip2DeltaNoFactorOptimalBP(),
                SharpBZip2DeltaNoFactorOptimalBP(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaAutoFactorOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaAutoFactorOptimal)
            {
                //RwcAutoFactorOptimalBP(), 
                //RwcAutoFactorNoneBP(underline: true),
                DeflateDeltaAutoFactorOptimalBP(),
                IonicDeflateDeltaAutoFactorOptimalBP(),
                IonicZlibDeltaAutoFactorOptimalBP(),
                SharpDeflateDeltaAutoFactorOptimalBP(underline: true),
                QuickLZDeltaAutoFactorOptimalBP(),
                LZ4DeltaAutoFactorOptimalBP(underline: true),
                IonicBZip2DeltaAutoFactorOptimalBP(),
                SharpBZip2DeltaAutoFactorOptimalBP(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaGranularOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaGranularOptimal)
            {
                //RwcGranularOptimalBP(), 
                //RwcGranularNoneBP(underline: true),
                DeflateDeltaGranularOptimalBP(),
                IonicDeflateDeltaGranularOptimalBP(),
                IonicZlibDeltaGranularOptimalBP(),
                SharpDeflateDeltaGranularOptimalBP(underline: true),
                QuickLZDeltaGranularOptimalBP(),
                LZ4DeltaGranularOptimalBP(underline: true),
                IonicBZip2DeltaGranularOptimalBP(),
                SharpBZip2DeltaGranularOptimalBP(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelFast)
            {
                //RwcGranularFastBP(), 
                //RwcGranularNoneBP(underline: true), // We always include this for RWC
                DeflateFastBP(),
                IonicDeflateFastBP(),
                IonicZlibFastBP(),
                SharpDeflateFastBP(underline: true),
                QuickLZFastBP(),
                LZ4FastBP(underline: true),
                IonicBZip2OptimalBP(), // No fast version
                SharpBZip2OptimalBP(underline: true), // No fast version
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaNoFactorFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaNoFactorFast)
            {
                //RwcGranularFastBP(), 
                //RwcGranularNoneBP(underline: true),
                DeflateDeltaNoFactorFastBP(),
                IonicDeflateDeltaNoFactorFastBP(),
                IonicZlibDeltaNoFactorFastBP(),
                SharpDeflateDeltaNoFactorFastBP(underline: true),
                QuickLZDeltaNoFactorFastBP(),
                LZ4DeltaNoFactorFastBP(underline: true),
                IonicBZip2DeltaNoFactorOptimalBP(),
                SharpBZip2DeltaNoFactorOptimalBP(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaAutoFactorFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaAutoFactorFast)
            {
                //RwcAutoFactorFastBP(), 
                //RwcAutoFactorNoneBP(underline: true),
                DeflateDeltaAutoFactorFastBP(),
                IonicDeflateDeltaAutoFactorFastBP(),
                IonicZlibDeltaAutoFactorFastBP(),
                SharpDeflateDeltaAutoFactorFastBP(underline: true),
                QuickLZDeltaAutoFactorFastBP(),
                LZ4DeltaAutoFactorFastBP(underline: true),
                IonicBZip2DeltaAutoFactorOptimalBP(),
                SharpBZip2DeltaAutoFactorOptimalBP(),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaGranularFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaGranularFast)
            {
                //RwcGranularFastBP(), 
                //RwcGranularNoneBP(underline: true),
                DeflateDeltaGranularFastBP(),
                IonicDeflateDeltaGranularFastBP(),
                IonicZlibDeltaGranularFastBP(),
                SharpDeflateDeltaGranularFastBP(underline: true),
                QuickLZDeltaGranularFastBP(),
                LZ4DeltaGranularFastBP(underline: true),
                IonicBZip2DeltaGranularOptimalBP(),
                SharpBZip2DeltaGranularOptimalBP(),
            };
            group.InitializeRunners();
            return group;
        }

        #endregion // Parallel

        #region SerialVersusParallel

        public static CodecTestGroup SerialVersusParallelOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialVersusParallelOptimal)
            {
                //RwcGranularOptimal(), 
                //RwcGranularOptimalBP(underline: true), 
                DeflateOptimal(),
                DeflateOptimalBP(underline: true),
                IonicDeflateOptimal(),
                IonicDeflateOptimalBP(underline: true),
                IonicZlibOptimal(),
                IonicZlibOptimalBP(underline: true),
                SharpDeflateOptimal(),
                SharpDeflateOptimalBP(underline: true),
                QuickLZOptimal(),
                QuickLZOptimalBP(underline: true),
                LZ4Optimal(),
                LZ4OptimalBP(underline: true),
                IonicBZip2Optimal(),
                IonicBZip2OptimalBP(underline: true),
                SharpBZip2Optimal(),
                SharpBZip2OptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialVersusParallelDeltaOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialVersusParallelDeltaOptimal)
            {
                //RwcGranularOptimal(), 
                //RwcGranularOptimalBP(underline: true), 
                DeflateDeltaGranularOptimal(),
                DeflateDeltaGranularOptimalBP(underline: true),
                IonicDeflateDeltaGranularOptimal(),
                IonicDeflateDeltaGranularOptimalBP(underline: true),
                IonicZlibDeltaGranularOptimal(),
                IonicZlibDeltaGranularOptimalBP(underline: true),
                SharpDeflateDeltaGranularOptimal(),
                SharpDeflateDeltaGranularOptimalBP(underline: true),
                QuickLZDeltaGranularOptimal(),
                QuickLZDeltaGranularOptimalBP(underline: true),
                LZ4DeltaGranularOptimal(),
                LZ4DeltaGranularOptimalBP(underline: true),
                IonicBZip2DeltaGranularOptimal(),
                IonicBZip2DeltaGranularOptimalBP(underline: true),
                SharpBZip2DeltaGranularOptimal(),
                SharpBZip2DeltaGranularOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialVersusParallelFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialVersusParallelFast)
            {
                //RwcGranularFast(), 
                //RwcGranularFastBP(underline: true), 
                DeflateFast(),
                DeflateFastBP(underline: true),
                IonicDeflateFast(),
                IonicDeflateFastBP(underline: true),
                IonicZlibFast(),
                IonicZlibFastBP(underline: true),
                SharpDeflateFast(),
                SharpDeflateFastBP(underline: true),
                QuickLZFast(),
                QuickLZFastBP(underline: true),
                LZ4Fast(),
                LZ4FastBP(underline: true),
                IonicBZip2Optimal(),
                IonicBZip2OptimalBP(underline: true),
                SharpBZip2Optimal(),
                SharpBZip2OptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialVersusParallelDeltaFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialVersusParallelDeltaFast)
            {
                //RwcGranularFast(), 
                //RwcGranularFastBP(underline: true), 
                DeflateDeltaGranularFast(),
                DeflateDeltaGranularFastBP(underline: true),
                IonicDeflateDeltaGranularFast(),
                IonicDeflateDeltaGranularFastBP(underline: true),
                IonicZlibDeltaGranularFast(),
                IonicZlibDeltaGranularFastBP(underline: true),
                SharpDeflateDeltaGranularFast(),
                SharpDeflateDeltaGranularFastBP(underline: true),
                QuickLZDeltaGranularFast(),
                QuickLZDeltaGranularFastBP(underline: true),
                LZ4DeltaGranularFast(),
                LZ4DeltaGranularFastBP(underline: true),
                IonicBZip2DeltaGranularOptimal(),
                IonicBZip2DeltaGranularOptimalBP(underline: true),
                SharpBZip2DeltaGranularOptimal(),
                SharpBZip2DeltaGranularOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        #endregion // SerialVersusParallel

        #region NullTransformVersusDelta

        public static CodecTestGroup SerialNullTransformVersusDeltaOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialNullTransformVersusDeltaOptimal)
            {
                //RwcGranularFast(underline: true), 
                DeflateFast(),
                DeflateDeltaNoFactorFast(underline: true),
                IonicDeflateFast(),
                IonicDeflateDeltaNoFactorFast(underline: true),
                IonicZlibFast(),
                IonicZlibDeltaNoFactorFast(underline: true),
                SharpDeflateFast(),
                SharpDeflateDeltaNoFactorFast(underline: true),
                QuickLZFast(),
                QuickLZDeltaNoFactorFast(underline: true),
                LZ4Fast(),
                LZ4DeltaNoFactorFast(underline: true),
                IonicBZip2Optimal(),
                IonicBZip2DeltaNoFactorOptimal(underline: true),
                SharpBZip2Optimal(),
                SharpBZip2DeltaNoFactorOptimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelNullTransformVersusDeltaOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelNullTransformVersusDeltaOptimal)
            {
                //RwcGranularFastBP(underline: true), 
                DeflateFastBP(),
                DeflateDeltaNoFactorFastBP(underline: true),
                IonicDeflateFastBP(),
                IonicDeflateDeltaNoFactorFastBP(underline: true),
                IonicZlibFastBP(),
                IonicZlibDeltaNoFactorFastBP(underline: true),
                SharpDeflateFastBP(),
                SharpDeflateDeltaNoFactorFastBP(underline: true),
                QuickLZFastBP(),
                QuickLZDeltaNoFactorFastBP(underline: true),
                LZ4FastBP(),
                LZ4DeltaNoFactorFastBP(underline: true),
                IonicBZip2OptimalBP(),
                IonicBZip2DeltaNoFactorOptimalBP(underline: true),
                SharpBZip2OptimalBP(),
                SharpBZip2DeltaNoFactorOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialNullTransformVersusDeltaFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialNullTransformVersusDeltaFast)
            {
                //RwcGranularFast(underline: true), 
                DeflateFast(),
                DeflateDeltaNoFactorFast(underline: true),
                IonicDeflateFast(),
                IonicDeflateDeltaNoFactorFast(underline: true),
                IonicZlibFast(),
                IonicZlibDeltaNoFactorFast(underline: true),
                SharpDeflateFast(),
                SharpDeflateDeltaNoFactorFast(underline: true),
                QuickLZFast(),
                QuickLZDeltaNoFactorFast(underline: true),
                LZ4Fast(),
                LZ4DeltaNoFactorFast(underline: true),
                IonicBZip2Optimal(),
                IonicBZip2DeltaNoFactorOptimal(underline: true),
                SharpBZip2Optimal(),
                SharpBZip2DeltaNoFactorOptimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelNullTransformVersusDeltaFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelNullTransformVersusDeltaFast)
            {
                //RwcGranularFastBP(underline: true), 
                DeflateFastBP(),
                DeflateDeltaNoFactorFastBP(underline: true),
                IonicDeflateFastBP(),
                IonicDeflateDeltaNoFactorFastBP(underline: true),
                IonicZlibFastBP(),
                IonicZlibDeltaNoFactorFastBP(underline: true),
                SharpDeflateFastBP(),
                SharpDeflateDeltaNoFactorFastBP(underline: true),
                QuickLZFastBP(),
                QuickLZDeltaNoFactorFastBP(underline: true),
                LZ4FastBP(),
                LZ4DeltaNoFactorFastBP(underline: true),
                IonicBZip2OptimalBP(),
                IonicBZip2DeltaNoFactorOptimalBP(underline: true),
                SharpBZip2OptimalBP(),
                SharpBZip2DeltaNoFactorOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        #endregion // NullTransformVersusDelta

        #region FactoringComparison

        public static CodecTestGroup SerialFactoringComparisonOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.SerialFactoringComparisonOptimal)
            {
                //RwcNoFactorOptimal(), 
                //RwcAutoFactorOptimal(), 
                //RwcGranularOptimal(underline: true), 
                DeflateDeltaNoFactorOptimal(),
                DeflateDeltaAutoFactorOptimal(),
                DeflateDeltaGranularOptimal(underline: true),
                IonicDeflateDeltaNoFactorOptimal(),
                IonicDeflateDeltaAutoFactorOptimal(),
                IonicDeflateDeltaGranularOptimal(underline: true),
                IonicZlibDeltaNoFactorOptimal(),
                IonicZlibDeltaAutoFactorOptimal(),
                IonicZlibDeltaGranularOptimal(underline: true),
                SharpDeflateDeltaNoFactorOptimal(),
                SharpDeflateDeltaAutoFactorOptimal(),
                SharpDeflateDeltaGranularOptimal(underline: true),
                QuickLZDeltaNoFactorOptimal(),
                QuickLZDeltaAutoFactorOptimal(),
                QuickLZDeltaGranularOptimal(underline: true),
                LZ4DeltaNoFactorOptimal(),
                LZ4DeltaAutoFactorOptimal(),
                LZ4DeltaGranularOptimal(underline: true),
                IonicBZip2DeltaNoFactorOptimal(),
                IonicBZip2DeltaAutoFactorOptimal(),
                IonicBZip2DeltaGranularOptimal(underline: true),
                SharpBZip2DeltaNoFactorOptimal(),
                SharpBZip2DeltaAutoFactorOptimal(),
                SharpBZip2DeltaGranularOptimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelFactoringComparisonOptimal()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelFactoringComparisonOptimal)
            {
                //RwcNoFactorOptimalBP(), 
                //RwcAutoFactorOptimalBP(), 
                //RwcGranularOptimalBP(underline: true), 
                DeflateDeltaNoFactorOptimalBP(),
                DeflateDeltaAutoFactorOptimalBP(),
                DeflateDeltaGranularOptimalBP(underline: true),
                IonicDeflateDeltaNoFactorOptimalBP(),
                IonicDeflateDeltaAutoFactorOptimalBP(),
                IonicDeflateDeltaGranularOptimalBP(underline: true),
                IonicZlibDeltaNoFactorOptimalBP(),
                IonicZlibDeltaAutoFactorOptimalBP(),
                IonicZlibDeltaGranularOptimalBP(underline: true),
                SharpDeflateDeltaNoFactorOptimalBP(),
                SharpDeflateDeltaAutoFactorOptimalBP(),
                SharpDeflateDeltaGranularOptimalBP(underline: true),
                QuickLZDeltaNoFactorOptimalBP(),
                QuickLZDeltaAutoFactorOptimalBP(),
                QuickLZDeltaGranularOptimalBP(underline: true),
                LZ4DeltaNoFactorOptimalBP(),
                LZ4DeltaAutoFactorOptimalBP(),
                LZ4DeltaGranularOptimalBP(underline: true),
                IonicBZip2DeltaNoFactorOptimalBP(),
                IonicBZip2DeltaAutoFactorOptimalBP(),
                IonicBZip2DeltaGranularOptimalBP(underline: true),
                SharpBZip2DeltaNoFactorOptimalBP(),
                SharpBZip2DeltaAutoFactorOptimalBP(),
                SharpBZip2DeltaGranularOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialFactoringComparisonFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialFactoringComparisonFast)
            {
                //RwcNoFactorFast(), 
                //RwcAutoFactorFast(), 
                //RwcGranularFast(underline: true), 
                DeflateDeltaNoFactorFast(),
                DeflateDeltaAutoFactorFast(),
                DeflateDeltaGranularFast(underline: true),
                IonicDeflateDeltaNoFactorFast(),
                IonicDeflateDeltaAutoFactorFast(),
                IonicDeflateDeltaGranularFast(underline: true),
                IonicZlibDeltaNoFactorFast(),
                IonicZlibDeltaAutoFactorFast(),
                IonicZlibDeltaGranularFast(underline: true),
                SharpDeflateDeltaNoFactorFast(),
                SharpDeflateDeltaAutoFactorFast(),
                SharpDeflateDeltaGranularFast(underline: true),
                QuickLZDeltaNoFactorFast(),
                QuickLZDeltaAutoFactorFast(),
                QuickLZDeltaGranularFast(underline: true),
                LZ4DeltaNoFactorFast(),
                LZ4DeltaAutoFactorFast(),
                LZ4DeltaGranularFast(underline: true),
                IonicBZip2DeltaNoFactorOptimal(),
                IonicBZip2DeltaAutoFactorOptimal(),
                IonicBZip2DeltaGranularOptimal(underline: true),
                SharpBZip2DeltaNoFactorOptimal(),
                SharpBZip2DeltaAutoFactorOptimal(),
                SharpBZip2DeltaGranularOptimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelFactoringComparisonFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelFactoringComparisonFast)
            {
                //RwcNoFactorFastBP(), 
                //RwcAutoFactorFastBP(), 
                //RwcGranularFastBP(underline: true), 
                DeflateDeltaNoFactorFastBP(),
                DeflateDeltaAutoFactorFastBP(),
                DeflateDeltaGranularFastBP(underline: true),
                IonicDeflateDeltaNoFactorFastBP(),
                IonicDeflateDeltaAutoFactorFastBP(),
                IonicDeflateDeltaGranularFastBP(underline: true),
                IonicZlibDeltaNoFactorFastBP(),
                IonicZlibDeltaAutoFactorFastBP(),
                IonicZlibDeltaGranularFastBP(underline: true),
                SharpDeflateDeltaNoFactorFastBP(),
                SharpDeflateDeltaAutoFactorFastBP(),
                SharpDeflateDeltaGranularFastBP(underline: true),
                QuickLZDeltaNoFactorFastBP(),
                QuickLZDeltaAutoFactorFastBP(),
                QuickLZDeltaGranularFastBP(underline: true),
                LZ4DeltaNoFactorFastBP(),
                LZ4DeltaAutoFactorFastBP(),
                LZ4DeltaGranularFastBP(underline: true),
                IonicBZip2DeltaNoFactorOptimalBP(),
                IonicBZip2DeltaAutoFactorOptimalBP(),
                IonicBZip2DeltaGranularOptimalBP(underline: true),
                SharpBZip2DeltaNoFactorOptimalBP(),
                SharpBZip2DeltaAutoFactorOptimalBP(),
                SharpBZip2DeltaGranularOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        #endregion // FactoringComparison

        #region OptimalVersusFast

        public static CodecTestGroup SerialOptimalVersusFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialOptimalVersusFast)
            {
                //RwcGranularOptimal(), 
                //RwcGranularFast(underline: true), 
                DeflateOptimal(),
                DeflateFast(underline: true),
                IonicDeflateOptimal(),
                IonicDeflateFast(underline: true),
                IonicZlibOptimal(),
                IonicZlibFast(underline: true),
                SharpDeflateOptimal(),
                SharpDeflateFast(underline: true),
                QuickLZOptimal(),
                QuickLZFast(underline: true),
                LZ4Optimal(),
                LZ4Fast(underline: true),
                IonicBZip2Optimal(),
                SharpBZip2Optimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup SerialDeltaOptimalVersusFast()
        {
            var group = new CodecTestGroup(TestGroupType.SerialDeltaOptimalVersusFast)
            {
                //RwcGranularOptimal(), 
                //RwcGranularFast(underline: true), 
                DeflateDeltaGranularOptimal(),
                DeflateDeltaGranularFast(underline: true),
                IonicDeflateDeltaGranularOptimal(),
                IonicDeflateDeltaGranularFast(underline: true),
                IonicZlibDeltaGranularOptimal(),
                IonicZlibDeltaGranularFast(underline: true),
                SharpDeflateDeltaGranularOptimal(),
                SharpDeflateDeltaGranularFast(underline: true),
                QuickLZDeltaGranularOptimal(),
                QuickLZDeltaGranularFast(underline: true),
                LZ4DeltaGranularOptimal(),
                LZ4DeltaGranularFast(underline: true),
                IonicBZip2DeltaGranularOptimal(),
                SharpBZip2DeltaGranularOptimal(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelOptimalVersusFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelOptimalVersusFast)
            {
                //RwcGranularOptimalBP(), 
                //RwcGranularFastBP(underline: true), 
                DeflateOptimalBP(),
                DeflateFastBP(underline: true),
                IonicDeflateOptimalBP(),
                IonicDeflateFastBP(underline: true),
                IonicZlibOptimalBP(),
                IonicZlibFastBP(underline: true),
                SharpDeflateOptimalBP(),
                SharpDeflateFastBP(underline: true),
                QuickLZOptimalBP(),
                QuickLZFastBP(underline: true),
                LZ4OptimalBP(),
                LZ4FastBP(underline: true),
                IonicBZip2OptimalBP(),
                SharpBZip2OptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        public static CodecTestGroup ParallelDeltaOptimalVersusFast()
        {
            var group = new CodecTestGroup(TestGroupType.ParallelDeltaOptimalVersusFast)
            {
                //RwcGranularOptimalBP(), 
                //RwcGranularFastBP(underline: true), 
                DeflateDeltaGranularOptimalBP(),
                DeflateDeltaGranularFastBP(underline: true),
                IonicDeflateDeltaGranularOptimalBP(),
                IonicDeflateDeltaGranularFastBP(underline: true),
                IonicZlibDeltaGranularOptimalBP(),
                IonicZlibDeltaGranularFastBP(underline: true),
                SharpDeflateDeltaGranularOptimalBP(),
                SharpDeflateDeltaGranularFastBP(underline: true),
                QuickLZDeltaGranularOptimalBP(),
                QuickLZDeltaGranularFastBP(underline: true),
                LZ4DeltaGranularOptimalBP(),
                LZ4DeltaGranularFastBP(underline: true),
                IonicBZip2DeltaGranularOptimalBP(),
                SharpBZip2DeltaGranularOptimalBP(underline: true),
            };
            group.InitializeRunners();
            return group;
        }

        #endregion // OptimalVersusFast

        #endregion // Group

    }
}
