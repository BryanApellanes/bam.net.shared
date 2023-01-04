/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net;
using Bam.Net.Configuration;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.Incubation;
using Bam.Net.Logging;
using Bam.Net.Presentation;
using Bam.Net.Presentation.Html;
using Bam.Net.Server.Listeners;
using Bam.Net.Server.Renderers;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;
using Bam.Net.UserAccounts;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Bam.Net.Application;
using Bam.Net.Logging.Http;

namespace Bam.Net.Server
{
    /// <summary>
    /// The core monolithic BamServer.
    /// </summary>
    public partial class BamAppServer : Loggable, IInitialize<BamAppServer>, IManagedServer
    {
        private readonly HashSet<IHttpResponder> _responders;
        private readonly Dictionary<string, IHttpResponder> _respondersByName;
        private HttpServer _server;

        public BamAppServer() : this(new BamConf()) 
        {
            Type type = this.GetType();
            this.ServerName = $"{type.Namespace}.{type.Name}_{Environment.MachineName}_{Guid.NewGuid()}";
            this.DefaultHostBinding = new ManagedServerHostBinding(this);
        }

        public BamAppServer(BamConf conf)
        {
            _responders = new HashSet<IHttpResponder>();
            _respondersByName = new Dictionary<string, IHttpResponder>();
            Initialized += HandlePostInitialization;
            SetConf(conf);
            BindEventListeners(conf);
            EnableDao = true;
            EnableServiceProxy = true;
            SQLiteRegistrar.RegisterFallback();
            AppDomain.CurrentDomain.DomainUnload += (s, a) => Stop();
            _ = LoadApplicationServiceRegistryAsync();
        }

        public BamAppServer(HostBinding hostBinding): this()
        {
            this.DefaultHostBinding = hostBinding;
        }

        private static BamAppServer _bamAppServer;
        private static object _bamServerSync = new object();
        public static BamAppServer Current
        {
            get
            {
                return _bamServerSync.DoubleCheckLock(ref _bamAppServer, () => new BamAppServer(BamConf.Load()));
            }
        }

        public string ServerName { get; private set; }

        private ApplicationServiceRegistry _appServiceRegistry;
        public async Task<ApplicationServiceRegistry> LoadApplicationServiceRegistryAsync()
        {
            return await Task.Run(() => 
            {
                if (_appServiceRegistry == null)
                {
                    _appServiceRegistry = ApplicationServiceRegistry.Current;
                    _appServiceRegistry
                        .For<ContentResponder>().Use(ContentResponder)
                        .For<ITemplateNameResolver>().Use<ContentTemplateNameResolver>()
                        .For<ITemplateManager>().Use<CommonHandlebarsRenderer>()
                        .For<Logging.Http.RequestLog>().Use(RequestLog);

                    _appServiceRegistry.SetInjectionProperties(ContentResponder);
                    ContentResponder.ApplicationServiceRegistry = _appServiceRegistry;
                }
                return _appServiceRegistry;
            });
        }
        
        /// <summary>
        /// The event that fires when server initialization begins
        /// </summary>
        public event Action<BamAppServer> Initializing;
        
        /// <summary>
        /// The event that fires when server initialization is complete
        /// </summary>
        public event Action<BamAppServer> Initialized;
        
        /// <summary>
        /// The event that fires when a schema is about to be initialized
        /// </summary>
        public event Action<BamAppServer, SchemaInitializer> SchemaInitializing;
        
        /// <summary>
        /// The event that fires when a schema is done initializing
        /// </summary>
        public event Action<BamAppServer, SchemaInitializer> SchemaInitialized; 
        
        /// <summary>
        /// The event that fires before beginning any schema initialization
        /// </summary>
        public event Action<BamAppServer> SchemasInitializing;
        
        /// <summary>
        /// The event that fires when all schemas have completed initialization
        /// </summary>
        public event Action<BamAppServer> SchemasInitialized;
        
        /// <summary>
        /// The event that fires before loading the server configuration
        /// </summary>
        public event Action<BamAppServer, BamConf> LoadingConf;
        
        /// <summary>
        /// the event that fires when loading the server configuration is complete
        /// </summary>
        public event Action<BamAppServer, BamConf> LoadedConf; 
        
        /// <summary>
        /// The event that fires before creating an application
        /// </summary>
        public event Action<BamAppServer, AppConf> CreatingApp;
        
        /// <summary>
        /// The event that fires when creating an application is complete
        /// </summary>
        public event Action<BamAppServer, AppConf> CreatedApp;
        
        /// <summary>
        /// The event that fires before a response is flushed.
        /// </summary>
        public event Action<BamAppServer, IHttpResponder, IResponse> ResponseFlushing;
        
        /// <summary>
        /// The event that fires when a response is flushed.
        /// </summary>
        public event Action<BamAppServer, IHttpResponder, IRequest> ResponseFlushed;
        
        /// <summary>
        /// The event that fires when a response is not sent
        /// </summary>
        public event Action<BamAppServer, IRequest> NotResponded;
        
        /// <summary>
        /// The event that fires when a responder is added
        /// </summary>
        public event Action<BamAppServer, IHttpResponder> ResponderAdded;
        
        /// <summary>
        /// The event that fires before setting the configuration
        /// </summary>
        public event Action<BamAppServer, BamConf> SettingConf;
        
        /// <summary>
        /// The event that fires when setting the configuration is complete
        /// </summary>
        public event Action<BamAppServer, BamConf> SettedConf; 
        
        /// <summary>
        /// The event that fires when the configuration is saved
        /// </summary>
        public event Action<BamAppServer, BamConf> SavedConf;
        
        /// <summary>
        /// The event that fires before starting the server
        /// </summary>
        public event Action<BamAppServer> Starting;
        
        /// <summary>
        /// The event that fires when the server has started
        /// </summary>
        public event Action<BamAppServer> Started;
        
        /// <summary>
        /// The event that fires before the server is stopped
        /// </summary>
        public event Action<BamAppServer> Stopping;
        
        /// <summary>
        /// The event that fires when the server has stopped
        /// </summary>
        public event Action<BamAppServer> Stopped;

        private readonly object _requestLogLock = new object();
        private RequestLog _requestLog;
        
        public RequestLog RequestLog
        {
            get { return _requestLogLock.DoubleCheckLock(ref _requestLog, () => new RequestLog()); }
            set => _requestLog = value;
        }
        
        private string ServerWorkspace => Path.Combine("common", "workspace");

        protected void BindEventListeners(BamConf conf)
        {
            BamServerEventListenerBinder binder = new BamServerEventListenerBinder(conf);
            binder.Bind();
        }

        public PostServerInitializationHandler PostInitializationHandler { get; set; }

        public bool IsInitialized
        {
            get;
            private set;
        }

        public SchemaInitializer[] SchemaInitializers // gets set by CopyProperties in SetConf
        {
            get;
            set;
        }
        
        public Dictionary<string, List<AppPageRendererManager>> ReloadAppPageRendererManagers()
        {
            _appPageRendererManagers = null;
            return AppPageRendererManagers;
        }
        
        private Dictionary<string, List<AppPageRendererManager>> _appPageRendererManagers;
        private readonly object _appPageRendererManagerLock = new object();
        public Dictionary<string, List<AppPageRendererManager>> AppPageRendererManagers
        {
            get
            {
                return _appPageRendererManagerLock.DoubleCheckLock(ref _appPageRendererManagers, () =>
                {
                    Dictionary<string, List<AppPageRendererManager>> result = new Dictionary<string, List<AppPageRendererManager>>();
                    foreach (AppConf appToServe in _conf.AppsToServe)
                    {
                        if (string.IsNullOrEmpty(appToServe.Name))
                        {
                            Log.Warn("AppPageRendererManagers: Application name not specified in AppConf: \r\n{0}", appToServe.ToJson(true));
                        }
                        if (!result.ContainsKey(appToServe.Name))
                        {
                            result.Add(appToServe.Name, new List<AppPageRendererManager>());
                        }

                        if (AppContentResponders.ContainsKey(appToServe.Name))
                        {
                            if (AppContentResponders[appToServe.Name].PageRenderer is AppPageRendererManager current)
                            {
                                result[appToServe.Name].Add(current);
                            }
                        }
                    }
                    return result;
                }); 
            }
        }
        
        protected void OnInitializing()
        {
            Initializing?.Invoke(this);
        }

        protected void OnInitialized()
        {
            Initialized?.Invoke(this);
        }

        protected void OnSchemasInitializing()
        {
            SchemasInitializing?.Invoke(this);
        }

        protected void OnSchemasInitialized()
        {
            SchemasInitialized?.Invoke(this);
        }

        protected void OnSchemaInitializing(SchemaInitializer initializer)
        {
            SchemaInitializing?.Invoke(this, initializer);
        }
        
        protected void OnSchemaInitialized(SchemaInitializer initializer)
        {
            SchemaInitialized?.Invoke(this, initializer);
        }

        public virtual void Initialize()
        {
            if (!this.IsInitialized)
            {
                OnInitializing();
                LoadConf();

                Subscribe(MainLogger);
                SubscribeResponders(MainLogger);
                
                MainLogger.AddEntry("{0} initializing: \r\n{1}", this.GetType().Name, this.PropertiesToString());

                InitializeCommonSchemas();

                InitializeResponders();

                InitializeUserManagers();

                InitializeApps();

                ConfigureHttpServer();

                RegisterWorkspaceDaos();

                OnInitialized();
            }
            else
            {
                MainLogger.AddEntry("Initialize called but the {0} was already initialized", LogEventType.Warning, this.GetType().Name);
            }
        }

        public event Action<BamAppServer, AppConf> AppInitializing;
        protected void OnAppInitializing(AppConf conf)
        {
            AppInitializing?.Invoke(this, conf);
        }
        
        public event Action<BamAppServer, AppConf> AppInitialized;
        protected void OnAppInitialized(AppConf conf)
        {
            AppInitialized?.Invoke(this, conf);
        }
        
        protected internal void InitializeApps()
        {
            InitializeApps(_conf.AppsToServe);
        }

        private void InitializeApps(AppConf[] configs)
        {
            configs.Each(appConf =>
            {
                OnAppInitializing(appConf);
                if (!string.IsNullOrEmpty(appConf.AppInitializer))
                {
                    Type appInitializer = null;
                    if (!string.IsNullOrEmpty(appConf.AppInitializerAssemblyPath))
                    {
                        Assembly assembly = Assembly.LoadFrom(appConf.AppInitializerAssemblyPath);
                        appInitializer = assembly.GetType(appConf.AppInitializer);
                        if (appInitializer == null)
                        {
                            appInitializer = assembly.GetTypes().FirstOrDefault(t => t.AssemblyQualifiedName.Equals(appConf.AppInitializer));
                        }

                        if (appInitializer == null)
                        {
                            Args.Throw<InvalidOperationException>("The specified AppInitializer type ({0}) wasn't found in the specified assembly ({1})", appConf.AppInitializer, appConf.AppInitializerAssemblyPath);
                        }
                    }
                    else
                    {
                        appInitializer = Type.GetType(appConf.AppInitializer);
                        if (appInitializer == null)
                        {
                            Args.Throw<InvalidOperationException>("The specified AppInitializer type ({0}) wasn't found", appConf.AppInitializer);
                        }
                    }

                    IAppInitializer initializer = appInitializer.Construct<IAppInitializer>();
                    initializer.Subscribe(MainLogger);
                    initializer.Initialize(appConf);
                }
                OnAppInitialized(appConf);
            });
        }
        /// <summary>
        /// Initialize server level schemas
        /// </summary>
        protected virtual void InitializeCommonSchemas()
        {
            OnSchemasInitializing();
            SchemaInitializers.Each(schemaInitializer =>
            {
                OnSchemaInitializing(schemaInitializer);
                if (!schemaInitializer.Initialize(MainLogger, out Exception ex))
                {
                    MainLogger.AddEntry("An error occurred initializing schema ({0}): {1}", ex, schemaInitializer.SchemaName, ex.Message);
                }
                OnSchemaInitialized(schemaInitializer);
            });
            OnSchemasInitialized();
        }

        protected virtual void InitializeUserManagers()
        {
            ContentResponder.AppConfigs.Each(appConfig =>
            {
                try
                {
                    UserManager mgr = appConfig.GetUserManager();
                    mgr.ApplicationNameProvider = new BamApplicationNameProvider(appConfig);
                    AddAppService(appConfig.Name, mgr);
                }
                catch (Exception ex)
                {
                    MainLogger.AddEntry("An error occurred initializing user manager for app ({0}): {1}", ex, appConfig.Name, ex.Message);
                }
            });
        }

        protected virtual void RegisterWorkspaceDaos()
        {
            DirectoryInfo workspaceDir = new DirectoryInfo(Workspace);
            DaoResponder.RegisterCommonDaoFromDirectory(workspaceDir);
        }

        protected virtual void InitializeResponders()
        {
            foreach (IHttpResponder responder in _responders)
            {
                responder.Subscribe(MainLogger);
                responder.Initialize();
            }
        }

        /// <summary>
        /// Subscribe the specified logger to the events of the
        /// ContentResponder.  Will also subscribe to the DaoResponder
        /// if EnableDao is true and the ServiceProxyResponder if
        /// EnableServiceProxy is true.  Additionally, will subscribe to
        /// any other responders that have been added using AddResponder
        /// </summary>
        /// <param name="logger"></param>
        protected virtual void SubscribeResponders(ILogger logger)
        {
            foreach (IHttpResponder responder in _responders)
            {
                responder.Subscribe(logger);
            }
        }

        private HashSet<ILogger> _subscribers = new HashSet<ILogger>();
        private readonly object _subscriberLock = new object();
        public override ILogger[] Subscribers
        {
            get
            {
                _subscribers = _subscribers ?? new HashSet<ILogger>();
                lock (_subscriberLock)
                {
                    return _subscribers.ToArray();
                }
            }
        }

        public override bool IsSubscribed(ILogger logger)
        {
            lock (_subscriberLock)
            {
                return _subscribers.Contains(logger);
            }
        }

        /// <summary>
        /// Subscribe the specified logger to the 
        /// events of the current BamServer
        /// </summary>
        /// <param name="logger"></param>
        public override void Subscribe(ILogger logger)
        {
            if (!IsSubscribed(logger))
            {
                lock (_subscriberLock)
                {
                    _subscribers.Add(logger);
                }
                const string className = nameof(BamAppServer);

                this.Initializing += (s) =>
                {
                    logger.AddEntry("{0}::Initializ(ING)", className);
                };

                this.Initialized += (s) =>
                {
                    logger.AddEntry("{0}::Initializ(ED)", className);
                };

                this.LoadingConf += (s, c) =>
                {
                    logger.AddEntry("{0}::Load(ING) configuration, current config: \r\n{1}", className, c.PropertiesToString());
                };

                this.LoadedConf += (s, c) =>
                {
                    logger.AddEntry("{0}::Load(ED) configuration, current config: \r\n{1}", className, c.PropertiesToString());
                };

                this.SettingConf += (s, c) =>
                {
                    logger.AddEntry("{0}::Sett(ING) configuration, current config: \r\n{1}", className, c.PropertiesToString());
                };

                this.SettedConf += (s, c) =>
                {
                    logger.AddEntry("{0}::Sett(ED) configuration, current config: \r\n{1}", className, c.PropertiesToString());
                };

                this.SchemasInitializing += (s) =>
                {
                    logger.AddEntry("{0}::Initializ(ING) schemas", className);
                };

                this.SchemasInitialized += (s) =>
                {
                    logger.AddEntry("{0}::Initializ(ED) schemas", className);
                };

                this.Starting += (s) =>
                {
                    logger.AddEntry("{0}::Start(ING)", className);
                };

                this.Started += (s) =>
                {
                    logger.AddEntry("{0}::Start(ED)", className);
                };

                this.Stopping += (s) =>
                {
                    logger.AddEntry("{0}::stopping", className);
                };

                this.Stopped += (s) =>
                {
                    logger.AddEntry("{0}::stopped", className);
                };
            }
        }

        ILogger _logger;
        readonly object _loggerLock = new object();
        public ILogger MainLogger
        {
            get
            {
                return _loggerLock.DoubleCheckLock(ref _logger, () =>
                {
                    Log.Restart();
                    return Log.Default;
                });
            }
            set
            {
                _logger?.StopLoggingThread();

                _logger = value;
                _logger.RestartLoggingThread();
                if (IsRunning)
                {
                    Restart();
                }
            }
        }

        public ILogger[] AdditionalLoggers { get; set; }

        public HostBinding[] GetHostBindings()
        {
            BamConf serverConfig = GetCurrentConf(false);
            HashSet<HostBinding> results = new HashSet<HostBinding> {DefaultHostBinding};
            serverConfig.AppsToServe.Each(appConf => { appConf.Bindings.Each(hp => results.Add(hp)); });

            return results.ToArray();
        }

        HostBinding _defaultHostBinding;
        readonly object _defaultHostBindingLock = new object();
        public HostBinding DefaultHostBinding
        {
            get
            {
                return _defaultHostBindingLock.DoubleCheckLock(ref _defaultHostBinding, () => new HostBinding("localhost", 8080));
            }
            set => _defaultHostBinding = value;
        }

        // config values here to ensure proper sync
        public DaoConf[] DaoConfigs { get; set; }
        public ProxyAlias[] ProxyAliases { get; set; }
        public bool GenerateDao { get; set; }
        public bool UseCache { get; set; }
        public bool InitializeWebBooks { get; set; }
        public string DaoSearchPattern { get; set; }
        public ProcessModes[] ProcessModes { get; set; }
        public string ServiceSearchPattern { get; set; }
        public string MainLoggerName { get; set; }
        public string InitializeFileSystemFrom { get; set; }
        public string[] ServerEventListenerSearchPaths { get; set; }
        public string ServerEventListenerAssemblySearchPattern { get; set; }
        // -end config values

        string _contentRoot;
        public string ContentRoot
        {
            get => _contentRoot;
            set
            {
                _contentRoot = new Fs(value).Root;
                ContentResponder.BamConf = GetCurrentConf();
            }
        }

        public Fs ContentRootFs => new Fs(ContentRoot);

        protected void OnLoadingConf()
        {
            LoadingConf?.Invoke(this, GetCurrentConf());
        }

        protected void OnLoadedConf(BamConf conf)
        {
            LoadedConf?.Invoke(this, conf);
        }

        /// <summary>
        /// Loads the server configuration from either a json file, yaml file
        /// or the default config depending on which is found first in that 
        /// order.
        /// </summary>
        public BamConf LoadConf()
        {
            OnLoadingConf();
            BamConf conf = BamConf.Load(ContentRoot);
            SetConf(conf);
            OnLoadedConf(conf);
            return conf;
        }

        protected void OnCreatingApp(AppConf conf)
        {
            CreatingApp?.Invoke(this, conf);
        }

        protected void OnCreatedApp(AppConf conf)
        {
            CreatedApp?.Invoke(this, conf);
        }

        public AppContentResponder CreateApp(string appName, string defaultLayout = null, int port = 8080, bool ssl = false)
        {
            AppConf conf = new AppConf(appName, port, ssl);
            if (!string.IsNullOrEmpty(defaultLayout))
            {
                conf.DefaultLayout = defaultLayout;
            }
            OnCreatingApp(conf);

            AppContentResponder responder = new AppContentResponder(ContentResponder, conf)
            {
                Logger = MainLogger
            };
            responder.Initialize();

            OnCreatedApp(conf);
            return responder;
        }

        protected void OnSettingConf(BamConf conf)
        {
            SettingConf?.Invoke(this, conf);
        }

        protected void OnSettedConf(BamConf conf)
        {
            SettedConf?.Invoke(this, conf);
        }

        public void SetConf(BamConf conf)
        {
            OnSettingConf(conf);

            this.MainLogger = Log.Default = conf.GetMainLogger(out Type loggerType);
            this.MainLogger.RestartLoggingThread();
            if (!loggerType.Name.Equals(conf.MainLoggerName))
            {
                MainLogger.AddEntry("Configured MainLogger was ({0}) but the Logger found was ({1})", LogEventType.Warning, conf.MainLoggerName, loggerType.Name);
            }
            this.TryAddAdditionalLoggers(conf);
            conf.AppServer = this;

            DefaultConfiguration.CopyProperties(conf, this);
            SetWorkspace();

            OnSettedConf(conf);
        }
        
        protected void OnSavedConf(BamConf conf)
        {
            SavedConf?.Invoke(this, conf);
        }

        /// <summary>
        /// Saves the current configuration if the config 
        /// file doesn't currently exist
        /// </summary>
        /// <param name="format">The format to save the configuration in</param>
        /// <param name="overwrite">If true overwrite the existing cofig file</param>
        /// <returns>The BamConf</returns>
        public BamConf SaveConf(bool overwrite = false, ConfFormat format = ConfFormat.Json)
        {
            BamConf conf = GetCurrentConf();
            conf.Save(ContentRoot, overwrite, format);
            OnSavedConf(conf);
            return conf;
        }

        ContentResponder _contentResponder;
        /// <summary>
        /// The primary responder for all content files
        /// </summary>
        public ContentResponder ContentResponder
        {
            get
            {
                if(_contentResponder == null)
                {
                    return SetContentResponder();
                }
                return _contentResponder;
            }
        }


        DaoResponder _daoResponder;
        public DaoResponder DaoResponder
        {
            get
            {
                if (_daoResponder == null)
                {
                    SetDaoResponder();
                }
                return _daoResponder;
            }
        }

        ServiceProxyResponder _serviceProxyResponder;
        public ServiceProxyResponder ServiceProxyResponder
        {
            get
            {
                if (_serviceProxyResponder == null)
                {
                    SetServiceProxyResponder();
                }
                return _serviceProxyResponder;
            }
        }

        public void SubscribeToResponded<T>(ResponderEventHandler subscriber) where T : class, IHttpResponder
        {
            Responders.Each(r =>
            {
                if (r is T responder)
                {
                    responder.Responded += subscriber;
                }
            });
        }

        public void SubscribeToNotResponded<T>(ResponderEventHandler subscriber) where T : class, IHttpResponder
        {
            Responders.Each(r =>
            {
                if (r is T responder)
                {
                    responder.DidNotRespond += subscriber;
                }
            });
        }

        public void SubscribeToResponded(ResponderEventHandler subscriber)
        {
            Responders.Each(r => r.Responded += subscriber);
        }

        public void SubscribeToNotResponded(ResponderEventHandler subscriber)
        {
            Responders.Each(r => r.DidNotRespond += subscriber);
        }

        public void Start()
        {
            Start(false);
        }

        public void Start(bool usurpedKnownListeners = false)
        {
            Start(usurpedKnownListeners, new HostBinding[] { });
        }

        public void Start(bool usurpedKnownListeners, params HostBinding[] hostBindings)
        {
            if (!IsRunning)
            {
                SetWorkspace();
                ListenForDaoGenServices();

                Initialize();

                OnStarting();
                _server.Start(usurpedKnownListeners, hostBindings);
                IsRunning = true;
                OnStarted();
            }
        }

        public void Stop()
        {
            if (IsInitialized && IsRunning)
            {
                SaveConf();

                OnStopping();
                _server.Stop();
                IsRunning = false;
                OnStopped();
            }
        }
        public void TryStop()
        {
            try
            {
                Stop();
            }
            catch { }
        }

        public void Restart()
        {
            Stop();
            this.IsInitialized = false;
            Start();
        }

        public static BamAppServer Serve(string contentRoot)
        {
            BamConf conf = BamConf.Load(contentRoot);
            BamAppServer appServer = new BamAppServer(conf);
            appServer.Start();
            return appServer;
        }

        public Incubator CommonServiceProvider => ServiceProxyResponder.CommonServiceProvider;

        public Dictionary<string, Incubator> AppServiceProviders => ServiceProxyResponder.AppServiceProviders;

        public Dictionary<string, AppContentResponder> AppContentResponders => ContentResponder.AppContentResponders;

        public Dictionary<string, UserManager> AppUserManagers
        {
            get
            {
                Dictionary<string, UserManager> result = new Dictionary<string, UserManager>();
                AppServiceProviders.Keys.Each(appName =>
                {
                    result.Add(appName, AppServiceProviders[appName].Get<UserManager>());
                });
                return result;
            }
        }

        string _workspace;
        public string Workspace => ContentRootFs.CleanAppPath(_workspace);

        public void AddCommonDaoFromDirectory(DirectoryInfo daoDir)
        {
            DaoResponder.RegisterCommonDaoFromDirectory(daoDir);
        }

        public void AddAppDaoFromDirectory(string appName, DirectoryInfo daoDir)
        {
            DaoResponder.RegisterAppDaoFromDirectory(appName, daoDir);
        }

        /// <summary>
        /// Adds a service available to all applications served by this server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddCommonService<T>()
        {
            ServiceProxyResponder.AddCommonService<T>((T)typeof(T).Construct());
        }

        public void AddCommonService<T>(T instance)
        {
            ServiceProxyResponder.AddCommonService<T>(instance);
        }

        public T GetAppService<T>(string appName)
        {
            if (ServiceProxyResponder.AppServiceProviders.ContainsKey(appName))
            {
                return ServiceProxyResponder.AppServiceProviders[appName].Get<T>();
            }
            return default(T);
        }

        public void AddAppService<T>(string appName)
        {
            ServiceProxyResponder.AddAppService<T>(appName, (T)typeof(T).Construct());
        }

        public void AddAppService<T>(string appName, T instance)
        {
            ServiceProxyResponder.AddAppService<T>(appName, instance);
        }

        public void AddAppService<T>(string appName, Func<T> instanciator)
        {
            ServiceProxyResponder.AddAppService<T>(appName, instanciator);
        }

        public void AddAppService(string appName, Type type, Func<object> instanciator)
        {
            ServiceProxyResponder.AddAppService(appName, type, instanciator);
        }

        public void AddAppServices(string appName, Incubator incubator)
        {
            ServiceProxyResponder.AddAppServices(appName, incubator);
        }

        /// <summary>
        /// Add or update the app service using the specified instanciator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appName"></param>
        /// <param name="instanciator"></param>
        public void AddAppService<T>(string appName, Func<Type, T> instanciator)
        {
            ServiceProxyResponder.AddAppService<T>(appName, instanciator);
        }

        public void AddLogger(ILogger logger)
        {
            MultiTargetLogger mtl = new MultiTargetLogger();
            if (MainLogger != null)
            {
                if (MainLogger.GetType() == typeof(MultiTargetLogger))
                {
                    mtl = (MultiTargetLogger)MainLogger;
                }
                else
                {
                    mtl.AddLogger(MainLogger);
                }
            }

            mtl.AddLogger(logger);
            MainLogger = mtl;
        }

        /// <summary>
        /// Add an IResponder implementation to this
        /// request handler
        /// </summary>
        /// <param name="responder"></param>
        public void AddResponder(Responder responder)
        {
            if (!_respondersByName.ContainsKey(responder.ResponderName))
            {
                _respondersByName.AddMissing(responder.ResponderName, responder);
                _responders.Add(responder);
                ResponderAdded?.Invoke(this, responder);
            }
        }

        public void RemoveResponder(Responder responder)
        {
            if(responder == null)
            {
                return;
            }
            if (_responders.Contains(responder))
            {
                _responders.Remove(responder);
            }
            if (_respondersByName.ContainsKey(responder.ResponderName))
            {
                _respondersByName.Remove(responder.ResponderName);
            }
        }

        public IHttpResponder[] Responders => _responders.ToArray();

        Action<IHttpContext> _responderNotFoundHandler;
        readonly object _responderNotFoundHandlerLock = new object();
        /// <summary>
        /// Get or set the default handler used when no appropriate
        /// responder is found for a given request.  This is the 
        /// Action responsible for responding with a 404 status code
        /// and supplying any additional information to the client.
        /// </summary>
        public Action<IHttpContext> ResponderNotFoundHandler
        {
            get
            {
                return _responderNotFoundHandlerLock.DoubleCheckLock(ref _responderNotFoundHandler, () => HandleResponderNotFound);
            }
            set => _responderNotFoundHandler = value;
        }

        Action<IHttpContext, Exception> _exceptionHandler;
        readonly object _exceptionHandlerLock = new object();
        /// <summary>
        /// Get or set the default exception handler.  This is the
        /// Action responsible for responding with a 500 status code
        /// and supplying any additional information to the client
        /// pertaining to exceptions that may occur on the server.
        /// </summary>
        public Action<IHttpContext, Exception> ExceptionHandler
        {
            get
            {
                return _exceptionHandlerLock.DoubleCheckLock(ref _exceptionHandler, () => HandleException);
            }
            set => _exceptionHandler = value;
        }

        public Task HandleRequestAsync(IHttpContext context)
        {
            return Task.Run(() => HandleRequest(context));
        }

        public void HandleRequest(IHttpContext context)
        {
            IRequest request = context.Request;
            IResponse response = context.Response;

            ResponderList responder = new ResponderList(_conf, _responders);
            try
            {   
                if (!responder.Respond(context))
                {
                    NotResponded?.Invoke(this, request);
                    ResponderNotFoundHandler(context);
                }
                else
                {
                    TriggerResponseFlushing(response, responder.HandlingResponder);
                    FlushResponse(response);
                    TriggerResponseFlushed(request, responder.HandlingResponder);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler(context, ex);                
            }
        }

        public ITemplateManager GetAppTemplateRenderer(string appName)
        {
            Dictionary<string, AppContentResponder> container = ContentResponder.AppContentResponders;
            if (container.ContainsKey(appName))
            {
                return container[appName].AppTemplateManager;
            }
            else
            {
                MainLogger.AddEntry("Unable to retrieve AppDustRenderer for the specified app name: {0}", LogEventType.Warning, appName);
                return null;
            }
        }

        private static void FlushResponse(IResponse response)
        {
            response?.OutputStream?.Flush();
            response?.OutputStream?.Close();
        }

        private void TriggerResponseFlushed(IRequest request, IHttpResponder responder)
        {
            ResponseFlushed?.Invoke(this, responder, request);
        }

        private void TriggerResponseFlushing(IResponse response, IHttpResponder responder)
        {
            ResponseFlushing?.Invoke(this, responder, response);
        }

        BamConf _conf;
        readonly object _confLock = new object();
        /// <summary>
        /// Get a BamConf instance which represents the current
        /// state of the BamServer
        /// </summary>
        /// <returns></returns>
        public BamConf GetCurrentConf(bool reload = true)
        {
            lock (_confLock)
            {
                if (reload || _conf == null)
                {
                    BamConf conf = this.CopyAs<BamConf>();
                    conf.AppServer = this;
                    _conf = conf;
                }
            }
            return _conf;
        }

        protected void TryAddAdditionalLoggers(BamConf conf)
        {
            Type[] loggerTypes;
            ILogger[] loggers = new ILogger[] { };
            try
            {
                loggers = conf.GetAdditionalLoggers(out loggerTypes);
            }
            catch (Exception ex)
            {
                MainLogger.AddEntry("An error occurred getting additional loggers: {0}", ex, ex.Message);
            }

            loggers.Each(logger =>
            {
                try
                {
                    AddLogger(logger);
                }
                catch (Exception ex)
                {
                    MainLogger.AddEntry("An error occurred trying to add a logger: {0}", ex, ex.Message);
                }
            });

            AdditionalLoggers = loggers;
        }

        protected HttpServer HttpServer => _server;

        bool _enableDao;
        /// <summary>
        /// If true will cause the initialization of the 
        /// DaoResponder which processes all *.db.js
        /// and *.db.json files.  See http://breviteedocs.wordpress.com/dao/
        /// for information about the expected format 
        /// of a *.db.js file.  The format of *db.json 
        /// would be the json equivalent of the referenced
        /// database object (see link).  See
        /// Bam.Net.Data.Schema.DataTypes for valid
        /// data types.
        /// </summary>
        protected bool EnableDao
        {
            get => _enableDao;
            set
            {
                _enableDao = value;
                RemoveResponder(_daoResponder);
                if (_enableDao)
                {
                    SetDaoResponder();
                }
            }
        }

        bool _enableServiceProxy;
        /// <summary>
        /// If true will cause the initialization of the
        /// ServiceProxyResponder which will register
        /// all classes adorned with the Proxy attribute
        /// as service proxy executors
        /// </summary>
        protected bool EnableServiceProxy
        {
            get => _enableServiceProxy;
            set
            {
                _enableServiceProxy = value;
                if (_enableServiceProxy)
                {
                    SetServiceProxyResponder();
                }
                else
                {
                    RemoveResponder(_serviceProxyResponder);
                }
            }
        }

        protected void SetDaoResponder()
        {
            _daoResponder = new DaoResponder(GetCurrentConf(true), MainLogger);
            AddResponder(_daoResponder);
        }

        protected void SetServiceProxyResponder()
        {
            _serviceProxyResponder = new ServiceProxyResponder(GetCurrentConf(true), MainLogger)
            {
                ContentResponder = ContentResponder,
                ServiceCompilationExceptionReporter =
                {
                    Reporter = (o, args) =>
                    {
                        Args.ThrowIfNull(args, "args");
                        Args.ThrowIfNull(args.AppConf, "args.AppConf");
                        args.AppConf.AppRoot.Write("~/services/bin/logs.txt", args.Exception.GetMessageAndStackTrace());
                    }
                }
            };

            AddResponder(_serviceProxyResponder);
        }

        public event EventHandler FileUploading;
        public event EventHandler FileUploaded;

        protected ContentResponder SetContentResponder()
        {
            ContentResponder responder = new ContentResponder(GetCurrentConf(true), MainLogger);
            responder.FileUploading += (o, a) => FileUploading?.Invoke(o, a);
            responder.FileUploaded += (o, a) => FileUploaded?.Invoke(o, a);
            AddResponder(responder);
            _contentResponder = responder;
            return responder;
        }

        protected void OnStopping()
        {
            Stopping?.Invoke(this);
        }

        protected void OnStopped()
        {
            Stopped?.Invoke(this);
        }
        protected void OnStarting()
        {
            Starting?.Invoke(this);
        }

        protected void OnStarted()
        {
            Started?.Invoke(this);
        }

        protected internal bool IsRunning
        {
            get;
            private set;
        }

        private void HandlePostInitialization(BamAppServer appServer)
        {
            PostInitializationHandler = new PostServerInitializationHandler(); // TODO: get this from ServiceRegistry (DI)

            PostInitializationHandler.HandleInitialization(this);

            this.IsInitialized = true;
        }

        private void ConfigureHttpServer()
        {
            _server = new HttpServer(MainLogger)
            {
                HostBindings = GetHostBindings()
            };
            _server.PreProcessRequest += PreProcessRequest;
            _server.ProcessRequest += ProcessRequest;
            
            _subscribers.Each(l => _server.Subscribe(l));
        }
        
        private void ListenForDaoGenServices()
        {
            ServiceProxyResponder.CommonServiceAdded += (t, o) =>
            {
                if (o is IGeneratesDaoAssembly daoGen)
                {
                    daoGen.GenerateDaoAssemblySucceeded += (io, a) =>
                    {
                        GenerateDaoAssemblyEventArgs args = (GenerateDaoAssemblyEventArgs)a;
                        DaoResponder.RegisterCommonDaoFromDirectory(args.GeneratedAssemblyInfo.GetAssembly().GetFileInfo().Directory);
                    };
                }
            };
        }

        private void SetWorkspace()
        {
            _workspace = Path.Combine(ContentRoot, ServerWorkspace);
            if (!Directory.Exists(_workspace))
            {
                Directory.CreateDirectory(_workspace);
            }
            Directory.SetCurrentDirectory(_workspace);
        }
    }
}