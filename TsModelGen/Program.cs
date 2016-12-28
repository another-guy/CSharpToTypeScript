﻿using System;
using System.Linq;
using TsModelGen.Core;
using TsModelGen.Core.Targets;

namespace TsModelGen
{
    public class Program
    {

        public static void Main(string[] args)
        {
            // TODO Move this to input parameters
            // TODO Translate into a list of namespaces, types, rules on types (such as 
            var targetNameSpace = "TsModelGen.TargetNamespace";

            var translationContext = new TranslationContextBuilder().Build(targetNameSpace);

            foreach (var typeTranslationContext in translationContext)
                if (typeTranslationContext.IsProcessed == false)
                    typeTranslationContext.Process();

            var generatedCode = translationContext
                .Select(typeContext => typeContext.TranslatedTypeMetadata.Definition)
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .ToList();

            Console.WriteLine(generatedCode);
            Console.ReadKey();
        }
    }
}