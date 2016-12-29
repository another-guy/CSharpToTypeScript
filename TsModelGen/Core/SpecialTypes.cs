using System;
using System.Collections;
using System.Collections.Generic;
using TsModelGen.Core.TypeTranslationContext;
using TsModelGen.Core.TypeTranslationContext.Special;

namespace TsModelGen.Core
{
    public sealed class SpecialTypes
    {
        public static readonly List<ITypeTranslationContext> AllProcessors = new List<ITypeTranslationContext>
        {
            // TODO Replace DummySpecialTranslationType with specific entity type translation object
            new SpecialTypeTranslationContext(typeof(Enum), "?ENUM?"), // Not ok
            new SpecialTypeTranslationContext(typeof(ValueType), "?valuetype?"), // Not ok

            new SpecialTypeTranslationContext(typeof(IDictionary), "any"), // Ok
            new SpecialTypeTranslationContext(typeof(IDictionary<,>), "any"), // Can be better, if we discover types
            new SpecialTypeTranslationContext(typeof(IEnumerable), "any[]"), // Not ok, make strongly typed
            new SpecialTypeTranslationContext(typeof(IEnumerable<>), "any[]"), // Not ok, make strongly typed
            new SpecialTypeTranslationContext(typeof(Nullable<>), "any") // Not ok, make strongly typed
        };
    }
}