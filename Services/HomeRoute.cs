using Bam.Net.Server;
using System;
using System.Collections.Generic;

namespace Bam.Net.Services
{
/*    public class HomeRoute
    {
        public HomeRoute(string url)
        {
            Uri = new Uri(url);
            Parse();
        }

        public HomeRoute(Uri uri)
        {
            Uri = uri;
            Parse();
        }

        public bool IsValid => !string.IsNullOrEmpty(Protocol) && !string.IsNullOrEmpty(Domain);

        Dictionary<string, string> values; 
        
        public Uri Uri { get; set; }
        public string Protocol  
        {
            get => values["Protocol"];
            set => values["Protocol"] = value;
        }
        
        public string Domain
        {
            get => values["Domain"];
            set => values["Domain"] = value;
        }

        private void Parse()
        {
            // RouteParser doesn't work; implment with ReadUntil

            throw new NotImplementedException();
            *//*     RouteParser parser = new RouteParser("{Protocol}://{Domain}");
                 values = parser.ParseRouteInstance(Uri.ToString());*//*
        }
    }*/
}