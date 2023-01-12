/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Configuration;
using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server
{
    public class HostBinding
    {
        /// <summary>
        /// Instantiate a HostBinding with the hostname of "localhost" and port set to 8080.
        /// </summary>
        public HostBinding()
        {
            this.HostName = "localhost";
            this.Port = 8080;
            this.Ssl = true;
        }

        public HostBinding(int port) : this("localhost", port)
        {
        }

        public HostBinding(string hostName) : this(hostName, 8080)
        {
        }

        /// <summary>
        /// Instantiate a HostBinding with the specified hostname and port.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public HostBinding(string hostName, int port)
        {
            this.HostName = hostName;
            this.Port = port;
            this.Ssl = true;
        }

        public string HostName { get; set; }
        public virtual int Port { get; set; }

        public bool Ssl { get; set; }

        public override string ToString()
        {
            string protocol = Ssl ? "https://": "http://";
            return $"{protocol}{HostName}:{Port}/";
        }

        public override bool Equals(object obj)
        {
            if (obj is HostBinding compareTo)
            {
                return compareTo.ToString().Equals(this.ToString());
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().ToSha1Int();
        }

        public HostBinding FromServiceSubdomain(ServiceSubdomainAttribute attr)
        {
            HostBinding result = this.CopyAs<HostBinding>();
            result.HostName = $"{attr.Subdomain}.{HostName}";
            return result;
        }

        public static HostBinding[] FromHostAppMaps(IEnumerable<HostAppMap> hostAppMaps)
        {
            return hostAppMaps.Select(hm => new HostBinding { HostName = hm.Host, Port = 80 }).ToArray();
        }

        public static HostBinding[] FromBamProcessConfig(string defaultHostName = "localhost", int defaultPort = 80)
        {
            int port = int.Parse(Config.Current["Port", defaultPort.ToString()]);
            bool ssl = Config.Current["Ssl"].IsAffirmative();
            List<HostBinding> results = new List<HostBinding>();
            foreach (string hostName in Config.Current["HostNames"].Or(defaultHostName).DelimitSplit(",", true))
            {
                AddHostBinding(hostName, port, ssl, results);
            }
            return results.ToArray();
        }

        public static HostBinding[] FromDefaultConfiguration(string defaultHostName = "localhost", int defaultPort = 80)
        {
            int port = int.Parse(DefaultConfiguration.GetAppSetting("Port", defaultPort.ToString()));
            bool ssl = DefaultConfiguration.GetAppSetting("Ssl").IsAffirmative();
            List<HostBinding> results = new List<HostBinding>();
            foreach (string hostName in DefaultConfiguration.GetAppSetting("HostNames").Or(defaultHostName).DelimitSplit(",", true))
            {
                AddHostBinding(hostName, port, ssl, results);
            }
            return results.ToArray();
        }
        
        private static void AddHostBinding(string hostName, int port, bool ssl, List<HostBinding> results)
        {
            HostBinding hostPrefix = new HostBinding()
            {
                HostName = hostName,
                Port = port,
                Ssl = ssl
            };
            results.Add(hostPrefix);
            Trace.Write($"Default Config Hostname: {hostPrefix.ToString()}");
        }
    }
}