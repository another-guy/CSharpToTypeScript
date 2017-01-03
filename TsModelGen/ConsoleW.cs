using System;

namespace TsModelGen
{
    // TODO Move to Peppermint
    public static class ConsoleW
    {
        public static void WriteColor(string text, ConsoleColor foregroundColor)
        {
            var originalFgColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ForegroundColor = originalFgColor;
        }
    }
}