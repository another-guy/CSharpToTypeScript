using System.Text;

namespace CSharpToTypeScript.Core.Output
{
    // TODO This code is naive and generally poor
    public static class NaiveNaming
    {
        public static string PascalToKebab(string text)
        {
            var sb = new StringBuilder();
            for (var index = 0; index < text.Length; index++)
            {
                var chr = text[index];
                sb.Append(char.ToLowerInvariant(chr));
                if (index != text.Length - 1 && char.IsUpper(text[index + 1]))
                    sb.Append('-');
            }
            return sb.ToString();
        }
    }
}