using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TsModelGen.Core
{
    public class DotNetToTypeScript
    {
        private Dictionary<string, ProcessingInfo> _processingContext;

        public string Translate(IEnumerable<string> targetNameSpaces)
        {
            _processingContext = GetTargetModelTypes(targetNameSpaces);

            var iteration = 1;
            while (true)
            {
                if (iteration >= 32) throw new InvalidOperationException("Too many iteration have passed. We seem to be in an infinite loop. Time to crash...");

                var processInfos = _processingContext
                    .Where(pair => string.IsNullOrWhiteSpace(pair.Value.GeneratedDefinition))
                    .ToList();

                if (processInfos.Count <= 0) break;

                foreach (var processInfo in processInfos)
                    GenerateTypescriptModel(processInfo.Value);

                iteration += 1;
            }
            
            return AllGeneratedTypesAsText();
        }

        private static Dictionary<string, ProcessingInfo> GetTargetModelTypes(IEnumerable<string> targetNameSpaces)
        {
            return Assembly
                .GetEntryAssembly()
                .DefinedTypes
                .Where(typeInfo => targetNameSpaces.Any(@namespace => typeInfo.Namespace.StartsWith(@namespace)))
                .ToDictionary(typeInfo => typeInfo.FullName, ProcessingInfo.ForPassedType);
        }

        private void GenerateTypescriptModel(ProcessingInfo processInfo)
        {
            var type = processInfo.Type;

            var generatedTypeName = GeneratedTypeNameFromShortName(type.Name);

            var sb = new StringBuilder();
            sb.Append($"export class {generatedTypeName}");

            if (type.BaseType.FullName != "System.Object")
                sb.Append($" extends {GeneratedTypeNameFromShortName(type.BaseType.Name)} ");

            sb.AppendLine($" {{");

            foreach (var serializableMember in GetTranslatableMembers(processInfo))
            {
                var fieldName = serializableMember.Item1;
                var fieldType = GenerateTypeReference(serializableMember.Item2);
                sb.AppendLine($"  public {fieldName}: {fieldType};");
            }

            sb.AppendLine($"}}");
            var generatedDefinition = sb.ToString();

            processInfo.SaveGeneratedDefinition(generatedTypeName, generatedDefinition);
        }

        private static IEnumerable<Tuple<string, Type>> GetTranslatableMembers(ProcessingInfo processInfo)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var type = processInfo.Type;
            var declaredPublicInstanceProperties = type.GetProperties(flags).Select(property => Tuple.Create(property.Name, property.PropertyType));
            var declaredPublicInstanceFields = type.GetFields(flags).Select(field => Tuple.Create(field.Name, field.FieldType));
            return declaredPublicInstanceProperties.Concat(declaredPublicInstanceFields);
        }

        private string GenerateTypeReference(Type propertyInfoPropertyType)
        {
            var fullTypeName = propertyInfoPropertyType.FullName;
            string specificTypeName;
            if (DotNetToTypeScriptType.Mapping.TryGetValue(fullTypeName, out specificTypeName))
                return specificTypeName;

            ProcessingInfo processingInfo;
            if (_processingContext.TryGetValue(fullTypeName, out processingInfo))
                return GeneratedTypeNameFromShortName(processingInfo.Type.Name);

            // TODO Handle nullables
            // TODO Handle generics
            // TODO Handle collections

            return "any";
        }

        private string AllGeneratedTypesAsText()
        {
            return _processingContext
                .Values
                .Aggregate("", (result, processingInfo) => result + processingInfo.GeneratedDefinition + "\n");
        }

        private static string GeneratedTypeNameFromShortName(string typeName)
        {
            return $"{typeName}Generated";
        }
    }
}
