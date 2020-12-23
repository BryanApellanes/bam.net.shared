using System.IO;
using Bam.Net.Logging;

namespace Bam.Net
{
    public class AssemblySemanticVersion: FileSystemSemanticVersion
    {
        public string Product { get; set; }
        
        /// <summary>
        /// The same as Revision.
        /// </summary>
        public uint BuildNumber
        {
            get => Revision;
            set => _revision = value;
        }

        private uint? _revision;
        /// <summary>
        /// The revision number; this value is based on the
        /// Build or Commit.  The Revision is the SHA1 of the Build
        /// converted to an uint.
        /// </summary>
        public uint Revision
        {
            get
            {
                if (_revision == null)
                {
                    _revision = (Commit?.ToSha1Uint()).Value;
                }

                return _revision ?? 0;
            }
        }

        private string _commit;
        public string Commit
        {
            get
            {
                if (string.IsNullOrEmpty(_commit))
                {
                    _commit = GitLog.AbbreviatedCommitHash;
                }

                return _commit;
            }
            set
            {
                _commit = value;
            } 
        }

        private string _build;
        public override string Build
        {
            get => _build;
            set => _build = value;
        }

        private string _description;
        public string Description
        {
            get
            {
                string suffix = GetDescriptionSuffix().Replace("\r", "").Replace("\n", "");
                if (string.IsNullOrEmpty(_description))
                {
                    return suffix;
                }

                return $"{_description}; {suffix}";
            }
            set => _description = value;
        }
        
        public override string ToString()
        {
            return $"{Major}.{Minor}.{BuildNumber}.{Revision}";
        }
        
        public static string WriteProjectSemanticAssemblyInfo(FileInfo projectFile, SemanticVersion version, string description = null)
        {
            AssemblySemanticVersion file = From(Path.GetFileNameWithoutExtension(projectFile.FullName), version);
            if (!string.IsNullOrEmpty(description))
            {
                file.Description = description;
            }
            return file.WriteSemanticAssemblyInfo(projectFile.Directory.FullName);
        }

        public static AssemblySemanticVersion From(string product, SemanticVersion version)
        {
            AssemblySemanticVersion result = From(version);
            result.Product = product;
            return result;
        }
        
        public static AssemblySemanticVersion From(SemanticVersion semanticVersion)
        {
            AssemblySemanticVersion assemblySemanticVersion = semanticVersion.CopyAs<AssemblySemanticVersion>();
            assemblySemanticVersion.Commit = semanticVersion.Build;
            return assemblySemanticVersion;
        }
        
        /// <summary>
        /// Write this AssemblySemanticVersion to a SemanticAssemblyInfo.cs file
        /// </summary>
        /// <param name="overwrite"></param>
        /// <param name="path"></param>
        public string WriteSemanticAssemblyInfo(string path = ".", bool overwrite = true)
        {
            FileInfo file = new FileInfo(Path.Combine(path, "SemanticAssemblyInfo.cs"));
            Log.Info("Writing file {0}", file.FullName);
            ToSemanticAssemblyInfo().SafeWriteToFile(file.FullName, overwrite);
            return file.FullName;
        }
        
        public string ToSemanticAssemblyInfo()
        {
            string semanticVersion = base.ToString();
            string content = $"using System.Reflection;\r\n" +
                             $"using Bam.Net;\r\n\r\n" +
                             $"[assembly: AssemblyVersion(\"{Major}.{Minor}.{Patch}.0\")]\r\n" +
                             $"[assembly: AssemblyFileVersion(\"{Major}.{Minor}.{Patch}.0\")]\r\n" +
                             $"[assembly: AssemblyCommit(\"{Commit}\")]\r\n" +
                             $"[assembly: AssemblySemanticVersion(\"{semanticVersion}\")]\r\n" +
                             $"[assembly: AssemblyDescription(\"{Description}\")]\r\n";

            if (!string.IsNullOrEmpty(Product))
            {
                content += $"[assembly: AssemblyProduct(\"{Product}\")]\r\n";
            }

            return content;
        }
        
        protected string GetDescriptionSuffix()
        {
            return $"SemanticVersion={base.ToString()}, Revision={Major}.{Minor}.{Patch}.{Revision}";
        }
    }
}