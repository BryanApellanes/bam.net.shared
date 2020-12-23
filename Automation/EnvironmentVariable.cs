using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Bam.Net.Automation
{
    public class EnvironmentVariable
    {
        public const string DefaultVarFile = "./.bam/BAMVARS.yaml";
        public string Name { get; set; }
        public string Value { get; set; }

        public string Read()
        {
            return Read(Name);
        }

        public void Write()
        {
            Environment.SetEnvironmentVariable(Name, Value);
        }
        
        public static IEnumerable<EnvironmentVariable> FromInstance(object instance)
        {
            Type type = instance.GetType();
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (propertyInfo.HasCustomAttributeOfType<EnvironmentVariableAttribute>(out EnvironmentVariableAttribute attr))
                {
                    yield return new EnvironmentVariable {Name = attr.Name, Value = (string) propertyInfo.GetValue(instance)};
                }
            }
        }

        public static EnvironmentVariable[] FromDirectory(DirectoryInfo directoryInfo)
        {
            return FromDirectory(directoryInfo.FullName);
        }
        
        public static EnvironmentVariable[] FromDirectory(string directoryPath)
        {
            return EnvironmentVariableDirectory.FromDirectory(directoryPath);
        }
        
        public static string Read(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
        
        public static IEnumerable<EnvironmentVariable> ReadAll()
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (object key in environmentVariables.Keys)
            {
                yield return new EnvironmentVariable {Name = (string) key, Value = (string) environmentVariables[key]};
            }
        }
    }
}