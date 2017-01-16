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
                TypeTranslationContextFactory.Direct(typeof(char), Expression.String()), // TODO consider better options if possible
                TypeTranslationContextFactory.Direct(typeof(DateTime), Expression.Date()), // TODO consider better options if possible
                // TODO TimeSpan -> ???
                TypeTranslationContextFactory.Enum(),
                TypeTranslationContextFactory.Nullable(),
                TypeTranslationContextFactory.GenericDictionary(),
                TypeTranslationContextFactory.Special(typeof(IDictionary), Expression.UntypedDictionary()),
                TypeTranslationContextFactory.Array(),
                TypeTranslationContextFactory.GenericEnumerable(),
                TypeTranslationContextFactory.Special(typeof(IEnumerable), Expression.UntypedArray())
            };
        }
    }

    public interface ITypeTranslationContextFactory
    {
        ITypeTranslationContext Direct(Type type, string symbol);
        ITypeTranslationContext Enum();
        ITypeTranslationContext Nullable();
        ITypeTranslationContext GenericDictionary();
        ITypeTranslationContext Special(Type type, string symbol);
        ITypeTranslationContext Array();
        ITypeTranslationContext GenericEnumerable();
    }

    public sealed class TypeTranslationContextFactory : ITypeTranslationContextFactory
    {
        private ITypeScriptExpression Expression { get; }
        private ITranslationContext TranslationContext { get; }
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }

        public TypeTranslationContextFactory(
            ITypeScriptExpression expression,
            ITranslationContext translationContext,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
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
    }
}