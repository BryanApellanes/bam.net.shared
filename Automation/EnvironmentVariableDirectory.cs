using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bam.Net.CommandLine;
using Bam.Net.Configuration;

namespace Bam.Net.Automation
{
    public class EnvironmentVariableDirectory
    {
        public const string DefaultName = "./.bam/BAMVARS";

        public EnvironmentVariableDirectory()
        {
        }

        public EnvironmentVariableDirectory(string directoryPath)
        {
            this.DirectoryPath = directoryPath;
        }

        public static EnvironmentVariableDirectory Default
        {
            get => new EnvironmentVariableDirectory(DefaultName);
        }

        private string _directoryPath;
        public string DirectoryPath
        {
            get => _directoryPath;
            set
            {
                _directoryPath = value;
                EnvironmentVariables = ReadEnvironmentVariablesFromFiles().ToArray();
            }
        }
        
        public EnvironmentVariable[] EnvironmentVariables { get; set; }

        public void FromDirectory()
        {
            EnvironmentVariables = FromDirectory(DirectoryPath);
        }

        public void Create()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(DirectoryPath);
            foreach (EnvironmentVariable environmentVariable in EnvironmentVariables)
            {
                string file = Path.Combine(directoryInfo.FullName, environmentVariable.Name);
                string oldValue = string.Empty;
                if (File.Exists(file))
                {
                    oldValue = File.ReadAllText(file).Trim();
                }

                if (!string.IsNullOrEmpty(oldValue))
                {
                    Message.PrintLine("Updating {0}: old value = {1}, new value {2}", ConsoleColor.Yellow, environmentVariable.Name, oldValue, environmentVariable.Value);
                }
                environmentVariable.Value.SafeWriteToFile(file, true);
            }
        }

        public static EnvironmentVariableDirectory Create(EnvironmentVariable[] variables, string directoryPath = DefaultName)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            EnvironmentVariableDirectory directory = new EnvironmentVariableDirectory(directoryPath)
            {
                EnvironmentVariables = variables
            };
            directory.Create();
            return directory;
        }

        public static EnvironmentVariableDirectory FromInstance(object instance, string directoryPath = DefaultName)
        {
            return Create(EnvironmentVariable.FromInstance(instance).ToArray(), directoryPath);
        }
        
        public static EnvironmentVariable[] FromDirectory(DirectoryInfo directoryInfo)
        {
            return FromDirectory(directoryInfo.FullName);
        }
        
        public static EnvironmentVariable[] FromDirectory(string path)
        {
            return new EnvironmentVariableDirectory(path).EnvironmentVariables;
        }

        private IEnumerable<EnvironmentVariable> ReadEnvironmentVariablesFromFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(DirectoryPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                string fileName = file.Name;
                string variableName = Path.GetFileNameWithoutExtension(file.Name);
                if (fileName.Equals(variableName))
                {
                    string fileContent = file.FullName.SafeReadFile().Trim();
                    yield return new EnvironmentVariable {Name = variableName, Value = fileContent};
                }
            }
        }
    }
}