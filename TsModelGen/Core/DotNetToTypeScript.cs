using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TsModelGen.Core.Targets;

namespace TsModelGen.Core
{
    // TODO Fundamental issue: How to get assembly metadata without loading it into AppDomain? If loaded, how to unload? Does AppDomain exist in Net Core?
    public sealed class DotNetToTypeScript
    {
        private readonly Dictionary<string, ProcessingInfo> _processingContext;

        public DotNetToTypeScript(IEnumerable<string> targetNameSpaces)
        {
            _processingContext = GetTargetModelTypes(targetNameSpaces);
        }

        private static Dictionary<string, ProcessingInfo> GetTargetModelTypes(IEnumerable<string> targetNameSpaces)
        {
            // TODO Assembly list can be a parameter
            return Assembly
                .GetEntryAssembly()
                .DefinedTypes
                .Where(typeInfo => targetNameSpaces.Any(@namespace => typeInfo.Namespace.StartsWith(@namespace)))
                .ToDictionary(typeInfo => typeInfo.FullName, ProcessingInfo.ForPassedType);
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
                var sourceType = serializableMember.Item2;
                var generatedType = GenerateTypeReference(sourceType);
                sb.AppendLine(TypeScriptExpression.MemberDefinitionExpression(memberName, generatedType, HumanFriendly(sourceType)));
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

            // TODO Move this to a separate class that uses Chain of Responsibility to do the job

            // Primitive types
            string specificTypeName;
            if (Map.DotNetToTypeScriptType.TryGetValue(fullTypeName, out specificTypeName))
                return specificTypeName;

            // Previously processed type
            ProcessingInfo processingInfo;
            if (_processingContext.TryGetValue(fullTypeName, out processingInfo))
                return GeneratedType.Name(processingInfo.Type.Name);

            
            // TODO Test this part
            // Dictionary types
            if (
                propertyInfoPropertyType.IsChildTypeOf(typeof(IDictionary)) ||
                propertyInfoPropertyType.IsChildTypeOfPossiblyOpenGeneric(typeof(IDictionary<,>))
            )
            {
                // TODO May want to handle in more detals branching on   propertyInfoPropertyType.IsConstructedGenericType
                return "any";
            }

            // Array types
            if (propertyInfoPropertyType.IsChildTypeOf(typeof(IEnumerable)))
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

        private string HumanFriendly(Type sourceType)
        {
            var baseTypeName = sourceType.Name;
            var genericTypeArguments = "";

            if (sourceType.IsConstructedGenericType)
            {
                baseTypeName = baseTypeName.Substring(0, baseTypeName.Length - 2);

                var typeList = sourceType
                    .GenericTypeArguments
                    .Select(HumanFriendly)
                    .Aggregate((result, type) => result + ", " + type);
                genericTypeArguments = $"<{typeList}>";
            }
            
            return $"{sourceType.Namespace}.{baseTypeName}{genericTypeArguments}";
        }

        private string AllGeneratedTypesAsText()
        {
            var preamble = $"// These TypeScript definitions are generated from .NET classes.\n" +
                           $"// Any direct changes to these definition will be lost when the code is regenerated.";
            var generatedText =
                _processingContext.Values.Aggregate("", (result, processingInfo) => $"{result}{processingInfo.GeneratedDefinition}\n");
            return $"{preamble}\n\n{generatedText}";
        }
    }
}
