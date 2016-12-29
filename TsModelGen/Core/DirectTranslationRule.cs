using System;

namespace TsModelGen.Core
{
    public sealed class DirectTranslationRule
    {
        public Type SourceType { get; }
        public string DestinationType { get; }

        public DirectTranslationRule(Type sourceType, string destinationType)
        {
            SourceType = sourceType.NullToException(new ArgumentNullException(nameof(sourceType)));
            DestinationType = destinationType.NullToException(new ArgumentNullException(nameof(destinationType)));
        }
    }
}