using System;
using System.Collections.Generic;
using System.Linq;
using Bam.Net.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.CoreServices.ApplicationRegistration.Data;
using Bam.Net.CoreServices.Diagnostic;
using CsQuery.ExtensionMethods.Internal;

namespace Bam.Net.CoreServices
{
    public partial class DiagnosticInfo
    {
        protected ServiceRegistry ServiceRegistry { get; set; }

        public ProcessDescriptor ProcessDescriptor => ProcessDescriptor.Current;

        public string[] Assemblies
        {
            get;
            set;
        }

        public ApplicationDiagnosticInfo AppDiagInfo
        {
            get;
            set;
        }

        public DatabaseInfo[] Databases
        {
            get;
            set;
        }

        public ServiceProxyInfo[] ServiceProxies
        {
            get;
            set;
        }

        public DiagnosableSettings[] DiagnosableSettings
        {
            get;
            set;
        }

        private void SetDiagnosableSettings()
        {
            List<DiagnosableSettings> settings = new List<CoreServices.DiagnosableSettings>();
            if (ServiceRegistry != null)
            {                
                foreach(string className in ServiceRegistry.ClassNames)
                {
                    if( ServiceRegistry.Get(className) is IDiagnosable diagnosable)
                    {
                        settings.Add(new DiagnosableSettings(diagnosable));
                    }
                }
            }
            DiagnosableSettings = settings.ToArray();
        }

        private void SetServiceProxies()
        {
            List<ServiceProxyInfo> serviceProxies = new List<ServiceProxyInfo>();
            ServiceProxySystem.Incubator.ClassNames.Each(className =>
            {
                serviceProxies.Add(new ServiceProxyInfo(ServiceProxySystem.Incubator[className]));
            });
            this.ServiceProxies = serviceProxies.ToArray();
        }

        private void SetDatabases()
        {
            HashSet<DatabaseInfo> dbInfos = new HashSet<DatabaseInfo>();
            Db.DefaultContainer.GetInfos().Each(dbInfo => dbInfos.Add(dbInfo));
            dbInfos.AddRange(Database.Infos);
            Databases = dbInfos.ToArray();
        }

        private void SetAssemblies()
        {
            HashSet<string> assemblyNames = new HashSet<string>();
            AppDomain.CurrentDomain.GetAssemblies().Each(ass => assemblyNames.Add(ass.FullName));
            Assemblies = assemblyNames.ToArray();
        }
    }
}
