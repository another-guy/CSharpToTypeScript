using System.Reflection;

namespace CSharpToTypeScript.Core
{
    public sealed class TranslationResult
    {
        public string Definition { get; }
        public TypeInfo TranslatedType { get; }

        public TranslationResult(TypeInfo translatedType, string definition)
        {
            TranslatedType = translatedType;
            Definition = definition;
        }
    }
}