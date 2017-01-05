using System;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Direct
{
    public sealed class DirectTypeTranslationContext : ITypeTranslationContext
    {
        public DirectTypeTranslationContext(Type type, string symbol)
        {
            Type = type.NullToException(new ArgumentNullException(nameof(type))).GetTypeInfo();
            Symbol = symbol.NullToException(new ArgumentNullException(nameof(symbol)));
        }

        public TypeInfo Type { get; }
        private string Symbol { get; }

        public bool AreDependenciesResolved => true;
        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return Type.AsType() == type;
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(CanProcess(specificTargetType));
            return new TranslatedTypeMetadata { Symbol = Symbol };
        }
    }
}