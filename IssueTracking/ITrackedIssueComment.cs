using Bam.Net.IssueTracking.Data;
using System;

namespace Bam.Net.IssueTracking
{
    public interface ITrackedIssueComment
    {
        string User { get; }

        DateTimeOffset CreatedAt { get; }

        string Text { get; }

        CommentData ToCommentData();
    }
}