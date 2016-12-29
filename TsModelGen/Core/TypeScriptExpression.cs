namespace TsModelGen.Core
{
    public static class TypeScriptExpression
    {
        public static string ClassNameExpression(string generatedTypeName)
        {
            return $"export class {generatedTypeName} ";
        }

        public static string InheritedClassExpression(string parentClassName)
        {
            return $"extends {parentClassName} ";
        }

        public static string StartClassBodyExpression()
        {
            return "{\n";
        }

        public static string MemberDefinitionExpression(string memberName, string memberType, string sourceType)
        {
            return $"  public {memberName}: {memberType}; // {sourceType}\n";
        }

        public static string EndClassBodyExpression()
        {
            return "}";
        }
    }
}