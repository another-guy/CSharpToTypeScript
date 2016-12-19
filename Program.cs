using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TsModelGen
{
    public class Program
    {
        private static Dictionary<string, ProcessingInfo> _listOfTargetModelTypes;

        public static void Main(string[] args)
        {
            var targetNameSpace = "TsModelGen.TargetNamespace"; // TODO Move this to input parameters

            _listOfTargetModelTypes = Assembly
                .GetEntryAssembly()
                .DefinedTypes
                .Where(typeInfo => typeInfo.Namespace.StartsWith(targetNameSpace))
                .ToDictionary(typeInfo => typeInfo.FullName, ProcessingInfo.ForPassedType);

            var iteration = 1;
            while (true)
            {
                if (iteration >= 32) throw new InvalidOperationException("");

                var processInfos = _listOfTargetModelTypes
                    .Where(pair => string.IsNullOrWhiteSpace(pair.Value.GeneratedDefinition))
                    .ToList();

                if (processInfos.Count <= 0) break;
                
                foreach (var processInfo in processInfos)
                    GenerateTypescriptModel(processInfo.Value);

                iteration += 1;
            }

            var generatedText = _listOfTargetModelTypes
                .Values
                .Aggregate("", (result, processingInfo) => result + processingInfo.GeneratedDefinition + "\n");
            Console.WriteLine(generatedText);
            Console.ReadKey();
        }

        private static void GenerateTypescriptModel(ProcessingInfo processInfo)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var declaredPublicInstanceProperties = processInfo.Type.GetProperties(flags);
            var declaredPublicInstanceFields = processInfo.Type.GetFields(flags);
            var declaredSerializableMembers =
                declaredPublicInstanceProperties.Select(p => Tuple.Create(p.Name, p.PropertyType))
                .Concat(declaredPublicInstanceFields.Select(t => Tuple.Create(t.Name, t.FieldType)));


            var type = processInfo.Type;

            var generatedTypeName = GenTypeName(type.Name);

            var sb = new StringBuilder();
            sb.Append($"export class {generatedTypeName}");
            
            if (type.BaseType.FullName != "System.Object")
                sb.Append($" extends {GenTypeName(type.BaseType.Name)} ");

            sb.AppendLine($" {{");

            foreach (var memberInfo in declaredSerializableMembers)
            {
                var fieldName = memberInfo.Item1;
                var fieldType = GenerateTypeReference(memberInfo.Item2);
                sb.AppendLine($"  public {fieldName}: {fieldType};");
            }

            sb.AppendLine($"}}");
            var generatedDefinition = sb.ToString();

            processInfo.SaveGeneratedDefinition(generatedTypeName, generatedDefinition);
        }

        private static string GenTypeName(string typeName)
        {
            return $"{typeName}Generated";
        }

        private static string GenerateTypeReference(Type propertyInfoPropertyType)
        {
            string result;

            // Simple cases:
            // * object -> any
            // * primitive types to their TS direct translations
            var mapping = new Dictionary<string, string>
            {
                { typeof(object).FullName, "any" },

                { typeof(short).FullName, "number" },
                { typeof(int).FullName, "number" },
                { typeof(long).FullName, "number" },
                { typeof(ushort).FullName, "number" },
                { typeof(uint).FullName, "number" },
                { typeof(ulong).FullName, "number" },
                { typeof(byte).FullName, "number" },
                { typeof(sbyte).FullName, "number" },
                { typeof(float).FullName, "number" },
                { typeof(double).FullName, "number" },

                { typeof(bool).FullName, "boolean" },

                { typeof(string).FullName, "string" },

                { typeof(DateTime).FullName, "boolean" }

                // { typeof(char).FullName, "any" },
                // { typeof(TimeSpan).FullName, "boolean" },
                // { typeof(Nullable<>).FullName, "boolean" },
            };

            var fullTypeName = propertyInfoPropertyType.FullName;
            if (mapping.TryGetValue(fullTypeName, out result))
                return result;

            // TODO Handle references to known Generated types
            ProcessingInfo processingInfo;
            if (_listOfTargetModelTypes.TryGetValue(fullTypeName, out processingInfo))
                return processingInfo.GeneratedName;

            // TODO Handle nullables
            // TODO Handle generics
            // TODO Handle collections

            return "any";
        }
    }

    public sealed class ProcessingInfo
    {
        public TypeInfo Type { get; }

        public string GeneratedName { get; private set; }
        public string GeneratedDefinition { get; private set; }

        private ProcessingInfo(TypeInfo targetType)
        {
            Type = targetType;
            GeneratedName = null;
            GeneratedDefinition = null;
        }

        public static ProcessingInfo ForPassedType(TypeInfo typeInfo)
        {
            return new ProcessingInfo(typeInfo);
        }

        public void SaveGeneratedDefinition(string generatedName, string generatedDefinition)
        {
            GeneratedName = generatedName;
            GeneratedDefinition = generatedDefinition;
        }
    }
}
