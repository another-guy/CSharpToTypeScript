using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class NullableTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslationContext TranslationContext { get; }
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }

        public NullableTypeTranslationContext(
            ITranslationContext translationContext,
            ITranslatedTypeMetadata translatedTypeMetadata)
        {
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TranslatedTypeMetadata = translatedTypeMetadata.NullToException(new ArgumentNullException(nameof(translatedTypeMetadata)));
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
                .UseAsArgFor(argumentType => TranslationContext.GetByType(argumentType).Process(argumentType))
                .Symbol;
            return TranslatedTypeMetadata;
        }
    }
}
