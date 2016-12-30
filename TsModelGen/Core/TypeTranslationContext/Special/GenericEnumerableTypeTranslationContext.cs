﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TsModelGen.Core.TypeTranslationContext.Special
{
    public class GenericEnumerableTypeTranslationContext : ITypeTranslationContext
    {
        private readonly TranslationContext _translationContext;

        public GenericEnumerableTypeTranslationContext(TranslationContext translationContext)
        {
            _translationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public bool AreDependenciesResolved { get; } = true;

        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            var canProcess = type.IsChildTypeOfPossiblyOpenGeneric(typeof(IEnumerable<>));
            return canProcess;
        }

        public bool IsProcessed { get; } = true;

        public TranslatedTypeMetadata Process(Type specificTargetType)
        {
            // TODO Think if can and should Assert here
            var genericArgumentType = specificTargetType.GetGenericArguments().Single();
            var genericArgumentTranslatesSymbol = _translationContext
                .GetByType(genericArgumentType)
                .Process(genericArgumentType)
                .Symbol;
            
            return new TranslatedTypeMetadata
            {
                Symbol = TypeScriptExpression.GenericArrayOf(genericArgumentTranslatesSymbol)
            };
        }
    }
}