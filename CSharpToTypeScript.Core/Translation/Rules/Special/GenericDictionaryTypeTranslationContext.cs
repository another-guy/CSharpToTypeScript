﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Special
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
            Debug.Assert(CanProcess(specificTargetType));
            var genericArgumentTypes = specificTargetType.GetGenericArguments();
            Debug.Assert(genericArgumentTypes.Length == 2);
            
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