﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TranslationContext : IEnumerable<ITypeTranslationContext>
    {
        public static TranslationContext BuildFor(
            IEnumerable<TypeInfo> translationRootTargetTypes,
            CompleteConfiguration configuration)
        {
            var skipRule = new SkipRule(configuration.Input.SkipTypesWithAttribute);
            var translationContext = new TranslationContext(configuration);
            foreach (var sourceType in translationRootTargetTypes)
                if (skipRule.AppliesTo(sourceType) == false)
                    translationContext.AddTypeTranslationContextForType(sourceType);

            ITypeTranslationContext unprocessed;
            Func<ITypeTranslationContext, bool> withUnresolvedDependencies =
                typeContext => typeContext.AreDependenciesResolved == false;
            while ((unprocessed = translationContext.FirstOrDefault(withUnresolvedDependencies)) != null)
                unprocessed.ResolveDependencies();

            return translationContext;
        }

        public InputConfiguration InputConfiguration { get; }
        public OutputConfiguration OutputConfiguration { get; }
        public TranslationConfiguration TranslationConfiguration { get; }

        // TODO The right way of doing that is using a Graph data structure.
        // Naive list consumption can't guarantee precedence of parent types.
        public IList<TypeInfo> OrderedTargetTypes { get; } = // TODO Make it immutable for clients
            new List<TypeInfo>();

        // TODO Make this dynamic -- let clients alter the chain to fit their need
        private IList<ITypeTranslationContext> TranslationChain { get; } =
            new List<ITypeTranslationContext>();

        private TranslationContext(CompleteConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            InputConfiguration = configuration.Input;
            OutputConfiguration = configuration.Output;
            TranslationConfiguration = configuration.Translation;

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
            var symbolRule = TranslationConfiguration.GeneratedSymbols;
            return $"{symbolRule.Prefix}{symbolBase}{symbolRule.Suffix}";
        }

        public string TypeCommentFor(TypeInfo typeInfo)
        {
            string typeRef;
            switch (TranslationConfiguration.SourceTypeReferenceKind)
            {
                case SourceTypeReferenceKind.AssemblyQualifiedName:
                    typeRef = typeInfo.AssemblyQualifiedName;
                    break;
                case SourceTypeReferenceKind.FullName:
                    typeRef = typeInfo.FullName;
                    break;
                case SourceTypeReferenceKind.Name:
                    typeRef = typeInfo.Name;
                    break;
                case SourceTypeReferenceKind.None:
                default:
                    return "";
            }
            return TypeScriptExpression.SingleLineComment(typeRef);
        }
    }
}