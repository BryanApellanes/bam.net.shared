/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bam.Net;
using Bam.Net.Web;
using Bam.Net.Incubation;
using Bam.Net.Presentation.Html;
using System.Reflection;
using Bam.Net.Logging;
using Bam.Net.Presentation;

namespace Bam.Net.Server.Renderers
{
    [Obsolete("This class is obsolete use AppHandlebarsRenderer instead")]
    public partial class AppDustRenderer: CommonDustRenderer, IApplicationTemplateManager, IHasCompiledTemplates
    {
        public AppDustRenderer(AppContentResponder appContent)
            : base(appContent.ContentResponder)
        {
            AppContentResponder = appContent;
            Logger = appContent.Logger;
        }
        
        public AppContentResponder AppContentResponder
        {
            get;
            set;
        }

        public string ApplicationName => AppContentResponder.ApplicationName;

        private string _combinedCompiledTemplates;
        private readonly object _combinedCompiledTemplatesLock = new object();
        /// <summary>
        /// All application compiled dust templates including Server level
        /// layouts, templates.
        /// </summary>
        public override string CombinedCompiledTemplates
        {
            get
            {
                return _combinedCompiledTemplatesLock.DoubleCheckLock(ref _combinedCompiledTemplates, () =>
                {
                    StringBuilder templates = new StringBuilder();
                    Logger.AddEntry("AppDustRenderer::Appending compiled layout templates");
                    templates.AppendLine(CombinedCompiledLayoutTemplates);
                    Logger.AddEntry("AppDustRenderer::Appending compiled common templates");
                    if(ContentResponder.CommonTemplateManager is IHasCompiledTemplates templateManager)
                    {
                        templates.AppendLine(templateManager.CombinedCompiledTemplates);
                    }

                    foreach(string templateDirectoryName in TemplateDirectoryNames)
                    {
                        DirectoryInfo appDust = new DirectoryInfo(Path.Combine(AppContentResponder.AppRoot.Root, templateDirectoryName));
                        AppendTemplatesFromDirectory(appDust, templates);
                    }
                    return templates.ToString();
                });
            }
        }

        private string _combinedCompiledLayoutTemplates;
        private readonly object _combinedCompiledLayoutTemplatesLock = new object();
        /// <summary>
        /// Represents the compiled javascript result of doing dust.compile
        /// against all the files found in ~s:/common/views/layouts.
        /// </summary>
        public override string CombinedCompiledLayoutTemplates
        {
            get
            {
                return _combinedCompiledLayoutTemplatesLock.DoubleCheckLock(ref _combinedCompiledLayoutTemplates, () =>
                {
                    StringBuilder templates = new StringBuilder();

                    foreach(string templateDirectoryName in TemplateDirectoryNames)
                    {
                        DirectoryInfo layouts = new DirectoryInfo(Path.Combine(AppContentResponder.AppRoot.Root, templateDirectoryName, "layouts"));
                        string compiledLayouts = DustScript.CompileTemplates(layouts.FullName, "*.dust", Logger);
                        templates.Append(compiledLayouts);
                    }

                    templates.Append(base.CombinedCompiledLayoutTemplates);
                    return templates.ToString();
                });
            }
        }

        private List<ICompiledTemplate> _compiledTemplates;
        private readonly object _compiledTemplatesLock = new object();
        public override IEnumerable<ICompiledTemplate> CompiledTemplates
        {
            get
            {
                return _compiledTemplatesLock.DoubleCheckLock(ref _compiledTemplates, () =>
                {
                    List<ICompiledTemplate> allResults = base.CompiledTemplates.ToList();
                    foreach (string templateDirectoryName in TemplateDirectoryNames)
                    {
                        DirectoryInfo appDustDirectory = new DirectoryInfo(Path.Combine(AppContentResponder.AppRoot.Root, templateDirectoryName));
                        DustScript.CompileTemplates(appDustDirectory, out List<ICompiledTemplate> results, "*.dust");
                        allResults.AddRange(results);
                    }
                    return allResults;
                });
            }
        }

        protected internal bool TemplateExists(Type anyType, string templateFileNameWithoutExtension, out string fullPath)
        {
            string relativeFilePath = "~/views/{0}/{1}.dust"._Format(anyType.Name, templateFileNameWithoutExtension);
            fullPath = AppContentResponder.AppRoot.GetAbsolutePath(relativeFilePath);
            return File.Exists(fullPath);
        }

        private void SetTemplateProperties(object instance)
        {
            Type type = instance.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.PropertyType == typeof(string) ||
                    prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(long))
                {
                    prop.SetValue(instance, "{" + prop.Name + "}", null);
                }
            }
        }

        private void AppendTemplatesFromDirectory(DirectoryInfo appDust, StringBuilder templates)
        {
            string domAppName = AppConf.DomApplicationIdFromAppName(this.AppContentResponder.AppConf.Name);
            Logger.AddEntry("AppDustRenderer::Compiling directory {0}", appDust.FullName);
            string appCompiledTemplates = DustScript.CompileTemplates(appDust, "*.dust", SearchOption.AllDirectories, domAppName + ".", Logger);

            templates.Append(appCompiledTemplates);
        }
    }
}
