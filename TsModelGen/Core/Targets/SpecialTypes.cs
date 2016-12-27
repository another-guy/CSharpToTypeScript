using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace TsModelGen.Core.Targets
{
    public sealed class SpecialTypes
    {
        public static readonly List<ITypeTranslationContext> AllProcessors = new List<ITypeTranslationContext>
        {
            new Dummy(typeof(IDictionary)),
            new Dummy(typeof(IDictionary<,>)),
            new Dummy(typeof(IEnumerable)),
            new Dummy(typeof(IEnumerable<>)),
            new Dummy(typeof(Nullable<>))
        };
    }

    public sealed class Dummy : ITypeTranslationContext
    {
        public Dummy(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Type = type.GetTypeInfo();
        }

        public TypeInfo Type { get; }

        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(Type.AsType());
        }
        public bool IsProcessed => true;
        public void ResolveDependencies()
        {
            throw new NotImplementedException();
        }
    }
}
