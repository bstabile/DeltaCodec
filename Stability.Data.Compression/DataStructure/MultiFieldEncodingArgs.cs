#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : MultiFieldEncodingArgs.cs
// Created   : 2015-7-3
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.IO.Compression;

namespace Stability.Data.Compression.DataStructure
{
    public interface IMultiFieldEncodingArgs
    {
        /// <summary>
        /// This is to expose the List when the generic type is unknown (e.g. agnostic tests).
        /// Since the return type is dynamic the client should test the type before use.
        /// It is also used to retrieve the element count in non-generic base classes.
        /// </summary>
        dynamic DynamicData { get; }

        int DataCount { get; }
        int NumBlocks { get; set; }

        CompressionLevel[] Levels { get; }
        object[] Granularities { get; }
        Monotonicity[] Monotonicities { get; }

        dynamic Custom { get; set; }

        void ResetLevels();
        void ResetLevels(CompressionLevel level);

        void ResetMonotonicities();
        void ResetMonotonicities(Monotonicity monotonicity);

        void ResetGranularities();
    }

    /// <summary>
    /// The generic <see cref="Tuple"/> and <see cref="IStruple"/> (structure tuple) 
    /// types require an ability to specify arguments for multiple fields. 
    /// This family of generic argument classes make it easier to pass the required information.
    /// </summary>
    /// 
    public abstract class MultiFieldEncodingArgs : IMultiFieldEncodingArgs
    {
        public const CompressionLevel DefaultLevel = CompressionLevel.Fastest;
        public const Monotonicity DefaultMonotonicity = Monotonicity.None;

        protected MultiFieldEncodingArgs(
            int numBlocks = 1, 
            dynamic custom = null)
        {
            NumBlocks = numBlocks;
            Custom = custom;
        }

       /// <summary>
        /// Derived types should implement this by returning the strongly-typed List.
        /// </summary>
        public abstract dynamic DynamicData { get; }

        /// <summary>
        /// Allows clients to get the count of elements without casting to the generic list type.
        /// </summary>
        public int DataCount
        {
            get { return DynamicData == null ? 0 : DynamicData.Count; }
        }

        /// <summary>
        /// The number of blocks that will encoded/decoded in parallel.
        /// </summary>
        public int NumBlocks { get; set; }

        public CompressionLevel[] Levels { get; protected set; }
        public object[] Granularities { get; protected set; }
        public Monotonicity[] Monotonicities { get; protected set; }

        public dynamic Custom { get; set; }

        /// <summary>
        /// Resets compression level for all fields.
        /// </summary>
        public void ResetLevels()
        {
            ResetLevels(DefaultLevel);
        }

        /// <summary>
        /// Resets compression level for all fields.
        /// </summary>
        public void ResetLevels(CompressionLevel level)
        {
            for (var i = 0; i < Levels.Length; i++)
            {
                Levels[i] = level;
            }
        }

        /// <summary>
        /// Resets monotonicity for all fields to <see cref="Monotonicity.None"/>.
        /// </summary>
        public void ResetMonotonicities()
        {
            ResetMonotonicities(DefaultMonotonicity);
        }

        /// <summary>
        /// Resets monotonicity for all fields. 
        /// </summary>
        /// <remarks>
        /// This should be used with extreme caution because some transforms 
        /// may have trouble if the field (vector) isn't actually monotonic. 
        /// Generally, you should only use this to reset to <see cref="Monotonicity.None"/>. 
        /// You can do this by using the overload that takes no parameters. 
        /// Optimizations related to this setting will usually be negligible.
        /// </remarks>
        public void ResetMonotonicities(Monotonicity monotonicity)
        {
            for (var i = 0; i < Monotonicities.Length; i++)
            {
                Monotonicities[i] = monotonicity;
            }
        }

        /// <summary>
        /// Granularities are set to defaults for all fields using reflection.
        /// </summary>
        public void ResetGranularities()
        {
            var types = GetType().GenericTypeArguments;
            for (var i = 0; i < types.Length; i++)
            {
                var t = types[i];
                Granularities[i] = t.IsValueType ? GetDefault(t) : null;
            }
        }

        #region Private Static Methods

        private static object GetDefault(Type t)
        {
            Func<object> f = GetDefault<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, null);
        }

        private static T GetDefault<T>()
        {
            return default(T);
        }

        #endregion // Private Static Methods
    }

}
