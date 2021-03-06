/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Bam.Net;
using Bam.Net.Web;
using Bam.Net.Data;
using Bam.Net.Data.Schema;
using Bam.Net.Configuration;
using Bam.Net.Logging;
using Bam.Net.Incubation;
using Bam.Net.Javascript;
using Bam.Net.ServiceProxy;
using Bam.Net.Server.Renderers;
using Bam.Net.Server;
using Yahoo.Yui.Compressor;
using NCuid;

using System.Reflection;
using Bam.Net.Data.Qi;
using System.Collections;
using Bam.Net.Data.Repositories;

namespace Bam.Net.Server
{
    /// <summary>
    /// The responder responsible for generating dynamic 
    /// proxy javascripts that enable client side code to
    /// execute server side .Net Dao methods over Ajax.
    /// </summary>
    public class DaoResponder : Responder, IInitialize<DaoResponder>
    {
        const string ViewsRelativePath = "~/views";
        Dictionary<string, Func<string, bool, string>> _dynamicResponders;

        public DaoResponder(BamConf conf)
            : base(conf)
        {
            Init();
        }

        public DaoResponder(BamConf conf, ILogger logger)
            : base(conf, logger)
        {
            Init();
        }

        private void Init()
        {
            _dynamicResponders = new Dictionary<string, Func<string, bool, string>>
            {
                { "proxies", Proxies },
                { "ctors", Ctors },
                { "templates", Templates }
            };
            Dao.BeforeWriteCommitAny += (db, dao) =>
            {
                dao.PropertyIfNullOrBlank("Created", DateTime.UtcNow, false);
                dao.PropertyIfNullOrBlank("Uuid", Guid.NewGuid().ToString(), false);
                dao.PropertyIfNullOrBlank("Cuid", Cuid.Generate(), false);
                dao.Property("Modified", DateTime.UtcNow, false);
            };
        }

        Dictionary<string, string> _compiledTemplates;
        object _compiledTemplatesLock = new object();
        public string Templates(string appName, bool min = false)
        {
            string result = string.Empty;

            if (_compiledTemplates == null)
            {
                _compiledTemplates = new Dictionary<string, string>();
            }

            lock (_compiledTemplatesLock)
            {
                if (_compiledTemplates.ContainsKey(appName))
                {
                    result = _compiledTemplates[appName];
                }
                else
                {
                    // templates are in ~s:/dao/dust and ~a:/dao/dust
                    DirectoryInfo commonTemplateDir = new DirectoryInfo(ServerRoot.GetAbsolutePath(ViewsRelativePath));
                    Fs appFs = BamConf.AppFs(appName); //AppFs(appName);
                    DirectoryInfo appTemplateDir = new DirectoryInfo(appFs.GetAbsolutePath(ViewsRelativePath));

                    StringBuilder tmp = new StringBuilder();
                    tmp.AppendLine(DustScript.CompileTemplates(commonTemplateDir));
                    tmp.AppendLine(DustScript.CompileTemplates(appTemplateDir));
                    _compiledTemplates[appName] = tmp.ToString();
                    result = _compiledTemplates[appName];
                }
            }

            return result;
        }

        public string Ctors(string appName, bool min = false)
        {
            StringBuilder result = new StringBuilder();
            CommonDaoProxyRegistrations.Keys.Each(cx =>
            {
                string ctorScript = min ? CommonDaoProxyRegistrations[cx].MinCtors.ToString() : CommonDaoProxyRegistrations[cx].Ctors.ToString();
                result.AppendLine(";\r\n");
                result.AppendLine(ctorScript);
            });

            if (AppDaoProxyRegistrations.ContainsKey(appName))
            {
                AppDaoProxyRegistrations[appName].Each(reg =>
                {
                    string ctorScript = min ? reg.MinCtors.ToString() : reg.Ctors.ToString();
                    result.AppendLine(";\r\n");
                    result.AppendLine(ctorScript);
                });
            }

            return result.ToString();
        }

        public string Proxies(string appName, bool min = false)
        {
            StringBuilder result = new StringBuilder();
            CommonDaoProxyRegistrations.Keys.Each(cx =>
            {
                string ctorScript = min ? CommonDaoProxyRegistrations[cx].MinProxies.ToString() : CommonDaoProxyRegistrations[cx].Proxies.ToString();
                result.AppendLine(";\r\n");
                result.AppendLine(ctorScript);
            });

            if (AppDaoProxyRegistrations.ContainsKey(appName))
            {
                AppDaoProxyRegistrations[appName].Each(reg =>
                {
                    string ctorScript = min ? reg.MinProxies.ToString() : reg.Proxies.ToString();
                    result.AppendLine(";\r\n");
                    result.AppendLine(ctorScript);
                });
            }

            return result.ToString();
        }

        #region IResponder Members

        public override bool TryRespond(IHttpContext context)
        {
            if (!this.IsInitialized)
            {
                Initialize();
            }

            IRequest request = context.Request;
            IResponse response = context.Response;
            bool handled = false;
            string path = request.Url.AbsolutePath;
            string appName = ApplicationNameResolver.ResolveApplicationName(context);
            string[] chunks = path.DelimitSplit("/");

            HttpArgs queryString = new HttpArgs(request.Url.Query);
            bool min = !string.IsNullOrEmpty(queryString["min"]);

            if (chunks[0].ToLowerInvariant().Equals("dao") && chunks.Length > 1)
            {
                string method = chunks[1];
                if (_dynamicResponders.ContainsKey(method))
                {
                    response.ContentType = GetContentTypeForExtension(".js");
                    string script = _dynamicResponders[method](appName, min);
                    WriteResponse(response, script);
                    handled = true;
                    OnResponded(context);
                }
                else
                {
                    handled = TryExecuteCrudRequest(chunks, context, handled, appName);
                    OnDidNotRespond(context);
                }
            }

            return handled;
        }

        private bool TryExecuteCrudRequest(string[] chunks, IHttpContext context, bool handled, string appName)
        {
            IRequest request = context.Request;
            IResponse response = context.Response;
            string connectionName;
            string methodName;
            string daoName;
            GetDaoInfo(chunks, out connectionName, out methodName, out daoName);
            DaoProxyRegistration daoProxyReg = null;
            connectionName = connectionName.ToLowerInvariant();
            if (CommonDaoProxyRegistrations.ContainsKey(connectionName))
            {
                daoProxyReg = CommonDaoProxyRegistrations[connectionName];
            }
            else if (AppDaoProxyRegistrations.ContainsKey(appName))
            {
                daoProxyReg = AppDaoProxyRegistrations[appName].FirstOrDefault(dpr => dpr.ContextName.Equals(connectionName));
            }

            if (daoProxyReg != null)
            {
                Type daoType = daoProxyReg.ServiceProvider[daoName];
                if (daoType != null)
                {
                    if (daoProxyReg.Database == null)
                    {
                        RegisterDatabase(daoProxyReg);
                    }

                    DaoCrudResponseProvider responseProvider = new DaoCrudResponseProvider(daoProxyReg, context);
                    CrudResponse crudResponse = responseProvider.Execute();
                    if (crudResponse.Success)
                    {
                        handled = true;
                        WriteResponse(response, crudResponse.ToJson());
                    }
                }
            }
            return handled;
        }

        private static void GetDaoInfo(string[] chunks, out string connectionName, out string methodName, out string daoName)
        {
            connectionName = chunks[1];
            methodName = chunks[2];
            daoName = chunks[3];
        }

        #endregion

        private string GetRequestBody(IRequest request)
        {
            string result = string.Empty;
            using (StreamReader sr = new StreamReader(request.InputStream))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        public bool IsInitialized
        {
            get;
            set;
        }

        List<ILogger> _subscribers = new List<ILogger>();
        object _subscriberLock = new object();
        public ILogger[] Subscribers
        {
            get
            {
                if (_subscribers == null)
                {
                    _subscribers = new List<ILogger>();
                }
                lock (_subscriberLock)
                {
                    return _subscribers.ToArray();
                }
            }
        }

        public bool IsSubscribed(ILogger logger)
        {
            lock (_subscriberLock)
            {
                return _subscribers.Contains(logger);
            }
        }
        public void Subscribe(ILogger logger)
        {
            if (!IsSubscribed(logger))
            {
                this.Logger = logger;
                lock (_subscriberLock)
                {
                    _subscribers.Add(logger);
                }

                string className = typeof(DaoResponder).Name;
                this.Initializing += (d) =>
                {
                    logger.AddEntry("{0}::Initializ(ING)", className);
                };
                this.Initialized += (d) =>
                {
                    logger.AddEntry("{0}::Initializ(ED)", className);
                };
                this.GeneratingCommonDao += (dbJs, daoBin) =>
                {
                    StringBuilder format = new StringBuilder();
                    format.AppendLine("{0}::Generat(ING) dao classes");
                    format.AppendLine("\t{0}::DbJSFile::{1}");
                    format.AppendLine("\t{0}::DaoBin::{2}");
                    logger.AddEntry(format.ToString(), className, dbJs.FullName, daoBin.FullName);
                };
                this.GenerateCommonDaoSucceeded += (dbJsFi, daoBin, result) =>
                {
                    StringBuilder format = new StringBuilder();
                    format.AppendLine("*** Dao Generation SUCCEEDED ***");
                    format.AppendLine("\t{0}::DbJSFile::{1}");
                    format.AppendLine("\t{0}::DaoBin::{2}");
                    format.AppendLine("*** /end Dao Generation SUCCEEDED ***");
                    logger.AddEntry(format.ToString(), className, dbJsFi.FullName, daoBin.FullName);
                };
                this.GenerateCommonDaoFailed += (dbJsFi, result) =>
                {
                    StringBuilder format = new StringBuilder();
                    format.AppendLine("*** Dao Generation FAILED ***");
                    format.AppendLine("\t{0}::DbJSFile::{1}");
                    format.AppendLine("\t{0}::Result.Message::{2}");
                    format.AppendLine("\t{0}::Result.ExceptionMessage::{3}");
                    format.AppendLine("\t{0}::Result.StackTrace::{4}");
                    format.AppendLine("*** /end Dao Generation FAILED ***");
                    logger.AddEntry(format.ToString(), LogEventType.Error, className, dbJsFi.FullName, result.Message, result.ExceptionMessage, result.StackTrace);
                };
                this.GeneratingAppDao += (name, dbJs, daoBin) =>
                {
                    StringBuilder format = new StringBuilder();
                    format.AppendLine("{0}::Generat(ING) (APP[{1}]) dao classes;");
                    format.AppendLine("\t{0}::DbJSFile::{2}");
                    format.AppendLine("\t{0}::DaoBin::{3}");
                    logger.AddEntry(format.ToString(), className, name, dbJs.FullName, daoBin.FullName);
                };
                this.GenerateAppDaoSucceeded += (appName, dbJs, daoBin, result) =>
                {
                    StringBuilder format = new StringBuilder();
                    format.AppendLine("*** APP Dao Generation SUCCEEDED ***");
                    format.AppendLine("{0}::AppName::{1}");
                    format.AppendLine("\t{0}::DbJSFile::{2}");
                    format.AppendLine("\t{0}::DaoBin::{3}");
                    format.AppendLine("*** /end APP Dao Generation SUCCEEDED ***");
                    logger.AddEntry(format.ToString(), className, appName, dbJs.FullName, daoBin.FullName);
                };
                this.GenerateAppDaoFailed += (appName, dbJs, result) =>
                {
                    StringBuilder format = new StringBuilder();
                    format.AppendLine("*** APP Dao Generation FAILED ***");
                    format.AppendLine("{0}::AppName::{1}");
                    format.AppendLine("\t{0}::DbJSFile::{2}");
                    format.AppendLine("\t{0}::Message::{3}");
                    format.AppendLine("\t{0}::Exception::{4}");
                    format.AppendLine("*** /end APP Dao Generation FAILED ***");
                    logger.AddEntry(format.ToString(), LogEventType.Error, className, appName, dbJs.FullName, result.Message, result.ExceptionMessage);
                };
                this.SchemaInitializing += (appName, si) =>
                {
                    logger.AddEntry("{0}::Schema Initializ(ING) AppName=({1}), SchemaContext=({2})", className, appName, si.SchemaContext); // SchemaName isn't set until Initialize completes
                };
                this.SchemaInitialized += (appName, si) =>
                {
                    logger.AddEntry("{0}::Schema Initializ(ED) AppName=({1}), SchemaName=({2})", className, appName, si.SchemaName);
                };
                this.RegisteringCommonDaoFromDirectory += (daoBin) =>
                {
                    logger.AddEntry("{0}::Register(ING) dao bin {1}", className, daoBin.FullName);
                };
                this.RegisteredCommonDaoFromDirectory += (daoBin) =>
                {
                    logger.AddEntry("{0}::Register(ED) dao bin {1}", className, daoBin.FullName);
                };
            }
        }

        public event Action<string, SchemaInitializer> SchemaInitializing;
        public event Action<string, SchemaInitializer> SchemaInitialized;

        protected void OnSchemaInitializing(string appName, SchemaInitializer initializer)
        {
            SchemaInitializing?.Invoke(appName, initializer);
        }
        protected void OnSchemaInitialized(string appName, SchemaInitializer initializer)
        {
            SchemaInitialized?.Invoke(appName, initializer);
        }

        Dictionary<string, DaoProxyRegistration> _commonDaoProxyRegistrations;
        /// <summary>
        /// The DaoProxyRegistrations keyed by connectionName/contextName
        /// </summary>
        public Dictionary<string, DaoProxyRegistration> CommonDaoProxyRegistrations
        {
            get
            {
                if (_commonDaoProxyRegistrations == null)
                {
                    _commonDaoProxyRegistrations = new Dictionary<string, DaoProxyRegistration>();
                }
                return _commonDaoProxyRegistrations;
            }
        }

        Dictionary<string, HashSet<DaoProxyRegistration>> _appDaoProxyRegistrations;
        /// <summary>
        /// The DaoProxyRegistrations keyed by application name
        /// </summary>
        public Dictionary<string, HashSet<DaoProxyRegistration>> AppDaoProxyRegistrations
        {
            get
            {
                if (_appDaoProxyRegistrations == null)
                {
                    _appDaoProxyRegistrations = new Dictionary<string, HashSet<DaoProxyRegistration>>();
                }

                return _appDaoProxyRegistrations;
            }
        }
        
        private void RegisterNewAppDaoDll(string appName, FileInfo dbJs, DirectoryInfo daoBin, SchemaManagerResult managerResult)
        {
            FileInfo daoDll = new FileInfo(Path.Combine(daoBin.FullName, "{0}.dll"._Format(managerResult.Namespace)));
            DaoProxyRegistration reg = DaoProxyRegistration.Register(daoDll);
            string name = appName.ToLowerInvariant();

            AppDaoProxyRegistrations[name].Add(reg);
        }
        private void RegisterNewCommonDaoDll(FileInfo dbJs, DirectoryInfo daoBin, SchemaManagerResult managerResult)
        {
            FileInfo daoDll = new FileInfo(Path.Combine(daoBin.FullName, "{0}.dll"._Format(managerResult.Namespace)));
            RegisterCommonDaoDll(daoDll);
        }

        readonly object _initializeLock = new object();
        public void Initialize()
        {
            OnInitializing();
            lock (_initializeLock)
            {
                // if server.conf.generatedao
                if (BamConf.GenerateDao)
                {
                    GenerateCommonDao("~/common");
                }

                DirectoryInfo commonDaoDir = BamConf.Fs.GetDirectory("~/common/dao");
                RegisterCommonDaoFromDirectory(commonDaoDir);

                BamConf.ReloadAppConfigs();

                // for each appconfig
                BamConf.AppConfigs.Each(appConf =>
                {
                    DirectoryInfo appDaoRoot = appConf.AppRoot.GetDirectory("~/dao");

                    string name = appConf.Name.ToLowerInvariant();
                    if (!AppDaoProxyRegistrations.ContainsKey(name))
                    {
                        AppDaoProxyRegistrations.Add(name, new HashSet<DaoProxyRegistration>());
                    }

                    //  if appconf.generatedao
                    if (appConf.GenerateDao)
                    {
                        GenerateAppDao("~/dao", appConf, appDaoRoot);
                    }

                    // register each dao type using DaoRegistration
                    RegisterAppDaoFromDirectory(appConf.Name, appDaoRoot);

                    appConf.SchemaInitializers.Each(si =>
                    {
                        OnSchemaInitializing(name, si);
                        if (!si.Initialize(Logger, out Exception ex))
                        {
                            Logger.AddEntry("Failed to initialize schema ({0}): {1}", ex, si.SchemaContext, ex.Message);
                        }
                        OnSchemaInitialized(name, si);
                    });
                });

                IsInitialized = true;
            }
            OnInitialized();
        }

        protected internal virtual void GenerateCommonDao(string dbJsRoot)
        {
            GenerateCommonDaoSucceeded += RegisterNewCommonDaoDll;
            //  generate common dao for each *.db.js in ~s:/dao/
            DirectoryInfo daoRoot = BamConf.Fs.GetDirectory(Path.Combine(dbJsRoot, "dao"));

            GenerateCommonDao(dbJsRoot, daoRoot, "*.db.js");
            GenerateCommonDao(dbJsRoot, daoRoot, "*.db.json");

            GenerateCommonDaoSucceeded -= RegisterNewCommonDaoDll; // only stays attached for the generation process if GenerateDao is true
        }

        protected internal virtual void GenerateAppDao(string dbJsRoot, AppConf appConf, DirectoryInfo appDaoDir)
        {
            GenerateAppDaoSucceeded += RegisterNewAppDaoDll;

            GenerateAppDaos(dbJsRoot, appConf, appDaoDir, "*.db.js");
            GenerateAppDaos(dbJsRoot, appConf, appDaoDir, "*.db.json");

            GenerateAppDaoSucceeded -= RegisterNewAppDaoDll;
        }

        protected internal void GenerateAppDaos(string dbJsRoot, AppConf appConf, DirectoryInfo appDaoBinDir, string fileSearchPattern)
        {
            DirectoryInfo daoTemp = appConf.AppRoot.GetDirectory(Path.Combine(dbJsRoot, "app_dao_tmp_".RandomLetters(4)));

            // get the saved hashes to determine if changes were made
            string hashPath = GetHashFilePath(appDaoBinDir);
            List<FileContentHash> hashes = GetHashes(hashPath);

            //      generate app dao from *.db.js ~a:/dao/
            FileInfo[] dbJsFiles = appConf.AppRoot.GetFiles(dbJsRoot, fileSearchPattern);
            //      compile into ~a:/dao/bin
            dbJsFiles.Each(dbJs =>
            {
                string path = dbJs.FullName;
                FileContentHash currentHash = new FileContentHash(path);

                if (!hashes.Contains(currentHash) && appConf.CheckDaoHashes)
                {
                    FileContentHash remove = hashes.FirstOrDefault(h => h.FilePath.ToLowerInvariant().Equals(path));
                    if (remove != null)
                    {
                        hashes.Remove(remove);
                    }
                    hashes.Add(currentHash);
                    hashes.ToArray().ToJsonFile(hashPath);
                    GenerateAppDao(appConf.Name, appDaoBinDir, daoTemp, dbJs);
                }
                else if (!appConf.CheckDaoHashes)
                {
                    GenerateAppDao(appConf.Name, appDaoBinDir, daoTemp, dbJs);
                }
            });
        }

        private static string GetHashFilePath(DirectoryInfo parentDir)
        {
            string hashPath = Path.Combine(parentDir.FullName, "{0}.json"._Format(typeof(FileContentHash).Name.Pluralize()));
            return hashPath;
        }

        private void GenerateCommonDao(string dbjsRoot, DirectoryInfo daoBinDir, string fileSearchPattern)
        {
            DirectoryInfo dbjsRootDir = BamConf.Fs.GetDirectory(dbjsRoot);
            DirectoryInfo daoTemp = BamConf.Fs.GetDirectory(Path.Combine(dbjsRoot, "common_dao_tmp_".RandomLetters(4)));

            string hashPath = GetHashFilePath(dbjsRootDir);
            List<FileContentHash> hashes = GetHashes(hashPath);

            FileInfo[] dbJsFiles = BamConf.Fs.GetFiles(dbjsRoot, fileSearchPattern);
            dbJsFiles.Each(dbJs =>
            {
                string path = dbJs.FullName.ToLowerInvariant();
                FileContentHash currentHash = new FileContentHash(path);

                if (!hashes.Contains(currentHash))
                {
                    FileContentHash remove = hashes.FirstOrDefault(h => h.FilePath.ToLowerInvariant().Equals(path));
                    if (remove != null)
                    {
                        hashes.Remove(remove);
                    }
                    hashes.Add(currentHash);
                    hashes.ToArray().ToJsonFile(hashPath);

                    GenerateCommonDao(daoBinDir, daoTemp, dbJs);
                }
            });
        }

        private DaoProxyRegistration RegisterCommonDaoDll(FileInfo daoDll)
        {
            DaoProxyRegistration reg = DaoProxyRegistration.Register(daoDll);
            CommonDaoProxyRegistrations[reg.ContextName.ToLowerInvariant()] = reg;

            return reg;
        }

        private void RegisterDatabase(DaoProxyRegistration reg)
        {
            DaoConf daoConf = BamConf.DaoConfigs.FirstOrDefault(d => d.ConnectionName.Equals(reg.ContextName)) ?? DaoConf.GetDefault(reg.ContextName, BamConf);
            daoConf.Register();
            reg.Database = Db.For(reg.ContextName);
            reg.Database.TryEnsureSchema(reg.Assembly, Logger);
        }

        public event Action<DirectoryInfo> RegisteringCommonDaoFromDirectory;
        protected void OnRegisteringCommonDaoFromDirectory(DirectoryInfo daoBin)
        {
            if (RegisteringCommonDaoFromDirectory != null)
            {
                RegisteringCommonDaoFromDirectory(daoBin);
            }
        }
        public event Action<DirectoryInfo> RegisteredCommonDaoFromDirectory;
        protected void OnRegisteredCommonDaoFromDirectory(DirectoryInfo daoBin)
        {
            if (RegisteredCommonDaoFromDirectory != null)
            {
                RegisteredCommonDaoFromDirectory(daoBin);
            }
        }
        public event Action<DirectoryInfo, Exception> RegisterCommonDaoFromDirectoryFailed;
        protected void OnRegisterCommonDaoFromDirectoryFailed(DirectoryInfo daoBin, Exception ex)
        {
            if (RegisterCommonDaoFromDirectoryFailed != null)
            {
                RegisterCommonDaoFromDirectoryFailed(daoBin, ex);
            }
        }
        internal void RegisterCommonDaoFromDirectory(DirectoryInfo daoBinDir)
        {
            try
            {
                OnRegisteringCommonDaoFromDirectory(daoBinDir);
                DaoProxyRegistration[] daoRegistrations = DaoProxyRegistration.Register(daoBinDir, BamConf.DaoSearchPattern);
                daoRegistrations.Each(daoReg =>
                {
                    CommonDaoProxyRegistrations[daoReg.ContextName.ToLowerInvariant()] = daoReg;
                });
                OnRegisteredCommonDaoFromDirectory(daoBinDir);
            }
            catch (Exception ex)
            {
                OnRegisterCommonDaoFromDirectoryFailed(daoBinDir, ex);
            }
        }

        public event Action<DirectoryInfo> RegisteringAppDaoFromDirectory;
        protected void OnRegisteringAppDaoFromDirectory(DirectoryInfo daoDir)
        {
            if (RegisteringAppDaoFromDirectory != null)
            {
                RegisteringAppDaoFromDirectory(daoDir);
            }
        }
        public event Action<DirectoryInfo> RegisteredAppDaoFromDirectory;
        protected void OnRegisteredAppDaoFromDirectory(DirectoryInfo daoDir)
        {
            if (RegisteredAppDaoFromDirectory != null)
            {
                RegisteredAppDaoFromDirectory(daoDir);
            }
        }
        public event Action<DirectoryInfo, Exception> RegisterAppDaoDirectoryFailed;
        protected void OnRegisterAppDaoDirectoryFailed(DirectoryInfo daoDir, Exception ex)
        {
            if (RegisterAppDaoDirectoryFailed != null)
            {
                RegisterAppDaoDirectoryFailed(daoDir, ex);
            }
        }
        internal void RegisterAppDaoFromDirectory(string appName, DirectoryInfo daoDir)
        {
            try
            {
                OnRegisteringAppDaoFromDirectory(daoDir);
                string name = appName.ToLowerInvariant();
                DaoProxyRegistration[] daoRegistrations = DaoProxyRegistration.Register(daoDir, BamConf.DaoSearchPattern);
                daoRegistrations.Each(daoReg =>
                {
                    HashSet<DaoProxyRegistration> list = AppDaoProxyRegistrations[name];
                    if (!list.Contains(daoReg))
                    {
                        AppDaoProxyRegistrations[name].Add(daoReg);
                    }
                });
                OnRegisteredAppDaoFromDirectory(daoDir);
            }
            catch (Exception ex)
            {
                OnRegisterAppDaoDirectoryFailed(daoDir, ex);
            }
        }

        public event Action<string, FileInfo, SchemaManagerResult> GenerateAppDaoFailed;
        protected void OnGenerateAppDaoFailed(string appName, FileInfo dbJsFile, SchemaManagerResult managerResult)
        {
            if (GenerateAppDaoFailed != null)
            {
                GenerateAppDaoFailed(appName, dbJsFile, managerResult);
            }
        }

        public event Action<string, FileInfo, DirectoryInfo, SchemaManagerResult> GenerateAppDaoSucceeded;
        protected void OnGenerateAppDaoSucceeded(string appName, FileInfo dbJsFile, DirectoryInfo daoBin, SchemaManagerResult managerResult)
        {
            if (GenerateAppDaoSucceeded != null)
            {
                GenerateAppDaoSucceeded(appName, dbJsFile, daoBin, managerResult);
            }
        }

        public event Action<string, FileInfo, DirectoryInfo> GeneratingAppDao;
        protected void OnGeneratingAppDao(string appName, FileInfo dbJsFile, DirectoryInfo daoBin)
        {
            if (GeneratingAppDao != null)
            {
                GeneratingAppDao(appName, dbJsFile, daoBin);
            }
        }

        private void GenerateAppDao(string appName, DirectoryInfo daoBinDir, DirectoryInfo daoTemp, FileInfo jsOrJsonDb)
        {
            OnGeneratingAppDao(appName, jsOrJsonDb, daoBinDir);

            SchemaManagerResult schemaManagerResult = Dao.GenerateAssembly(jsOrJsonDb, daoBinDir, daoTemp);

            if (!schemaManagerResult.Success)
            {
                OnGenerateAppDaoFailed(appName, jsOrJsonDb, schemaManagerResult);
            }
            else
            {
                OnGenerateAppDaoSucceeded(appName, jsOrJsonDb, daoBinDir, schemaManagerResult);
            }
        }

        private static List<FileContentHash> GetHashes(string hashPath)
        {
            List<FileContentHash> hashes = new List<FileContentHash>();
            if (File.Exists(hashPath))
            {
                hashes = new List<FileContentHash>(hashPath.FromJsonFile<FileContentHash[]>());
            }
            return hashes;
        }

        public event Action<FileInfo, SchemaManagerResult> GenerateCommonDaoFailed;
        protected void OnGenerateCommonDaoFailed(FileInfo dbJsFile, SchemaManagerResult managerResult)
        {
            GenerateCommonDaoFailed?.Invoke(dbJsFile, managerResult);
        }

        public event Action<FileInfo, DirectoryInfo, SchemaManagerResult> GenerateCommonDaoSucceeded;
        protected void OnGenerateCommonDaoSucceeded(FileInfo dbJsFile, DirectoryInfo daoBin, SchemaManagerResult managerResult)
        {
            GenerateCommonDaoSucceeded?.Invoke(dbJsFile, daoBin, managerResult);
        }

        public event Action<FileInfo, DirectoryInfo> GeneratingCommonDao;
        protected void OnGeneratingCommonDao(FileInfo dbJsFile, DirectoryInfo daoBin)
        {
            GeneratingCommonDao?.Invoke(dbJsFile, daoBin);
        }

        private void GenerateCommonDao(DirectoryInfo daoBinDir, DirectoryInfo daoTemp, FileInfo jsOrJsonDb)
        {
            OnGeneratingCommonDao(jsOrJsonDb, daoBinDir);

            SchemaManagerResult schemaManagerResult = Dao.GenerateAssembly(jsOrJsonDb, daoBinDir, daoTemp);

            if (!schemaManagerResult.Success)
            {
                OnGenerateCommonDaoFailed(jsOrJsonDb, schemaManagerResult);
            }
            else
            {
                OnGenerateCommonDaoSucceeded(jsOrJsonDb, daoBinDir, schemaManagerResult);
            }
        }

        public event Action<DaoResponder> Initializing;

        protected void OnInitializing()
        {
            Initializing?.Invoke(this);
        }

        public event Action<DaoResponder> Initialized;
        protected void OnInitialized()
        {
            Initialized?.Invoke(this);
        }
    }
}
