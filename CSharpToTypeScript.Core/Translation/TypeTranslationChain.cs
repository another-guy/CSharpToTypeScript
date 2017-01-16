using System;
using System.Collections;
using System.Collections.Generic;
using CSharpToTypeScript.Core.Translation.Rules;

namespace CSharpToTypeScript.Core.Translation
{
    // TODO IoC -- interface?
    // TODO IoC -- factory with static instance
    public sealed class TypeTranslationChain
    {
        private ITypeScriptExpression Expression { get; }
        private ITypeTranslationContextFactory TypeTranslationContextFactory { get; }

        public TypeTranslationChain(ITypeScriptExpression expression, ITypeTranslationContextFactory typeTranslationContextFactory)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            TypeTranslationContextFactory = typeTranslationContextFactory.NullToException(new ArgumentNullException(nameof(typeTranslationContextFactory)));
        }

        public IList<ITypeTranslationContext> BuildDefault()
        {
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
}