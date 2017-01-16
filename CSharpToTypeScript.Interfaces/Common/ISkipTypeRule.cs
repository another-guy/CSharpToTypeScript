using System.Reflection;

namespace CSharpToTypeScript.Core.Common
{
    public interface ISkipTypeRule
    {
        bool AppliesTo(MemberInfo sourceType);
    }
}
