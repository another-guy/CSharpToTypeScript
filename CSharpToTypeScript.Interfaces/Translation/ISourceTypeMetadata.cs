using System.Collections.Generic;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation
{
    public interface ISourceTypeMetadata
    {
        TypeInfo BaseType { get; set; }

        IEnumerable<string> Members { get; }

        MemberInfo this[string memberName] { get; set; }
    }
}
