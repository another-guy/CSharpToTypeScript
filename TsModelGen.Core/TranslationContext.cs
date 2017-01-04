using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TsModelGen.Core.Configuration;
using TsModelGen.Core.TypeTranslationContext;

namespace TsModelGen.Core
{
    public sealed class TranslationContext : ITypeTranslationEnumerable
    {
        public static TranslationContext BuildFor(
            IEnumerable<TypeInfo> translationRootTargetTypes,
            OutputConfiguration outputConfiguration,
            TranslationConfiguration translationConfiguration)
        {
            var translationContext = new TranslationContext(outputConfiguration, translationConfiguration);
            foreach (var sourceType in translationRootTargetTypes)
                translationContext.AddTypeTranslationContextForType(sourceType);

            ITypeTranslationContext unprocessed;
            Func<ITypeTranslationContext, bool> withUnresolvedDependencies =
                typeContext => typeContext.AreDependenciesResolved == false;
            while ((unprocessed = translationContext.FirstOrDefault(withUnresolvedDependencies)) != null)
                unprocessed.ResolveDependencies();

            return translationContext;
        }

        private readonly OutputConfiguration _outputConfiguration;
        private readonly TranslationConfiguration _translationConfiguration;

        // TODO The right way of doing that is using a Graph data structure.
        // Naive list consumption can't guarantee precedence of parent types.
        public IList<TypeInfo> OrderedTargetTypes { get; } = // TODO Make it immutable for clients
            new List<TypeInfo>();

        // TODO Make this dynamic -- let clients alter the chain to fit their need
        private IList<ITypeTranslationContext> TranslationChain { get; } =
            new List<ITypeTranslationContext>();

        private TranslationContext(OutputConfiguration outputConfiguration, TranslationConfiguration translationConfiguration)
        {
            _outputConfiguration = outputConfiguration;
            _translationConfiguration = translationConfiguration;

            TypeTranslationChain
                .BuildDefault(this)
                .ForEach(AddTypeTranslationContext);
        }

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }

        public void AddTypeTranslationContextForType(TypeInfo typeInfo)
        {
            OrderedTargetTypes.Insert(0, typeInfo);
            AddTypeTranslationContext(new RegularTypeTranslationContext(this, typeInfo));
        }

        private void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext)
        {
            TranslationChain.Add(typeTranslationContext);
        }

        public ITypeTranslationContext GetByType(Type type)
        {
            return this.First(typeTranslationContext => typeTranslationContext.CanProcess(type));
        }

        public IEnumerator<ITypeTranslationContext> GetEnumerator()
        {
            return TranslationChain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<string> TranslateTargets()
        {
            return OrderedTargetTypes
                .Select(targetType =>
                        this.First(typeTranslationContext => typeTranslationContext.CanProcess(targetType.AsType()))
                            .Process(targetType.AsType())
                            .Definition
                );
        }

        public string SymbolFor(string symbolBase)
        {
            var symbolRule = _translationConfiguration.GeneratedSymbols;
            return $"{symbolRule.Prefix}{symbolBase}{symbolRule.Suffix}";
        }
    }
}