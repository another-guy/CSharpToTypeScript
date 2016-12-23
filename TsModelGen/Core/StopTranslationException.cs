using System;

namespace TsModelGen.Core
{
    public sealed class StopTranslationException : InvalidOperationException
    {
        public StopTranslationException()
            : base("Too many iteration have passed. We seem to be in an infinite loop. Time to crash...")
        {
        }
    }
}