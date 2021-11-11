using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bam.Net.IssueTracking.Data
{
    public class IssueData : AuditRepoData, ITrackedIssue
    {
        public IssueData() { }

        public IssueData(ITrackedIssue trackedIssue)
        {
            this.ExternalId = trackedIssue?.Id?.ToString();
            this.Title = trackedIssue.Title;
            this.Body = trackedIssue.Body;
            this.CommentDatas = new List<CommentData>(trackedIssue.Comments?.Select(issue => new CommentData(issue)).ToList());
        }

        public string ExternalId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public virtual List<CommentData> CommentDatas { get; set; }

        public long? RepositoryId => throw new NotImplementedException();

        public DateTimeOffset CreatedAt
        {
            get
            {
                return new DateTimeOffset(Created.Value);
            }
            set
            {
                Created = value.UtcDateTime;
            }
        }

        public Uri IssueUri { get; set; }

        int? ITrackedIssue.Id
        {
            get
            {
                if(int.TryParse(ExternalId, out int result))
                {
                    return result;
                }
                return -1;
            }
        }

        List<ITrackedIssueComment> ITrackedIssue.Comments
        {
            get
            {
                return CommentDatas.Select(c => c.Cast<ITrackedIssueComment>()).ToList();
            }
        }
    }
}