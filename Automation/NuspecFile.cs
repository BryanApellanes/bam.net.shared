using System.IO;
using System.Xml.Linq;

namespace Bam.Net.Automation
{
    public class NuspecFile
    {
        public NuspecFile()
        {
            Path = "bam.nuspec";
        }

        public NuspecFile(string path)
        {
            Path = path;
        }

        public string Id { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public string Description { get; set; }
        private string Path { get; set; }

        public static string DefaultAuthors => "Bryan Apellanes";
        
        public static XElement NoPackageAnalysisElement => new XElement("NoPackageAnalysis", true);
        public static XElement GetNuspecFileElement(string projectFile)
        {
            string projectName = System.IO.Path.GetFileNameWithoutExtension(projectFile);
            return new XElement("NuspecFile", $"{projectName}.nuspec");
        }
        public static XElement IntermediatePackDirElement => new XElement("IntermediatePackDir", "$(MSBuildProjectDirectory)/bin/$(Configuration)/"); 
        public static XElement PublishDirElement => new XElement("PublishDir", "$(IntermediatePackDir)$(TargetFramework)/");
        public static XElement NuspecPropertiesElement => new XElement("NuspecProperties", "publishDir=$([MSBuild]::NormalizeDirectory($(IntermediatePackDir)))");

        public static XElement PublishAllTargetElement
        {
            get
            {
                XElement result = new XElement("Target", 
                    new XElement("ItemGroup",
                        new XElement("_TargetFramework", new XAttribute("Include", "$(TargetFrameworks)")),
                        new XElement("MSBuild", new XAttribute("Projects", "$(MSBuildProjectFullPath)"), new XAttribute("Targets", "Publish"), new XAttribute("Properties", "TargetFramework=%(_TargetFramework.Identity)" ))
                    ));
                result.Add(new XAttribute("Name", "PublishAll"));
                result.Add(new XAttribute("BeforeTargets", "GenerateNuspec"));
                return result;
            }
        }
        
        public static XNamespace Namespace => XNamespace.Get("http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"); 
        
        public FileInfo Write()
        {
            FileInfo result = new FileInfo(Path);
            XNamespace ns = Namespace;
            XDocument nuspec = new XDocument
            (
                new XElement(ns + "package",//, new XAttribute("xmlns", "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"),
                    new XElement(ns + "metadata",
                        new XElement(ns + "id", Id),
                        new XElement(ns + "version", Version),
                        new XElement(ns + "authors", Authors),
                        new XElement(ns + "description", Description)
                    ),
                    new XElement(ns + "files",
                        new XElement(ns + "file", new XAttribute("src", "_._"), new XAttribute("target", "lib/net5.0")),
                        new XElement(ns + "file", new XAttribute("src", "$publishdir$\\net5.0\\**\\*"), new XAttribute("target", "tools/net5.0")))
                )
            );

            nuspec.ToString().SafeWriteToFile(result.FullName, true);
            return result;
        }
    }
}