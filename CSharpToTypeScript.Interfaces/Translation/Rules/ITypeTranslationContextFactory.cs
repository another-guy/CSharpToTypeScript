using System;
using System.Reflection;

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
        ITypeTranslationContext Regular(TypeInfo typeInfo); // TODO TypeInfo vs Type...
        ITypeTranslationContext GenericType(TypeInfo typeInfo);
        
        // TODO GenericArgument
    }
}
