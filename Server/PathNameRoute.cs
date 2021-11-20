using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class PathNameRoute
    {
        public const string Route = "{PathName}/{Path}";

        public string PathName { get; set; }
        public string Path { get; set; }

        public static PathNameRoute FromRequestRoute(RequestRoute requestRoute)
        {
            return FromPath(requestRoute.PathAndQuery);
        }

        public static PathNameRoute FromRequest(IRequest request)
        {
            return FromPath(request.Url.PathAndQuery);
        }

        public static PathNameRoute FromPath(string path)
        {
            RouteParser parser = new RouteParser(Route);
            Dictionary<string, string> values = parser.ParseRouteInstance(path);
            return values.ToInstance<PathNameRoute>();
        }
    }
}
