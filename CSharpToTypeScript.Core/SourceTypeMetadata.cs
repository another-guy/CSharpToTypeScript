using System.Collections.Generic;
using System.Reflection;

namespace CSharpToTypeScript.Core
{
    public sealed class SourceTypeMetadata
    {
        private IDictionary<string, MemberInfo> MembersSourceInfo { get; } =
            new Dictionary<string, MemberInfo>();

        public TypeInfo BaseType { get; set; }

        // TODO INTERFACES ???

        public IEnumerable<string> Members => MembersSourceInfo.Keys;

        public MemberInfo this[string memberName]
        {
            get { return MembersSourceInfo[memberName]; }
            set { MembersSourceInfo[memberName] = value; }
        }
    }
}