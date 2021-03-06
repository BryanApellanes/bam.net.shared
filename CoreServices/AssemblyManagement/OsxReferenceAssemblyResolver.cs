
using System;
using System.IO;
using System.Reflection;
using Bam.Net.Logging;

namespace Bam.Net.CoreServices.AssemblyManagement
{
    public class OsxReferenceAssemblyResolver : IReferenceAssemblyResolver
    {
        public OsxReferenceAssemblyResolver()
        {
            NugetReferenceAssemblyResolver = new ProfileReferenceAssemblyResolver();
            RuntimeSettingsConfigReferenceAssemblyResolver = new RuntimeSettingsConfigReferenceAssemblyResolver();
        }

        private NugetReferenceAssemblyResolver NugetReferenceAssemblyResolver { get; set; }
        private RuntimeSettingsConfigReferenceAssemblyResolver RuntimeSettingsConfigReferenceAssemblyResolver { get; set; }

        public Assembly ResolveReferenceAssembly(Type type)
        {
            return NugetReferenceAssemblyResolver.ResolveReferenceAssembly(type);
        }

        public string ResolveReferenceAssemblyPath(Type type)
        {
            return NugetReferenceAssemblyResolver.ResolveReferenceAssemblyPath(type);
        }

        public string ResolveReferenceAssemblyPath(string nameSpace, string typeName)
        {
            return NugetReferenceAssemblyResolver.ResolveReferenceAssemblyPath(nameSpace, typeName);
        }

        public string ResolveSystemRuntimePath()
        {
            string netCoreDir = new FileInfo(typeof(object).Assembly.GetFilePath()).Directory.FullName;
            string systemRuntime = Path.Combine(netCoreDir, "System.Runtime.dll");
            if (!File.Exists(systemRuntime))
            {
                try
                {
                    systemRuntime = RuntimeSettingsConfigReferenceAssemblyResolver.ResolveSystemRuntimePath();
                }
                catch (Exception ex)
                {
                    Log.Error("Error trying to resolve `System.Runtime.dll` path");
                
                    return NugetReferenceAssemblyResolver.ResolveSystemRuntimePath();
                }
            }

            return systemRuntime;
        }

        public string ResolveNetStandardPath()
        {
            return ReferenceAssemblyResolver.ResolveNetStandardPath(ResolveSystemRuntimePath());
        }
        
        public string ResolvePackageRootDirectory(string typeNamespace, string typeName)
        {
            return NugetReferenceAssemblyResolver.ResolvePackageRootDirectory(typeNamespace, typeName);
        }
        
        public string ResolveReferenceAssemblyPath(string assemblyName)
        {
            FileInfo runtime = new FileInfo(ResolveSystemRuntimePath());
            string path = Path.Combine(runtime.Directory.FullName, assemblyName);
            if (!File.Exists(path) && !File.Exists(path.ToLowerInvariant()))
            {
                path = NugetReferenceAssemblyResolver.ResolveReferenceAssemblyPath(assemblyName);
                if (!File.Exists(path) && !File.Exists(path.ToLowerInvariant()))
                {
                    throw new ReferenceAssemblyNotFoundException(assemblyName);
                }
            }

            return path;
        }

        public string ResolveReferencePackage(string packageName)
        {
            return NugetReferenceAssemblyResolver.ResolveReferencePackage(packageName);
        }
    }
}