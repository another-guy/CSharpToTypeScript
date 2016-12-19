using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TsModelGen.Core
{
    public sealed class DotNetToTypeScript
    {
        private readonly Dictionary<string, ProcessingInfo> _processingContext;

        public DotNetToTypeScript(IEnumerable<string> targetNameSpaces)
        {
            _processingContext = GetTargetModelTypes(targetNameSpaces);
        }

        public string Translate(ushort totalIterationLimit = 32)
        {
            var iteration = 1;
            while (true)
            {
                if (iteration++ >= totalIterationLimit) throw new StopTranslationException();

                var typesWithoutTranslatedDefinitions = _processingContext
                    .Where(pair => string.IsNullOrWhiteSpace(pair.Value.GeneratedDefinition))
                    .ToList();

                if (typesWithoutTranslatedDefinitions.Count <= 0) break;

                foreach (var typeEntry in typesWithoutTranslatedDefinitions)
                    GenerateTypescriptModel(typeEntry.Value);
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

            var generatedTypeName = GeneratedType.Name(type.Name);

            var sb = new StringBuilder();
            sb.Append(TypeScriptExpression.ClassNameExpression(generatedTypeName));

            if (type.BaseType.FullName != "System.Object")
                sb.Append(TypeScriptExpression.InheritedTypeExpression(type));

            sb.AppendLine(TypeScriptExpression.StartClassBodyExpression());

            foreach (var serializableMember in GetTranslatableMembers(processInfo))
            {
                var memberName = serializableMember.Item1;
                var memberType = GenerateTypeReference(serializableMember.Item2);
                sb.AppendLine(TypeScriptExpression.MemberDefinitionExpression(memberName, memberType));
            }

            sb.AppendLine(TypeScriptExpression.EndClassBodyExpression());

            processInfo.SaveGeneratedDefinition(generatedTypeName, generatedDefinition: sb.ToString());
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

            // Primitive types
            string specificTypeName;
            if (DotNetToTypeScriptType.Mapping.TryGetValue(fullTypeName, out specificTypeName))
                return specificTypeName;

            // Previously processed type
            ProcessingInfo processingInfo;
            if (_processingContext.TryGetValue(fullTypeName, out processingInfo))
                return GeneratedType.Name(processingInfo.Type.Name);

            
            // TODO Test this part
            // Dictionary types
            if (
                propertyInfoPropertyType.GetInterfaces().Contains(typeof(IDictionary)) ||
                propertyInfoPropertyType.GetInterfaces().Contains(typeof(IDictionary<,>))
            )
            {
                // TODO May want to handle in more detals branching on   propertyInfoPropertyType.IsConstructedGenericType
                return "any";
            }

            // Array types
            if (propertyInfoPropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                if (propertyInfoPropertyType.IsConstructedGenericType)
                {
                    var generatedTypeName = GenerateTypeReference(propertyInfoPropertyType.GetGenericArguments().First());
                    return $"{generatedTypeName}[]";
                }
                else if (propertyInfoPropertyType.IsArray)
                {
                    var generatedTypeName = GenerateTypeReference(propertyInfoPropertyType.GetElementType());
                    return $"{generatedTypeName}[]";
                }
                else
                    return "any[]";
            }

            // Generic type
            if (propertyInfoPropertyType.IsConstructedGenericType)
            {
                if (propertyInfoPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return GenerateTypeReference(propertyInfoPropertyType.GetGenericArguments().First());
                
                // TODO Handle complex generics
            }

            // Unrecognized type
            return "any";
        }

        private string AllGeneratedTypesAsText()
        {
            return _processingContext.Values.Aggregate("", (result, processingInfo) => $"{result}{processingInfo.GeneratedDefinition}\n");
        }
    }
}
