#region Derivative Work License (Stability.Data.Compression.ThirdParty)

// Disclaimer: All of the compression algorithms in this assembly are the work of others.
//             They are aggregated here to provide an easy way to learn about and test
//             alternative techniques. In the subfolders "Internal\<libname>" you will
//             find the minimal subset of files needed to expose each algorithm.
//             In the "Licenses" folder you will find the licensing information for each
//             of the third-party libraries. Those licenses (if more restrictive than
//             GPL v3) are meant to override.
//
// Namespace : Stability.Data.Compression.ThirdParty
// FileName  : LZ4Codec.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile (Original and Derivative Work)
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // Derivative Work License (Stability.Data.Compression.ThirdParty)

using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Transforms;

namespace Stability.Data.Compression.ThirdParty
{
    public sealed class LZ4Codec : DeltaCodec<NullTransform>
    {
        #region Constructors

        /// <summary>
        /// We rely on the base class to expose all of the standard encoding and decoding methods.
        /// What we would want to do here is expose more advanced methods that perhaps handle
        /// complex time series types. For example, we might have multidimensional data objects
        /// such as, say, "MarketBars" with DateTime, Open, High, Low, Close, Volume, OpenInterest.
        /// We would need to provide the "Framing" semantics to encode and decode the various fields.
        /// We can either use the default Finisher for all fields. Or we can provide different
        /// finishers for each.
        /// 
        /// Even if you plan on using multiple finishers, you need to provide a default. Although
        /// the base class and all <see cref="IDeltaTransform"/> implementations should provide
        /// a default, it is considered a bad idea to get lazy about it. Be explicit!
        /// 
        /// The default finisher is <see cref="DeflateFinisher"/> for all implementations
        /// included with the library. When you derive your own types, be sure to pick something
        /// with licensing appropriate to your situation. The third-party finishers included are
        /// selected because of their liberal open-source permissions.
        /// 
        /// The <see cref="IDeltaTransform"/> implementations included in this library are very
        /// basic. Use them as a starting point for exploring more advanced performance.
        /// </summary>
        private LZ4Codec()
            : this(LZ4Finisher.Instance)
        {
        }

        /// <summary>
        /// By providing a different default finisher you can experiment to find the best match
        /// for your particular times series data and selected <see cref="IDeltaTransform"/>.
        /// Since this is declared with the internal access modifier, you are reminded that this
        /// should only to be used for testing purposes. If you want several different finishers
        /// for particular data series that you deal with, then you should create specific
        /// implementations that will always use the same transform and finisher combination.
        /// Keep in mind that if you use an alternative finisher when directly creating instances 
        /// for testing, then your static instance will not be affected. This is actually useful
        /// for comparing the performance of the standard finisher side-by-side with prospective
        /// alternatives.
        /// </summary>
        /// <param name="defaultFinisher">
        /// The assembly shares internals with the test projects included with the library. 
        /// In production environments you should always be using the "factory" instance. 
        /// That's because there will otherwise not be any reasonable guarantee that encoded 
        /// frames or blocks can be deserialized properly. You may not be able to detect and
        /// instantiate the types you used in ad hoc testing scenarios. That is also the
        /// reason we declare codec implementations sealed. If you choose to ignore this
        /// convention then you are doing so at your own peril.
        /// </param>
        internal LZ4Codec(IFinisher defaultFinisher)
            : base(defaultFinisher)
        {

        }

        #endregion // Constructors

        #region Static

        /// <summary>
        /// The static "Instance" property gets initialized on first use. This makes it easier
        /// to reuse a single codec when there is any kind of more complicated setup logic that
        /// must apply across all instances.
        /// </summary>
        /// <remarks>
        /// The implementor of advanced codecs must ensure that the instance reflects any
        /// setup logic required.
        /// </remarks>
        static LZ4Codec()
        {
            Instance = new LZ4Codec();
        }

        public static LZ4Codec Instance { get; protected set; }

        #endregion // Static

    }
}
