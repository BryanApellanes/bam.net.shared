using Bam.Net.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bam.Net.IssueTracking
{
    public interface IIssueTracker : IServiceLevelAgreementProvider
    {
        Task<List<ITrackedIssue>> GetAllIssuesAsync();

        Task<List<ITrackedIssueComment>> GetAllCommentsAsync(ITrackedIssue managedIssue);

        Task<ITrackedIssue> CreateIssueAsync(string externalRepoId, string title, string body);

        Task<ITrackedIssueComment> AddCommentAsync(ITrackedIssue issue, string commenter, string commentText);
    }
}