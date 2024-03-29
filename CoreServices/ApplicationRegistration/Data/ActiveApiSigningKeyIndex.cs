﻿using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.CoreServices.ApplicationRegistration.Data
{
    [Serializable]
    public class ActiveApiSigningKeyIndex: AuditRepoData
    {
        public string ApplicationIdentifier { get; set; }
        public int Value { get; set; }
    }
}
