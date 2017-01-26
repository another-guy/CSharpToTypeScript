using System;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Translation.Rules.Regular;
using CSharpToTypeScript.Core.Translation.Rules.Special;

namespace CSharpToTypeScript.Core.Translation.Rules
{
    public sealed class DiscoveredTypeRegistrator : IDiscoveredTypeRegistrator
    {
        private ISourceTypeMetadataFactory SourceTypeMetadataFactory { get; }
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }
        private ITranslationContext TranslationContext { get; }
        private ISkipTypeRule SkipTypeRule { get; }
        private ITypeScriptExpression Expression { get; }
        private ISymbolNamer SymbolNamer { get; }
        private ICommenter Commenter { get; }

        public DiscoveredTypeRegistrator(
            ISourceTypeMetadataFactory sourceTypeMetadataFactory,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ITranslationContext translationContext,
            ISkipTypeRule skipTypeRule,
            ITypeScriptExpression expression,
            ISymbolNamer symbolNamer,
            ICommenter commenter)
        {
            SourceTypeMetadataFactory = sourceTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(sourceTypeMetadataFactory)));
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            SkipTypeRule = skipTypeRule.NullToException(new ArgumentNullException(nameof(skipTypeRule)));
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            SymbolNamer = symbolNamer.NullToException(new ArgumentNullException(nameof(symbolNamer)));
            Commenter = commenter.NullToException(new ArgumentNullException(nameof(commenter)));
        }

        // TODO use dependency
        public void RegisterType(TypeInfo dependency, TypeInfo dependentType)
        {
            if (dependentType == null)
                return;

            var noTypeTranslationContextRegistered = TranslationContext.CanProcess(dependentType) == false;
            if (noTypeTranslationContextRegistered)
            {
                ITypeTranslationContext typeTranslationContext;

                if (dependentType.IsGenericType)
                {
                    typeTranslationContext =
                        new GenericTypeTranslationContext(this, TranslatedTypeMetadataFactory, SourceTypeMetadataFactory, TranslationContext, SkipTypeRule, Expression, SymbolNamer, Commenter, dependentType);
                }
                else if (dependentType.IsGenericParameter)
                {
                    typeTranslationContext =
                        new GenericParameterTranslationContext(TranslatedTypeMetadataFactory, dependentType);
                }
                else if (dependentType.IsGenericTypeDefinition)
                {
                    throw new InvalidOperationException("Ooops");
                }
                else
                {
                    typeTranslationContext =
                        new RegularTypeTranslationContext(this, TranslatedTypeMetadataFactory, SourceTypeMetadataFactory, TranslationContext, SkipTypeRule, Expression, SymbolNamer, Commenter, dependentType);
                }

                TranslationContext.AddTypeTranslationContext(typeTranslationContext);
                TranslationContext.RegisterDependency(dependentType, dependency);
            }
        }
    }
}
