using System.Collections;
using System.Collections.Generic;
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
                new DirectTypeTranslationContext(typeof(object), "any"),
                new DirectTypeTranslationContext(typeof(short), "number"),
                new DirectTypeTranslationContext(typeof(int), "number"),
                new DirectTypeTranslationContext(typeof(long), "number"),
                new DirectTypeTranslationContext(typeof(ushort), "number"),
                new DirectTypeTranslationContext(typeof(uint), "number"),
                new DirectTypeTranslationContext(typeof(ulong), "number"),
                new DirectTypeTranslationContext(typeof(byte), "number"),
                new DirectTypeTranslationContext(typeof(sbyte), "number"),
                new DirectTypeTranslationContext(typeof(float), "number"),
                new DirectTypeTranslationContext(typeof(double), "number"),
                new DirectTypeTranslationContext(typeof(decimal), "number"),
                new DirectTypeTranslationContext(typeof(bool), "boolean"),
                new DirectTypeTranslationContext(typeof(string), "string"),
                new DirectTypeTranslationContext(typeof(char), "string"),  // TODO consider better options if possible

                // {TypeInfoOf<DateTime>(), "boolean"}
                // { char -> ??? },
                // { TimeSpan -> ??? },

                new EnumTypeTranslationContext(), // Ok
                // TODO Replace DummySpecialTranslationType with specific entity type translation object
                new NullableTypeTranslationContext(globalTranslationContext), // Ok
                new SpecialTypeTranslationContext(typeof(IDictionary), "{ [id: any]: any; }"),
                // TODO 1. Is it better than `any` ? 2. Or `{ [id: any]: any; }` ?
                new SpecialTypeTranslationContext(typeof(IDictionary<,>), "any"), // Can be better, if we discover types
                new DirectTypeTranslationContext(typeof(IEnumerable), "any[]"), // Ok
                new GenericEnumerableTypeTranslationContext(globalTranslationContext)  // Not ok, make strongly typed
            };
        }
    }
}