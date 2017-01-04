using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CSharpToTypeScript.Core.Configuration
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OutputMode
    {
        SingleFile,
        FlatDirectory,
        NamespaceHierarchyDirecory
    }
}