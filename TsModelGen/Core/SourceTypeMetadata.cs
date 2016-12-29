using System.Collections.Generic;

namespace TsModelGen.Core
{
    public sealed class SourceTypeMetadata
    {
        private IDictionary<string, SourceMemberInfo> MembersSourceInfo { get; } =
            new Dictionary<string, SourceMemberInfo>();

        public SourceParentInfo BaseType { get; set; }

        // TODO INTERFACES ???

        public IEnumerable<string> Members => MembersSourceInfo.Keys;

        public SourceMemberInfo this[string memberName]
        {
            get { return MembersSourceInfo[memberName]; }
            set { MembersSourceInfo[memberName] = value; }
        }
    }
}