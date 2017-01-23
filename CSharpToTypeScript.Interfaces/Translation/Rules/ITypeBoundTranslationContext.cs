using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules
{
    // TODO Apply everywhere possible
    public interface ITypeBoundTranslationContext
    {
        TypeInfo TypeInfo { get; }
    }
}
