using System.Reflection;

namespace CSharpToTypeScript.Core.Common
{
    public sealed class TranslationResult : ITranslationResult
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