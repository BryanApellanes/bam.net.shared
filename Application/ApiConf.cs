using Bam.Net.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bam.Net.Application
{
    public class ApiConf
    {
        public ApiConf()
        {
            BamConf = BamConf.Load();
            Verbose = true;
        }

        public bool Verbose { get; set; }

        public int SessionExpirationMinutes { get; set; }

        public BamConf BamConf { get; private set; }

        public string BamConfPath
        {
            get
            {
                return BamConf?.LoadedFrom;
            }
            set
            {
                BamConf = LoadBamConf(value);
            }
        }
        
        Dictionary<string, Func<string, BamConf>> _bamConfLoaders = new Dictionary<string, Func<string, BamConf>>
        {
            { ".json", (path) => BamConf.LoadJsonConfig(path) },
            { ".yaml", (path) => BamConf.LoadYamlConfig(path) },
            { ".yml", (path) => BamConf.LoadYamlConfig(path) },
            { ".xml", (path) => BamConf.LoadXmlConfig(path) },
        };

        private BamConf LoadBamConf(string path)
        {
            string ext = Path.GetExtension(path);
            if (_bamConfLoaders.ContainsKey(ext))
            {
                return _bamConfLoaders[ext](path);
            }

            return BamConf.LoadJsonConfig(path);
        }
    }
}
