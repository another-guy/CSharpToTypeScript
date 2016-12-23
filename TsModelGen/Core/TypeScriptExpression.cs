using System.Reflection;

namespace TsModelGen.Core
{
    public static class TypeScriptExpression
    {
        public static string ClassNameExpression(string generatedTypeName)
        {
            return $"export class {generatedTypeName} ";
        }

        public static string InheritedTypeExpression(TypeInfo parentType)
        {
            var generatedParentTypeName = GeneratedType.Name(parentType.BaseType.Name);
            return $"extends {generatedParentTypeName} ";
        }

        public static string StartClassBodyExpression()
        {
            return "{";
        }

        public static string MemberDefinitionExpression(string memberName, string memberType, string sourceType)
        {
            return $"  public {memberName}: {memberType}; // {sourceType}";
        }

        public static string EndClassBodyExpression()
        {
            return "}";
        }
    }
}
