using System;
using System.Collections;
using System.Collections.Generic;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.Core.Translation.Rules.Direct;
using CSharpToTypeScript.Core.Translation.Rules.Special;

namespace CSharpToTypeScript.Core.Translation
{
    // TODO IoC -- interface?
    // TODO IoC -- factory with static instance
    public sealed class TypeTranslationChain
    {
        private ITypeScriptExpression Expression { get; }
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }
        private ITypeTranslationContextFactory TypeTranslationContextFactory { get; }

        public TypeTranslationChain(
            ITypeScriptExpression expression,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ITypeTranslationContextFactory typeTranslationContextFactory)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
            TypeTranslationContextFactory = typeTranslationContextFactory.NullToException(new ArgumentNullException(nameof(typeTranslationContextFactory)));
        }

        public IList<ITypeTranslationContext> BuildDefault(
            ITypeScriptExpression expression,
            ITranslationContext translationContext)
        {
            // TODO Ioc -- Fix direct binding...
            return new List<ITypeTranslationContext>
            {
                TypeTranslationContextFactory.Direct(typeof(object), Expression.Any()),
                TypeTranslationContextFactory.Direct(typeof(short), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(int), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(long), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(ushort), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(uint), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(ulong), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(byte), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(sbyte), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(float), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(double), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(decimal), Expression.Number()),
                TypeTranslationContextFactory.Direct(typeof(bool), Expression.Bool()),
                TypeTranslationContextFactory.Direct(typeof(string), Expression.String()),
                // TODO consider better options if possible
                TypeTranslationContextFactory.Direct(typeof(char), Expression.String()),
                TypeTranslationContextFactory.Direct(typeof(DateTime), Expression.Date()),
                // TODO TimeSpan -> ???
                new EnumTypeTranslationContext(expression, translationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new NullableTypeTranslationContext(translationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new GenericDictionaryTypeTranslationContext(expression, translationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new SpecialTypeTranslationContext(typeof(IDictionary), Expression.UntypedDictionary(), TranslatedTypeMetadataFactory.CreateNew()),
                new ArrayTypeTranslationContext(expression, translationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new GenericEnumerableTypeTranslationContext(expression, translationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new SpecialTypeTranslationContext(typeof(IEnumerable), Expression.UntypedArray(), TranslatedTypeMetadataFactory.CreateNew())
            };
        }
    }

    public interface ITypeTranslationContextFactory
    {
        ITypeTranslationContext Direct(Type type, string symbol);
    }

    public sealed class TypeTranslationContextFactory : ITypeTranslationContextFactory
    {
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }

        public TypeTranslationContextFactory(ITranslatedTypeMetadataFactory translatedTypeMetadataFactory)
        {
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
        }

        public ITypeTranslationContext Direct(Type type, string symbol)
        {
            return new DirectTypeTranslationContext(type, symbol, TranslatedTypeMetadataFactory);
        }
    }
}