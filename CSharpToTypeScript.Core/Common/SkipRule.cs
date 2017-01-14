using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Core.Common
{
    public sealed class SkipRule
    {
        private readonly List<string> _skipTypeAttributeNames;

        public SkipRule(List<string> skipTypeAttributeNames)
        {
            _skipTypeAttributeNames = skipTypeAttributeNames.NullToException(new ArgumentNullException(nameof(skipTypeAttributeNames)));
        }

        public bool AppliesTo(MemberInfo sourceType)
        {
            var assignedCustomAttributes = sourceType
                .GetCustomAttributes()
                .Select(attribute => attribute.GetType().FullName)
                .ToList();

            return assignedCustomAttributes
                .Intersect(_skipTypeAttributeNames)
                .Any();
        }
    }
}