using System;
using System.Collections.Generic;

namespace TsModelGen.Core
{
    public static class DotNetToTypeScriptType
    {
        public static Dictionary<string, string> Mapping => MappingLazy.Value;

        private static readonly Lazy<Dictionary<string, string>> MappingLazy =
            new Lazy<Dictionary<string, string>>(
                // Simple cases:
                // * object -> any
                // * primitive types to their TS direct translations
                () => new Dictionary<string, string>
                {
                    {typeof(object).FullName, "any"},

                    {typeof(short).FullName, "number"},
                    {typeof(int).FullName, "number"},
                    {typeof(long).FullName, "number"},
                    {typeof(ushort).FullName, "number"},
                    {typeof(uint).FullName, "number"},
                    {typeof(ulong).FullName, "number"},
                    {typeof(byte).FullName, "number"},
                    {typeof(sbyte).FullName, "number"},
                    {typeof(float).FullName, "number"},
                    {typeof(double).FullName, "number"},

                    {typeof(bool).FullName, "boolean"},

                    {typeof(string).FullName, "string"},

                    {typeof(DateTime).FullName, "boolean"}

                    // { typeof(char).FullName, "any" },
                    // { typeof(TimeSpan).FullName, "boolean" },
                });
    }
}