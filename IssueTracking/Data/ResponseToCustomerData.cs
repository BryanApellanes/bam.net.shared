using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.IssueTracking.Data
{
    public class ResponseToCustomerData : NamedAuditRepoData
    {
        public virtual PreambleData PreambleDataText { get; set; }

        public virtual ActionTextData ActionText { get; set; }

        public virtual FollowupTextData FollowupText { get; set; }

        public virtual OutroTextData OutroText { get; set; }

        public virtual CommentData ResultingComment { get; set; }
    }
}
