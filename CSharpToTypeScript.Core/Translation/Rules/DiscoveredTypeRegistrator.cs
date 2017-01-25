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
            // TODO NullCheck
            SourceTypeMetadataFactory = sourceTypeMetadataFactory;
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory;
            TranslationContext = translationContext;
            SkipTypeRule = skipTypeRule;
            Expression = expression;
            SymbolNamer = symbolNamer;
            Commenter = commenter;
        }

        public void RegisterType(TypeInfo typeInfo)
        {
            if (typeInfo == null)
                return;

            var noTypeTranslationContextRegistered = TranslationContext.CanProcess(typeInfo) == false;
            if (noTypeTranslationContextRegistered)
            {
                ITypeTranslationContext regularTypeTranslationContext;
                if (typeInfo.IsGenericParameter)
                {
                    regularTypeTranslationContext =
                        new GenericParameterTranslationContext(TranslatedTypeMetadataFactory, typeInfo);
                }
                else
                {
                    regularTypeTranslationContext =
                        new RegularTypeTranslationContext(this, TranslatedTypeMetadataFactory, SourceTypeMetadataFactory, TranslationContext, SkipTypeRule, Expression, SymbolNamer, Commenter, typeInfo);
                }
                TranslationContext.AddTypeTranslationContext(regularTypeTranslationContext, true);
            }
        }
    }
}
