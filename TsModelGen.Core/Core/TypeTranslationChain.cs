﻿using System;
using System.Collections;
using TsModelGen.Core.TypeTranslationContext;
using TsModelGen.Core.TypeTranslationContext.Direct;
using TsModelGen.Core.TypeTranslationContext.Special;

namespace TsModelGen.Core
{
    public static class TypeTranslationChain
    {
        public static ITypeTranslationContext[] BuildDefault(TranslationContext globalTranslationContext)
        {
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
                // TODO consider better options if possible
                new DirectTypeTranslationContext(typeof(char), TypeScriptExpression.String()),
                new DirectTypeTranslationContext(typeof(DateTime), TypeScriptExpression.Date()),
                // TODO TimeSpan -> ???
                new EnumTypeTranslationContext(),
                new NullableTypeTranslationContext(globalTranslationContext),
                new GenericDictionaryTypeTranslationContext(globalTranslationContext),
                new SpecialTypeTranslationContext(typeof(IDictionary), TypeScriptExpression.UntypedDictionary()),
                new ArrayTypeTranslationContext(globalTranslationContext),
                new GenericEnumerableTypeTranslationContext(globalTranslationContext),
                new SpecialTypeTranslationContext(typeof(IEnumerable), TypeScriptExpression.UntypedArray())
            };
        }
    }
}