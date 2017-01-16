using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Translation;
using SimpleInjector;

namespace CSharpToTypeScript.SimpleInjector
{
    public sealed class ContainerBuilder
    {
        private Container Container { get; }
        private CompleteConfiguration Configuration { get; set; }

        public ContainerBuilder()
        {
            Container = new Container();
        }

        public ContainerBuilder With(CompleteConfiguration configuration)
        {
            Configuration = configuration;

            Container.RegisterSingleton<ITypeScriptExpression, TypeScriptExpression>();
            Container.RegisterSingleton(configuration);
            Container.RegisterSingleton(() => configuration.Input);
            Container.RegisterSingleton(() => configuration.Translation);
            Container.RegisterSingleton(() => configuration.Output);
            Container.Register<ISkipRule, SkipRule>();
            Container.Register<TypeTranslationChain>(); // TODO IoC -- interface? Singletone (when factory)

            Container.Register<ITargetTypesLocator, TargetTypesLocator>();

            Container.Register<ITranslationContext, TranslationContext>();

            Container.Register<RegularTypeTranslationContextFactory>();

            Container.Register<ISourceTypeMetadataFactory, SourceTypeMetadataFactory>();
            Container.Register<ITranslatedTypeMetadataFactory, TranslatedTypeMetadataFactory>();

            return this;
        }

        public ContainerBuilder Validated()
        {
            Container.Verify();

            return this;
        }

        public Container Build()
        {
            return Container;
        }
    }
}
