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

        public TypeTranslationChain(
            ITypeScriptExpression expression,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory.NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));
        }

        public List<ITypeTranslationContext> BuildDefault(
            ITypeScriptExpression expression,
            TranslationContext globalTranslationContext)
        {
            // TODO Ioc -- Fix direct binding...
            return new List<ITypeTranslationContext>
            {
                new DirectTypeTranslationContext(typeof(object), Expression.Any(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(short), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(int), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(long), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(ushort), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(uint), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(ulong), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(byte), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(sbyte), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(float), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(double), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(decimal), Expression.Number(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(bool), Expression.Bool(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(string), Expression.String(), TranslatedTypeMetadataFactory.CreateNew()),
                // TODO consider better options if possible
                new DirectTypeTranslationContext(typeof(char), Expression.String(), TranslatedTypeMetadataFactory.CreateNew()),
                new DirectTypeTranslationContext(typeof(DateTime), Expression.Date(), TranslatedTypeMetadataFactory.CreateNew()),
                // TODO TimeSpan -> ???
                new EnumTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new NullableTypeTranslationContext(globalTranslationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new GenericDictionaryTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new SpecialTypeTranslationContext(typeof(IDictionary), Expression.UntypedDictionary(), TranslatedTypeMetadataFactory.CreateNew()),
                new ArrayTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new GenericEnumerableTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadataFactory.CreateNew()),
                new SpecialTypeTranslationContext(typeof(IEnumerable), Expression.UntypedArray(), TranslatedTypeMetadataFactory.CreateNew())
            };
        }
    }
}