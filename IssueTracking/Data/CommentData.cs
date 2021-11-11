using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.IssueTracking.Data
{
    public class CommentData: AuditRepoData, ITrackedIssueComment
    {
        public CommentData() { }

        public CommentData(ITrackedIssueComment comment) 
        {
            this.Text = comment?.Text;
            this.Created = comment?.CreatedAt.UtcDateTime;
            this.CreatedBy = comment?.User;
        }

        public CommentData(ITrackedIssue issue)
        {
            this.RepositoryId = issue.RepositoryId.ToString();
        }

        public virtual ulong IssueDataId { get; set; } // required for foreign key relationship

        public virtual IssueData IssueData { get; set; } // required to auto associate

        /// <summary>
        /// Gets or sets the external repository identifier.  For example, the repo id for a GitHub repo.
        /// </summary>
        public string RepositoryId { get; set; }

        /// <summary>
        /// The comment text.
        /// </summary>
        public string Text { get; init; }

        public string User
        {
            get
            {
                return CreatedBy;
            }
            set
            {
                CreatedBy = value;
            }
        }

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

        public CommentData ToCommentData()
        {
            return this;
        }
    }
}
