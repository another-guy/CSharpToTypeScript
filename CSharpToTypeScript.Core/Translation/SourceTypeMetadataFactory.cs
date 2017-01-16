namespace CSharpToTypeScript.Core.Translation
{
    public sealed class SourceTypeMetadataFactory : ISourceTypeMetadataFactory
    {
        public ISourceTypeMetadata CreateNew()
        {
            return new SourceTypeMetadata();
        }
    }
}
