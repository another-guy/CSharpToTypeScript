using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using clipr;
using clipr.Usage;

namespace TsModelGen
{
    public static class ArgumentParser
    {
        public static Arguments ParseArguments(string[] rawArgs)
        {
            var args = new Arguments();
            var parser = new CliParser<Arguments>(args);
            var help = new AutomaticHelpGenerator<Arguments>();
            try
            {
                parser.Parse(rawArgs);
            }
            catch (Exception caught)
            {
                var originalFgColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(caught);
                Console.ForegroundColor = originalFgColor;

                Console.WriteLine(help.GetHelp(parser.Config));

                return null;
            }
            return args;
        }
    }
}
