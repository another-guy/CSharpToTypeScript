using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class NullableTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        private ITranslationContext TranslationContext { get; }

        public NullableTypeTranslationContext(
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ITranslationContext translationContext)
        {
            TranslatedTypeMetadata =
                translatedTypeMetadataFactory
                    .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)))
                    .CreateNew();

            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(typeof(Nullable<>));
        }
        public bool IsProcessed => true;

        public ITranslatedTypeMetadata Process(Type specificEnumType)
        {
            Debug.Assert(CanProcess(specificEnumType));
            TranslatedTypeMetadata.Symbol = specificEnumType
                .GetGenericArguments()
                .Single()
                .UseAsArgFor(argumentType => TranslationContext.GetTranslationContextFor(argumentType).Process(argumentType))
                .Symbol;
            return TranslatedTypeMetadata;
        }
    }
}
