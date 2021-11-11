using Bam.Net.Data.Repositories;

namespace Bam.Net.IssueTracking.Data
{
    public class OwnedRepoListData : AuditRepoData
    {
        public string Owner { get; set; }
        public string[] Repositories { get; set; }
    }
}