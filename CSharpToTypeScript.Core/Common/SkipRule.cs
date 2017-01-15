using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Common
{
    // TODO IoC -- move to new location
    public interface ISkipRule
    {
        bool AppliesTo(MemberInfo sourceType);
    }

    public sealed class SkipRule : ISkipRule
    {
        private readonly List<string> _skipTypeAttributeNames;

        public SkipRule(InputConfiguration inputConfiguration)
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