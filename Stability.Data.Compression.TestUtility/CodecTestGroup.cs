#region License

// Namespace : Stability.Data.Compression.TestUtility
// FileName  : CodecTestGroup.cs
// Created   : 2015-5-29
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stability.Data.Compression.TestUtility
{
    /// <summary>
    /// This class is just used to link the group type to the list of test runners for that group.
    /// </summary>
    public class CodecTestGroup : List<CodecTestRunner>
    {
        public CodecTestGroup(string displayName = null)
            : this(TestGroupType.Custom, displayName)
        {           
        }

        public CodecTestGroup(TestGroupType groupType, string displayName = null)
        {
            GroupType = groupType;
            DisplayName = displayName ?? groupType.ToString();
        }

        public TestGroupType GroupType { get; set; }
        public string DisplayName { get; set; }

        public void InitializeRunners()
        {
            foreach (var r in this)
            {
                r.Group = this;
            }
        }
    }
}
