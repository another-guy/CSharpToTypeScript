using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class GenericDictionaryTypeTranslationContext : ITypeTranslationContext
    {
        private ITypeScriptExpression Expression { get; }
        private ITranslationContext TranslationContext { get; }
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }

        public GenericDictionaryTypeTranslationContext(
            ITypeScriptExpression expression,
            ITranslationContext translationContext,
            ITranslatedTypeMetadata translatedTypeMetadata)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TranslatedTypeMetadata = translatedTypeMetadata.NullToException(new ArgumentNullException(nameof(translatedTypeMetadata)));
        }

        public bool AreDependenciesResolved { get; } = true;

        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(typeof(IDictionary<,>));
        }

        public bool IsProcessed { get; } = true;

        public ITranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(CanProcess(specificTargetType));
            var genericArgumentTypes = specificTargetType.GetGenericArguments();
            Debug.Assert(genericArgumentTypes.Length == 2);
            
            var keyType = genericArgumentTypes[0];
            var valueType = genericArgumentTypes[1];

            var translatedKeySymbol = TranslationContext
                .GetByType(keyType)
                .Process(keyType)
                .Symbol;
            var translatedValueSymbol = TranslationContext
                .GetByType(valueType)
                .Process(valueType)
                .Symbol;

            TranslatedTypeMetadata.Symbol = Expression.GenericDictionaryOf(translatedKeySymbol, translatedValueSymbol);
            return TranslatedTypeMetadata;
        }
    }
}
