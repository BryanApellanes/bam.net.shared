﻿using System;
using System.Collections.Generic;
using Bam.Net.Incubation;
using Bam.Net.Messaging;
using Bam.Net.ServiceProxy;
using Bam.Net.UserAccounts;
using System.IO;
using Bam.Net.Data.Repositories;
using Bam.Net.Server;
using Bam.Net.Logging;
using Bam.Net.Data;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.CoreServices.Files;
using Bam.Net.CoreServices.AssemblyManagement.Data.Dao.Repository;
using Bam.Net.CoreServices.ServiceRegistration.Data.Dao.Repository;
using Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Repository;

namespace Bam.Net.CoreServices
{
    /// <summary>
    /// Core application registry container for applications running locally.
    /// </summary>
    [ServiceRegistryContainer]
    public static class CoreServiceRegistryContainer
    {
        public const string RegistryName = "CoreServiceRegistry";
        static readonly object _coreIncubatorLock = new object();
        static ServiceRegistry _coreServiceRegistry;

        static readonly Dictionary<ProcessModes, Func<ServiceRegistry>> _factories;
        static CoreServiceRegistryContainer()
        {
            ConfigureDev = (sr) => sr;
            ConfigureTest = (sr) => sr;
            ConfigureProd = (sr) => sr;
            _factories = new Dictionary<ProcessModes, Func<ServiceRegistry>>
            {
                { ProcessModes.Dev, Dev },
                { ProcessModes.Test, Test },
                { ProcessModes.Prod, Prod }
            };
        }

        [ServiceRegistryLoader(RegistryName)]
        public static ServiceRegistry GetServiceRegistry()
        {
            return _coreIncubatorLock.DoubleCheckLock(ref _coreServiceRegistry, _factories[ProcessMode.Current.Mode]);
        }

        static ServiceRegistry _instance;
        static readonly object _instanceLock = new object();
        public static ServiceRegistry Instance
        {
            get
            {
                return _instanceLock.DoubleCheckLock(ref _instance, Create);
            }
        }

        /// <summary>
        /// Gets or sets the configure dev function.  Used to further configure the service
        /// registry when in Dev mode.
        /// </summary>
        public static Func<ServiceRegistry, ServiceRegistry> ConfigureDev { get; set; }

        /// <summary>
        /// Gets or sets the configure test function.  Used to further configure the service
        /// registry when in Test mode.
        /// </summary>
        public static Func<ServiceRegistry, ServiceRegistry> ConfigureTest { get; set; }

        /// <summary>
        /// Gets or sets the configure prod function.  Used to further configure the service
        /// registry when in Prod mode.
        /// </summary>
        public static Func<ServiceRegistry, ServiceRegistry> ConfigureProd { get; set; }
                
        public static ServiceRegistry Dev()
        {
            return ConfigureDev(Create());
        }

        public static ServiceRegistry Test()
        {
            return ConfigureTest(Create());
        }

        public static ServiceRegistry Prod()
        {
            return ConfigureProd(Create());
        }
        // --

        public static ServiceRegistry Create()
        {
            DataProvider dataSettings = DataProvider.Current;
            string databasesPath = dataSettings.GetSysDatabaseDirectory().FullName;
            string userDatabasesPath = Path.Combine(databasesPath, "UserDbs");

            AppConf conf = new AppConf(BamConf.Load(ServiceConfig.ContentRoot), ServiceConfig.ProcessName.Or(RegistryName));
            UserManager userMgr = conf.UserManagerConfig.Create();
            DaoUserResolver userResolver = new DaoUserResolver();
            DaoRoleResolver roleResolver = new DaoRoleResolver();
            SQLiteDatabaseProvider dbProvider = new SQLiteDatabaseProvider(databasesPath, Log.Default);
            ApplicationRegistrationRepository applicationRegistrationRepository = new ApplicationRegistrationRepository();
            dbProvider.SetDatabases(applicationRegistrationRepository);
            dbProvider.SetDatabases(userMgr);
            userMgr.Database.TryEnsureSchema(typeof(UserAccounts.Data.User), Log.Default);
            userResolver.Database = userMgr.Database;
            roleResolver.Database = userMgr.Database;

            ServiceRegistryRepository serviceRegistryRepo = new ServiceRegistryRepository();
            serviceRegistryRepo.Database = dataSettings.GetSysDatabaseFor(serviceRegistryRepo);
            serviceRegistryRepo.EnsureDaoAssemblyAndSchema();

            DaoRoleProvider daoRoleProvider = new DaoRoleProvider(userMgr.Database);
            RoleService coreRoleService = new RoleService(daoRoleProvider, conf);
            AssemblyManagementRepository assSvcRepo = new AssemblyManagementRepository();
            assSvcRepo.Database = dataSettings.GetSysDatabaseFor(assSvcRepo);
            assSvcRepo.EnsureDaoAssemblyAndSchema();

            ConfigurationService configSvc = new ConfigurationService(applicationRegistrationRepository, conf, userDatabasesPath);
            CompositeRepository compositeRepo = new CompositeRepository(applicationRegistrationRepository);
            SystemLoggerService loggerSvc = new SystemLoggerService(conf);
            dbProvider.SetDatabases(loggerSvc);
            loggerSvc.SetLogger();

            ServiceRegistry reg = (ServiceRegistry)(new ServiceRegistry())
                .ForCtor<ConfigurationService>("databaseRoot").Use(userDatabasesPath)
                .ForCtor<ConfigurationService>("conf").Use(conf)
                .ForCtor<ConfigurationService>("applicationRegistrationRepository").Use(applicationRegistrationRepository)
                .For<ILogger>().Use(Log.Default)
                .For<IRepository>().Use(applicationRegistrationRepository)
                .For<DaoRepository>().Use(applicationRegistrationRepository)
                .For<ApplicationRegistrationRepository>().Use(applicationRegistrationRepository)
                .For<AppConf>().Use(conf)
                .For<IDatabaseProvider>().Use(dbProvider)
                .For<IUserManager>().Use(userMgr)
                .For<UserManager>().Use(userMgr)
                .For<IUserResolver>().Use(userResolver)
                .For<IDaoUserResolver>().Use(userResolver)
                .For<DaoUserResolver>().Use(userResolver)
                .For<IRoleResolver>().Use(roleResolver)
                .For<DaoRoleResolver>().Use(roleResolver)
                .For<IRoleProvider>().Use(coreRoleService)
                .For<RoleService>().Use(coreRoleService)
                .For<EmailComposer>().Use(userMgr.EmailComposer)
                .For<IApplicationNameProvider>().Use<ApplicationRegistryService>()
                .For<ApplicationRegistryService>().Use<ApplicationRegistryService>()
                .For<IApiHmacKeyResolver>().Use<ApplicationRegistryService>()
                .For<ISmtpSettingsProvider>().Use(userMgr)
                .For<UserRegistryService>().Use<UserRegistryService>()
                .For<ConfigurationService>().Use(configSvc)
                .For<IStorableTypesProvider>().Use<NamespaceRepositoryStorableTypesProvider>()
                .For<FileService>().Use<FileService>()
                .For<IFileService>().Use<FileService>()
                .For<AssemblyManagementRepository>().Use(assSvcRepo)
                .For<IAssemblyService>().Use<AssemblyService>()
                .For<ServiceRegistryRepository>().Use(serviceRegistryRepo)
                .For<ServiceRegistryService>().Use<ServiceRegistryService>()
                .For<AuthService>().Use<AuthService>()
                .For<ILog>().Use(loggerSvc)
                .For<SystemLoggerService>().Use(loggerSvc)
                .For<IDataDirectoryProvider>().Use(DataProvider.Current)
                .For<DataProvider>().Use(DataProvider.Current)
                .For<IApplicationNameResolver>().Use<ClientApplicationNameResolver>()
                .For<ClientApplicationNameResolver>().Use<ClientApplicationNameResolver>()
                .For<SmtpSettingsProvider>().Use(DataProviderSmtpSettingsProvider.Default)
                .For<NotificationService>().Use<NotificationService>()
                .For<ILogReader>().Use<SystemLogReaderService>()
                .For<SystemLogReaderService>().Use<SystemLogReaderService>()
                .For<CredentialManagementService>().Use<CredentialManagementService>()
                .For<AuthSettingsService>().Use<AuthSettingsService>()
                .For<ProxyAssemblyGeneratorService>().Use<ProxyAssemblyGeneratorService>();

            reg.For<ServiceRegistry>().Use(reg)
                .For<DiagnosticService>().Use<DiagnosticService>();

            reg.SetProperties(userMgr);
            userMgr.ServiceProvider = reg;

            reg.For<CompositeRepository>().Use(() =>
            {
                compositeRepo.AddTypes(reg.Get<IStorableTypesProvider>().GetTypes());
                return compositeRepo;
            });

            ServiceProxySystem.UserResolvers.Clear();
            ServiceProxySystem.RoleResolvers.Clear();
            ServiceProxySystem.UserResolvers.AddResolver(userResolver);
            ServiceProxySystem.RoleResolvers.AddResolver(roleResolver);
            reg.Name = RegistryName;
            return reg;
        }
    }
}
