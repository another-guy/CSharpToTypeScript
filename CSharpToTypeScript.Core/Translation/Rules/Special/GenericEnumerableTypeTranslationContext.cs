﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class GenericEnumerableTypeTranslationContext : ITypeTranslationContext
    {
        private ITypeScriptExpression Expression { get; }
        private ITranslationContext TranslationContext { get; }

        public GenericEnumerableTypeTranslationContext(
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
            return type.IsChildTypeOfPossiblyOpenGeneric(typeof(IEnumerable<>));
        }

        public bool IsProcessed { get; } = true;

        public ITranslatedTypeMetadata Process(Type specificTargetType)
        {
            var genericArguments = specificTargetType.GetGenericArguments();
            Debug.Assert(genericArguments.Length == 1);

            var genericArgumentType = genericArguments.Single();
            var genericArgumentTranslatesSymbol = TranslationContext
                .GetByType(genericArgumentType)
                .Process(genericArgumentType)
                .Symbol;
            
            return new TranslatedTypeMetadata
            {
                Symbol = Expression.GenericArrayOf(genericArgumentTranslatesSymbol)
            };
        }
    }
}
