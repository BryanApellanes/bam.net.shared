﻿using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.IssueTracking.Data
{
    public class FollowupTextData : NamedAuditRepoData
    {
        public string Value { get; set; }
    }
}
