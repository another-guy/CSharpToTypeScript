﻿using System;
using System.Collections.Generic;
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

            // TODO Target types to be discovered here
            var translationContext = new TranslationContextBuilder().Build(targetNameSpace);

            // TODO Target types to use for iteration instead of RegularTypeTranslationContext
            var generatedCode = translationContext
                .OfType<RegularTypeTranslationContext>()
                .Select(typeContext => typeContext.Process(typeContext.TypeInfo.AsType()).Definition)
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .Aggregate((accumulated, typeDefinition) => accumulated + "\n" + typeDefinition);

            Console.WriteLine(generatedCode);
            Console.ReadKey();
        }
    }
}