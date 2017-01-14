using System;
using System.Collections;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.Core.Translation.Rules.Direct;
using CSharpToTypeScript.Core.Translation.Rules.Special;

namespace CSharpToTypeScript.Core.Translation
{
    public static class TypeTranslationChain
    {
        public static ITypeTranslationContext[] BuildDefault(
            ITypeScriptExpression expression,
            TranslationContext globalTranslationContext)
        {
            return new ITypeTranslationContext[]
            {
                new DirectTypeTranslationContext(typeof(object), expression.Any()),
                new DirectTypeTranslationContext(typeof(short), expression.Number()),
                new DirectTypeTranslationContext(typeof(int), expression.Number()),
                new DirectTypeTranslationContext(typeof(long), expression.Number()),
                new DirectTypeTranslationContext(typeof(ushort), expression.Number()),
                new DirectTypeTranslationContext(typeof(uint), expression.Number()),
                new DirectTypeTranslationContext(typeof(ulong), expression.Number()),
                new DirectTypeTranslationContext(typeof(byte), expression.Number()),
                new DirectTypeTranslationContext(typeof(sbyte), expression.Number()),
                new DirectTypeTranslationContext(typeof(float), expression.Number()),
                new DirectTypeTranslationContext(typeof(double), expression.Number()),
                new DirectTypeTranslationContext(typeof(decimal), expression.Number()),
                new DirectTypeTranslationContext(typeof(bool), expression.Bool()),
                new DirectTypeTranslationContext(typeof(string), expression.String()),
                // TODO consider better options if possible
                new DirectTypeTranslationContext(typeof(char), expression.String()),
                new DirectTypeTranslationContext(typeof(DateTime), expression.Date()),
                // TODO TimeSpan -> ???
                new EnumTypeTranslationContext(expression, globalTranslationContext),
                new NullableTypeTranslationContext(globalTranslationContext),
                new GenericDictionaryTypeTranslationContext(expression, globalTranslationContext),
                new SpecialTypeTranslationContext(typeof(IDictionary), expression.UntypedDictionary()),
                new ArrayTypeTranslationContext(expression, globalTranslationContext),
                new GenericEnumerableTypeTranslationContext(expression, globalTranslationContext),
                new SpecialTypeTranslationContext(typeof(IEnumerable), expression.UntypedArray())
            };
        }
    }
}