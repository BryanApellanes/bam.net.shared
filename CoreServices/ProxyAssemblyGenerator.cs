/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Bam.Net.Logging;
using Bam.Net.ServiceProxy;
using System.Collections.Generic;
using Bam.Net.Services.Clients;
using Bam.Net.Services;
using System.Net.Http;

namespace Bam.Net.CoreServices
{
    public partial class ProxyAssemblyGenerator : ProxyAssemblyGenerationEventSource, IAssemblyGenerator
    {
        /// <summary>
        /// The event that is raised when an exception occurs during proxy generation.
        /// </summary>
        public event EventHandler<ProxyAssemblyGenerationEventArgs> AssemblyGenerationExceptionThrown;

        Dictionary<ClientCodeSource, Func<string>> _clientAssemblyWriters;
        public ProxyAssemblyGenerator(ProxySettings settings, string workspaceDirectory = ".", ILogger logger = null, HashSet<Assembly> addedReferenceAssemblies = null)
        {
            AdditionalReferenceAssemblies = addedReferenceAssemblies;
            ServiceType = settings.ServiceType;
            ProxySettings = settings;
            WorkspaceDirectory = workspaceDirectory;
            Code = new StringBuilder();
            Logger = logger ?? Log.Default;
            ProxyAssemblyGeneratorService = ProxyAssemblyGeneratorService.DefaultProxy;

            _clientAssemblyWriters = new Dictionary<ClientCodeSource, Func<string>>
            {
                { ClientCodeSource.Invalid,  WriteAssemblyFromLocal },
                { ClientCodeSource.Local, WriteAssemblyFromLocal },
                { ClientCodeSource.HostDownload, RetrieveAssemblyFromHostDownload },
                { ClientCodeSource.AssemblyService, RetrieveAssemblyFromAssemblyService }
            };
        }

        /// <summary>
        /// Gets or sets the ProxyAssemblyGeneratorService used if ProxySettings.ClientCodeSource is set to `AssemblyService`.  
        /// The ProxyAssemblyGeneratorService is not used for other ClientCodeSource values.
        /// </summary>
        public ProxyAssemblyGeneratorService ProxyAssemblyGeneratorService {get;set;}

        public HashSet<Assembly> AdditionalReferenceAssemblies { get; set; }

        /// <summary>
        /// The logger used to log events for the current ProxyAssemblyGenerator
        /// </summary>
        public ILogger Logger { get; set; }

        public ProxySettings ProxySettings { get; set; } 

        public StringBuilder Code { get; set; }

        public string WorkspaceDirectory { get; set; }

        public Type ServiceType { get; set; }

        public string AssemblyFilePath => Path.Combine(WorkspaceDirectory, GetAssemblyName());

        public GeneratedAssemblyInfo GetAssembly()
        {
            return GenerateAssembly();
        }

        static readonly Dictionary<Type, GeneratedAssemblyInfo> _generatedAssemblyInfos = new Dictionary<Type, GeneratedAssemblyInfo>();

        public GeneratedAssemblyInfo GenerateAssembly()
        {
            if (_generatedAssemblyInfos.ContainsKey(ServiceType))
            {
                return _generatedAssemblyInfos[ServiceType];
            }

            string path = _clientAssemblyWriters[ProxySettings.ClientCodeSource]();

            // load the assembly from the file
            GeneratedAssemblyInfo info = new GeneratedAssemblyInfo(Assembly.LoadFile(path));
            _generatedAssemblyInfos[ServiceType] = info;
            return info;
        }

        public string GetSource()
        {
            RenderCode();
            return Code.ToString();
        }

        public void WriteSource(string writeSourceTo)
        {
            RenderCode();
            Code.ToString().SafeWriteToFile(Path.Combine(WorkspaceDirectory, "src", $"{ServiceType.Name}.Proxy.cs"));
        }

        public void WriteSource(Stream stream)
        {
            RenderCode();
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.Write(Code.ToString());
            }
        }

        public ProxyCode GenerateProxyCode()
        {
            Code = new StringBuilder();
            ProxyCode code = new ProxyCode
            {
                ProxyModel = RenderCode(),
                Code = Code.ToString()
            };
            return code;
        }

        protected virtual void OnAssemblyGenerationExceptionThrown(ProxyAssemblyGenerationEventArgs args)
        {
            AssemblyGenerationExceptionThrown?.Invoke(this, args);
        }

        private string WriteAssemblyFromLocal()
        {
            try
            {
                if (ProxySettings.ClientCodeSource == ClientCodeSource.Local)
                {
                    return CompileAssembly();
                }
                else
                {
                    Logger.Warning("Executed {0} but ProxySettings.ClientCodeSource setting is: {1}", nameof(WriteAssemblyFromLocal), ProxySettings?.ClientCodeSource.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                ProxyAssemblyGenerationEventArgs args = this.CopyAs<ProxyAssemblyGenerationEventArgs>();
                args.Exception = ex;
                this.OnAssemblyGenerationExceptionThrown(args);
                return null;
            }
        }

        private string RetrieveAssemblyFromHostDownload()
        {
            try
            {
                if (ProxySettings.ClientCodeSource == ClientCodeSource.HostDownload)
                {
                    return CompileAssembly();
                }
                else
                {
                    Logger.Warning("Executed {0} but ProxySettings.ClientCodeSource setting is: {1}", nameof(RetrieveAssemblyFromHostDownload), ProxySettings?.ClientCodeSource.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                ProxyAssemblyGenerationEventArgs args = this.CopyAs<ProxyAssemblyGenerationEventArgs>();
                args.Exception = ex;
                this.OnAssemblyGenerationExceptionThrown(args);
                return null;
            }
        }

        private string CompileAssembly()
        {
            RenderCode();
            // compile Code and save assembly
            RoslynCompiler compiler = new RoslynCompiler();
            compiler.AddAssemblyReference(typeof(System.ComponentModel.Component).Assembly);
            byte[] assembly = compiler.Compile(GetAssemblyName(), Code.ToString(), typeof(HttpClient));
            string path = Path.Combine(BamProfile.GeneratedPath, GetAssemblyName());
            FileInfo assemblyFile = new FileInfo(path);
            if(!assemblyFile.Directory.Exists)
            {
                assemblyFile.Directory.Create();
            }
            File.WriteAllBytes(path, assembly);
            return path;
        }

        private string RetrieveAssemblyFromAssemblyService()
        {
            try
            {
                ProxyAssemblyGeneratorService genSvc = ProxyAssemblyGeneratorService;
                ServiceResponse response = genSvc.GetBase64ProxyAssembly(ServiceType.Namespace, ServiceType.Name);
                if (response == null)
                {
                    throw new ApplicationException($"No {nameof(ServiceResponse)} received; response from {nameof(ProxyAssemblyGeneratorService)} was null");
                }
                if (!response.Success)
                {
                    throw new ApplicationException(response.Message);
                }

                byte[] assembly = response.Data.ToString().FromBase64();
                string path = Path.Combine(BamProfile.GeneratedPath, GetAssemblyName());
                File.WriteAllBytes(path, assembly);
                return path;
            }
            catch(Exception ex)
            {
                ProxyAssemblyGenerationEventArgs args = this.CopyAs<ProxyAssemblyGenerationEventArgs>();
                args.Exception = ex;
                this.OnAssemblyGenerationExceptionThrown(args);
                return null;
            }
        }

        private string GetAssemblyName()
        {
            return $"{ServiceType.Name}_{ProxySettings}_proxy.dll";
        }

        private ProxyModel RenderCode()
        {
            EnsureWorkspace();
            SetClientCode();
            ProxyModel proxyModel = GetProxyModel();
            WarnNonVirtualMethods(proxyModel);
            Code.AppendLine(proxyModel.Render());
            return proxyModel;
        }

        private ProxyModel GetProxyModel()
        {
            HashSet<Assembly> referenceAssemblies = new HashSet<Assembly>(AdditionalReferenceAssemblies ?? new HashSet<Assembly>())
            {
                ServiceType.Assembly
            };
            return new ProxyModel(ServiceType, ProxySettings.Protocol.ToString().ToLowerInvariant(), ProxySettings.Host, ProxySettings.Port, referenceAssemblies);
        }

        private void WarnNonVirtualMethods(ProxyModel model)
        {
            model.ServiceGenerationInfo.MethodGenerationInfos.Each(mgi =>
            {
                MethodInfo method = mgi.Method;
                if (!method.IsVirtual)
                {
                    Logger.AddEntry("The method {0}.{1} is not marked virtual and as a result the generated proxy will not delegate properly to the designated remote", LogEventType.Warning, method.DeclaringType.Name, method.Name);
                    OnMethodWarning(new ProxyAssemblyGenerationEventArgs { NonVirtualMethod = method });
                }
            });
        }

        private void SetClientCode()
        {
            if (ProxySettings.ClientCodeSource == ClientCodeSource.HostDownload)
            {
                Code = ProxySettings.DownloadClientCode(ServiceType);
            }
            else
            {
                Code = ServiceProxySystem.GenerateCSharpProxyCode(ProxySettings.Protocol.ToString(), ProxySettings.Host, ProxySettings.Port, ServiceType.Namespace, ServiceType);                
            }
        }

        private void EnsureWorkspace()
        {
            DirectoryInfo root = new DirectoryInfo(WorkspaceDirectory);
            if (!root.Exists)
            {
                root.Create();
            }
        }
    }
}
