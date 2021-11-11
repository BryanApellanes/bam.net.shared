using Bam.Net.Caching;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.IssueTracking.Data;
using Bam.Net.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.IssueTracking
{
    public class DaoIssueTracker : IIssueTracker
    {
        private readonly IServiceLevelAgreementProvider _serviceLevelAgreementProvider;
        private readonly ILogger _logger;

        private List<RepositoryEventArgs> _creationFailureData;
        private List<RepositoryEventArgs> _updateFailureData;
        private List<RepositoryEventArgs> _retrievalFailureData;

        public DaoIssueTracker(IServiceLevelAgreementProvider serviceLevelAgreementProvider, Database issueStorage, ILogger logger)
        {
            this._serviceLevelAgreementProvider = serviceLevelAgreementProvider;
            this._logger = logger;

            this.DaoRepository = new DaoRepository(issueStorage, logger, "Bam.Net.IssueTracking");
            this.CachingRepository = new CachingRepository(DaoRepository, logger);

            this.Sla = 4;

            this._creationFailureData = new List<RepositoryEventArgs>();
            this._updateFailureData = new List<RepositoryEventArgs>();
            this._retrievalFailureData = new List<RepositoryEventArgs>();

            this.DaoRepository.AddNamespace(typeof(IssueData), type => type.ExtendsType<RepoData>());
        }

        protected void SubscribeToDaoRepositoryFailures()
        {
            DaoRepository.CreateFailed += (sender, args) => _creationFailureData.Add((RepositoryEventArgs)args);
            DaoRepository.UpdateFailed += (sender, args) => _updateFailureData.Add((RepositoryEventArgs)args);
            DaoRepository.RetrieveFailed += (sender, args) => _retrievalFailureData.Add((RepositoryEventArgs)args);
        }

        public DaoRepository DaoRepository { get; set; }

        public CachingRepository CachingRepository { get; init;}

        public int Sla
        {
            get
            {
                return _serviceLevelAgreementProvider.Sla;
            }
            set
            {
                _serviceLevelAgreementProvider.Sla = value;
            }
        }

        public async Task<ITrackedIssueComment> AddCommentAsync(ITrackedIssue issue, string commenter, string commentText)
        {
            CommentData commentData = new CommentData(issue)
            {
                CreatedBy = commenter,
                Created = DateTime.UtcNow,
                Text = commentText
            };

            IssueData issueData = EnsureIssueExists(issue);
            issueData.CommentDatas.Add(commentData);

            issueData = DaoRepository.Save<IssueData>(issueData);

            return commentData;
        }

        public async Task<ITrackedIssue> CreateIssueAsync(string externalId, string title, string body)
        {
            return DaoRepository.Save(new IssueData()
            {
                ExternalId = externalId,
                Title = title,
                Body = body
            });
        }

        public async Task<List<ITrackedIssueComment>> GetAllCommentsAsync(ITrackedIssue managedIssue)
        {
            IssueData issueData = EnsureIssueExists(managedIssue);
            return issueData?.CommentDatas?.Select(c=> c.Cast<ITrackedIssueComment>()).ToList() ?? new List<ITrackedIssueComment>();
        }

        public Task<List<ITrackedIssue>> GetAllIssuesAsync()
        {
            throw new NotImplementedException();
        }

        public bool SlaWasMet(ITrackedIssue trackedIssue)
        {
            return _serviceLevelAgreementProvider.SlaWasMet(trackedIssue);
        }

        protected IssueData EnsureIssueExists(ITrackedIssue trackedIssue)
        {
            IssueData issueData = CachingRepository.Query<IssueData>(issueData => issueData.ExternalId == trackedIssue?.Id.ToString()).FirstOrDefault();
            if(issueData == null)
            {
                issueData = new IssueData(trackedIssue);
                issueData = DaoRepository.Save(issueData);
            }
            return issueData;
        }
    }
}
