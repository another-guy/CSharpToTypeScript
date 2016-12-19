using System;
using System.Linq;
using System.Reflection;

namespace TsModelGen.Core
{
    public static class TypeExtensions
    {
        public static bool Is(this Type checkedType, Type targetType)
        {
            return targetType.IsAssignableFrom(checkedType) ||
                   checkedType.GetInterfaces().ToList().Contains(targetType);
        }
    }
}
