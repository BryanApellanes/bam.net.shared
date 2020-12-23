using System;

namespace Bam.Net.Automation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EnvironmentVariableAttribute : Attribute
    {
        public EnvironmentVariableAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
        public string Value => Environment.GetEnvironmentVariable(Name);

        public void SetEnvironmentVariable(string value)
        {
            Environment.SetEnvironmentVariable(Name, value);
        }
    }
}