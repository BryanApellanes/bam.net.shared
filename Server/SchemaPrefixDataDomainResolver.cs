using System.Collections.Generic;
using System;

namespace Bam.Net.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class SchemaPrefixDataDomainResolver : IDataDomainResolver
    {
        public SchemaPrefixDataDomainResolver()
        {
            PrimaryHostName = "localhost";
        }

        public SchemaPrefixDataDomainResolver(string primaryHostName)
        {
            PrimaryHostName = primaryHostName;
            Port = 7400;
        }
        
        public string PrimaryHostName { get; set; }
        public int Port { get; set; }
        public IEnumerable<HostBinding> ResolveDataSubdomains(AppConf appConf)
        {
            throw new NotImplementedException();
        }
    }
}