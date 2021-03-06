﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class SourceTypeMetadata : ISourceTypeMetadata
    {
        private IDictionary<string, MemberInfo> MembersSourceInfo { get; } =
            new Dictionary<string, MemberInfo>();

        public TypeInfo BaseType { get; set; }

        // TODO INTERFACES ???

        private Guid guid = Guid.NewGuid();

        public IEnumerable<string> Members => MembersSourceInfo.Keys;

        public MemberInfo this[string memberName]
        {
            get { return MembersSourceInfo[memberName]; }
            set { MembersSourceInfo[memberName] = value; }
        }

        public override string ToString()
        {
            return guid.ToString("N");
        }
    }
}