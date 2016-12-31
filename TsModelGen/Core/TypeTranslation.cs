using System;
using System.Collections;
using TsModelGen.Core.TypeTranslationContext;
using TsModelGen.Core.TypeTranslationContext.Direct;
using TsModelGen.Core.TypeTranslationContext.Special;

namespace TsModelGen.Core
{
    public static class TypeTranslation
    {
        // TODO Clean this up
        public static ITypeTranslationContext[] CreateContextChain(TranslationContext globalTranslationContext)
        {
            // Simple cases:
            // * object -> any
            // * primitive types to their TS direct translations
            return new ITypeTranslationContext[]
            {
                new DirectTypeTranslationContext(typeof(object), TypeScriptExpression.Any()),
                new DirectTypeTranslationContext(typeof(short), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(int), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(long), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(ushort), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(uint), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(ulong), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(byte), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(sbyte), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(float), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(double), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(decimal), TypeScriptExpression.Number()),
                new DirectTypeTranslationContext(typeof(bool), TypeScriptExpression.Bool()),
                new DirectTypeTranslationContext(typeof(string), TypeScriptExpression.String()),
                new DirectTypeTranslationContext(typeof(char), TypeScriptExpression.String()),  // TODO consider better options if possible
                new DirectTypeTranslationContext(typeof(DateTime), TypeScriptExpression.Date()),
                // { TimeSpan -> ??? },

                new EnumTypeTranslationContext(), // Ok
                new NullableTypeTranslationContext(globalTranslationContext), // Ok
                
                new GenericDictionaryTypeTranslationContext(globalTranslationContext), // Can be better, if we discover types
                new SpecialTypeTranslationContext(typeof(IDictionary), TypeScriptExpression.UntypedDictionary()),

                new ArrayTypeTranslationContext(globalTranslationContext), // Ok
                new GenericEnumerableTypeTranslationContext(globalTranslationContext), // Not ok, make strongly typed
                new SpecialTypeTranslationContext(typeof(IEnumerable), TypeScriptExpression.UntypedArray()) // Ok
            };
        }
    }
}