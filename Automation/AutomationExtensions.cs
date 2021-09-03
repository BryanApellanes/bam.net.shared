using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bam.Net.Automation
{
    public static class AutomationExtensions
    {
        public static void Set(this EnvironmentVariable[] environmentVariables)
        {
            foreach(EnvironmentVariable environmentVariable in environmentVariables)
            {
                environmentVariable.Write();
            }
        }

        public static void ToFile(this EnvironmentVariable[] environmentVariables, FileInfo fileInfo)
        {
            ToFile(environmentVariables, fileInfo.FullName);
        }

        public static void ToFile(this EnvironmentVariable[] environmentVariables, string filePath)
        {
            using(StreamWriter streamWriter = new StreamWriter(filePath))
            {
                foreach (EnvironmentVariable environmentVariable in environmentVariables)
                {
                    streamWriter.WriteLine(string.Format("{0}={1}", environmentVariable.Name, environmentVariable.Value));
                }
            }
        }
    }
}
