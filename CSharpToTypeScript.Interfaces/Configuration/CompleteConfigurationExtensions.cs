using System;
using System.IO;
using System.Linq;

namespace CSharpToTypeScript.Core.Configuration
{
    public static class CompleteConfigurationExtensions
    {
        public static CompleteConfiguration WithAllPathsRelativeToFile(this CompleteConfiguration configuration, string basePath, Func<string, string> preprocessOriginalPath = null)
        {
            return configuration.WithAllPathsRelativeToDirectory(new FileInfo(basePath).DirectoryName, preprocessOriginalPath);
        }

        public static CompleteConfiguration WithAllPathsRelativeToDirectory(this CompleteConfiguration configuration, string basePath, Func<string, string> preprocessOriginalPath = null)
        {
            configuration.Input.Assemblies =
                configuration
                    .Input
                    .Assemblies
                    .Select(path => MakePathRelativeToDirectory(path, basePath, preprocessOriginalPath))
                    .ToList();
            configuration.Output.Location = MakePathRelativeToDirectory(configuration.Output.Location, basePath, preprocessOriginalPath);
            return configuration;
        }

        private static string MakePathRelativeToDirectory(string path, string basePath, Func<string, string> preprocessOriginalPath = null)
        {
            if (preprocessOriginalPath != null)
                path = preprocessOriginalPath(path);
            var relativePath = Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
            return Path.GetFullPath(relativePath);
        }
    }
}
