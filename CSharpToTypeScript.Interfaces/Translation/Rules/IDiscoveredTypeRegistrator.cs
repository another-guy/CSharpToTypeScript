using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules
{
    public interface IDiscoveredTypeRegistrator
    {
        void RegisterType(TypeInfo typeInfo);
    }
}
