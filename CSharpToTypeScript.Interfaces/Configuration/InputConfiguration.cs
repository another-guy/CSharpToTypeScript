using System.Collections.Generic;

namespace CSharpToTypeScript.Core.Configuration
{
    public sealed class InputConfiguration
    {
        public List<string> Assemblies { get; set; }

        public List<string> IncludeTypes { get; set; }
        public List<string> ExcludeTypes { get; set; }

        public List<string> SkipTypesWithAttribute { get; set; }
        public List<string> SkipMembersWithAttribute { get; set; }
    }
}
