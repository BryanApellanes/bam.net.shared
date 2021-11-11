using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.IssueTracking
{
    public interface IServiceLevelAgreementProvider
    {
        /// <summary>
        /// Gets or sets the minimum number of hours a response must be provided in.
        /// </summary>
        int Sla { get; set; }

        bool SlaWasMet(ITrackedIssue trackedIssue);
    }
}
