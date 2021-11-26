﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Server;

namespace Bam.Net.Server.PathHandlers
{
    public class NamedPath
    {
       public NamedPath()
        {
            PathFormat = "{Protocol}://{Domain}:{Port}/{PathName}/{Path}?{Query}";
        }

        public virtual string PathFormat { get; }
        public string PathName { get; set; }
        public string Protocol { get; set; }
        public string Domain { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }      
        public string Query { get; set; }
        
        public static NamedPath FromUri(Uri uri)
        {
            NamedPath result = new NamedPath();
            string url = uri.ToString();
            string protocol = url.ReadUntil("://", out string domainAndPortAndPathAndQuery);
            string domainAndPort = domainAndPortAndPathAndQuery.ReadUntil("/", out string pathNameAndPathAndQuery);
            string domain = domainAndPort.ReadUntil(":", out string port);
            string pathName = pathNameAndPathAndQuery.ReadUntil("/", out string pathAndQuery);
            string path = pathAndQuery.ReadUntil("?", out string queryString);

            result.Protocol = protocol;
            result.Domain = domain;
            result.Port = string.IsNullOrEmpty(port) ? 80 : Convert.ToInt32(port);
            result.PathName = pathName;
            result.Path = path;
            result.Query = queryString;
            return result;
        }

        public static T FromUri<T>(Uri uri) where T: NamedPath, new()
        {
            return FromUri(uri).CopyAs<T>();
        }

        public virtual bool IsMatch(Uri uri)
        {
            string matchFormat = "{Protocol}://{Domain}:{Port}/{PathName}/{Path}";
            NamedPath namedPathOfUri = FromUri(uri);

            string myPath = matchFormat.NamedFormat(this);
            string pathOfUri = matchFormat.NamedFormat(namedPathOfUri);
            return myPath.Equals(pathOfUri);
        }

        public override string ToString()
        {
            return this.PathFormat.NamedFormat(this);
        }
    }
}
