using System;
using System.Collections;
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
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }

        public TypeTranslationChain(
            ITypeScriptExpression expression,
            ITranslatedTypeMetadata translatedTypeMetadata)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TranslatedTypeMetadata = translatedTypeMetadata.NullToException(new ArgumentNullException(nameof(translatedTypeMetadata)));
        }

        public ITypeTranslationContext[] BuildDefault(
            ITypeScriptExpression expression,
            TranslationContext globalTranslationContext)
        {
            // TODO Ioc -- Fix direct binding...
            return new ITypeTranslationContext[]
            {
                new DirectTypeTranslationContext(typeof(object), Expression.Any(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(short), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(int), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(long), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(ushort), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(uint), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(ulong), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(byte), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(sbyte), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(float), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(double), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(decimal), Expression.Number(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(bool), Expression.Bool(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(string), Expression.String(), TranslatedTypeMetadata),
                // TODO consider better options if possible
                new DirectTypeTranslationContext(typeof(char), Expression.String(), TranslatedTypeMetadata),
                new DirectTypeTranslationContext(typeof(DateTime), Expression.Date(), TranslatedTypeMetadata),
                // TODO TimeSpan -> ???
                new EnumTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadata),
                new NullableTypeTranslationContext(globalTranslationContext, TranslatedTypeMetadata),
                new GenericDictionaryTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadata),
                new SpecialTypeTranslationContext(typeof(IDictionary), Expression.UntypedDictionary(), TranslatedTypeMetadata),
                new ArrayTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadata),
                new GenericEnumerableTypeTranslationContext(expression, globalTranslationContext, TranslatedTypeMetadata),
                new SpecialTypeTranslationContext(typeof(IEnumerable), Expression.UntypedArray(), TranslatedTypeMetadata)
            };
        }
    }
}