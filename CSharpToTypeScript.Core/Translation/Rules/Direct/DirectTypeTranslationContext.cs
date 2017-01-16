using System;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Direct
{
    public sealed class DirectTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        public TypeInfo Type { get; }
        private string Symbol { get; }

        public DirectTypeTranslationContext(ITranslatedTypeMetadataFactory translatedTypeMetadataFactory, Type type, string symbol)
        {
            TranslatedTypeMetadata = translatedTypeMetadataFactory
                .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)))
                .CreateNew();

            Type = type.NullToException(new ArgumentNullException(nameof(type))).GetTypeInfo();

            Symbol = symbol.NullToException(new ArgumentNullException(nameof(symbol)));
        }


        public bool AreDependenciesResolved => true;
        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return Type.AsType() == type;
        }
        public bool IsProcessed => true;

        public ITranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(CanProcess(specificTargetType));
            TranslatedTypeMetadata.Symbol = Symbol;
            return TranslatedTypeMetadata;
        }
    }
}