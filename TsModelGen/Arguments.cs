using clipr;

namespace TsModelGen
{
    [ApplicationInfo(
         Description =
             "TsModelGen is a tool " +
             "that translates target CompleteConfiguration# model (data transfer) classes, structs, and enums " +
             "into compatible TypeScript definitions."
     )]
    public class Arguments
    {
        [NamedArgument(
             shortName: 'c',
             longName: "cfgLocation",
             Description = "Locates the mandatory configuration file",
             Required = true
         )]
        public string ConfigLocation { get; set; }
    }
}