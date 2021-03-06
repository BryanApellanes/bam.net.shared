using System;
using System.Collections.Generic;
using System.IO;

namespace Bam.Net
{
    /// <summary>
    /// Paths rooted in the current process' user profile.
    /// </summary>
    public static class BamProfile
    {
        static BamProfile()
        {
            EnsureDirectoryExists(ToolkitPath);
            EnsureDirectoryExists(NugetOutputPath);
            EnsureDirectoryExists(ConfigPath);
            EnsureDirectoryExists(TestsPath);
            EnsureDirectoryExists(ContentPath);
            EnsureDirectoryExists(AppsPath);
            EnsureDirectoryExists(SvcScriptsSrcPath);
            EnsureDirectoryExists(DataPath);
            EnsureDirectoryExists(RecipesPath);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// The path to the .bam directory in the home directory of the current process' user.
        /// This value is the same as BamHome.Profile.
        /// </summary>
        public static string Path => System.IO.Path.Combine(UserHome, ".bam");
        
        /// <summary>
        /// The path to the home directory of the current process' user.
        /// This value is the same as BamHome.UserHome.
        /// </summary>
        public static string UserHome
        {
            get
            {
                if (OSInfo.Current == OSNames.Windows)
                {
                    return Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                }
                else
                {
                    return Environment.GetEnvironmentVariable("HOME");
                }
            }
        }
        
        public static string ReferenceAssembliesPath => System.IO.Path.Combine(Path, "referenceassemblies");
        
        public static string ToolkitPath => System.IO.Path.Combine(ToolkitSegments);
        public static string[] ToolkitSegments => new List<string>() {Path, "toolkit"}.ToArray();
        public static string NugetOutputPath => System.IO.Path.Combine(NugetOutputSegments);
        
        public static string[] NugetOutputSegments => new List<string>() {Path, "nupkg"}.ToArray();
        
        /// <summary>
        /// ~/.bam/config
        /// </summary>
        public static string ConfigPath => System.IO.Path.Combine(ConfigSegments);
        public static string[] ConfigSegments => new List<string>() {Path, "config"}.ToArray();

        public static string TestsPath => System.IO.Path.Combine(TestsSegments);
        public static string[] TestsSegments => new List<string>() {Path, "tests"}.ToArray();
        public static string ContentPath => System.IO.Path.Combine(ContentSegments);
        public static string[] ContentSegments => new List<string>() {Path, "content"}.ToArray();

        public static string AppsPath => System.IO.Path.Combine(AppsSegments);
        public static string[] AppsSegments => new List<string>(ContentSegments) {"apps"}.ToArray();
        
        public static string SvcScriptsSrcPath => System.IO.Path.Combine(SvcScriptsSrcSegments);
        public static string[] SvcScriptsSrcSegments => new List<string>() {Path, "svc", "scripts"}.ToArray();

        public static string DataPath => System.IO.Path.Combine(DataSegments);

        public static string[] DataSegments => new List<string>() {Path, "data"}.ToArray();
        
        public static string RecipesPath => System.IO.Path.Combine(RecipeSegments);
        public static string[] RecipeSegments => new List<string>() {Path, "recipes"}.ToArray();

        public static string ReadDataFile(string relativeFilePath)
        {
            FileInfo file = new FileInfo(System.IO.Path.Combine(DataPath, relativeFilePath));
            if (!file.Exists)
            {
                File.Create(file.FullName).Dispose();
            }

            return File.ReadAllText(file.FullName);
        }
        
        public static T LoadJsonData<T>(string relativeFilePath) where T : new()
        {
            FileInfo file = new FileInfo(System.IO.Path.Combine(DataPath, relativeFilePath));
            if (!file.Exists)
            {
                File.Create(file.FullName).Dispose();
            }
            return file.FromJsonFile<T>() ?? new T();
        }
        
        public static T LoadYamlData<T>(string relativeFilePath) where T : new()
        {
            FileInfo file = new FileInfo(System.IO.Path.Combine(DataPath, relativeFilePath));
            if (!file.Exists)
            {
                File.Create(file.FullName).Dispose();
            }
            return file.FromYamlFile<T>() ?? new T();
        }

        public static string SaveJsonData(object instance, string relativeFilePath)
        {
            FileInfo file = new FileInfo(System.IO.Path.Combine(DataPath, relativeFilePath));
            instance.ToJson().SafeWriteToFile(file.FullName, true);
            return file.FullName;
        }
        
        public static string SaveYamlData(object instance, string relativeFilePath)
        {
            FileInfo file = new FileInfo(System.IO.Path.Combine(DataPath, relativeFilePath));
            instance.ToYaml().SafeWriteToFile(file.FullName, true);
            return file.FullName;
        }
    }
}