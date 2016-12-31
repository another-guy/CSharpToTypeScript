using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TsModelGen.Core.TypeTranslationContext;

namespace TsModelGen.Core
{
    public sealed class TranslationContextBuilder
    {
        private static readonly Func<ITypeTranslationContext, bool> WithUnresolvedDependencies =
            typeContext => typeContext.AreDependenciesResolved == false;

        public TranslationContext Build(IEnumerable<TypeInfo> translationRootTargetTypes)
        {
            var translationContext = CreateTranslationContext(translationRootTargetTypes);

            ITypeTranslationContext unprocessed;
            while ((unprocessed = translationContext.FirstOrDefault(WithUnresolvedDependencies)) != null)
                unprocessed.ResolveDependencies();

            return translationContext;
        }

        private TranslationContext CreateTranslationContext(IEnumerable<TypeInfo> translationRootTargetTypes)
        {
            var translationContext = new TranslationContext();
            foreach (var sourceType in translationRootTargetTypes)
                translationContext.AddTypeTranslationContextForType(sourceType);
            return translationContext;
        }

    }
}