using System;
using System.Linq;
using System.Reflection;

namespace Bam.Net.Automation
{
    public static class EnvironmentVariableExtensions
    {
        public static T ToInstance<T>(this EnvironmentVariable[] environmentVariables) where T: new()
        {
            T result = new T();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (EnvironmentVariable environmentVariable in environmentVariables)
            {
                PropertyInfo propertyInfo = properties.FirstOrDefault(p => p.Name.Equals(environmentVariable.Name));
                if (propertyInfo == null)
                {
                    propertyInfo = properties.FirstOrDefault(p =>
                    {
                        if (p.HasCustomAttributeOfType<EnvironmentVariableAttribute>(out EnvironmentVariableAttribute environmentVariableAttribute))
                        {
                            return (bool) environmentVariableAttribute.Name?.Equals(environmentVariable.Name);
                        }

                        return false;

                    });
                }

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(result, environmentVariable.Value);
                }
            }

            return result;
        } 
    }
}