using System.Reflection;

namespace CSharpToTypeScript.Core.Translation
{
    public interface ICommenter
    {
        string TypeCommentFor(TypeInfo typeInfo);
    }
}
