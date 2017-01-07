using System;
using clipr;
using clipr.Usage;

namespace CSharpToTypeScript
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
                return args;
            }
            catch (Exception)
            {
                Cli.Write(help.GetHelp(parser.Config), ConsoleColor.Yellow);
                throw;
            }
        }
    }
}
