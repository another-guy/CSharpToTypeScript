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
        private ITypeScriptExpression Expression { get; }
        private ITranslationContext TranslationContext { get; }
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }
        private ISourceTypeMetadataFactory SourceTypeMetadataFactory { get; }
        private ISkipTypeRule SkipTypeRule { get; }

        public TypeTranslationContextFactory(
            ITypeScriptExpression expression,
            ITranslationContext translationContext,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ISourceTypeMetadataFactory sourceTypeMetadataFactory,
            ISkipTypeRule skipTypeRule)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
            SourceTypeMetadataFactory = sourceTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(sourceTypeMetadataFactory)));
            SkipTypeRule = skipTypeRule.NullToException(new ArgumentNullException(nameof(skipTypeRule)));
        }

        public ITypeTranslationContext Direct(Type type, string symbol)
        {
            return new DirectTypeTranslationContext(TranslatedTypeMetadataFactory, type, symbol);
        }

        public ITypeTranslationContext Enum()
        {
            return new EnumTypeTranslationContext(TranslatedTypeMetadataFactory, TranslationContext, Expression);
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
            return new RegularTypeTranslationContext(TranslatedTypeMetadataFactory, SourceTypeMetadataFactory, TranslationContext, SkipTypeRule, Expression, typeInfo);
        }
    }
}
