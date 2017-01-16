using System;
using System.Diagnostics;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class ArrayTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        private ITranslationContext TranslationContext { get; }
        private ITypeScriptExpression Expression { get; }

        public ArrayTypeTranslationContext(
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ITranslationContext translationContext,
            ITypeScriptExpression expression)
        {
            TranslatedTypeMetadata =
                translatedTypeMetadataFactory
                    .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)))
                    .CreateNew();

            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));

            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
        }

        public bool AreDependenciesResolved { get; } = true;

        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return type.IsArray;
        }

        public bool IsProcessed { get; } = true;

        public ITranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(specificTargetType.IsArray);

            var elementType = specificTargetType.GetElementType();
            var elementTypeSymbol = TranslationContext.GetByType(elementType).Process(elementType).Symbol;
            TranslatedTypeMetadata.Symbol = Expression.GenericArrayOf(elementTypeSymbol);
            return TranslatedTypeMetadata;
        }
    }
}
