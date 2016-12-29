using System.Reflection;
using TsModelGen.Core.TypeTranslationContext;

namespace TsModelGen.Core
{
    public interface ITypeRegistry
    {
        bool CanProcess(TypeInfo typeInfo);
        void AddTypeTranslationContextForType(TypeInfo typeInfo);
        void AddTypeTranslationContext(ITypeTranslationContext specialTypeProcessor);
    }
}