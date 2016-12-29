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

        public TranslationContext Build(IEnumerable<TypeInfo> translationTargetTypes)
        {
            var translationContext = CreateTranslationContext(translationTargetTypes);

            ITypeTranslationContext unprocessed;
            while ((unprocessed = translationContext.FirstOrDefault(WithUnresolvedDependencies)) != null)
                unprocessed.ResolveDependencies();

            return translationContext;
        }

        private TranslationContext CreateTranslationContext(IEnumerable<TypeInfo> translationTargetTypes)
        {
            var translationContext = new TranslationContext();
            foreach (var sourceType in translationTargetTypes)
                translationContext.AddTypeTranslationContextForType(sourceType);
            return translationContext;
        }

    }
}