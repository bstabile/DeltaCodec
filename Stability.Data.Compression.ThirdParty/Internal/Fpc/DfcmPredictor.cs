#region Derivative Work License (Stability.Data.Compression.ThirdParty)

// Disclaimer: All of the compression algorithms in this assembly are the work of others.
//             They are aggregated here to provide an easy way to learn about and test
//             alternative techniques. In the subfolders "Internal\<libname>" you will
//             find the minimal subset of files needed to expose each algorithm.
//             In the "Licenses" folder you will find the licensing information for each
//             of the third-party libraries. Those licenses (if more restrictive than
//             GPL v3) are meant to override.
//
// Namespace : Stability.Data.Compression.ThirdParty.Internal.Fpc
// FileName  : DfcmPredictor.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)
namespace Stability.Data.Compression.ThirdParty.Internal.Fpc
{
    public class DfcmPredictor
    {
        private readonly long[] _table;
        private int _dfcmHash;
        private long _lastValue;

        public DfcmPredictor(int logOfTableSize)
        {
            _table = new long[1 << logOfTableSize];
        }

        public long GetPrediction()
        {
            return _table[_dfcmHash] + _lastValue;
        }

        public void Update(long trueValue)
        {
            _table[_dfcmHash] = trueValue - _lastValue;
            _dfcmHash = (int)(((_dfcmHash << 2) ^ ((trueValue - _lastValue) >> 40)) &
                    (_table.Length - 1));
            _lastValue = trueValue;
        }

    }
}
