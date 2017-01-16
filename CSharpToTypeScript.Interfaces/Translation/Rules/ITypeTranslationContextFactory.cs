using System;

namespace CSharpToTypeScript.Core.Translation.Rules
{

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
}
