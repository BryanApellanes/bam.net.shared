/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using Bam.Net;
using Bam.Net.Logging;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Bam.Net.ServiceProxy;

namespace Bam.Net.Server
{
    public class HttpServer : Loggable, IDisposable
    {
        private static readonly ConcurrentDictionary<HostBinding, HttpServer> _listening = new ConcurrentDictionary<HostBinding, HttpServer>();
        private readonly HttpListener _listener;
        private readonly Thread _handlerThread;
        private readonly ILogger _logger;
        
        public HttpServer(ILogger logger = null)
        {
            logger = logger ?? Log.Default;
            
            _listener = new HttpListener();
            _handlerThread = new Thread(HandleRequests);
            _logger = logger;
            _hostBindings = new HashSet<HostBinding>();
        }

        HashSet<HostBinding> _hostBindings;
        public HostBinding[] HostBindings
        {
            get => _hostBindings.ToArray();
            set => _hostBindings = new HashSet<HostBinding>(value);
        }
        
        public bool IsListening
        {
            get=> (bool)_listener?.IsListening;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this HttpServer, when started, will attempt to stop
        /// other HttpServers that are listening on the same port and hostname.
        /// </summary>
        public bool Usurped
        {
            get;
            set;
        }

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        public event EventHandler Usurping;

        [Verbosity(VerbosityLevel.Information, EventArgsMessageFormat = "HostPrefixAdded: {HostPrefixString}")]
        public event EventHandler HostPrefixAdded;

        public void Start()
        {
            Start(HostBindings);
        }

        public void Start(params HostBinding[] hostBindings)
        {
            Start(Usurped, hostBindings);
        }

        static readonly object _startLock = new object();
        public void Start(bool usurped, params HostBinding[] hostBindings)
        {
            if(hostBindings.Length == 0)
            {
                hostBindings = HostBindings;
            }
            lock (_startLock)
            {
                hostBindings.Each(hp =>
                {
                    if (!_listening.ContainsKey(hp))
                    {
                        AddHostBinding(hp);
                    }
                    else if (usurped && _listening.ContainsKey(hp))
                    {
                        FireEvent(Usurping, new HttpServerEventArgs { HostBindings = new HostBinding[] { hp } });
                        _listening[hp].Stop();                        
                        _listening.TryRemove(hp, out HttpServer ignore);
                        AddHostBinding(hp);
                    }
                    else
                    {
                        _logger.AddEntry("HttpServer: Another HttpServer is already listening for host {0}", LogEventType.Warning, hp.ToString());
                    }
                });
                FireEvent(Starting, new HttpServerEventArgs { HostBindings = HostBindings });
                _stopRequested = false;
                _listener.Start();
                _handlerThread.Start();
                FireEvent(Started, new HttpServerEventArgs { HostBindings = HostBindings });
            }
        }

        private void AddHostBinding(HostBinding hp)
        {
            _listening.TryAdd(hp, this);
            string path = hp.ToString();
            _logger.AddEntry("HttpServer: {0}", path);

            if (!OSInfo.IsUnix)
            {
                _listener.Prefixes.Add(path);
            }
            else
            {
                string protocol = hp.Ssl ? "https" : "http";
                _listener.Prefixes.Add($"{protocol}://*:{hp.Port}/");
            }
            FireEvent(HostPrefixAdded, new HttpServerEventArgs { HostBindings = new HostBinding[] { hp } });
        }

        public void Dispose()
        {
			IsDisposed = true;
            Stop();
        }

		public bool IsDisposed { get; private set; }

        bool _stopRequested;
        public void Stop()
        {
            _stopRequested = true;
            try
            {
                _listener.Stop();
                _logger.AddEntry("HttpServer listener stopped");
            }
            catch (Exception ex)
            {
                _logger.AddEntry("Error stopping HttpServer: {0}", ex, ex.Message);
            }

            foreach (HostBinding hp in _listening.Keys)
            {
                try
                {
                    if (_listening[hp] == this)
                    {
                        if (_listening.TryRemove(hp, out HttpServer server))
                        {
                            FireEvent(Stopping, new HttpServerEventArgs { HostBindings = new HostBinding[] { hp } });
                            server.Stop();
                            FireEvent(Stopped, new HttpServerEventArgs { HostBindings = new HostBinding[] { hp } });
                        }
                    }
                }
                catch { }
            }
            
            if (_handlerThread.ThreadState == ThreadState.Running)
			{
                try
                {
                    _handlerThread.Abort();
                    _handlerThread.Join();
                }
                catch { }
			}
        }
        
        private void HandleRequests()
        {
            while (_listener != null && _listener.IsListening && !_stopRequested)
            {
                try
                {
                    HttpListenerContext listenerContext = _listener.GetContext();
                    IHttpContext context = new HttpContextWrapper(listenerContext);
                    Task.Run(() =>
                    {
                        try
                        {
                            PreProcessRequest(context);
                        }
                        catch (Exception ex)
                        {
                            Log.Warn("Exception PRE-processing request {0}", ex.Message);
                        }
                        ProcessRequest(context);
                    });
                }
                catch { }
            }
        }

        public event Action<IHttpContext> PreProcessRequest;
        public event Action<IHttpContext> ProcessRequest;
    }
}
