using System;
using System.Diagnostics;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class ArrayTypeTranslationContext : ITypeTranslationContext
    {
        private readonly TranslationContext _translationContext;

        public ArrayTypeTranslationContext(TranslationContext translationContext)
        {
            _translationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public bool AreDependenciesResolved { get; } = true;

        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return type.IsArray;
        }

        public bool IsProcessed { get; } = true;

        public TranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(specificTargetType.IsArray);

            var elementType = specificTargetType.GetElementType();
            var elementTypeSymbol = _translationContext.GetByType(elementType).Process(elementType).Symbol;

            return new TranslatedTypeMetadata
            {
                Symbol = TypeScriptExpression.GenericArrayOf(elementTypeSymbol)
            };
        }
    }
}
