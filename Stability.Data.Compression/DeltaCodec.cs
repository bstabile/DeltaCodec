#region License

// Namespace : Stability.Data.Compression
// FileName  : DeltaCodec.cs
// Created   : 2015-4-25
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.IO;
using Stability.Data.Compression.Finishers;
using Stability.Data.Compression.Transforms;

namespace Stability.Data.Compression
{
    /// <summary>
    /// This non-generic "base" class is ONLY used for shared constants and static members.
    /// Anything that needs to be adjusted individually in constructed types should be added 
    /// to the generic class definition. We only currently use this to do sanity checks on
    /// the number of blocks specified by the client. If the number is too high, this will
    /// almost surely degrade performance very significantly. A core multiplier of greater
    /// than 4 is almost certainly not a good idea.
    /// </summary>
    /// <remarks>
    /// Remember that ProcessorCount is based on logical cores, not on physical processors!
    /// </remarks>
    public abstract class DeltaCodec
    {
        /// <summary>
        /// The TPL seems to pick 3 as the default (non-dynamic) multiplier.
        /// It also usually adds 1 to the total number of partition ranges.
        /// (which is only to handle residuals). This might make sense when
        /// only a few trivial lines of code need to be executed. But for 
        /// relatively complex algorithms it doesn't make any sense at all.
        /// We also want even distributions in our range partitioning. 
        /// In testing, it seems as though using a multiplier of 1 
        /// results in the best overall performance.
        /// </summary>
        /// <remarks>
        /// Compression algorithms are compute intensive. The cost of context
        /// switching is something we most definitely want to try and limit.
        /// We also want to avoid writing unnecessary block header information.
        /// The only reason for defining these values here is so that clients
        /// can avoid making ad hoc decisions about the value of the "numBlocks"
        /// parameter. If there is a good reason for using something a little
        /// different, that is fine. But we always use max values to throttle.
        /// </remarks>
        public const int DefaultCoreMultiplier = 1;
        public const int MaxCoreMultiplier = 4;

        public static readonly int AbsMaxNumParallelBlocks = 255;
        public static readonly int DefaultNumParallelBlocks = 
            Math.Min(AbsMaxNumParallelBlocks, Environment.ProcessorCount * DefaultCoreMultiplier);
        public static readonly int MaxNumParallelBlocks = 
            Math.Min(AbsMaxNumParallelBlocks, Environment.ProcessorCount * MaxCoreMultiplier);
    }

    public abstract partial class DeltaCodec<TTransform> : DeltaCodec, IDeltaCodec
        where TTransform : IDeltaTransform, new()
    {
        #region Private Static Fields

        private static readonly IDeltaTransform TransformInstance = new TTransform();

        #endregion // Private Static Fields

        #region Constructors

        /// <summary>
        /// Using the default constructor means the finisher will be <see cref="DeflateFinisher"/>.
        /// That is just a wrapper around <see cref="System.IO.Compression.DeflateStream"/>.
        /// In most cases this seems to be a good choice when your <see cref="IDeltaTransform"/>
        /// implementation is doing a reasonable job of sqeezing out redundant information.
        /// </summary>
        protected DeltaCodec()
        {
            // This just establishes a default name in case the derivation is being lazy :)
            DisplayName = GetType().Name;
        }

        /// <summary>
        /// Derivations that use an alternative <see cref="IFinisher"/> will use this
        /// constructor. The finisher can be changed internally but we don't normally 
        /// want clients to be passing in finishers at run time. That's because
        /// some coordination between the encoding and decoding needs to be maintained.
        /// To get around this (e.g. for testing), direct calls to the transform
        /// methods can be used.
        /// </summary>
        /// <param name="defaultFinisher">
        /// This will normally be passed to all tranform methods.
        /// </param>
        protected DeltaCodec(IFinisher defaultFinisher) : this()
        {
            // We always need to have a finisher available!
            DefaultFinisher = defaultFinisher ?? DeflateFinisher.Instance;
        }

        #endregion // Constructors

        #region Public Properties

        public int MagicNumber { get { return GetType().FullName.GetHashCode(); } }

        public string DisplayName { get; protected set; }

        /// <summary>
        /// Derivations can change the  default finisher as needed. But setting it
        /// to null will raise an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <remarks>
        /// If multiple finishers are needed for whatever reason, individual
        /// encoding and deconding methods will need to be overridden.
        /// </remarks>
        public IFinisher DefaultFinisher
        {
            get { return _defaultFinisher; }
            protected set
            {
                if (_defaultFinisher == value) return;
                if (value == null)
                    throw new ArgumentNullException("value","DefaultFinisher cannot be null.");
                _defaultFinisher = value;
            }
        }
        private IFinisher _defaultFinisher = DeflateFinisher.Instance;

        public static IDeltaTransform Transform
        {
            get { return TransformInstance; }
        }

        #endregion // Public Properties

        #region Protected Methods

        protected List<byte[]>[] ReadEncodedBlocks(byte[] bytes, int numVectorsExpected, out ushort flags)
        {
            var blocks = new List<byte[]>[numVectorsExpected];
            for (var i = 0; i < numVectorsExpected; i++)
            {
                blocks[i] = new List<byte[]>();
            }
            using (var ms = new MemoryStream(bytes))
            using (var reader = new BinaryReader(ms))
            {
                var magicNumber = reader.ReadInt32();
                if (magicNumber != MagicNumber)
                {
                    throw new InvalidOperationException("Codec hashcode does not match the encoded magic number!");
                }

                var numBlocks = (int)reader.ReadByte();
                var numVectors = reader.ReadByte();
                if (numVectors != numVectorsExpected)
                    throw new BadImageFormatException(string.Format(
                        "The number of vectors is invalid! (Expected={0}, Actual={1})", numVectorsExpected, numVectors));

                flags = reader.ReadUInt16(); // reserved for future use

                for (var i = 0; i < numBlocks; i++)
                {
                    for (var j = 0; j < numVectors; j++)
                    {
                        var n = reader.ReadInt32();
                        var b = reader.ReadBytes(n);
                        blocks[j].Add(b);
                    }
                }
            }
            return blocks;
        }

        protected byte[] WriteEncodedBlocks(int numBlocks, ushort flags, List<byte[][]> encodedBlocks)
        {
            var numVectors = encodedBlocks.Count;

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(MagicNumber);
                writer.Write((byte) numBlocks); // byte
                writer.Write((byte) numVectors); // byte
                writer.Write(flags); // ushort
                for (var i = 0; i < numBlocks; i++)
                {
                    for (var j = 0; j < numVectors; j++)
                    {
                        var block = encodedBlocks[j][i];
                        writer.Write(block.Length);
                        writer.Write(block);
                    }
                }
                return ms.ToArray();
            }
        }

        #endregion // Protected Methods
    }
}
