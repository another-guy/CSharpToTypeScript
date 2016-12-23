using System;
using System.Collections.Generic;

namespace TsModelGen.Core.Targets
{

    // TODO Clean this up
    public static class DirectMapping
    {
        public static Dictionary<Type, string> DotNetToTypeScriptType => DotNetToTypeScriptTypeLazy.Value;

        private static readonly Lazy<Dictionary<Type, string>> DotNetToTypeScriptTypeLazy =
            new Lazy<Dictionary<Type, string>>(
                // Simple cases:
                // * object -> any
                // * primitive types to their TS direct translations
                () => new Dictionary<Type, string>
                {
                    {KeyFor<object>(), "any"},

                    {KeyFor<short>(), "number"},
                    {KeyFor<int>(), "number"},
                    {KeyFor<long>(), "number"},
                    {KeyFor<ushort>(), "number"},
                    {KeyFor<uint>(), "number"},
                    {KeyFor<ulong>(), "number"},
                    {KeyFor<byte>(), "number"},
                    {KeyFor<sbyte>(), "number"},
                    {KeyFor<float>(), "number"},
                    {KeyFor<double>(), "number"},
                    {KeyFor<decimal>(), "number"},

                    {KeyFor<bool>(), "boolean"},

                    {KeyFor<string>(), "string"},

                    {KeyFor<DateTime>(), "boolean"}

                    // { char -> ??? },
                    // { TimeSpan -> ??? },
                });

        private static Type KeyFor<T>()
        {
            return typeof(T);
        }
    }
}