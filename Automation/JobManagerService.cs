/*
	Copyright © Bryan Apellanes 2015  
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Bam.Net.Configuration;
using Bam.Net.Data;
using Bam.Net.Logging;
using Bam.Net.Profiguration;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;

namespace Bam.Net.Automation
{
    /// <summary>
    /// The manager for all jobs.
    /// </summary>
    [Proxy("jobManagerSvc")]
    [ServiceSubdomain("jobs")]
    public partial class JobManagerService: AsyncProxyableService
    {
        static readonly string ProfigurationSetKey = $"{nameof(JobManagerService)}Settings";

        AutoResetEvent _enqueueSignal;
        AutoResetEvent _runCompleteSignal;
        Thread _runnerThread;

        protected internal JobManagerService() : this(DefaultConfigurationApplicationNameProvider.Instance,
            Net.Data.Repositories.DataProvider.Current, null)
        {
        }

        public JobManagerService(IApplicationNameProvider applicationNameProvider, IDataDirectoryProvider dataProvider) : this(
            applicationNameProvider, dataProvider, null)
        {
        }

        public JobManagerService(IApplicationNameProvider appNameProvider, IDataDirectoryProvider dataProvider,
            ProfigurationSet profiguration) : this(appNameProvider, dataProvider, Log.Default, profiguration)
        {
        }

        public JobManagerService(IApplicationNameProvider appNameProvider, IDataDirectoryProvider dataProvider,
            ILogger logger, ProfigurationSet profiguration = null)
        {
            WorkerTypeProvider = new ScanningWorkerTypeProvider(logger ?? Log.Default);
            TypeResolver = new TypeResolver();
            DataProvider = dataProvider;
            ApplicationNameProvider = appNameProvider;
            JobsDirectory = dataProvider.GetAppDataDirectory(appNameProvider, "Jobs").FullName;
            ProfigurationSet = profiguration ?? ProfigurationSet.In(JobsDirectory);
            MaxConcurrentJobs = 3;
            _enqueueSignal = new AutoResetEvent(false);
            _runCompleteSignal = new AutoResetEvent(false);
        }

        public override object Clone()
        {
            JobManagerService clone = new JobManagerService(ApplicationNameProvider, DataProvider, ProfigurationSet);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        static JobManagerService _current;
        static object _currentLock = new object();
        public static JobManagerService Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current,
                    () => new JobManagerService(ProcessApplicationNameProvider.Current,
                        Net.Data.Repositories.DataProvider.Current));
            }
        }
        
        public IDataDirectoryProvider DataProvider { get; }        
        public IWorkerTypeProvider WorkerTypeProvider { get; }
        public ITypeResolver TypeResolver { get; set; }
        public int MaxConcurrentJobs
        {
            get;
            set;
        }

        List<Job> _running;
        object _runningLock = new object();
        public List<Job> Running
        {
            get
            {
                return _runningLock.DoubleCheckLock(ref _running, () => new List<Job>());
            }
        }

        ProfigurationSet _profigurationSet;
        object _profigurationSetLock = new object();
        protected internal ProfigurationSet ProfigurationSet
        {
            get
            {
				return _profigurationSetLock.DoubleCheckLock(ref _profigurationSet, () => new ProfigurationSet(Path.Combine(JobsDirectory, "ProfigurationSet")));
            }
            private set
            {
                _profigurationSet = value;
            }
        }

        public virtual string[] GetWorkerTypes()
        {
            return WorkerTypeProvider.GetWorkerTypes().Select(type => type.FullName).ToArray();
        }

        /// <summary>
        /// Add a worker of the specified type to the job with the specified
        /// jobName assigning the specified workerName .
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="workerTypeName"></param>
        /// <param name="workerName"></param>
        /// <returns></returns>
        [RoleRequired("/", "Admin")]
        public virtual void AddWorker(string jobName, string workerTypeName, string workerName)
        {
            Type type = TypeResolver.ResolveType(workerTypeName);
            if (type == null)
            {
                throw new ArgumentNullException("workerTypeName", $"Unable to find the specified WorkerType: {workerTypeName}");
            }

            JobConf jobConf = GetJob(jobName);
            if (jobConf == null)
            {
                jobConf = CreateJobConf(jobName);
            }
            
            AddWorker(jobConf, type, workerName);
        }

        /// <summary>
        /// Adds a worker of generic type T to the
        /// specified JobConf.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Local]
        public void AddWorker<T>(JobConf conf, string name)
        {
            AddWorker(conf, typeof(T), name);
        }

        [Local]
        public void AddWorker(JobConf conf, Type type, string name)
        {
            conf.AddWorker(type, name);
        }

        [Exclude]
        public string SecureGet(string key)
        {
            Profiguration.Profiguration prof = ProfigurationSet[ProfigurationSetKey];
            return prof.AppSettings[key];
        }

        [Exclude]
        public string SecureSet(string key, string value)
        {
            Profiguration.Profiguration prof = ProfigurationSet[ProfigurationSetKey];
            prof.AppSettings[key] = value;
            ProfigurationSet.Save();

            return value;
        }

        public virtual JobConf CreateJob(string name)
        {
            Args.ThrowIf<InvalidOperationException>(JobExists(name), "A job with the specified name already exists: ({0})", name);
            return CreateJobConf(name);
        }

        [Verbosity(LogEventType.Information, SenderMessageFormat = "Worker of type {WorkTypeName} and Step Number {StepNumber} of Job {JobName} started")]
        public event EventHandler WorkStarting;

        protected void OnWorkerStarting(WorkState state)
        {
            WorkStarting?.Invoke(this, new WorkStateEventArgs(state));
        }

        [Verbosity(LogEventType.Information, SenderMessageFormat = "Worker of type {WorkTypeName} and Step Number {StepNumber} of Job {JobName} finished")]
        public event EventHandler WorkerFinished;

        protected void OnWorkerFinished(WorkState state)
        {
            WorkerFinished?.Invoke(this, new WorkStateEventArgs(state));
        }

        [Verbosity(LogEventType.Error, SenderMessageFormat = "EXCEPTION:{LastMessage}:Worker of type {WorkTypeName} and Step Number {StepNumber} of Job {JobName}")]
        public event EventHandler WorkerException;

        protected void OnWorkerException(WorkState state)
        {
            WorkerException?.Invoke(this, new WorkStateEventArgs(state));
        }

        [Verbosity(LogEventType.Error, SenderMessageFormat = "EXCEPTION:{LastMessage}:Exception running job")]
        public event EventHandler JobRunException;

        protected void OnJobRunException(Exception ex)
        {
            JobRunException?.Invoke(this, new WorkStateEventArgs(new WorkState(null, ex)));
        }

        [Verbosity(LogEventType.Information, SenderMessageFormat = "JobName={Name}")]
        public event EventHandler JobFinished;

        protected void OnJobFinished(WorkState state)
        {
            JobFinished?.Invoke(this, new WorkStateEventArgs(state));
        }

        public virtual string[] ListJobNames()
        {
            DirectoryInfo jobsDirectory = new DirectoryInfo(JobsDirectory);
            if (!jobsDirectory.Exists)
            {
                return new string[] { };
            }
            DirectoryInfo[] jobDirectories = jobsDirectory.GetDirectories();
            return jobDirectories.Select(jd => jd.Name).ToArray();
        }

        public virtual JobConf RenameJob(string name, string newName)
        {
            JobConf copy = CopyJob(name, newName);
            RemoveJob(name);
            return copy;
        }
        
        public virtual JobConf CopyJob(string name, string copyName = null)
        {
            if (!JobExists(name))
            {
                throw new InvalidOperationException("Specified job does not exist");
            }
            copyName = copyName ?? $"{name}-copy";
            int num = 1;
            while (JobExists(copyName))
            {
                copyName += num.ToString();
            }
            JobConf conf = GetJob(name);
            JobConf copy = GetJob(copyName);
            foreach (string workerName in conf.WorkerConfs.Keys)
            {
                WorkerConf workerConf = conf[workerName];
                AddWorker(copy.Name, workerConf.WorkerTypeName, workerName);
            }

            return copy;
        }
                
        public virtual void SaveJob(JobConf jobConf)
        {
            jobConf.JobDirectory = GetJobDirectoryPath(jobConf.Name);
            jobConf.Save();
        }

        public virtual void RemoveJob(string jobName)
        {
            DirectoryInfo jobDirectory = new DirectoryInfo(GetJobDirectoryPath(jobName));
            if (jobDirectory.Exists)
            {
                jobDirectory.Delete(true);
            }
        }
        
        /// <summary>
        /// Get a JobConf with the specified name creating it if necessary.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual JobConf GetJob(string name, bool create = true)
        {
            return GetJobConf(name, create);
        }

        protected internal JobConf GetJobConf(string name, bool create = true)
        {
            if (JobExists(name))
            {
                JobConf conf = new JobConf(name)
                {
                    JobDirectory = GetJobDirectoryPath(name)
                };
                return JobConf.Load(conf.GetFilePath());
            }
            else if(create)
            {
                return CreateJobConf(name);
            }

            return null;
        }

        protected internal JobConf CreateJobConf(string name)
        {
            JobConf conf = new JobConf(name)
            {
                JobDirectory = GetJobDirectoryPath(name)
            };
            conf.Save();
            return conf;
        }

        public virtual bool WorkerExists(string jobName, string workerName)
        {
            bool result = false;
            if (JobExists(jobName))
            {
                JobConf conf = GetJob(jobName);
                result = conf.WorkerExists(workerName);
            }

            return result;
        }

        /// <summary>
        /// Returns true if a job with the specified name
        /// exists under the current JobManager.  Determined
        /// by looking in the current JobManager's JobsDirectory.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool JobExists(string name)
        {
            return JobExists(name, out string ignore);
        }

        protected internal bool JobExists(string name, out string jobDirectoryPath)
        {
			jobDirectoryPath = Path.Combine(JobsDirectory, name);
            return Directory.Exists(jobDirectoryPath);
        }

        public virtual void StartJob(string jobName)
        {
            EnqueueJob(jobName);
        }
        
        /// <summary>
        /// Enqueue a job to be run next (typically instant if no other
        /// jobs are running).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stepNumber"></param>
        [Local]
        public void EnqueueJob(string name, int stepNumber = 0)
        {
            JobConf conf = GetJobConf(name);

            EnqueueJob(conf, stepNumber);
        }
        
        protected internal void EnqueueJob(JobConf conf, int stepNumber = 0)
        {
            Args.ThrowIfNull(conf, "JobConf");

            lock (_jobQueueLock)
            {
                if (!_isRunning)
                {
                    StartJobRunnerThread();
                }

                Job job = conf.CreateJob();
                job.StepNumber = stepNumber;
                JobQueue.Enqueue(job);
                _enqueueSignal.Set();
            }
        }

        Queue<Job> _jobQueue;
        object _jobQueueLock = new object();
        protected internal Queue<Job> JobQueue
        {
            get
            {
                return _jobQueueLock.DoubleCheckLock(ref _jobQueue, () => new Queue<Job>());
            }
        }

        bool _isRunning;
        /// <summary>
        /// Starts the JobRunner thread.  This method
        /// must be called prior to queueing up jobs
        /// or the jobs will not be run.
        /// </summary>
        [Local]
        public void StartJobRunnerThread()
        {
            _runnerThread = new Thread(JobRunner)
            {
                IsBackground = true
            };
            _runnerThread.Start();
            _isRunning = true;
        }

        [Local]
        public void StopJobRunnerThread()
        {
            try
            {
                if(_runnerThread != null && _runnerThread.ThreadState == ThreadState.Running)
                {
                    _runnerThread.Abort();
                    _runnerThread.Join(2500);
                }                
            }
            catch { }

            _isRunning = false;
        }

        private void JobRunner()
        {
            while (true)
            {
                _enqueueSignal.WaitOne();

                try
                {
                    while (JobQueue.Count > 0)
                    {
                        Job job = null;
                        lock (_jobQueueLock)
                        {
                            if (JobQueue.Count > 0)
                            {
                                job = JobQueue.Dequeue();
                            }
                        }

                        if (job != null)
                        {
                            job.JobFinished += (o, a) =>
                            {
                                Job j = (Job)o;
                                lock (_runningLock)
                                {
                                    if (Running.Contains(j))
                                    {
                                        Running.Remove(j);
                                    }
                                }

                                _runCompleteSignal.Set();
                            };

                            RunJob(job);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Trace("Exception running jobs: {0}", ex.Message);
                }
            }
        }

        protected internal void RunJob(Job job, int stepNumber = 0)
        {
            lock (_runningLock)
            {
                if (Running.Count >= MaxConcurrentJobs)
                {
                    _runCompleteSignal.WaitOne();
                }

                job.WorkerException += (o, a) => OnWorkerException(((WorkStateEventArgs)a).WorkState);
                job.WorkerStarting += (o, a) => OnWorkerStarting(((WorkStateEventArgs)a).WorkState);
                job.WorkerFinished += (o, a) => OnWorkerFinished(((WorkStateEventArgs)a).WorkState);
                job.JobFinished += (o, a) => OnJobFinished(((WorkStateEventArgs)a).WorkState);
                Running.Add(job);
            }

            try
            {
                job.StepNumber = stepNumber;
                job.Run();
            }
            catch (Exception ex)
            {
                OnJobRunException(ex);
                lock (_runningLock)
                {
                    try
                    {
                        if (Running.Contains(job))
                        {
                            Running.Remove(job);
                        }
                    }
                    catch (Exception inner)
                    {
                        Log.Trace("Exception untracking running job: {0}", inner.Message);
                    }
                }
            }
        }

        protected string GetJobDirectoryPath(string name)
        {
			return Path.Combine(JobsDirectory, name);
        }
    }
}
