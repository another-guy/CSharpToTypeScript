using System.Collections.Generic;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Input
{
    public interface ITargetTypesLocator
    {
        IEnumerable<TypeInfo> LocateRootTargetsUsing(InputConfiguration configuration);
    }
}
