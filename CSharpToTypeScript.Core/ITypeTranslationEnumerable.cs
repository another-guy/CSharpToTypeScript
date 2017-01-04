using System.Collections.Generic;
using CSharpToTypeScript.Core.TypeTranslation;

namespace CSharpToTypeScript.Core
{
    public interface ITypeTranslationEnumerable : IEnumerable<ITypeTranslationContext> { }
}