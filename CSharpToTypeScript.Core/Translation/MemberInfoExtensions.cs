using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation
{
    public static class MemberInfoExtensions
    {
        public static IEnumerable<Attribute> GetCustomAttributesSafe(this MemberInfo memberInfo)
        {
            try
            {
                return memberInfo.GetCustomAttributes();
            }
            catch (Exception)
            {
                // TODO Log the exception
                return new Attribute[0];
            }
        }

        public static TypeInfo GetPropertyTypeInfoSafe(this PropertyInfo propertyInfo)
        {
            try
            {
                return propertyInfo
                    .NullToException(new ArgumentNullException(nameof(propertyInfo)))
                    .PropertyType
                    .GetTypeInfo();
            }
            catch (FileNotFoundException)
            {
                // TODO Log the exception
                return null;
            }
        }

        public static TypeInfo GetFieldTypeInfoSafe(this FieldInfo fieldInfo)
        {
            try
            {
                return fieldInfo
                    .NullToException(new ArgumentNullException(nameof(fieldInfo)))
                    .FieldType
                    .GetTypeInfo();
            }
            catch (FileNotFoundException)
            {
                // TODO Log the exception
                return null;
            }
        }


        public static Type GetPropertyTypeSafe(this PropertyInfo propertyInfo)
        {
            try
            {
                return propertyInfo
                    .NullToException(new ArgumentNullException(nameof(propertyInfo)))
                    .PropertyType;
            }
            catch (FileNotFoundException)
            {
                // TODO Log the exception
                return null;
            }
        }

        public static Type GetFieldTypeSafe(this FieldInfo fieldInfo)
        {
            try
            {
                return fieldInfo
                    .NullToException(new ArgumentNullException(nameof(fieldInfo)))
                    .FieldType;
            }
            catch (FileNotFoundException)
            {
                // TODO Log the exception
                return null;
            }
        }
    }
}
