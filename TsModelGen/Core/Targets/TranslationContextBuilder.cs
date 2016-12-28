﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TsModelGen.Core.Targets
{
    public interface ITypeRegistry
    {
        bool CanProcess(TypeInfo typeInfo);
        void AddTypeTranslationContextForType(TypeInfo typeInfo);
        void AddTypeTranslationContext(ITypeTranslationContext specialTypeProcessor);
    }

    public interface ITypeTranslationEnumerable : IEnumerable<ITypeTranslationContext> { }

    public sealed class TranslationContextBuilder
    {
        private static Func<TypeInfo, bool> GetTypeFilterForNamespace(IEnumerable<string> targetNameSpaces)
        {
             return typeInfo =>
                targetNameSpaces.Any(n => typeInfo.Namespace.StartsWith(n));
        }

        private static readonly Func<ITypeTranslationContext, bool> WithUnresolvedDependencies =
            typeContext => typeContext.AreDependenciesResolved == false;

        public TranslationContext Build(string @namespace)
        {
            IEnumerable<string> targetNameSpaces = new[] { @namespace }; // TODO Make a parameter

            var translationContext = CreateTranslationContext(GetTypeFilterForNamespace(targetNameSpaces));

            ITypeTranslationContext unprocessed;
            while ((unprocessed = translationContext.FirstOrDefault(WithUnresolvedDependencies)) != null)
                unprocessed.ResolveDependencies();

            return translationContext;
        }

        private TranslationContext CreateTranslationContext(Func<TypeInfo, bool> typePredicateIsMet)
        {
            var translationContext = new TranslationContext();
            foreach (var sourceType in RootTypesFromAllAssemblies().Where(typePredicateIsMet))
                translationContext.AddTypeTranslationContextForType(sourceType);
            return translationContext;
        }

        private IEnumerable<TypeInfo> RootTypesFromAllAssemblies()
        {
            var emptyResult = (IEnumerable<TypeInfo>)new List<TypeInfo>();
            return new[] {Assembly.GetEntryAssembly().DefinedTypes} // TODO More assemblies to load here
                .Aggregate(emptyResult, (result, typees) => result.Concat(typees));
        }
    }

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
        bool AreDependenciesResolved { get; }
        void ResolveDependencies();

        bool CanProcess(Type type);
        bool IsProcessed { get; }
        void Process();

        TranslatedTypeMetadata TranslatedTypeMetadata { get; }
    }

    public sealed class DirectTypeTranslationContext : ITypeTranslationContext
    {
        public DirectTypeTranslationContext(TypeInfo type, string symbol)
        {
            Type = type.NullToException(new ArgumentNullException(nameof(type)));

            TranslatedTypeMetadata.Symbol = symbol;
        }

        public TypeInfo Type { get; }

        public bool AreDependenciesResolved => true;
        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return Type.AsType() == type;
        }
        public bool IsProcessed => true;

        public void Process() { }
        public TranslatedTypeMetadata TranslatedTypeMetadata { get; } = new TranslatedTypeMetadata();
    }

    public sealed class RegularTypeTranslationContext : ITypeTranslationContext
    {
        public RegularTypeTranslationContext(ITypeRegistry translationContext, TypeInfo typeInfo)
        {
            if (translationContext == null) throw new ArgumentNullException(nameof(translationContext));
            if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
            TypeInfo = typeInfo;
            GlobalContext = translationContext;
        }

        public TypeInfo TypeInfo { get; }
        private ITypeRegistry GlobalContext { get; }
        public string Id => TypeInfo.AssemblyQualifiedName;
        private SourceTypeMetadata SourceTypeMetadata { get; } = new SourceTypeMetadata();


        public bool AreDependenciesResolved { get; private set; } = false;

        public void ResolveDependencies()
        {
            AreDependenciesResolved = true;
            
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var propertyInfo in TypeInfo.GetProperties(flags))
            {
                SourceTypeMetadata[propertyInfo.Name] = new SourceMemberInfo(propertyInfo);
                EnsureTypeWillBeResolved(propertyInfo.PropertyType.GetTypeInfo());
            }

            foreach (var fieldInfo in TypeInfo.GetFields(flags))
            {
                SourceTypeMetadata[fieldInfo.Name] = new SourceMemberInfo(fieldInfo);
                EnsureTypeWillBeResolved(fieldInfo.FieldType.GetTypeInfo());
            }

            {
                var baseType = TypeInfo.BaseType;
                if (baseType != typeof(object) &&
                    baseType != typeof(ValueType) &&
                    baseType != typeof(Enum))
                {
                    var parentTypeInfo = baseType.GetTypeInfo();
                    SourceTypeMetadata.BaseType = new SourceParentInfo(parentTypeInfo);
                    EnsureTypeWillBeResolved(parentTypeInfo);
                }
            }
        }

        private void EnsureTypeWillBeResolved(TypeInfo typeInfo)
        {
            var noTypeTranslationContextRegistered = GlobalContext.CanProcess(typeInfo) == false;
            if (noTypeTranslationContextRegistered)
                GlobalContext.AddTypeTranslationContextForType(typeInfo);
        }

        public bool CanProcess(Type type)
        {
            return TypeInfo.AsType() == type;
        }

        public bool IsProcessed { get; } = false;

        public void Process()
        {
            throw new NotImplementedException();
        }
        public TranslatedTypeMetadata TranslatedTypeMetadata { get; } = new TranslatedTypeMetadata();
    }

    public sealed class SourceTypeMetadata
    {
        private IDictionary<string, SourceMemberInfo> MembersSourceInfo { get; } =
            new Dictionary<string, SourceMemberInfo>();

        public SourceParentInfo BaseType { get; set; }

        // TODO INTERFACES ???

        public IEnumerable<string> Members => MembersSourceInfo.Keys;

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

    public sealed class TranslatedTypeMetadata
    {
        public string Symbol { get; set; }
        public string Definition { get; set; }
    }



    // TODO Clean this up
    public static class DirectMapping
    {
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

    public sealed class DirectTranslationRule
    {
        public Type SourceType { get; }
        public string DestinationType { get; }

        public DirectTranslationRule(Type sourceType, string destinationType)
        {
            SourceType = sourceType.NullToException(new ArgumentNullException(nameof(sourceType)));
            DestinationType = destinationType.NullToException(new ArgumentNullException(nameof(destinationType)));
        }
    }

    public sealed class SpecialTypes
    {
        public static readonly List<ITypeTranslationContext> AllProcessors = new List<ITypeTranslationContext>
        {
            // TODO Replace DummySpecialTranslationType with specific entity type translation object
            new DummySpecialTranslationType(typeof(Enum)),
            new DummySpecialTranslationType(typeof(ValueType)),

            new DummySpecialTranslationType(typeof(IDictionary)),
            new DummySpecialTranslationType(typeof(IDictionary<,>)),
            new DummySpecialTranslationType(typeof(IEnumerable)),
            new DummySpecialTranslationType(typeof(IEnumerable<>)),
            new DummySpecialTranslationType(typeof(Nullable<>))
        };
    }

    public sealed class DummySpecialTranslationType : ITypeTranslationContext
    {
        public DummySpecialTranslationType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Type = type.GetTypeInfo();
        }

        public TypeInfo Type { get; }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(Type.AsType());
        }
        public bool IsProcessed => true;

        public void Process()
        {
            // TODO populate TranslatedTypeMetadata
        }
        public TranslatedTypeMetadata TranslatedTypeMetadata { get; } = new TranslatedTypeMetadata();
    }
}