using System;
using System.Diagnostics;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class ArrayTypeTranslationContext : ITypeTranslationContext
    {
        private ITypeScriptExpression Expression { get; }
        private ITranslationContext TranslationContext { get; }

        public ArrayTypeTranslationContext(
            ITypeScriptExpression expression,
            ITranslationContext translationContext)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
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

            return new TranslatedTypeMetadata
            {
                Symbol = Expression.GenericArrayOf(elementTypeSymbol)
            };
        }
    }
}
