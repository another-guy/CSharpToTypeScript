using System;

namespace TsModelGen.Core
{
    public static class TypeScriptExpression
    {
        public static string ClassNameExpression(string generatedTypeName)
        {
            return $"export class {generatedTypeName} ";
        }
        public static string EnumNameExpression(string generatedTypeName)
        {
            return $"export enum {generatedTypeName} ";
        }

        public static string InheritedClassExpression(string parentClassName)
        {
            return $"extends {parentClassName} ";
        }

        public static string BlockBegin()
        {
            return "{\n";
        }

        public static string MemberDefinitionExpression(string memberName, string memberType, string sourceType)
        {
            return $"  public {memberName}: {memberType}; // {sourceType}\n";
        }

        public static string BlockEnd()
        {
            return "}";
        }

        public static string EnumMemberExpression(string memberName, object memberValue)
        {
            return $"  {memberName} = {memberValue}";
        }

        public static readonly Func<string, string, string> CommaSeparatedLines =
            (allPreviousDeclarations, currentDeclaration) =>
                    $"{allPreviousDeclarations},\n{currentDeclaration}";

        public static string UntypedArray()
        {
            return GenericArrayOf("any");
        }

        public static string GenericArrayOf(string genericArgumentTranslatesSymbol)
        {
            return $"{genericArgumentTranslatesSymbol}[]";
        }
    }
}