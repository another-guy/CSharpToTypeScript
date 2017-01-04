namespace CSharpToTypeScript.Core.Configuration
{
    public sealed class CompleteConfiguration
    {
        public InputConfiguration Input { get; set; }
        public OutputConfiguration Output { get; set; }
        public TranslationConfiguration Translation { get; set; }
    }
}
