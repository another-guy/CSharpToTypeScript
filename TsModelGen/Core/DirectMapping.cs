namespace TsModelGen.Core
{
    public static class DirectMapping
    {
        // TODO Clean this up
        public static readonly DirectTranslationRule[] DirectTranslationRules =
            // Simple cases:
            // * object -> any
            // * primitive types to their TS direct translations
            {
                new DirectTranslationRule(typeof(object), "any"),
                new DirectTranslationRule(typeof(short), "number"),
                new DirectTranslationRule(typeof(int), "number"),
                new DirectTranslationRule(typeof(long), "number"),
                new DirectTranslationRule(typeof(ushort), "number"),
                new DirectTranslationRule(typeof(uint), "number"),
                new DirectTranslationRule(typeof(ulong), "number"),
                new DirectTranslationRule(typeof(byte), "number"),
                new DirectTranslationRule(typeof(sbyte), "number"),
                new DirectTranslationRule(typeof(float), "number"),
                new DirectTranslationRule(typeof(double), "number"),
                new DirectTranslationRule(typeof(decimal), "number"),
                new DirectTranslationRule(typeof(bool), "boolean"),
                new DirectTranslationRule(typeof(string), "string")

                // {TypeInfoOf<DateTime>(), "boolean"}
                // { char -> ??? },
                // { TimeSpan -> ??? },
            };
    }
}