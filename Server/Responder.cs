/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bam.Net.Logging;
using System.IO;
using Bam.Net.ServiceProxy;
using Bam.Net.Server.Renderers;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Bam.Net.Logging.Http;
using Bam.Net.Services;
using DefaultNamespace;

namespace Bam.Net.Server
{
    public abstract class Responder : Loggable, IHttpResponder
    {
        readonly Dictionary<string, string> _contentTypes;        
        public Responder(BamConf conf)
        {
            BamConf = conf;
            Logger = Log.Default;
            ApplicationNameResolver = new UriApplicationNameResolver(conf);
            RequestLog = new RequestLog();

            _contentTypes = new Dictionary<string, string>
            {
                { ".json", "application/json" },
                { ".js", "application/javascript" },
                { ".css", "text/css" },
                { ".jpg", "image/jpg" },
                { ".gif", "image/gif" },
                { ".png", "image/png" },
                { ".html", "text/html" }
            };
            _respondToPrefixes = new List<string>();
            _ignorePrefixes = new List<string>();

            AddRespondToPrefix(ResponderName);
            BamAppServer bamAppServer = conf?.AppServer ?? BamAppServer.Current;
            ApplicationServiceRegistry = bamAppServer.LoadApplicationServiceRegistryAsync()?.Result;
        }

        public Responder(BamConf conf, ILogger logger)
            : this(conf)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Registry resolved by the server.
        /// </summary>
        public ApplicationServiceRegistry ApplicationServiceRegistry { get; set; }

        ILogger _logger;
        object _loggerLock = new object();
        public ILogger Logger
        {
            get
            {
                return _loggerLock.DoubleCheckLock(ref _logger, () => Log.Default);
            }
            internal set => _logger = value;
        }

        [Inject]
        public RequestLog RequestLog { get; set; }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public virtual bool IsInitialized
        {
            get;
            protected set;
        }

        public IApplicationNameResolver ApplicationNameResolver { get; set; }

        /// <summary>
        /// The event that fires when a response is sent
        /// </summary>
        public event ResponderEventHandler Responded;
        /// <summary>
        /// The event that fires when a response is not sent
        /// </summary>
        public event ResponderEventHandler DidNotRespond;

        public event ContentNotFoundEventHandler ContentNotFound;
        
        BamConf _bamconf;
        public BamConf BamConf
        {
            get => _bamconf;
            set
            {
                _bamconf = value;
                _serverRoot = _bamconf.Fs;
            }
        }

        Fs _serverRoot;
        public virtual Fs ServerRoot
        {
            get => _serverRoot ?? BamConf.Fs;
            protected set => _serverRoot = value;
        }

        /// <summary>
        /// Returns true if the AbsolutePath of the requested
        /// Url starts with the name of the current class.  Extenders
        /// will provide different implementations based on their
        /// requirements
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool MayRespond(IHttpContext context)
        {
            string lowered = context.Request.Url.AbsolutePath.ToLowerInvariant();
            bool result = false;

            if (!ShouldIgnore(lowered))
            {
                _respondToPrefixes.Each(prefix =>
                {
                    if (!result)
                    {
                        result = lowered.StartsWith(string.Format("/{0}", prefix.ToLowerInvariant()));
                    }
                });
            }

            return result;
        }

        public virtual bool Respond(IHttpContext context)
        {
            bool result = false;
            if (MayRespond(context))
            {
                result = TryRespond(context);
            }
            return result;
        }

        public abstract bool TryRespond(IHttpContext context);
        public static void SendResponse(IHttpContext context, HttpStatusCodeHandler handler)
        {
            SendResponse(context, handler.Handle(), handler.Code);
        }

        public static void SendResponse(IHttpContext context, int statusCode, object dynamicObjectHeaders)
        {
            SendResponse(context, new HttpStatusCodeHandler(statusCode, string.Empty), dynamicObjectHeaders);
        }

        public static void SendResponse(IHttpContext context, HttpStatusCodeHandler handler, object dynamicObjectHeaders)
        {
            SendResponse(context, handler.Handle(), handler.Code, null, dynamicObjectHeaders.ToDictionary(pi => $"X-{pi.Name.PascalSplit("-")}", (o) => (string)o));
        }

        public static void SendResponse(IHttpContext context, string output, int statusCode = 200, Encoding encoding = null, Dictionary<string, string> headers = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            SendResponse(context.Response, output, statusCode, encoding, headers);
        }

        public static void SendResponse(IResponse response, string output, int statusCode = 200, Encoding encoding = null, Dictionary<string, string> headers = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] data = encoding.GetBytes(output);
            SendResponse(response, data, statusCode, headers);
        }

        public static void SendResponse(IResponse response, byte[] data, int statusCode, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                headers.Keys.Each(key =>
                {
                    if(response.Headers == null)
                    {
                        response.Headers = new System.Net.WebHeaderCollection();
                    }
                    response.Headers.Add(key, headers[key]);
                });
            }
            response.OutputStream.Write(data, 0, data.Length);
            response.StatusCode = statusCode;
            response.Close();
        }

        protected string GetContentTypeForExtension(string ext)
        {
            string contentType = string.Empty;
            if (this._contentTypes.ContainsKey(ext))
            {
                contentType = this._contentTypes[ext];
            }
            return contentType;
        }

        protected string GetContentTypeForPath(string path)
        {
            string ext = Path.GetExtension(path);
            return GetContentTypeForExtension(ext);
        }

        protected void SetContentDisposition(IResponse response, string path)
        {
            if(Path.GetExtension(path).Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                response.AddHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(path)}");
            }
        }

        protected void SetContentType(IResponse response, string path)
        {
            string contentType = GetContentTypeForPath(path);
            if (!string.IsNullOrEmpty(contentType))
            {
                response.ContentType = contentType;
            }
        }

        ConcurrentDictionary<string, byte[]> _pageCache;
        object _pageCacheLock = new object();
        protected ConcurrentDictionary<string, byte[]> Cache
        {
            get
            {
                return _pageCacheLock.DoubleCheckLock(ref _pageCache, () => new ConcurrentDictionary<string, byte[]>());
            }
        }

        ConcurrentDictionary<string, byte[]> _zippedPageCache;
        object _zippedPageCacheLock = new object();
        protected ConcurrentDictionary<string, byte[]> ZippedCache
        {
            get
            {
                return _zippedPageCacheLock.DoubleCheckLock(ref _zippedPageCache, () => new ConcurrentDictionary<string, byte[]>());
            }
        }
            

        protected Dictionary<string, string> ContentTypes => this._contentTypes;

        public virtual string Name => this.GetType().Name;

        /// <summary>
        /// Set the status code, flush the response and close the output 
        /// stream.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        public static void FlushResponse(IHttpContext context, int statusCode = 200)
        {
            context.Response.StatusCode = statusCode;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }

        protected internal void OnResponded(IHttpResponder responder, IHttpContext context)
        {
            Task.Run(() => Responded?.Invoke(responder, context));
        }
        
        protected internal void OnResponded(IHttpContext context)
        {
            Task.Run(() => Responded?.Invoke(this, context));
        }

        protected internal void OnDidNotRespond(IHttpContext context)
        {
            Task.Run(() => DidNotRespond?.Invoke(this, context));
        }

        protected internal void OnDidNotRespond(IHttpResponder responder, IHttpContext context)
        {
            Task.Run(() => DidNotRespond?.Invoke(responder, context));
        }
        
        protected internal void OnContentNotFound(IHttpResponder responder, IHttpContext context, string[] checkedPaths)
        {
            Task.Run(() =>
            {
                RequestLog.LogContentNotFound(responder, context, checkedPaths);
                ContentNotFound?.Invoke(responder, context, checkedPaths);
            });
        }

        readonly List<string> _respondToPrefixes;
        protected internal void AddRespondToPrefix(string prefix)
        {
            prefix = prefix.ToLowerInvariant();
            if (_ignorePrefixes.Contains(prefix))
            {
                _ignorePrefixes.Remove(prefix);
            }

            _respondToPrefixes.Add(prefix);
        }
        protected static void WireResponseLogging(IHttpResponder responder, ILogger logger)
        {
            responder.Responded += (r, context) => logger.AddEntry("*** ({0}) Responded ***\r\n{1}", LogEventType.Information, r.Name, context.Request.PropertiesToString());
            responder.DidNotRespond += (r, context) => logger.AddEntry("*** ({0}) Didn't Respond ***\r\n{1}", LogEventType.Warning, r.Name, context.Request.PropertiesToString());
            responder.ContentNotFound += (r, context, paths) =>
            {
                StringBuilder formattedPaths = new StringBuilder();
                paths.Each(path => formattedPaths.AppendLine($"{path}"));
                logger.AddEntry("*** ({0}) Content Not Found ***\r\n{1}\r\n{2}", LogEventType.Warning, r.Name, formattedPaths.ToString(), context.Request.PropertiesToString());
            };
        }

        readonly List<string> _ignorePrefixes;
        protected internal void AddIgnorPrefix(string prefix)
        {
            prefix = prefix.ToLowerInvariant();
            if (_respondToPrefixes.Contains(prefix))
            {
                _respondToPrefixes.Remove(prefix);
            }

            _ignorePrefixes.Add(prefix);
        }

        protected internal bool WillIgnore(IHttpContext context)
        {
            return ShouldIgnore(context.Request.Url.AbsolutePath.ToLowerInvariant());
        }

        protected internal bool ShouldIgnore(string path)
        {
            bool result = false;
            _ignorePrefixes.Each(ignore =>
            {
                if (!result)
                {
                    result = path.ToLowerInvariant().StartsWith(string.Format("/{0}", ignore));
                }
            });

            return result;
        }

        protected static void WriteResponse(IResponse response, byte[] data)
        {
            using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
            {
                bw.Write(data);
                bw.Flush();
            }
        }

        protected static void WriteResponse(IResponse response, string content)
        {
            WriteResponse(response, Encoding.UTF8.GetBytes(content));
        }

        protected WebRendererFactory RendererFactory
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the current responder with the "Responder" suffix removed.
        /// </summary>
        protected internal virtual string ResponderName
        {
            get
            {
                string responderName = this.Name;
                if (responderName.EndsWith("Responder", StringComparison.InvariantCultureIgnoreCase))
                {
                    responderName = responderName.Truncate(9);
                }
                return responderName;
            }
        }

        protected static bool ShouldZip(IRequest request)
        {
            if ((bool)request.Headers["Accept-Encoding"]?.DelimitSplit(",").ToList().Contains("gzip"))
            {
                return true;
            }
            return false;
        }

        protected static void SetGzipContentEncodingHeader(IResponse response)
        {
            response.AddHeader("Content-Encoding", "gzip");
        }

    }
}
