using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class GenericDictionaryTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        private ITranslationContext TranslationContext { get; }
        private ITypeScriptExpression Expression { get; }

        public GenericDictionaryTypeTranslationContext(
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
