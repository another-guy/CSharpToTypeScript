using System;
using System.Reflection;

namespace TsModelGen.Core.TypeTranslationContext.Direct
{
    public sealed class DirectTypeTranslationContext : ITypeTranslationContext
    {
        public DirectTypeTranslationContext(TypeInfo type, string symbol)
        {
            Type = type.NullToException(new ArgumentNullException(nameof(type)));
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

        public TranslatedTypeMetadata Process(Type _)
        {
            return new TranslatedTypeMetadata { Symbol = Symbol };
        }
    }
}