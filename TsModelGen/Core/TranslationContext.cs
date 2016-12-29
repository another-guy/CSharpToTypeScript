using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TsModelGen.Core.TypeTranslationContext;
using TsModelGen.Core.TypeTranslationContext.Direct;

namespace TsModelGen.Core
{
    public sealed class TranslationContext : ITypeRegistry, ITypeTranslationEnumerable
    {
        // TODO Make this dynamic -- let clients alter the chain to fit their need
        private IList<ITypeTranslationContext> TranslationChain { get; } =
            new List<ITypeTranslationContext>();

        public TranslationContext()
        {
            foreach (var rule in DirectMapping.DirectTranslationRules)
                AddTypeTranslationContext(new DirectTypeTranslationContext(rule.SourceType.GetTypeInfo(), rule.DestinationType));

            foreach (var specialTypeProcessor in SpecialTypes.AllProcessors)
                AddTypeTranslationContext(specialTypeProcessor);
        }

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }

        public void AddTypeTranslationContextForType(TypeInfo typeInfo)
        {
            AddTypeTranslationContext(new RegularTypeTranslationContext(this, typeInfo));
        }

        public void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext)
        {
            TranslationChain.Add(typeTranslationContext);
        }

        public ITypeTranslationContext GetByType(Type type)
        {
            return this.First(typeTranslationContext => typeTranslationContext.CanProcess(type));
        }

        public IEnumerator<ITypeTranslationContext> GetEnumerator()
        {
            return TranslationChain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}