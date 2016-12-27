using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TsModelGen.Core.Targets
{
    public sealed class ClosureBuilder
    {
        private static Func<TypeInfo, bool> GetTypeFilterForNamespace(IEnumerable<string> targetNameSpaces)
        {
             return typeInfo =>
                targetNameSpaces.Any(n => typeInfo.Namespace.StartsWith(n));
        }

        private static readonly Func<ITypeTranslationContext, bool> UnprocessedTypeTranslationContext =
            typeContext => typeContext.IsProcessed == false;

        public object Build(string @namespace)
        {
            IEnumerable<string> targetNameSpaces = new[] { @namespace }; // TODO Make a parameter

            var translationContext = CreateTranslationContext(GetTypeFilterForNamespace(targetNameSpaces));

            ITypeTranslationContext unprocessed;
            while ((unprocessed = translationContext.FirstOrDefault(UnprocessedTypeTranslationContext)) != null)
                unprocessed.ResolveDependencies();
            
            // iterate through serializable members of every unvisited type in graph
            //   and put them into the graph
            // when there are no more unvisited types, return 

            throw new NotImplementedException("Well, this is still a work in progress thing");
        }

        private TranslationContext CreateTranslationContext(Func<TypeInfo, bool> typePredicateIsMet)
        {
            var translationContext = new TranslationContext();
            foreach (var sourceType in RootTypesFromAllAssemblies().Where(typePredicateIsMet))
                translationContext.Add(sourceType);
            return translationContext;
        }

        private IEnumerable<TypeInfo> RootTypesFromAllAssemblies()
        {
            var emptyResult = (IEnumerable<TypeInfo>)new List<TypeInfo>();
            return new[] {Assembly.GetEntryAssembly().DefinedTypes} // TODO More assemblies to load here
                .Aggregate(emptyResult, (result, typees) => result.Concat(typees));
        }
    }

    public sealed class TranslationContext : IEnumerable<ITypeTranslationContext>
    {
        // TODO Make this dynamic -- let clients alter the chain to fit their need
        private IList<ITypeTranslationContext> TranslationChain { get; } =
            new List<ITypeTranslationContext>();

        public TranslationContext()
        {
            foreach (var directlyMappedType in DirectMapping.DotNetToTypeScriptType.Keys)
                AddDirect(directlyMappedType);

            foreach (var specialTypeProcessor in SpecialTypes.AllProcessors)
                AddSpecialTypeFrom(specialTypeProcessor);
        }

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }

        public void Add(TypeInfo typeInfo)
        {
            var typeTranslationContext = new RegularTypeTranslationContext(this, typeInfo);
            TranslationChain.Add(typeTranslationContext);
        }

        public void AddDirect(Type directlyMappedType)
        {
            TranslationChain.Add(new DirectTypeTranslationContext(directlyMappedType.GetTypeInfo()));
        }

        private void AddSpecialTypeFrom(ITypeTranslationContext specialTypeProcessor)
        {
            TranslationChain.Add(specialTypeProcessor);
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

    public interface ITypeTranslationContext
    {
        bool CanProcess(Type type);
        bool IsProcessed { get; }
        void ResolveDependencies();
    }

    public sealed class DirectTypeTranslationContext : ITypeTranslationContext
    {
        public DirectTypeTranslationContext(TypeInfo type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Type = type;
        }

        public TypeInfo Type { get; }

        public bool CanProcess(Type type)
        {
            return Type.AsType() == type;
        }
        public bool IsProcessed => true;
        public void ResolveDependencies() { }
    }

    public sealed class RegularTypeTranslationContext : ITypeTranslationContext
    {
        public RegularTypeTranslationContext(TranslationContext translationContext, TypeInfo type)
        {
            if (translationContext == null) throw new ArgumentNullException(nameof(translationContext));
            if (type == null) throw new ArgumentNullException(nameof(type));
            Type = type;
            GlobalContext = translationContext;
        }

        public TypeInfo Type { get; }
        private TranslationContext GlobalContext { get; }
        public string Id => Type.AssemblyQualifiedName;
        private TranslationMetadata Metadata { get; set; }


        public bool CanProcess(Type type)
        {
            return Type.AsType() == type;
        }
        public bool IsProcessed => Metadata != null;
        public void ResolveDependencies()
        {
            // TODO Change to immutable object
            Metadata = new TranslationMetadata();
            
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var propertyInfo in Type.GetProperties(flags))
            {
                Metadata[propertyInfo.Name] = new SourceMemberInfo(propertyInfo);
                EnsureTypeWillBeResolved(propertyInfo.PropertyType.GetTypeInfo());
            }

            foreach (var fieldInfo in Type.GetFields(flags))
            {
                Metadata[fieldInfo.Name] = new SourceMemberInfo(fieldInfo);
                EnsureTypeWillBeResolved(fieldInfo.FieldType.GetTypeInfo());
            }

            {
                var baseType = Type.BaseType;
                if (baseType != typeof(object) &&
                    baseType != typeof(ValueType) &&
                    baseType != typeof(Enum))
                {
                    var parentTypeInfo = baseType.GetTypeInfo();
                    Metadata.BaseType = new SourceParentInfo(parentTypeInfo);
                    EnsureTypeWillBeResolved(parentTypeInfo);
                }
            }
        }

        private void EnsureTypeWillBeResolved(TypeInfo typeInfo)
        {
            var noTypeTranslationContextRegistered = GlobalContext.CanProcess(typeInfo) == false;
            if (noTypeTranslationContextRegistered)
                GlobalContext.Add(typeInfo);
        }
    }

    public sealed class TranslationMetadata
    {
        private IDictionary<string, SourceMemberInfo> MembersSourceInfo { get; } =
            new Dictionary<string, SourceMemberInfo>();

        public SourceParentInfo BaseType { get; set; }

        // TODO INTERFACES ???

        public SourceMemberInfo this[string memberName]
        {
            get { return MembersSourceInfo[memberName]; }
            set { MembersSourceInfo[memberName] = value; }
        }
    }

    public sealed class SourceMemberInfo
    {
        public MemberInfo MemberInfo { get; }

        public SourceMemberInfo(MemberInfo memberInfo)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
            MemberInfo = memberInfo;
        }
    }

    public sealed class SourceParentInfo
    {
        public TypeInfo ParentInfo { get; }

        public SourceParentInfo(TypeInfo parentInfo)
        {
            if (parentInfo == null) throw new ArgumentNullException(nameof(parentInfo));
            ParentInfo = parentInfo;
        }
    }
}