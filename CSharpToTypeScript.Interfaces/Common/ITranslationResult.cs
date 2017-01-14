using System.Reflection;

namespace CSharpToTypeScript.Core.Common
{
    public interface ITranslationResult
    {
        string Definition { get; }
        TypeInfo TranslatedType { get; }
    }
}
