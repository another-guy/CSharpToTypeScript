﻿using System;
using System.Reflection;

namespace TsModelGen.Core.TypeTranslationContext.Special
{
    public sealed class SpecialTypeTranslationContext : ITypeTranslationContext
    {
        public SpecialTypeTranslationContext(Type type, string symbol)
        {
            Type = type.NullToException(new ArgumentNullException(nameof(type))).GetTypeInfo();
            Symbol = symbol.NullToException(new ArgumentNullException(nameof(symbol)));
        }

        public TypeInfo Type { get; }
        private string Symbol { get; }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(Type.AsType());
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type _)
        {
            return new TranslatedTypeMetadata { Symbol = Symbol };
        }
    }
}