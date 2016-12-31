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
            return GenericArrayOf(Any());
        }

        public static string GenericArrayOf(string genericArgumentTranslatesSymbol)
        {
            return $"{genericArgumentTranslatesSymbol}[]";
        }

        public static string UntypedDictionary()
        {
            return GenericDictionaryOf(String(), Any());
        }

        public static string GenericDictionaryOf(string translatedKeySymbol, string translatedValueSymbol)
        {
            return $"{{ [id: {translatedKeySymbol}]: {translatedValueSymbol}; }}";
        }

        public static string Any()
        {
            return "any";
        }

        public static string String()
        {
            return "string";
        }

        public static string Date()
        {
            return "Date";
        }

        public static string Number()
        {
            return "number";
        }

        public static string Bool()
        {
            return "boolean";
        }
    }
}