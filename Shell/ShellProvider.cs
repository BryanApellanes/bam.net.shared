/*using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bam.Net;
using Bam.Net.Application;
using Bam.Net.Data;
using Bam.Net.Presentation.Handlebars;
using Bam.Net.Services;
//using Bam.Net.Testing;
using Bam.Net.UserAccounts.Data;
using Bam.Net.CommandLine;
using Bam.Net.CoreServices;
using Bam.Net.UserAccounts;

namespace Bam.Shell
{
    public abstract class ShellProvider : CommandLineServiceInterface, IRegisterArguments
    {
        public abstract void List(Action<string> output = null, Action<string> error = null);
        public abstract void Add(Action<string> output = null, Action<string> error = null);
        public abstract void Show(Action<string> output = null, Action<string> error = null);
        public abstract void Remove(Action<string> output = null, Action<string> error = null);
        public abstract void Run(Action<string> output = null, Action<string> error = null);

        public virtual void Copy(Action<string> output = null, Action<string> error = null)
        {
            NotImplemented("Copy", error);
        }

        public virtual void Rename(Action<string> output = null, Action<string> error = null)
        {
            NotImplemented("Rename", error);
        }
        
        public virtual void Edit(Action<string> output = null, Action<string> error = null)
        {
            NotImplemented("Edit", error);
        }

        private void NotImplemented(string actionName, Action<string> outputter)
        {
            outputter = outputter ?? ((msg) => Message.PrintLine(msg));
            outputter($"{actionName} is not implemented for the specified shell provider: {GetType().FullName}");
        }
        
        static HandlebarsDirectory _handlebarsDirectory;
        static readonly object _handlebarsLock = new object();
        public static HandlebarsDirectory GetHandlebarsDirectory()
        {
            return _handlebarsLock.DoubleCheckLock(ref _handlebarsDirectory, () =>
            {
                DirectoryInfo bamDir = Assembly.GetExecutingAssembly().GetFileInfo().Directory;
                return new HandlebarsDirectory(Path.Combine(bamDir.FullName, "Templates"));
            });
        }

        string _providerType;
        public string ProviderType
        {
            get
            {
                if (string.IsNullOrEmpty(_providerType))
                {
                    Type type = this.GetType();
                    if (type.Name.EndsWith("Provider"))
                    {
                        _providerType = type.Name.Truncate("Provider".Length);
                    }
                    else
                    {
                        _providerType = type.Name;
                    }
                }

                return _providerType;
            }
        }
        public string[] RawArguments { get; private set; }
        public virtual void RegisterArguments(string[] args)
        {
            RawArguments = args;
            AddValidArgument("name", $"The name of the {ProviderType} to work with");
            AddValidArgument("format", "The desired output format: json | yaml");
        }
        
        protected virtual ProviderArguments GetProviderArguments()
        {
            string targetName = Arguments.Contains("name")
                ? Arguments["name"]
                : (
                    Arguments.Contains($"{ProviderType.ToLowerInvariant()}Name")
                        ? Arguments[$"{ProviderType.ToLowerInvariant()}Name"]
                        : (
                            GetTypeNameArgument(ProviderType.ToLowerInvariant(), $"Please enter the name of the {ProviderType.ToLowerInvariant()}") 
                            ??
                            GetArgument(ProviderType.ToLowerInvariant(),$"Please enter the name of the {ProviderType.ToLowerInvariant()}")
                          )
                  );
            
            return new ProviderArguments()
            {
                ProviderType = ProviderType,
                ProviderContextTarget = targetName,
            };
        }

        public string Serialize(object data)
        {
            return data.Serialize(GetFormat());
        }
        
        protected SerializationFormat GetFormat()
        {
            if (Arguments.Contains("format"))
            {
                SerializationFormat format = Arguments["format"].ToEnum<SerializationFormat>();
                if (format != SerializationFormat.Json && format != SerializationFormat.Yaml)
                {
                    format = SerializationFormat.Yaml;
                }

                return format;
            }

            return SerializationFormat.Yaml;
        }
        
        
        protected string GetTypeNameArgument(string type, string prompt = null)
        {
            if (Arguments.Contains(type))
            {
                return Arguments[type];
            }

            if (Arguments.Contains($"{type}Name"))
            {
                return Arguments[$"{type}Name"];
            }

            return Prompt(prompt ?? $"Please enter the {type} name");
        } 
        
        public static DirectoryInfo FindProjectParent(out FileInfo csprojFile)
        {
            string startDir = Environment.CurrentDirectory;
            DirectoryInfo startDirInfo = new DirectoryInfo(startDir);
            DirectoryInfo projectParent = startDirInfo;
            FileInfo[] projectFiles = projectParent.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
            while (projectFiles.Length == 0)
            {
                if(projectParent.Parent != null)
                {
                    projectParent = projectParent.Parent;
                    projectFiles = projectParent.GetFiles("*.csrpoj", SearchOption.TopDirectoryOnly);
                }
                else
                {
                    break;
                }
            }
            csprojFile = null;
            if (projectFiles.Length > 0)
            {
                csprojFile = projectFiles[0];
            }

            if (projectFiles.Length > 1)
            {
                Warn("Multiple csproject files found, using {0}\r\n{1}", csprojFile.FullName, string.Join("\r\n\t", projectFiles.Select(p => p.FullName).ToArray()));
            }
            return projectParent;
        }
        
        public static BamSettings GetSettings()
        {
            BamSettings settings = BamSettings.Load();
            if (!settings.IsValid(msg => OutLine(msg, ConsoleColor.Red)))
            {
                Exit(1);
            }

            return settings;
        }

        public static DirectoryInfo GetProjectParentDirectoryOrExit()
        {
            return GetProjectParentDirectoryOrExit(out FileInfo ignore);
        }

        public static DirectoryInfo GetProjectParentDirectoryOrExit(out FileInfo csprojFile)
        {
            DirectoryInfo projectParent = FindProjectParent(out csprojFile);
            if (csprojFile == null)
            {
                Message.PrintLine("Can't find csproj file", ConsoleColor.Magenta);
                Exit(1);
            }

            return projectParent;
        }
        
        public static FileInfo FindProjectFile()
        {
            FindProjectParent(out FileInfo csprojFile);
            return csprojFile;
        }
        
        protected void EnsureDirectoryExists(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
        }
        
        protected Role GetRole()
        {
            string roleName = GetArgument("roleName", "Please enter the name of the role.");
            return Role.FirstOneWhere(c => c.Name == roleName, GetUserDatabase());
        }
        
        protected User GetUser()
        {
            string userName = GetArgument("userName", "Please enter the name of the user.");
            return User.FirstOneWhere(c => c.UserName == userName, GetUserDatabase());
        }

        protected Database GetUserDatabase()
        {
            UserManager mgr = GetCoreService<UserManager>();
            Database userDatabase = mgr.Database;
            ConsoleColor color = ConsoleColor.DarkYellow;
            switch (ProcessMode.Current.Mode)
            {
                case ProcessModes.Dev:
                    color = ConsoleColor.Green;
                    break;
                case ProcessModes.Test:
                    color = ConsoleColor.Yellow;
                    break;
                case ProcessModes.Prod:
                    color = ConsoleColor.Red;
                    break;
            }
            Message.PrintLine(userDatabase.ConnectionString, color);
            return userDatabase;
        }

        private static T GetCoreService<T>()
        {
            ServiceRegistry svcRegistry = CoreServiceRegistryContainer.GetServiceRegistry();
            return svcRegistry.Get<T>();
        }
    }
}*/