using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TsModelGen.Core.TypeTranslationContext;

namespace TsModelGen.Core
{
    public sealed class TranslationContext : ITypeRegistry, ITypeTranslationEnumerable
    {
        // TODO Make this dynamic -- let clients alter the chain to fit their need
        private IList<ITypeTranslationContext> TranslationChain { get; } =
            new List<ITypeTranslationContext>();

        public TranslationContext()
        {
            TypeTranslation
                .CreateContextChain(this)
                .ForEach(AddTypeTranslationContext);
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