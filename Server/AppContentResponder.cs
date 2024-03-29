/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Bam.Net.Caching.File;
using Bam.Net.Logging;
using Bam.Net.Server.Renderers;
using Bam.Net.ServiceProxy;
using Bam.Net.UserAccounts;
using Bam.Net.UserAccounts.Data;
using Yahoo.Yui.Compressor;
using Bam.Net.Presentation;
using System.Reflection;
using System.Threading.Tasks;
using Bam.Net.Data.Repositories;
using System.Linq;
using Bam.Net.Configuration;
using Bam.Net.Services;
using UAParser;

namespace Bam.Net.Server
{
    public partial class AppContentResponder : ContentResponder
    {
        public AppContentResponder(ContentResponder commonResponder, AppConf conf, DataProvider dataSettings = null, ILogger logger = null)
            : base(commonResponder.BamConf, logger)
        {
            if (conf.BamConf == null)
            {
                conf.BamConf = commonResponder.BamConf;
            }
            DataProvider = dataSettings ?? DataProvider.Current;
            ContentResponder = commonResponder;
            ServerRoot = commonResponder.ServerRoot;
            AppConf = conf;
            AppRoot = AppConf.AppRoot;
            AppContentLocator = ContentLocator.Load(this);
            ContentHandlers = new Dictionary<string, ContentHandler>();
            AllRequestHandler = new ContentHandler($"{conf.Name}.AllRequestHandler", AppRoot) { CheckPaths = false };
            CustomHandlerMethods = new List<MethodInfo>();
            SetUploadHandler();
            SetDownloadHandlers();
            SetBaseIgnorePrefixes();
            ContentHandlerScanTask = ScanForContentHandlers();
            SetAllRequestHandler();
        }

        protected ContentHandler AllRequestHandler { get; set; }
        protected Dictionary<string, ContentHandler> ContentHandlers { get; set; }
        protected Task ContentHandlerScanTask { get; }

        protected Task ScanForContentHandlers()
        {
            Task scan = Task.Run(() =>
            {
                try
                {
                    // TODO: change this to check 'ContentHandlersSearchPattern' from the process config
                    string[] assemblySearchPatterns = DefaultConfiguration.GetAppSetting("AssemblySearchPattern", "*ContentHandlers.dll").DelimitSplit(",", true);
                    DirectoryInfo entryDir = Assembly.GetEntryAssembly().GetFileInfo().Directory;
                    DirectoryInfo sysAssemblies = DataProvider.GetSysAssemblyDirectory();
                    List<FileInfo> files = new List<FileInfo>();
                    foreach(string assemblySearchPattern in assemblySearchPatterns)
                    {
                        files.AddRange(entryDir.GetFiles(assemblySearchPattern));
                        if (sysAssemblies.Exists)
                        {
                            files.AddRange(sysAssemblies.GetFiles(assemblySearchPattern));
                        }
                    }
                    Parallel.ForEach(files, LoadCustomHandlers);
                }
                catch (Exception ex)
                {
                    Logger.AddEntry("Exception occurred scanning for custom content handlers: {0}", ex, ex.Message);
                }
            });
            return scan;
        }

        protected List<MethodInfo> CustomHandlerMethods { get; }

        /// <summary>
        /// Load all methods found in the specified file that are adorned with the ContentHandlerAttribute attribute
        /// into the CustomHandlerMethods list.
        /// </summary>
        /// <param name="file"></param>
        protected void LoadCustomHandlers(FileInfo file)
        {
            try
            {
                string extension = Path.GetExtension(file.FullName);
                if(!extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase) && !extension.Equals(".exe", StringComparison.InvariantCulture))
                {
                    return;
                }
                Assembly assembly = Assembly.LoadFrom(file.FullName);                
                CustomHandlerMethods.AddRange(
                    assembly.GetTypes()
                        .Where(type => type.HasCustomAttributeOfType<ContentHandlerAttribute>())
                            .SelectMany(type => type.GetMethods().Where(mi =>
                            {
                                bool hasAttribute = mi.HasCustomAttributeOfType<ContentHandlerAttribute>();
                                bool isStatic = mi.IsStatic;
                                if(hasAttribute && !isStatic)
                                {
                                    Logger.AddEntry("The method {0}.{1} is marked as a ContentHandler but it is not static", LogEventType.Warning, mi.DeclaringType.Name, mi.Name);
                                }
                                ParameterInfo[] parms = mi.GetParameters();
                                return hasAttribute && 
                                    isStatic &&
                                    mi.ReturnType == typeof(byte[]) &&                                    
                                    parms.Length == 2 &&
                                    parms[0].ParameterType == typeof(IHttpContext) &&
                                    parms[1].ParameterType == typeof(Fs);
                            }))
                );
            }
            catch (Exception ex)
            {
                Logger.AddEntry("Failed to load custom handlers from file {0}: {1}", ex, file.FullName, ex.Message);
            }
        }

        public DataProvider DataProvider { get; }

        public ContentLocator AppContentLocator
        {
            get;
            private set;
        }
        
        protected async void SetAllRequestHandler()
        {
            try
            {
                await ContentHandlerScanTask;
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    System.Console.WriteLine(e.Message);                    
                }
            }

            AllRequestHandler.GetContent = (ctx, fs) =>
            {
                byte[] result = null;
                foreach(MethodInfo method in CustomHandlerMethods)
                {
                    result = method.Invoke(null, ctx, fs) as byte[];
                    if(result != null)
                    {
                        Logger.AddEntry("{0}.{1} handled request {2}\r\n{3}", method.DeclaringType.Name, method.Name, ctx.Request.Url.ToString(), ctx.Request.PropertiesToString());
                        break;
                    }
                }
                return result;
            };
        }

        protected void SetUploadHandler()
        {
            SetCustomContentHandler("Application Upload", "/upload", (ctx, fs) =>
            {
                IRequest request = ctx.Request;
                HandleUpload(ctx, HttpPostedFile.FromRequest(request));
                string query = request.Url.Query.Length > 0 ? request.Url.Query : string.Empty;
                if (query.StartsWith("?"))
                {
                    query = query.TruncateFront(1);
                }
                return RenderLayout(request, ctx.Response, query);
            });
        }

        protected virtual void SetDownloadHandlers()
        {
            SetCustomContentHandler("Toolkit Download", "/download-toolkit", (ctx, fs) =>
            {
                IRequest request = ctx.Request;
                Parser parser = Parser.GetDefault();
                string runtime = "win10-x64";
                if (request.UserAgent != null)
                {
                    ClientInfo clientInfo = parser.Parse(request.UserAgent);
               
                    if (clientInfo.OS.Family.ToLowerInvariant().Contains("mac"))//, StringComparison.InvariantCultureIgnoreCase))
                    {
                        runtime = "osx-x64";
                    }
                    else if (clientInfo.OS.Family.ToLowerInvariant().Contains("linux"))//, StringComparison.InvariantCultureIgnoreCase))
                    {
                        runtime = "linux-x64";
                    }
                }

                return GetResponseData(ctx, runtime);                
            });
            
            SetCustomContentHandler("Windows Toolkit Download", "/download-windows-toolkit", (ctx, fs) => GetResponseData(ctx, "win10-x64"));
            SetCustomContentHandler("Linux Toolkit Download", "/download-linux-toolkit", (ctx, fs) => GetResponseData(ctx, "linux-x64"));
            SetCustomContentHandler("Mac Toolkit Download", "/download-mac-toolkit", (ctx, fs) => GetResponseData(ctx, "osx-x64"));
            
            SetCustomContentHandler("Download a named file", "/download", (ctx, fs) =>
            {
                IRequest request = ctx.Request;
                byte[] responseData = null;
                string requestedFileName = request.QueryString?.Get("fileName");
                string[] requestedSegments = new string[] {"~", "common", "files", requestedFileName};
                if (!string.IsNullOrEmpty(requestedFileName) && ServerRoot.FileExists(out string requestedFilePath, requestedSegments))
                {
                    responseData = GetResponseData(ctx, requestedFileName, requestedFilePath);
                }

                return responseData;
            });
            
            SetCustomContentHandler("Download toolkit install script", "/install.sh", (ctx, fs) =>
            {
                if (ServerRoot.FileExists(out string installScriptPath, "~", "common", "files", "install.sh"))
                {
                    return GetResponseData(ctx, "install.sh", installScriptPath);
                }
                
                Log.Warn("Install script is missing");
                return null;
            });
            
            SetCustomContentHandler("Download tool install script", "/install-tool.sh", (ctx, fs) =>
            {
                if (ServerRoot.FileExists(out string installToolScriptPath, "~", "common", "files", "install-tool.sh"))
                {
                    return GetResponseData(ctx, "install-tool.sh", installToolScriptPath);
                }
                
                Log.Warn("Tool install script is missing");
                return null;
            });
        }
        
        private byte[] GetResponseData(IHttpContext ctx, string runtime)
        {
            byte[] responseData = null;
            string fileName = $"bamtoolkit-{runtime}.zip";
            string[] segments = new string[] {"~", "common", "files", fileName};
            if (ServerRoot.FileExists(out string filePath, segments))
            {
                responseData = GetResponseData(ctx, fileName, filePath);
            }

            return responseData;
        }

        private static byte[] GetResponseData(IHttpContext ctx, string nameToGiveFile, string filePath)
        {
            IResponse response = ctx.Response;
            response.Headers.Add("Content-Disposition", $"attachment; filename={nameToGiveFile}");
            response.Headers.Add("Content-type", "application/zip");

            byte[] data = File.ReadAllBytes(filePath);
            return data;
        }

        /// <summary>
        /// Gets the main ContentResponder, which is the content responder
        /// for the server level of the current BamServer
        /// </summary>
        public ContentResponder ContentResponder
        {
            get;
            internal set;
        }

        IApplicationTemplateManager _appTemplateManager;
        public IApplicationTemplateManager AppTemplateManager
        {
            get => _appTemplateManager;
            internal set
            {
                _appTemplateManager = value;
                _appTemplateManager.AppContentResponder = this;
                _appTemplateManager.ContentResponder = ContentResponder;
                if (PageRenderer != null)
                {
                    PageRenderer.ApplicationTemplateManager = _appTemplateManager;
                }
            }
        }

        IPageRenderer _pageRenderer;
        public IPageRenderer PageRenderer
        {
            get => _pageRenderer;
            set
            {
                _pageRenderer = value;
                _pageRenderer.ApplicationTemplateManager = _appTemplateManager;
            }
        }

        public AppConf AppConf
        {
            get;
            private set;
        }

        public event Action<AppContentResponder> AppInitializing;
        public event Action<AppContentResponder> AppInitialized;

        protected void OnAppInitializing()
        {
            AppInitializing?.Invoke(this);
        }

        protected void OnAppInitialized()
        {
            AppInitialized?.Invoke(this);
        }

        public string ApplicationName => AppConf.Name;

        public User GetUser(IHttpContext context)
        {
            UserManager mgr = BamConf.AppServer.GetAppService<UserManager>(ApplicationName).Clone<UserManager>();
            mgr.HttpContext = context;
            return mgr.GetUser(context);
        }

        /// <summary>
        /// The server content root
        /// </summary>
        public Fs ServerRoot { get; private set; }

        /// <summary>
        /// The application content root
        /// </summary>
        public Fs AppRoot { get; private set; }

        public void SetCustomContentHandler(string path, Func<IHttpContext, Fs, byte[]> handler)
        {
            SetCustomContentHandler(path, path, handler);
        }

        public void SetCustomContentHandler(string name, string path, Func<IHttpContext, Fs, byte[]> handler)
        {
            SetCustomContentHandler(name, new string[] { path }, handler);
        }

        public void SetCustomContentHandler(string name, string[] paths, Func<IHttpContext, Fs, byte[]> handler)
        {
            ContentHandler customHandler = new ContentHandler(name, AppRoot, paths) { GetContent = handler };
            SetCustomContentHandler(customHandler);
        }

        public void SetCustomContentHandler(ContentHandler customContentHandler)
        {
            foreach(string path in customContentHandler.Paths)
            {
                ContentHandlers[path.ToLowerInvariant()] = customContentHandler;
            }
        }

        /// <summary>
        /// Writes the current application configuration to the file system.
        /// </summary>
        public override void Initialize()
        {
            OnAppInitializing();            

            AppRoot.WriteFile("appConf.json", AppConf.ToJson(true));

            OnAppInitialized();
        }

        public override bool TryRespond(IHttpContext context)
        {
            return TryRespond(context, out string[] ignore);
        }

        public bool TryRespond(IHttpContext context, out string[] checkedPaths)
        {
            checkedPaths = new string[] { };
            try
            {
                if (Etags.CheckEtags(context))
                {
                    return true;
                }
                IRequest request = context.Request;
                IResponse response = context.Response;
                string path = request.Url.AbsolutePath;

                string ext = Path.GetExtension(path);

                string[] split = path.DelimitSplit("/");
                byte[] content = new byte[] { };
                bool handled = AllRequestHandler.HandleRequest(context, out content);

                if (!handled)
                {
                    if (ContentHandlers.ContainsKey(path.ToLowerInvariant()))
                    {
                        handled = ContentHandlers[path.ToLowerInvariant()].HandleRequest(context, out content);
                    }
                    else if (AppContentLocator.Locate(path, out string locatedPath, out checkedPaths))
                    {
                        content = GetContent(locatedPath, request, response);
                        handled = true;
                        Etags.SetLastModified(response, request.Url.ToString(), new FileInfo(locatedPath).LastWriteTime);
                    }
                    else if (string.IsNullOrEmpty(ext) && !ShouldIgnore(path))
                    {
                        string relativePath = Path.Combine("~/", AppConf.HtmlDir, $"{path}.html");
                        if (AppRoot.FileExists(relativePath, out locatedPath))
                        {
                            content = GetContent(locatedPath, request, response);
                        }
                        else if (PageRenderer != null && PageRenderer.CanRender(request))
                        {
                            content = PageRenderer.RenderPage(request, response);
                        }
                        else
                        {
                            content = RenderLayout(request, response);
                        }

                        handled = content.Length > 0; // content was rendered and should be written to the output
                    }
                }

                if (handled)
                {
                    SetContentType(response, path);
                    SetContentDisposition(response, path);
                    Etags.Set(response, request.Url.ToString(), content);
                    SetResponseHeaders(response, path);
                    WriteResponse(response, content);
                    OnResponded(context);
                }
                else
                {
                    LogContentNotFound(request.Url.AbsolutePath, ApplicationName, checkedPaths);
                    OnContentNotFound(this, context, checkedPaths);
                    OnDidNotRespond(context);
                }
                return handled;
            }
            catch (Exception ex)
            {
                Logger.AddEntry("An error occurred in {0}.{1}: {2}", ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                OnDidNotRespond(context);
                return false;
            }
        }

        protected internal byte[] GetContent(string locatedPath, IRequest request, IResponse response)
        {
            byte[] content;
            string foundExt = Path.GetExtension(locatedPath);
            if (FileCachesByExtension.ContainsKey(foundExt) && !AppConf.IsDev)
            {
                FileCache cache = FileCachesByExtension[foundExt];
                if (ShouldZip(request))
                {
                    SetGzipContentEncodingHeader(response);
                    content = cache.GetZippedContent(locatedPath);
                }
                else
                {
                    content = cache.GetContent(locatedPath);
                }
            }
            else
            {
                content = File.ReadAllBytes(locatedPath);
            }

            return content;
        }

        protected virtual void SetResponseHeaders(IResponse response, string path)
        {
            if (path.StartsWith("/meta"))
            {
                path = path.TruncateFront("/meta".Length);
            }
            Fs meta = new Fs(Path.Combine(AppRoot.Root, "meta", "headers"));
            if (meta.FileExists(path, out string fullPath))
            {
                foreach (string header in fullPath.SafeReadFile().DelimitSplit("\n"))
                {
                    string[] split = header.Split(new char[] { ':' }, 2);
                    if (split.Length == 2)
                    {
                        response.AddHeader(split[0].Trim(), split[1].Trim());
                    }
                }
            }
        }

        [Verbosity(LogEventType.Information)]
        public new event EventHandler FileUploading;
        [Verbosity(LogEventType.Information)]
        public new event EventHandler FileUploaded;

        protected virtual void HandleUpload(IHttpContext context, HttpPostedFile file)
        {
            string userName = GetUser(context).UserName;
            FileUploadEventArgs args = new FileUploadEventArgs(context, file, ApplicationName) {UserName = userName};
            FireEvent(FileUploading, args);
            if (args.Continue)
            {
                string saveToPath = Path.Combine(AppRoot.Root, "workspace", "uploads", userName, "temp_".RandomLetters(8));
                FileInfo fileInfo = new FileInfo(saveToPath);
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                file.Save(saveToPath);
                string renameTo = Path.Combine(fileInfo.Directory.FullName, file.FileName);
                renameTo = renameTo.GetNextFileName();
                File.Move(saveToPath, renameTo);
                file.FullPath = renameTo;
                FireEvent(FileUploaded, args);
            }
        }

        // TODO: extract layout related handling methods into separate (LayoutPageRenderer : PageRenderer)
        ConcurrentDictionary<string, LayoutModel> _layoutModelsByPath;
        readonly object _layoutsByPathSync = new object();
        protected internal ConcurrentDictionary<string, LayoutModel> LayoutModelsByPath
        {
            get
            {
                return _layoutsByPathSync.DoubleCheckLock(ref _layoutModelsByPath, () => new ConcurrentDictionary<string, LayoutModel>());
            }
        }

        protected internal LayoutModel GetLayoutModelForPath(string path)
        {
            if (path.Equals("/"))
            {
                path = "/{0}"._Format(AppConf.DefaultPage.Or(AppConf.DefaultLayoutConst));
            }

            string lowered = path.ToLowerInvariant();
            string[] layoutSegments = $"~/{AppConf.HtmlDir}/{path}{LayoutFileExtension}".DelimitSplit("/", "\\");
            string[] htmlSegments = $"~/{AppConf.HtmlDir}/{path}.html".DelimitSplit("/", "\\");

            LayoutModel layoutModel = null;
            if (LayoutModelsByPath.ContainsKey(lowered))
            {
                layoutModel = LayoutModelsByPath[lowered];
            }
            else if (AppRoot.FileExists(out string absolutePath, layoutSegments))
            {
                LayoutConf layoutConf = new LayoutConf(AppConf);
                LayoutConf fromLayoutFile = FileCachesByExtension[LayoutFileExtension].GetText(new FileInfo(absolutePath)).FromJson<LayoutConf>();
                layoutConf.CopyProperties(fromLayoutFile);
                layoutModel = layoutConf.CreateLayoutModel(htmlSegments);
                LayoutModelsByPath[lowered] = layoutModel;
            }
            else
            {
                LayoutConf defaultLayoutConf = new LayoutConf(AppConf);
                layoutModel = defaultLayoutConf.CreateLayoutModel(htmlSegments);
                FileInfo file = new FileInfo(AppRoot.GetAbsolutePath(layoutSegments));
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                // write the file to disk                 
                defaultLayoutConf.ToJsonFile(file);
                LayoutModelsByPath[lowered] = layoutModel;
            }

            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                string page = path.TruncateFront(1);
                if (!string.IsNullOrEmpty(page))
                {
                    layoutModel.StartPage = page;
                }
            }
            return layoutModel;
        }

        private byte[] RenderLayout(IRequest request, IResponse response, string queryString = null)
        {
            string path = request.Url.AbsolutePath;
            AppTemplateManager.SetContentType(response);
            MemoryStream ms = new MemoryStream();
            LayoutModel layoutModel = GetLayoutModelForPath(path);
            layoutModel.QueryString = queryString ?? layoutModel.QueryString;
            AppTemplateManager.RenderLayout(layoutModel, ms);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] content = ms.GetBuffer();
            return content;
        }

        protected override void SetBaseIgnorePrefixes()
        {
            base.SetBaseIgnorePrefixes();
            AddIgnorPrefix("content");
        }
    }
}
