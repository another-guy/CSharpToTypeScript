using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Common
{
    public sealed class SkipTypeRule : ISkipTypeRule
    {
        private readonly List<string> _skipTypeAttributeNames;

        public SkipTypeRule(InputConfiguration inputConfiguration)
        {
            _skipTypeAttributeNames = inputConfiguration.SkipTypesWithAttribute.NullToException(new ArgumentNullException(nameof(inputConfiguration.SkipTypesWithAttribute)));
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