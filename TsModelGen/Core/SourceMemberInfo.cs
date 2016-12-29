using System;
using System.Reflection;

namespace TsModelGen.Core
{
    // TODO NOW ! No reasaon to exist at this point
    public sealed class SourceMemberInfo
    {
        public MemberInfo MemberInfo { get; }

        public SourceMemberInfo(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo.NullToException(new ArgumentNullException(nameof(memberInfo)));
        }
    }
}