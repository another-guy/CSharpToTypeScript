using System;
using System.Collections.Generic;

namespace TsModelGen.Core
{
    public static class Map
    {
        public static Dictionary<string, string> DotNetToTypeScriptType => DotNetToTypeScriptTypeLazy.Value;

        private static readonly Lazy<Dictionary<string, string>> DotNetToTypeScriptTypeLazy =
            new Lazy<Dictionary<string, string>>(
                // Simple cases:
                // * object -> any
                // * primitive types to their TS direct translations
                () => new Dictionary<string, string>
                {
                    {NameOf<object>(), "any"},

                    {NameOf<short>(), "number"},
                    {NameOf<int>(), "number"},
                    {NameOf<long>(), "number"},
                    {NameOf<ushort>(), "number"},
                    {NameOf<uint>(), "number"},
                    {NameOf<ulong>(), "number"},
                    {NameOf<byte>(), "number"},
                    {NameOf<sbyte>(), "number"},
                    {NameOf<float>(), "number"},
                    {NameOf<double>(), "number"},
                    {NameOf<decimal>(), "number"},

                    {NameOf<bool>(), "boolean"},

                    {NameOf<string>(), "string"},

                    {NameOf<DateTime>(), "boolean"}

                    // { char -> ??? },
                    // { TimeSpan -> ??? },
                });

        private static string NameOf<T>()
        {
            return typeof(T).FullName;
        }
    }
}