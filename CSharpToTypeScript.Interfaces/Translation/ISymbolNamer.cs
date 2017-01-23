using System;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation
{
    public interface ISymbolNamer
    {
        string GetNameFor(TypeInfo typeInfo, params Type[] genericTypeArguments);
    }
}
