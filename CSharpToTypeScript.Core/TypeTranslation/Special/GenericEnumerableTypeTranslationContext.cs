using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Core.TypeTranslation.Special
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
            return type.IsChildTypeOfPossiblyOpenGeneric(typeof(IEnumerable<>));
        }

        public bool IsProcessed { get; } = true;

        public TranslatedTypeMetadata Process(Type specificTargetType)
        {
            var genericArguments = specificTargetType.GetGenericArguments();
            Debug.Assert(genericArguments.Length == 1);

            var genericArgumentType = genericArguments.Single();
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
