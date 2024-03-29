﻿using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Application.Data
{
    public class ManagedComponent: NamedAuditRepoData
    {
        public string DomId { get; set; }

        public virtual List<ManagedPage> Pages { get; set; }

        public virtual List<ManagedComponentIdentifier> ChildComponents { get; set; }

        public SplitKind SplitKind { get; set; }
    }
}
