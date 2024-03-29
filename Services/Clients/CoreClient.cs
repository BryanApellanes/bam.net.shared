﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bam.Net.Logging;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using System.Reflection;
using Bam.Net.Data.Repositories;
using Bam.Net.Data.SQLite;
using Bam.Net.CoreServices.ApplicationRegistration;
using Bam.Net.UserAccounts;
using Bam.Net.Configuration;
using Bam.Net.Web;
using Bam.Net.CoreServices;
using Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Repository;
using Bam.Net.CoreServices.ApplicationRegistration.Data;
using Bam.Net.CoreServices.Auth;
using System.Net.Http;
using Bam.Net.Server.ServiceProxy;

namespace Bam.Net.Services.Clients
{
    /// <summary>
    /// A client to the core bam service server.
    /// </summary>
    /// <seealso cref="Bam.Net.Logging.Loggable" />
    /// <seealso cref="Bam.Net.ServiceProxy.Encryption.IApiHmacKeyResolver" />
    /// <seealso cref="Bam.Net.ServiceProxy.Encryption.IApiHmacKeyProvider" />
    /// <seealso cref="Bam.Net.IApplicationNameProvider" />
    public partial class CoreClient: Loggable, IApiHmacKeyResolver, IApiHmacKeyProvider, IApplicationNameProvider
    {
        internal CoreClient(string organizationName, string applicationName, string workingDirectory = null, ILogger logger = null)
        {
            string hostName = "localhost";
            int port = 9100;
            SetMainProperties(organizationName, applicationName, hostName, port, workingDirectory, logger);
            SetLocalServiceProxies();
            SetApiHmacKeyResolvers();
            SetClientApplicationNameProvider();
            SetLocalProperties(organizationName, applicationName, hostName, port);
            WireInvocationEventHandlers();
        }

        public CoreClient(ILogger logger = null) : this(Organization.Public.Name, DefaultApplicationName, "heart.bamapps.net", 80, null, logger ?? Log.Default)
        { }

        /// <summary>
        /// Instanciate a new CoreClient
        /// </summary>
        /// <param name="organizationName">The name of your organization</param>
        /// <param name="applicationName">The name of your application</param>
        /// <param name="hostName">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="workingDirectory">The local working directory to place temporary files in</param>
        /// <param name="logger">The logger to use to log activity</param>
        public CoreClient(string organizationName, string applicationName, string hostName, int port, string workingDirectory = null, ILogger logger = null)
        {
            SetMainProperties(organizationName, applicationName, hostName, port, workingDirectory, logger);
            SetDownloadedServiceProxies();
            SetApiHmacKeyResolvers();
            SetClientApplicationNameProvider();
            SetLocalProperties(organizationName, applicationName, hostName, port);
            WireInvocationEventHandlers();
        }
        
        public CoreClient(string applicationName, string hostName, int port, string workingDirectory = null, ILogger logger = null)
            : this(Organization.Public.Name, applicationName, hostName, port, workingDirectory, logger)
        {
        }

        public CoreClient(string organizationName, string applicationName, string hostName, int port, ILogger logger = null) 
            : this(organizationName, applicationName, hostName, port, null, logger)
        { }

        public CoreClient(string hostName, int port, ILogger logger = null) : this(Organization.Public.Name, CoreServices.ApplicationRegistration.Data.Application.Unknown.Name, hostName, port, logger)
        { }

        /// <summary>
        /// Instantiate a CoreClient configured to consume the CoreHostName and CorePort values specified in the 
        /// default configuration file (app.config or web.config).  If no values are specified then bamapps.net:80
        /// is used.
        /// </summary>
        public CoreClient(): this(DefaultConfiguration.GetAppSetting("CoreHostName", "heart.bamapps.net"), DefaultConfiguration.GetAppSetting("CorePort", "80").ToInt())
        {
        }

        static CoreClient _local;
        static readonly object _localLock = new object();
        /// <summary>
        /// A CoreClient configured for localhost on port 9100
        /// </summary>
        public static CoreClient Local
        {
            get
            {
                return _localLock.DoubleCheckLock(ref _local, () => new CoreClient(Organization.Public.Name, DefaultApplicationName));
            }
        }

        static CoreClient _heart;
        static readonly object _heartLock = new object();
        public static CoreClient Heart
        {
            get
            {
                return _heartLock.DoubleCheckLock(ref _heart, () => new CoreClient("heart.bamapps.net", 80));
            }
        }

        public static string DefaultApplicationName
        {
            get
            {
                return $"{UserUtil.GetCurrentWindowsUser(true)}:{ProcessDescriptor.Current.FilePath}@{ProcessDescriptor.Current.MachineName}";
            }
        }

        public ProcessDescriptor ProcessDescriptor { get; private set; }

        /// <summary>
        /// The local instance of the ApplicationRegistryRepository
        /// </summary>
        public ApplicationRegistrationRepository LocalCoreRegistryRepository { get; set; }
        
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "{OrganizationName}:{ApplicationName} initializING")]
        public event EventHandler Initializing;

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "{OrganizationName}:{ApplicationName} initializED")]
        public event EventHandler Initialized;

        [Verbosity(VerbosityLevel.Warning, SenderMessageFormat = "{OrganizationName}:{ApplicationName} initialization failed: {Message}")]
        public event EventHandler InitializationFailed;

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "{OrganizationName}:{ApplicationName}: {ApiKeyFilePath} saved")]
        public event EventHandler ApiKeyFileSaved;
        
        public string Message { get; set; } // used by InitializationFailed event
        public string ApplicationName { get; set; }
        public string OrganizationName { get; set; }
        public string WorkspaceDirectory { get; internal set; }
        public string ApiKeyFilePath => Path.Combine(WorkspaceDirectory, HostName, Port.ToString(), $"{GetApplicationName()}.apikey");
        public ILogger Logger { get; set; }
        #region IApiKeyResolver
        public HashAlgorithms HashAlgorithm
        {
            get; set;
        }

        public IApiArgumentEncoder ApiArgumentEncoder { get; set; }

        public string GetHmac(string stringToHash)
        {
            ApiHmacKeyInfo keyInfo = GetApiHmacKeyInfo(this);
            return $"{keyInfo.ApiHmacKey}:{stringToHash}".HashHexString(HashAlgorithm);
        }

/*        public bool IsValidRequest(ServiceProxyInvocation request)
        {
            Args.ThrowIfNull(request, "request");
            string stringToHash = ApiArgumentEncoder.GetValidationString(request.ClassName, request.MethodName, ApiArgumentEncoder.ArgumentsToJsonArgsMember(request.Arguments));
            string hmac = request.Context.Request.Headers[Headers.Hmac];
            bool result = false;
            if (!string.IsNullOrEmpty(hmac))
            {
                result = IsValidHmac(stringToHash, hmac);
            }
            return result;
        }*/

        public bool IsValidHmac(string stringToHash, string token)
        {
            string checkToken = GetHmac(stringToHash);
            return token.Equals(checkToken);
        }

        #endregion
        [Verbosity(VerbosityLevel.Warning, SenderMessageFormat = "ApiKeyFile {ApiKeyFilePath} was not found")]
        public event EventHandler ApiKeyFileNotFound;

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Writing ApiKeyFile {ApiKeyFilePath}")]
        public event EventHandler WritingApiKeyFile;

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Wrote ApiKeyFile {ApiKeyFilePath}")]
        public event EventHandler WroteApiKeyFile;
        #region IApiKeyProvider

        ApiHmacKeyInfo _apiKeyInfo;
        public ApiHmacKeyInfo GetApiHmacKeyInfo(IApplicationNameProvider nameProvider)
        {
            if (_apiKeyInfo == null)
            {
                if (File.Exists(ApiKeyFilePath))
                {
                    _apiKeyInfo = ApiKeyFilePath.FromJsonFile<ApiHmacKeyInfo>();
                }
                else
                {
                    FireEvent(ApiKeyFileNotFound);
                    _apiKeyInfo = ApplicationRegistryService.GetClientApiKeyInfo();
                    _apiKeyInfo.ApplicationName = nameProvider.GetApplicationName();
                    FireEvent(WritingApiKeyFile);
                    EnsureApiKeyFileDirectory();
                    _apiKeyInfo.ToJsonFile(ApiKeyFilePath);
                    FireEvent(WroteApiKeyFile);
                }
            }
            return _apiKeyInfo;
        }

        public ApiHmacKeyInfo AddApiKey()
        {
            return ApplicationRegistryService.AddApiKey();
        }

        public ApiHmacKeyInfo SetActiveApiKeyIndex(int index)
        {
            return ApplicationRegistryService.SetActiveApiKeyIndex(index);
        }

        public string GetApplicationApiHmacKey(string applicationClientId, int index) // index ignored in this implementation //TODO: take into account the index
        {
            ApiHmacKeyInfo key = GetApiHmacKeyInfo(this);
            if (key.ApplicationClientId.Equals(applicationClientId))
            {
                return key.ApiHmacKey;
            }
            throw new NotSupportedException("Specified applicationClientId not supported");
        }

        public string GetApplicationClientId()
        {
            return GetApplicationClientId(this);
        }

        public string GetApplicationClientId(IApplicationNameProvider nameProvider)
        {
            ApiHmacKeyInfo key = GetApiHmacKeyInfo(this);
            if (key.ApplicationName.Equals(nameProvider.GetApplicationName()))
            {
                return key.ApplicationClientId;
            }
            throw new NotSupportedException("Specified applicationClientId not supported");
        }

        public string GetCurrentApiHmacKey()
        {
            ApiHmacKeyInfo key = GetApiHmacKeyInfo(this);
            return key.ApiHmacKey;
        }
        #endregion

        #region IApplicationNameProvider

        public string GetApplicationName()
        {
            string appName = ApplicationName;
            if (string.IsNullOrEmpty(appName))
            {
                Logger.AddEntry("ApplicationName not specified: {0}", LogEventType.Warning, Assembly.GetEntryAssembly().GetFilePath());
            }
            return appName.Or($"{nameof(CoreClient)}.ApplicationName.Unspecified");
        }
        #endregion
        
        public SignUpResponse SignUp(string emailAddress, string password, string userName = null)
        {
            userName = userName ?? emailAddress;
            return UserRegistryService.SignUp(emailAddress, userName, password.Sha1(), true);
        }

        public CoreServices.ApplicationRegistration.Data.Application RegisterApplication(string applicationName)
        {
            CoreServiceResponse response = ApplicationRegistryService.RegisterApplication(applicationName);
            if (response.Success)
            {
                return ApplicationRegistryService.ApplicationRegistrationRepository.OneApplicationWhere(a => a.Name == applicationName);
            }
            throw new ApplicationException(response.Message);
        }

        public ApiHmacKeyInfo GetCurrentApplicationApiKeyInfo()
        {
            RegisterApplicationProcess();
            return ApiKeyFilePath.FromJsonFile<ApiHmacKeyInfo>();
        }

        public LoginResponse Login(string userName, string passHash)
        {
            return UserRegistryService.Login(userName, passHash);
        }

        readonly object _registerLock = new object();
        /// <summary>
        /// Register the current application and ensure that the local ApiKeyInfo is set and 
        /// written to ApiKeyFilePath
        /// </summary>
        /// <returns></returns>
        protected internal bool RegisterApplicationProcess()
        {
            lock (_registerLock)
            {
                if (!IsInitialized)
                {
                    FireEvent(Initializing);
                    CoreServiceResponse response = ApplicationRegistryService.RegisterApplicationProcess(ProcessDescriptor);
                    ApplicationRegistrationResult appRegistrationResult = response.Data.FromJObject<ApplicationRegistrationResult>();
                    if (response.Success)
                    {
                        IsInitialized = true;
                        FireEvent(Initialized);
                        ApiHmacKeyInfo keyInfo = new ApiHmacKeyInfo
                        {
                            ApiHmacKey = appRegistrationResult.ApiKey,
                            ApplicationClientId = appRegistrationResult.ClientId,
                            ApplicationName = GetApplicationName()
                        };
                        EnsureApiKeyFileDirectory();
                        keyInfo.ToJsonFile(ApiKeyFilePath);
                        FireEvent(ApiKeyFileSaved);
                    }
                    else
                    {
                        Message = response.Message;
                        FireEvent(InitializationFailed);
                    }
                }
                return IsInitialized;
            }
        }
        
        public event EventHandler OAuthSettingsNotFound;
        public event EventHandler OAuthSettingsWritten;
        public event EventHandler OAuthSettingsLoaded;

        public SupportedAuthProviders GetSupportedOAuthProviders()
        {
            string settingsPath = SupportedAuthProviders.GetSettingsPath(this);
            SupportedAuthProviders providers = new SupportedAuthProviders();
            if (!File.Exists(settingsPath))
            {
                FireEvent(OAuthSettingsNotFound);
                CoreServiceResponse response = AuthSettingsService.GetClientSettings(true);
                if (!response.Success)
                {
                    throw new ApplicationException(response.Message);
                }
                string json = response.Data.ToString();
                AuthClientSettings[] settings = json.FromJson<AuthClientSettings[]>();                
                foreach(AuthClientSettings setting in settings)
                {
                    providers.AddProvider(setting.CopyAs<AuthProviderInfo>());
                }
                providers.Save(settingsPath);
                FireEvent(OAuthSettingsWritten);
            }
            else
            {
                providers = SupportedAuthProviders.LoadFrom(settingsPath);
                FireEvent(OAuthSettingsLoaded);
            }

            return providers;
        }

        /// <summary>
        /// Register this client machine/process with the remote host
        /// </summary>
        /// <returns></returns>
        public CoreServiceResponse RegisterClient()
        {
            Client client = Client.Of(LocalCoreRegistryRepository, ApplicationName, HostName, Port);            
            CoreServiceResponse registrationResponse = ApplicationRegistryService.RegisterClient(client);
            if (registrationResponse == null || !registrationResponse.Success)
            {
                throw new ClientRegistrationFailedException(registrationResponse);
            }
            return registrationResponse;
        }

        /// <summary>
        /// Connect to the host specified by this client.  This entails
        /// "logging in" the current machine to the core services on the
        /// remote host
        /// </summary>
        /// <returns></returns>
        public CoreServiceResponse Connect()
        {
            Client current = Client.Of(LocalCoreRegistryRepository, ApplicationName, HostName, Port);
            List<CoreServiceResponse> responses = new List<CoreServiceResponse>();
            foreach(ProxyableService svc in ServiceClients)
            {
                responses.Add(svc.ConnectClient(current).CopyAs<CoreServiceResponse>());
            }
            return new CoreServiceResponse { Data = responses, Success = !responses.Where(r => !r.Success).Any() };
        }

        public T GetProxy<T>()
        {
            return ProxyFactory.GetProxy<T>(HostName, Port, new HashSet<Assembly>());
        }

        public bool UseServiceSubdomains
        {
            get => ProxyFactory.MungeHostNames;
            set => ProxyFactory.MungeHostNames = value;
        }

        public List<LogEntry> GetLogEntries(DateTime from, DateTime to)
        {
            return SystemLogReaderService.GetLogEntries(from, to);
        }

        protected ProxyFactory ProxyFactory { get; set; }
        protected bool IsInitialized { get; set; }

        /// <summary>
        /// The hostname of the server this client
        /// is a client of
        /// </summary>
        public string HostName { get; private set; }
        
        /// <summary>
        /// The port that the server is listening on
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Saves the proxy assemblies to /bam/proxies.
        /// </summary>
        public void SaveProxyAssemblies(string directory = null)
        {
            DirectoryInfo dir = new DirectoryInfo(directory ?? SystemPaths.Current.Proxies);
            if (!dir.Exists)
            {
                dir.Create();
            }
            foreach(ProxyableService svc in ServiceClients)
            {
                try
                {
                    FileInfo assembly = svc.GetType().Assembly.GetFileInfo();                    
                    assembly.CopyTo(Path.Combine(dir.FullName, assembly.Name), true);
                }
                catch (Exception ex)
                {
                    Logger.AddEntry("Exception saving service proxy ({0}): {1}", ex, svc?.GetType().Name ?? "null", ex.Message);
                }
            }
        }

        public void SaveProxySource(string directory = null)
        {
            DirectoryInfo dir = new DirectoryInfo(directory ?? "\\bam\\src\\_gen");
            foreach(Type type in ServiceClientTypes)
            {
                string fileName = $"{type.Name}_{ProxyFactory.DefaultSettings.Protocol.ToString()}_{HostName}_{Port}_Proxy.cs";
                FileInfo sourceFile = new FileInfo(Path.Combine(dir.FullName, fileName));
                ProxyFactory.GetProxySource(type, HostName, Port).SafeWriteToFile(sourceFile.FullName, true);
            }
        }
        
        public UserRegistryService UserRegistryService { get; set; }
        protected internal ApplicationRegistryService ApplicationRegistryService { get; set; }
        protected internal RoleService RoleService { get; set; }
        protected internal AuthService AuthService { get; set; }
        protected internal ConfigurationService ConfigurationService { get; set; }
        protected internal SystemLoggerService LoggerService { get; set; }
        protected internal DiagnosticService DiagnosticService { get; set; }
        protected internal ServiceRegistryService ServiceRegistryService { get; set; }
        protected internal SystemLogReaderService SystemLogReaderService { get; set; }
        protected internal AuthSettingsService AuthSettingsService { get; set; }
        protected internal ProxyAssemblyGeneratorService ProxyAssemblyGeneratorService { get; set; }

        /// <summary>
        /// Each of the Core service proxies
        /// </summary>
        protected internal IEnumerable<ProxyableService> ServiceClients
        {
            get
            {
                yield return UserRegistryService;
                yield return ApplicationRegistryService;                
                yield return ConfigurationService;
                yield return LoggerService;
                yield return RoleService;
                yield return DiagnosticService;
                yield return ServiceRegistryService;
                yield return SystemLogReaderService;
                yield return AuthService;
                yield return AuthSettingsService;
                yield return ProxyAssemblyGeneratorService;
            }
        }

        protected internal IEnumerable<Type> ServiceClientTypes
        {
            get
            {
                yield return typeof(UserRegistryService);
                yield return typeof(ApplicationRegistryService);
                yield return typeof(ConfigurationService);
                yield return typeof(SystemLoggerService);
                yield return typeof(RoleService);
                yield return typeof(DiagnosticService);
                yield return typeof(ServiceRegistryService);
                yield return typeof(SystemLogReaderService);
                yield return typeof(AuthService);
                yield return typeof(AuthSettingsService);
                yield return typeof(ProxyAssemblyGeneratorService);
            }
        }

        private void SetLocalProperties(string organizationName, string applicationName, string hostName, int port)
        {
            LocalCoreRegistryRepository = new ApplicationRegistrationRepository()
            {
                Database = new SQLiteDatabase(WorkspaceDirectory, nameof(CoreClient))
            };
            CoreServiceRegistryContainer.GetServiceRegistry().Get<IStorableTypesProvider>().AddTypes(LocalCoreRegistryRepository);
            ProcessDescriptor = ProcessDescriptor.ForApplicationRegistration(LocalCoreRegistryRepository, hostName, port, applicationName, organizationName);
        }

        private void SetMainProperties(string organizationName, string applicationName, string hostName, int port, string workingDirectory, ILogger logger)
        {
            OrganizationName = organizationName;
            ApplicationName = applicationName;
            HostName = hostName;
            Port = port;
            WorkspaceDirectory = workingDirectory ?? DataProvider.Current.GetWorkspaceDirectory(typeof(CoreClient)).FullName;
            HashAlgorithm = HashAlgorithms.SHA256;
            Logger = logger ?? Log.Default;
            ProxyFactory = new ProxyFactory(WorkspaceDirectory, Logger);
            ProxyFactory.AssemblyGenerated += (o, args) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        FileInfo assemblyFile = args.Assembly.GetFileInfo();
                        FileInfo destinationFile = new FileInfo(Path.Combine(SystemPaths.Current.Proxies, assemblyFile.Name));
                        if (!destinationFile.Directory.Exists)
                        {
                            destinationFile.Directory.Create();
                        }
                        assemblyFile.CopyTo(destinationFile.FullName);
                    }
                    catch(Exception ex)
                    {
                        Logger.AddEntry("Error copying generated proxy assembly: {0}", ex, ex.Message);
                    }
                });
            };
        }

        private void SetLocalServiceProxies()
        {
            ApplicationRegistryService = ProxyFactory.GetProxy<ApplicationRegistryService>();
            ConfigurationService = ProxyFactory.GetProxy<ConfigurationService>();
            DiagnosticService = ProxyFactory.GetProxy<DiagnosticService>();
            LoggerService = ProxyFactory.GetProxy<SystemLoggerService>();
            UserRegistryService = ProxyFactory.GetProxy<UserRegistryService>();
            RoleService = ProxyFactory.GetProxy<RoleService>();
            AuthService = ProxyFactory.GetProxy<AuthService>();
            ServiceRegistryService = ProxyFactory.GetProxy<ServiceRegistryService>();
            SystemLogReaderService = ProxyFactory.GetProxy<SystemLogReaderService>();
            AuthSettingsService = ProxyFactory.GetProxy<AuthSettingsService>();
            ProxyAssemblyGeneratorService = ProxyFactory.GetProxy<ProxyAssemblyGeneratorService>();
        }

        private void WireInvocationEventHandlers()
        {
            foreach(ProxyableService service in ServiceClients)
            {
                ServiceProxyClient client = service.Property<ServiceProxyClient>("Client");
                client.InvocationException += (o, a) => OnInvocationException(o, a);
                client.InvocationComplete += (o, a) => OnInvocation(o, a);
            }
        }
        public event EventHandler InvocationExceptionThrown; 
        public event EventHandler MethodInvoked;
        protected virtual void OnInvocationException(object sender, ServiceProxyInvocationRequestEventArgs args)
        {
            FireEvent(InvocationExceptionThrown, sender, args);
        }

        protected virtual void OnInvocation(object sender, ServiceProxyInvocationRequestEventArgs args)
        {
            FireEvent(MethodInvoked, sender, args);
        }

        private void EnsureApiKeyFileDirectory()
        {
            FileInfo apiKeyFile = new FileInfo(ApiKeyFilePath);
            if (!apiKeyFile.Directory.Exists)
            {
                apiKeyFile.Directory.Create();
            }
        }

        private void SetApiHmacKeyResolvers()
        {
            SetProperty("ApiHmacKeyResolver");
        }

        private void SetClientApplicationNameProvider()
        {
            SetProperty("ClientApplicationNameProvider");
        }

        private void SetProperty(string propertyName)
        {
            ApplicationRegistryService.Property(propertyName, this);
            ConfigurationService.Property(propertyName, this);
            DiagnosticService.Property(propertyName, this);
            LoggerService.Property(propertyName, this);
            UserRegistryService.Property(propertyName, this);
            RoleService.Property(propertyName, this);
            SystemLogReaderService.Property(propertyName, this);
        }
    }
}
