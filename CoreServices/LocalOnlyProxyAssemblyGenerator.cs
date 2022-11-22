using Bam.Net.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bam.Net.CoreServices
{
    public class LocalOnlyProxyAssemblyGenerator : ProxyAssemblyGenerator
    {
        public LocalOnlyProxyAssemblyGenerator(ProxySettings proxySettings, string workspaceDirectory = ".", ILogger logger = null, HashSet<Assembly> addedReferenceAssemblies = null) : base(proxySettings.CopyAs<LocalOnlyProxySettings>(), workspaceDirectory, logger, addedReferenceAssemblies)
        { }
    }
}
