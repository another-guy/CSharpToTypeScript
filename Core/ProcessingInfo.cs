using System.Reflection;

namespace TsModelGen.Core
{
    public sealed class ProcessingInfo
    {
        public static ProcessingInfo ForPassedType(TypeInfo typeInfo)
        {
            return new ProcessingInfo(typeInfo);
        }

        public TypeInfo Type { get; }

        public string GeneratedName { get; private set; }
        public string GeneratedDefinition { get; private set; }

        private ProcessingInfo(TypeInfo targetType)
        {
            Type = targetType;
            GeneratedName = null;
            GeneratedDefinition = null;
        }

        public void SaveGeneratedDefinition(string generatedName, string generatedDefinition)
        {
            GeneratedName = generatedName;
            GeneratedDefinition = generatedDefinition;
        }
    }
}
