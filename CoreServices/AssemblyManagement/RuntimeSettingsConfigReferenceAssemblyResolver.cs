using System;
using System.IO;
using System.Reflection;

namespace Bam.Net.CoreServices.AssemblyManagement
{
    public class RuntimeSettingsConfigReferenceAssemblyResolver : ReferenceAssemblyResolver
    {
        public override string ResolveReferenceAssemblyPath(Type type)
        {
            return ResolveReferenceAssemblyPath(type.Namespace, type.Name);
        }

        public virtual string ResolveReferenceAssemblyPath(string typeNamespace, string typeName)
        {
            string referenceAssembliesDir = RuntimeSettings.GetConfig().ReferenceAssembliesDir;

            string filePath = FindAssembly(typeNamespace, typeName, referenceAssembliesDir);

            if (!File.Exists(filePath))
            {
                filePath = FindAssembly(typeNamespace, typeName, Assembly.GetEntryAssembly().GetFileInfo().Directory.FullName);
            }

            if (!File.Exists(filePath))
            {
                throw new ReferenceAssemblyNotFoundException($"{typeNamespace}.{typeName}");
            }

            return filePath;
        }

        private static string FindAssembly(string typeNamespace, string typeName, string referenceAssembliesDir)
        {
            string filePath = Path.Combine(referenceAssembliesDir, $"{typeNamespace}.dll");
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(referenceAssembliesDir, $"{typeNamespace}.{typeName}.dll");
            }

            return filePath;
        }

        /// <summary>
        /// Resolves the specified package by reading the current RuntimeConfig
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public override string ResolveReferencePackage(string packageName)
        {
            return ResolveReferenceAssemblyPath(ResolveReferenceAssemblyPath(packageName, packageName));
        }
    }
}