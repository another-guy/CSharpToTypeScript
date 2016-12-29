using System;
using System.Reflection;

namespace TsModelGen.Core
{
    // TODO Now No strong reason to exist
    public sealed class SourceParentInfo
    {
        public TypeInfo ParentInfo { get; }

        public SourceParentInfo(TypeInfo parentInfo)
        {
            ParentInfo = parentInfo.NullToException(new ArgumentNullException(nameof(parentInfo)));
        }
    }
}