using Bam.Net.CoreServices.ApplicationRegistration.Data;
using Bam.Net.Logging;
using Bam.Net.Logging.Http;
using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bam.Net.Server
{
    public abstract class SimpleServer<TResponder> where TResponder: IHttpResponder, new()
    {
        HttpServer _server;
        public SimpleServer(): this(new TResponder(), Log.Default)
        {
        }

        public SimpleServer(int port, bool ssl = false) : this("localhost", port, ssl)
        { 
        }

        public SimpleServer(string hostName, int port, bool ssl = false): this(new TResponder(), Log.Default)
        {
            HostBindings = new HashSet<HostBinding> 
            { 
                new HostBinding
                {
                    Port = port,
                    HostName = hostName,
                    Ssl = ssl
                }
            };
        }

        public SimpleServer(TResponder responder, ILogger logger)
        {
            Responder = responder;
            Logger = logger ?? Log.Default;
            CreatedOrChangedHandler = (o, a) => { };
            RenamedHandler = (o, a) => { };
            HostBindings = new HashSet<HostBinding>
            {
                new HostBinding { Port = 80, HostName = "localhost", Ssl = false }
            };
            MonitorDirectories = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) };
        }

        /// <summary>
        /// Gets a value indicating whether this server is listening.
        /// </summary>
        public bool IsListening
        {
            get => (bool)_server?.IsListening;
        }

        /// <summary>
        /// An array of hosts that this server will respond to
        /// </summary>
        public HashSet<HostBinding> HostBindings { get; set; }
        
        /// <summary>
        /// The responder
        /// </summary>
        public TResponder Responder { get; set; }
        
        /// <summary>
        /// The logger
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The FileSystemWatchers; one each for create, changed and renamed
        /// </summary>
        public List<FileSystemWatcher> FileSystemWatchers { get; protected set; }

        /// <summary>
        /// An array of directories to monitor for
        /// created, changed or renamed files
        /// </summary>
        public string[] MonitorDirectories { get; set; }

        /// <summary>
        /// The delegate that is subscribed to the Create
        /// and Changed handler of the underlying FileSystemWatcher(s)
        /// </summary>
        public FileSystemEventHandler CreatedOrChangedHandler { get; set; }

        public virtual void Start()
        {
            Logger.RestartLoggingThread();
            this.FileSystemWatchers = new List<FileSystemWatcher>();
            this.WireEventHandlers();
            _server.Start(HostBindings.ToArray());
        }

        public virtual void Stop()
        {
            Logger.StopLoggingThread();
            _server.Stop();
        }

        public void TryStop()
        {
            try
            {
                Stop();
            }
            catch { }
        }

        /// <summary>
        /// The delegate that is subscribed to the renamed event of the underlying
        /// FileSystemWatcher(s)
        /// </summary>
        public RenamedEventHandler RenamedHandler { get; set; }

        /// <summary>
        /// Wire the event handlers
        /// </summary>
        protected void WireEventHandlers()
        {
            _server = new HttpServer(Logger ?? Log.Default);
            _server.ProcessRequest += ProcessRequest;
            _server.PreProcessRequest += PreProcessRequest;

            WireResponderEventHandlers();
            MonitorDirectories.Each(directory =>
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                FileSystemWatchers.Add(directoryInfo.OnChange(CreatedOrChangedHandler));
                FileSystemWatchers.Add(directoryInfo.OnCreated(CreatedOrChangedHandler));
                FileSystemWatchers.Add(directoryInfo.OnRenamed(RenamedHandler));
            });
        }

        private readonly object _requestLogLock = new object();
        private RequestLog _requestLog;

        public RequestLog RequestLog
        {
            get { return _requestLogLock.DoubleCheckLock(ref _requestLog, () => new RequestLog()); }
            set => _requestLog = value;
        }

        protected void PreProcessRequest(IHttpContext context)
        {
            if(context?.Request == null)
            {
                return;
            }

            context.SetRequestId();
            RequestLog.LogRequest(context);
        }

        protected void ProcessRequest(IHttpContext context)
        {
            Responder.Respond(context);
        }


        private void WireResponderEventHandlers()
        {
            Responder.Responded += (r, context) =>
            {
                Server.Responder.FlushResponse(context);
                Logger.AddEntry("*** ({0}) Responded ***\r\n{1}", LogEventType.Information, r.Name, context.Request.PropertiesToString());
            };
            Responder.DidNotRespond += (r, context) =>
            {
                Server.Responder.FlushResponse(context);
                Logger.AddEntry("*** ({0}) Didn't Respond ***\r\n{1}", LogEventType.Warning, r.Name, context.Request.PropertiesToString());
            };
        }
    }
}
