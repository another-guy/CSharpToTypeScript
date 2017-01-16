using System;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public sealed class SpecialTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        public TypeInfo Type { get; }
        private string Symbol { get; }

        public SpecialTypeTranslationContext(
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            Type type,
            string symbol)
        {
            TranslatedTypeMetadata =
               translatedTypeMetadataFactory
                   .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)))
                   .CreateNew();

            Type = type.NullToException(new ArgumentNullException(nameof(type))).GetTypeInfo();

            Symbol = symbol.NullToException(new ArgumentNullException(nameof(symbol)));
        }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(Type.AsType());
        }
        public bool IsProcessed => true;

        public ITranslatedTypeMetadata Process(Type specificTragetType)
        {
            Debug.Assert(CanProcess(specificTragetType));
            TranslatedTypeMetadata.Symbol = Symbol;
            return TranslatedTypeMetadata;
        }
    }
}