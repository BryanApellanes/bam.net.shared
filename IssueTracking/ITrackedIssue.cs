using System;
using System.Collections.Generic;

namespace Bam.Net.IssueTracking
{
    public interface ITrackedIssue
    {
        int? Id { get; }

        string Title { get; }

        string Body { get; }

        long? RepositoryId{ get; }

        string CreatedBy { get; }

        DateTimeOffset CreatedAt { get; }

        Uri IssueUri { get; }

        List<ITrackedIssueComment> Comments { get; }
    }
}