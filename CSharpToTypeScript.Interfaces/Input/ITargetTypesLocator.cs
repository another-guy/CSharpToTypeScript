using System.Collections.Generic;
using System.Reflection;

namespace CSharpToTypeScript.Core.Input
{
    public interface ITargetTypesLocator
    {
        IEnumerable<TypeInfo> LocateRootTargets();
    }
}
