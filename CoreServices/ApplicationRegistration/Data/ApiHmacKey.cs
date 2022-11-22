using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Configuration;
using Bam.Net.Data.Repositories;

namespace Bam.Net.CoreServices.ApplicationRegistration.Data
{
    [Serializable]
    public class ApiHmacKey: KeyedAuditRepoData
    {
        [CompositeKey]
        public ulong ApplicationKey { get; set; }
        public ulong ApplicationId { get; set; }
        public virtual Application Application { get; set; }
        
        [CompositeKey]
        public string ClientIdentifier { get; set; }
        public string SharedSecret { get; set; }
        public DateTime? Confirmed { get; set; }
        public bool Disabled { get; set; }
        public string DisabledBy { get; set; }
    }
}
