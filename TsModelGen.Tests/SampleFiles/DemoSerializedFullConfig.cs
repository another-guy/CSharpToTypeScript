using System.Collections.Generic;
using Newtonsoft.Json;
using TsModelGen.Configuration;

namespace TsModelGen.Tests.SampleFiles
{
    public class DemoSerializedFullConfig
    {
        public void CreateConfiguration()
        {
            var demoConfig = new CompleteConfiguration
            {
                Input = new InputConfiguration
                {
                    Assemblies = new List<string> { "a", "b" },
                    IncludeTypes = new List<string> { "a", "b" },
                    ExcludeTypes = new List<string> { "a", "b" },
                    ExcludeTypesWithAttribute = new List<string> { "a", "b" },
                    ExcludeMembersWithAttribute = new List<string> { "a", "b" }
                },
                Output = new OutputConfiguration
                {
                    Location = "location",
                    Mode = OutputMode.NamespaceHierarchyDirecory
                },
                Translation = new TranslationConfiguration
                {
                    GeneratedSymbols = new GeneratedSymbolsConfiguration
                    {
                        Prefix = "Pref",
                        Suffix = "Suff"
                    }
                }
            };
            var serialized = JsonConvert.SerializeObject(demoConfig);
            serialized.ToString();
        }
    }
}
