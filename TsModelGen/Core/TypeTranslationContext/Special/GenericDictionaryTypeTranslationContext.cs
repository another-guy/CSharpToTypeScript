using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TsModelGen.Core.TypeTranslationContext.Special
{
    public class GenericDictionaryTypeTranslationContext : ITypeTranslationContext
    {
        private readonly TranslationContext _translationContext;

        public GenericDictionaryTypeTranslationContext(TranslationContext translationContext)
        {
            _translationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public bool AreDependenciesResolved { get; } = true;

        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(typeof(IDictionary<,>));
        }

        public bool IsProcessed { get; } = true;

        public TranslatedTypeMetadata Process(Type specificTargetType)
        {
            // TODO Think if can and should Assert here
            var genericArgumentTypes = specificTargetType.GetGenericArguments();
            var keyType = genericArgumentTypes[0];
            var valueType = genericArgumentTypes[1];

            var translatedKeySymbol = _translationContext
                .GetByType(keyType)
                .Process(keyType)
                .Symbol;
            var translatedValueSymbol = _translationContext
                .GetByType(valueType)
                .Process(valueType)
                .Symbol;

            return new TranslatedTypeMetadata
            {
                Symbol =
                    TypeScriptExpression.GenericDictionaryOf(translatedKeySymbol, translatedValueSymbol)
            };
        }
    }
}
