using System;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Translation.Rules.Direct;
using CSharpToTypeScript.Core.Translation.Rules.Special;
using CSharpToTypeScript.Core.Translation.Rules.Regular;

namespace CSharpToTypeScript.Core.Translation.Rules
{
    public sealed class TypeTranslationContextFactory : ITypeTranslationContextFactory
    {
        private IDiscoveredTypeRegistrator DiscoveredTypeRegistrator { get; }
        private ITypeScriptExpression Expression { get; }
        private ITranslationContext TranslationContext { get; }
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }
        private ISourceTypeMetadataFactory SourceTypeMetadataFactory { get; }
        private ISkipTypeRule SkipTypeRule { get; }
        private ISymbolNamer SymbolNamer { get; }
        private ICommenter Commenter { get; }

        public TypeTranslationContextFactory(
            IDiscoveredTypeRegistrator discoveredTypeRegistrator,
            ITypeScriptExpression expression,
            ITranslationContext translationContext,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ISourceTypeMetadataFactory sourceTypeMetadataFactory,
            ISkipTypeRule skipTypeRule,
            ISymbolNamer symbolNamer,
            ICommenter commenter)
        {
            DiscoveredTypeRegistrator = discoveredTypeRegistrator.NullToException(new ArgumentNullException(nameof(discoveredTypeRegistrator)));
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
            SourceTypeMetadataFactory = sourceTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(sourceTypeMetadataFactory)));
            SkipTypeRule = skipTypeRule.NullToException(new ArgumentNullException(nameof(skipTypeRule)));
            SymbolNamer = symbolNamer.NullToException(new ArgumentNullException(nameof(symbolNamer)));
            Commenter = commenter.NullToException(new ArgumentNullException(nameof(commenter)));
        }

        public ITypeTranslationContext Direct(Type type, string symbol)
        {
            return new DirectTypeTranslationContext(TranslatedTypeMetadataFactory, type, symbol);
        }

        public ITypeTranslationContext Enum()
        {
            return new EnumTypeTranslationContext(TranslatedTypeMetadataFactory, TranslationContext, Expression, SymbolNamer, Commenter);
        }

        public ITypeTranslationContext Nullable()
        {
            return new NullableTypeTranslationContext(TranslatedTypeMetadataFactory, TranslationContext);
        }

        public ITypeTranslationContext GenericDictionary()
        {
            return new GenericDictionaryTypeTranslationContext(TranslatedTypeMetadataFactory, TranslationContext, Expression);
        }

        public ITypeTranslationContext Special(Type type, string symbol)
        {
            return new SpecialTypeTranslationContext(TranslatedTypeMetadataFactory, type, symbol);
        }

        public ITypeTranslationContext Array()
        {
            return new ArrayTypeTranslationContext(TranslatedTypeMetadataFactory, TranslationContext, Expression);
        }

        public ITypeTranslationContext GenericEnumerable()
        {
            return new GenericEnumerableTypeTranslationContext(TranslatedTypeMetadataFactory, TranslationContext, Expression);
        }

        public ITypeTranslationContext Regular(TypeInfo typeInfo)
        {
            return new RegularTypeTranslationContext(DiscoveredTypeRegistrator, TranslatedTypeMetadataFactory, SourceTypeMetadataFactory, TranslationContext, SkipTypeRule, Expression, SymbolNamer, Commenter, typeInfo);
        }

        public ITypeTranslationContext GenericType(TypeInfo typeInfo)
        {
            return new GenericTypeTranslationContext(DiscoveredTypeRegistrator, TranslatedTypeMetadataFactory, SourceTypeMetadataFactory, TranslationContext, SkipTypeRule, Expression, SymbolNamer, Commenter, typeInfo);
        }
    }
}
