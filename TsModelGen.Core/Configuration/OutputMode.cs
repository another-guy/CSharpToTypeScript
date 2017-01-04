using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TsModelGen.Core.Configuration
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OutputMode
    {
        SingleFile,
        FlatDirectory,
        NamespaceHierarchyDirecory
    }
}